using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class DishController : Controller
    {
        dbFoodExpressDataContext _db;
        public DishController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Dish
        public ActionResult Index(string message, string keyword, string currentFilter, int? page, int id)
        {
            if (Session["user"] != null)
            {
                Customer c = Session["user"] as Customer;
                if (c != null && (c.IDRole == 3 || c.IDRole == 4 || c.IDRole == 5))
                {
                    List<Models.Dish> lsDish = _db.Dishes.Where(x => x.IDRes == id).Select(x => new Models.Dish { IDDish = x.IDDish, NameDish = x.NameDish, Active = x.Active.Value, NameDishCate = x.Dish_Category.NameDishCate, Price = x.Price.Value }).ToList();
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
                        lsDish = lsDish.Where(x => x.NameDish.ToLower().Contains(keyword.ToLower())).ToList();
                    }
                    int pagesize = 20;
                    int pagenumber = page ?? 1;
                    Session["lsDishCate"] = _db.Dish_Categories.Where(x => x.Active == true).ToList();
                    Session["IDRes"] = id;
                    return View(lsDish.ToPagedList(pagenumber, pagesize));
                }
                return View("PageNotFound");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult CreateDish(string dishname, int? category, decimal dishprice, bool active = false)
        {
            Dish d = null;
            string notice = null;
            if (!string.IsNullOrEmpty(dishname) && !string.IsNullOrWhiteSpace(dishname) && category.HasValue)
            {
                if (Session["dish"] != null)
                {
                    d = Session["dish"] as Dish;
                    Dish cr = _db.Dishes.Where(x => x.IDDish == d.IDDish).SingleOrDefault();
                    cr.NameDish = dishname;
                    cr.IDDishCate = category;
                    cr.Price = dishprice;
                    cr.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("dish");
                }
                else
                {
                    d = new Dish
                    {
                        NameDish = dishname,
                        IDDishCate = category,
                        Price = dishprice,
                        CreatedOn = DateTime.Now,
                        IDRes = (int)Session["IDRes"],
                        Active = Convert.ToBoolean(active)
                    };
                    _db.Dishes.InsertOnSubmit(d);
                    _db.SubmitChanges();

                }
            }
            else
            {
                notice = "Check data again";
            }
            return RedirectToAction("Index", new { message = notice, id = (int)Session["IDRes"] });
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                Dish d = _db.Dishes.Where(x => x.IDDish == id).SingleOrDefault();
                Session["dish"] = d;
            }
            return RedirectToAction("Index", new { id = (int)Session["IDRes"] });
        }

        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.OrderDetails.Any(x => x.IDDish == id))
                {
                    _db.Dishes.DeleteOnSubmit(_db.Dishes.Where(x => x.IDDish == id).SingleOrDefault());
                    _db.SubmitChanges();
                    return RedirectToAction("Index", new { id = (int)Session["IDRes"] });
                }
            }
            return RedirectToAction("Index", new { message = "Can not delete" });
        }

        public ActionResult Cancel()
        {
            Session.Remove("dish");
            return RedirectToAction("Index", new { id = (int)Session["IDRes"] });
        }
    }
}