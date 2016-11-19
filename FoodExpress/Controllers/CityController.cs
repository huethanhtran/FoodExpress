using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class CityController : Controller
    {
        // GET: City
        dbFoodExpressDataContext _db;
        public CityController()
        {
            _db = new dbFoodExpressDataContext();
        }
        public ActionResult Index(string message, string keyword, string currentFilter, int? page)
        {
            if (Session["user"] != null)
            {
                Customer c = Session["user"] as Customer;
                if (c.IDRole == 2 || c.IDRole == 3)
                {
                    List<Models.City> lsCity = _db.Cities.Select(x => new Models.City { IDCity = x.IDCity, NameCity = x.NameCity, Active = x.Active.Value }).ToList();
                    if (message != null)
                    {
                        ViewBag.Notice = message;
                    }
                    if (keyword != null)
                    {
                        page = 1;
                    }
                    else
                    {
                        keyword = currentFilter;
                    }
                    ViewBag.CurrentFilter = keyword;
                    if (keyword != null)
                    {
                        lsCity = lsCity.Where(x => x.NameCity.ToLower().Contains(keyword.ToLower())).ToList();
                    }
                    int pagesize = 20;
                    int pagenumber = page ?? 1;
                    return View(lsCity.ToPagedList(pagenumber, pagesize));
                }
                return View("PageNotFound");
            }
            else
            {
                return RedirectToAction("Login","User");
            }
        }

        [HttpPost]
        public ActionResult CreateCity(string cityname, bool active = false)
        {
            City city = null;
            string notice = null;
            if (!string.IsNullOrEmpty(cityname) && !string.IsNullOrWhiteSpace(cityname))
            {
                if (Session["city"] != null)
                {
                    city = Session["city"] as City;
                    City ct = _db.Cities.Where(x => x.IDCity == city.IDCity).SingleOrDefault();
                    ct.NameCity = cityname;
                    ct.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("city");
                }
                else
                {
                    city = new City
                    {
                        NameCity = cityname,
                        Active = Convert.ToBoolean(active)
                    };
                    _db.Cities.InsertOnSubmit(city);
                    _db.SubmitChanges();

                }
            }
            else
            {
                notice = "Check data again";
            }
            return RedirectToAction("Index", new { message = notice });
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                City city = _db.Cities.Where(x => x.IDCity == id).SingleOrDefault();
                Session["city"] = city;
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.Res_Restaurants.Any(x => x.IDCity == id))
                {
                    _db.Cities.DeleteOnSubmit(_db.Cities.Where(x => x.IDCity == id).SingleOrDefault());
                    _db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", new { message = "Can not delete" });
        }

        public ActionResult Cancel()
        {
            Session.RemoveAll();
            return RedirectToAction("Index");
        }
    }
}