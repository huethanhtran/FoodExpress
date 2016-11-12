using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class DishCategoryController : Controller
    {
        dbFoodExpressDataContext _db;
        public DishCategoryController()
        {
            _db = new dbFoodExpressDataContext();
        }
        public ActionResult Index(string message, string keyword, string currentFilter, int? page)
        {
            List<Models.DishCategory> lsDish = _db.Dish_Categories.Select(x => new Models.DishCategory { IDDishCate = x.IDDishCate, NameCate = x.NameDishCate, Active = x.Active.Value }).ToList();
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
                lsDish = lsDish.Where(x => x.NameCate.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            return View(lsDish.ToPagedList(pagenumber, pagesize));
           
        }

        [HttpPost]
        public ActionResult CreateDishCategory(string dishcatename, bool active = false)
        {
            Dish_Category dc = null;
            string notice = null;
            if (!string.IsNullOrEmpty(dishcatename) && !string.IsNullOrWhiteSpace(dishcatename))
            {
                if (Session["dishcate"] != null)
                {
                    dc = Session["dishcate"] as Dish_Category;
                    Dish_Category cr = _db.Dish_Categories.Where(x => x.IDDishCate == dc.IDDishCate).SingleOrDefault();
                    cr.NameDishCate = dishcatename;
                    cr.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("dishcate");
                }
                else
                {
                    dc = new Dish_Category
                    {
                        NameDishCate = dishcatename,
                        Active = Convert.ToBoolean(active),
                        CreatedOn = DateTime.Now
                    };
                    _db.Dish_Categories.InsertOnSubmit(dc);
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
                Dish_Category dc = _db.Dish_Categories.Where(x => x.IDDishCate == id).SingleOrDefault();
                Session["dishcate"] = dc;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.Dishes.Any(x => x.IDDishCate == id))
                {
                    _db.Dish_Categories.DeleteOnSubmit(_db.Dish_Categories.Where(x => x.IDDishCate == id).SingleOrDefault());
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