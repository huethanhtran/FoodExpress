using FoodExpress.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class RestaurantController : Controller
    {
        dbFoodExpressDataContext _db;
        public RestaurantController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Restaurant
        public ActionResult Index(string keyword, string currentFilter, int? cityID, int? currentCityFilter, int? page)
        {
            List<Restaurant> lsRes = _db.Res_Restaurants.Select(x => new Restaurant
            {
                IDRes = x.IDRes,
                NameRes = x.NameRes,
                Active = x.Active.Value,
                Commission = x.Commission,
                CreatedOn = x.CreatedOn.Value,
                DescriptionRes = x.DescriptionRes,
                Summary = x.Summary,
                Email = x.Email,
                Fax = x.Fax,
                IDCity = x.IDCity,
                OwnerId = x.OwnerId,
                Phone = x.Phone,
                ResAddress = x.ResAddress,
                ServiceFee = x.ServiceFee,
                TimeBreakEnd = x.TimeBreakEnd.Value,
                TimeBreakStart = x.TimeBreakStart.Value,
                TimeEnd = x.TimeEnd.Value,
                TimeStart = x.TimeStart.Value,
                Website = x.Website,
                NameCity = x.City.NameCity
            }).ToList();
            if (keyword != null)
            {
                page = 1;
            }
            else
            {
                keyword = currentFilter;
            }
            if (!cityID.HasValue)
            {
                cityID = currentCityFilter;
            }
            ViewBag.CurrentFilter = keyword;
            ViewBag.CurrentCityFilter = cityID;
            if (keyword != null)
            {
                lsRes = lsRes.Where(x => x.NameRes.ToLower().Contains(keyword.ToLower()) || x.Email.ToLower().Contains(keyword.ToLower()) || x.Phone.ToLower().Contains(keyword.ToLower()) || x.Fax.ToLower().Contains(keyword.ToLower()) || x.ResAddress.ToLower().Contains(keyword.ToLower()) || x.Website.ToLower().Contains(keyword.ToLower())).ToList();
            }
            if (cityID.HasValue)
            {
                lsRes = lsRes.Where(x => x.IDCity == cityID.Value).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            TempData["Cities"] = _db.Cities.ToList();
            return View(lsRes.ToPagedList(pagenumber, pagesize));
        }
        [HttpPost]
        public ActionResult UnactiveRestaurant(int id, bool value)
        {
            if (id > 0)
            {
                Res_Restaurant res = _db.Res_Restaurants.Where(x => x.IDRes == id).SingleOrDefault();
                if (res != null)
                {
                    res.Active = value;
                    _db.SubmitChanges();
                }
                return Json("OK");
            }
            return Json("unsuccess");
        }

        public ActionResult Details(int id)
        {

            if (id > 0)
            {

                Res_Restaurant res = _db.Res_Restaurants.Where(x => x.IDRes == id).SingleOrDefault();
                if (res != null)
                {
                    Restaurant r = new Restaurant
                    {
                        IDRes = res.IDRes,
                        NameRes = res.NameRes,
                        Active = res.Active.Value,
                        Commission = res.Commission,
                        CreatedOn = res.CreatedOn.Value,
                        DescriptionRes = res.DescriptionRes,
                        Summary = res.Summary,
                        Email = res.Email,
                        Fax = res.Fax,
                        IDCity = res.IDCity,
                        OwnerId = res.OwnerId,
                        Phone = res.Phone,
                        ResAddress = res.ResAddress,
                        ServiceFee = res.ServiceFee,
                        TimeBreakEnd = res.TimeBreakEnd.HasValue ? res.TimeBreakEnd.Value : (TimeSpan?)null,
                        TimeBreakStart = res.TimeBreakStart.HasValue ? res.TimeBreakStart.Value : (TimeSpan?)null,
                        TimeEnd = res.TimeEnd.Value,
                        TimeStart = res.TimeStart.Value,
                        Website = res.Website,
                        NameCity = res.City.NameCity,
                        City = _db.Cities.ToList(),
                        Avatar = res.Avatar
                    };
                    Session["Res"] = r;
                    return View(r);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateRestaurant(Restaurant model, HttpPostedFileBase ufAvatar)
        {
            if (ModelState.IsValid)
            {
                Restaurant res = Session["Res"] as Restaurant;
                Res_Restaurant r = _db.Res_Restaurants.Where(x => x.IDRes == res.IDRes).SingleOrDefault();
                if (r != null)
                {
                    string imageName = null;
                    if (model.IDCity == null)
                    {
                        ViewBag.Notice = "Incorrect City";
                        return View("Details", Session["Res"] as Restaurant);
                    }
                    if (ufAvatar != null)
                    {
                        FileInfo fileinfo = new FileInfo(ufAvatar.FileName);
                        imageName = Guid.NewGuid().ToString("N") + fileinfo.Extension;
                        System.IO.File.Delete(Server.MapPath("~/Images/Restaurant/" + res.Avatar));
                        ufAvatar.SaveAs(Server.MapPath("~/Images/Restaurant/" + imageName));
                    }

                    r.NameRes = model.NameRes;
                    r.Avatar = imageName != null ? imageName : res.Avatar;
                    r.IDCity = model.IDCity;
                    r.Commission = model.Commission;
                    r.DescriptionRes = model.DescriptionRes;
                    r.Email = model.Email;
                    r.Fax = model.Fax;
                    r.Phone = model.Phone;
                    r.ResAddress = model.ResAddress;
                    r.ServiceFee = model.ServiceFee;
                    r.Summary = model.Summary;
                    r.TimeBreakEnd = model.TimeBreakEnd;
                    r.TimeEnd = model.TimeEnd;
                    r.TimeBreakStart = model.TimeBreakStart;
                    r.TimeStart = model.TimeStart;
                    r.Website = model.Website;
                    _db.SubmitChanges();
                    return RedirectToAction("Index");
                }

            }
            return View("Details", Session["Res"] as Restaurant);
        }

        public ActionResult Create(string message, int? type)
        {
            Restaurant res = new Restaurant();
            if (type == 1)
            {
                ViewBag.Notice = message;
            }
            if (type == 2)
            {
                ViewBag.Message = message;
            }
            res.City = _db.Cities.ToList();
            return View(res);
        }
        [HttpPost]
        public ActionResult CreateRestaurant(Restaurant model, HttpPostedFileBase ufAvatar)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    string imageName = null;
                    int ownerId = 0;
                    if (ufAvatar != null)
                    {
                        FileInfo fileinfo = new FileInfo(ufAvatar.FileName);
                        imageName = Guid.NewGuid().ToString("N") + fileinfo.Extension;
                        ufAvatar.SaveAs(Server.MapPath("~/Images/Restaurant/" + imageName));
                    }

                    ownerId = _db.Customers.Where(x => x.UserName == model.Owner || x.Email == model.Owner).SingleOrDefault().IDCustomer;
                    if (ownerId == 0)
                    {
                        return RedirectToAction("Create", new { message = "Incorrect Owner", type = 1 });
                    }

                    if (model.IDCity == null)
                    {
                        return RedirectToAction("Create", new { message = "Incorrect City", type = 2 });
                    }
                    Res_Restaurant res = new Res_Restaurant
                    {
                        NameRes = model.NameRes,
                        Summary = model.Summary,
                        DescriptionRes = model.DescriptionRes,
                        ResAddress = model.ResAddress,
                        Phone = model.Phone,
                        Email = model.Email,
                        Fax = model.Fax,
                        Website = model.Website,
                        IDCity = model.IDCity,
                        TimeStart = model.TimeStart.Value,
                        TimeEnd = model.TimeEnd.Value,
                        TimeBreakEnd = model.TimeBreakEnd,
                        TimeBreakStart = model.TimeBreakStart,
                        CreatedOn = DateTime.Now,
                        Active = true,
                        ServiceFee = model.ServiceFee.HasValue ? model.ServiceFee.Value : 0,
                        Commission = model.Commission.HasValue ? model.Commission.Value : 0,
                        Avatar = imageName != null ? imageName : "defaultAvatar.png",
                        OwnerId = ownerId
                    };
                    _db.Res_Restaurants.InsertOnSubmit(res);
                    _db.SubmitChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("Create", new Restaurant { City = _db.Cities.ToList() });
        }

        public ActionResult Cancel()
        {
            return View("Create", new Restaurant { City = _db.Cities.ToList() });
        }
    }
}