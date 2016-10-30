using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PagedList;

namespace FoodExpress.Controllers
{
    public class RestaurantCuisineController : Controller
    {
        // GET: RestaurantCuisine
        dbFoodExpressDataContext _db;
        public RestaurantCuisineController()
        {
            _db = new dbFoodExpressDataContext();
        }
        public ActionResult Index(string message, string keyword, string currentFilter, int? page)
        {
            List<Models.Res_Cuisine> lsCuisine = _db.Res_Cuisines.Select(x => new Models.Res_Cuisine { IDCuisine = x.IDCuisine, NameCuisine = x.NameCuisine, Active = x.Active.Value }).ToList();
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
                lsCuisine = lsCuisine.Where(x => x.NameCuisine.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            return View(lsCuisine.ToPagedList(pagenumber, pagesize));
            
        }

        [HttpPost]
        public ActionResult CreateCuisine(string cuisinename, bool active = false)
        {
            Res_Cuisine res = null;
            string notice = null;
            if (!string.IsNullOrEmpty(cuisinename) && !string.IsNullOrWhiteSpace(cuisinename))
            {
                if (Session["cuisine"] != null)
                {
                    res = Session["cuisine"] as Res_Cuisine;
                    Res_Cuisine rc = _db.Res_Cuisines.Where(x => x.IDCuisine == res.IDCuisine).SingleOrDefault();
                    rc.NameCuisine = cuisinename;
                    rc.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("cuisine");
                }
                else
                {
                    res = new Res_Cuisine
                    {
                        NameCuisine = cuisinename,
                        Active = Convert.ToBoolean(active)
                    };
                    _db.Res_Cuisines.InsertOnSubmit(res);
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
                Res_Cuisine cuisine = _db.Res_Cuisines.Where(x => x.IDCuisine == id).SingleOrDefault();
                Session["cuisine"] = cuisine;
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.Res_Cuisine_Mappings.Any(x => x.IDCuisine == id))
                {
                    _db.Res_Cuisines.DeleteOnSubmit(_db.Res_Cuisines.Where(x => x.IDCuisine == id).SingleOrDefault());
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