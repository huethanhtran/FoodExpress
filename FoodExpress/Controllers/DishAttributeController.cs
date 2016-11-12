using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class DishAttributeController : Controller
    {
        dbFoodExpressDataContext _db;
        public DishAttributeController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: DishAttribute
        public ActionResult Index(string message, string keyword, string currentFilter, int? page, int id)
        {
            List<Models.DishAttribute> lsDish = _db.Dish_Attributes.Where(x => x.IDDish == id).Select(x => new Models.DishAttribute { IDAttribute = x.IDAttribute, NameAttribute = x.NameAttribute, Active = x.Active.Value, DishName = x.Dish.NameDish, Price = x.Price.Value }).ToList();
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
                lsDish = lsDish.Where(x => x.NameAttribute.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 1;
            int pagenumber = page ?? 1;
            Session["IDDish"] = id;
            return View(lsDish.ToPagedList(pagenumber, pagesize));
        }

        [HttpPost]
        public ActionResult CreateDishAttribute(string attrname, decimal attprice, bool active = false)
        {
            Dish_Attribute d = null;
            string notice = null;
            if (!string.IsNullOrEmpty(attrname) && !string.IsNullOrWhiteSpace(attrname))
            {
                if (Session["dishattr"] != null)
                {
                    d = Session["dishattr"] as Dish_Attribute;
                    Dish_Attribute cr = _db.Dish_Attributes.Where(x => x.IDAttribute == d.IDAttribute).SingleOrDefault();
                    cr.NameAttribute = attrname;
                    cr.IDDish = (int)Session["IDDish"];
                    cr.Price = attprice;
                    cr.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("dishattr");
                }
                else
                {
                    d = new Dish_Attribute
                    {
                        NameAttribute = attrname,
                        IDDish = (int)Session["IDDish"],
                        Price = attprice,
                        CreatedOn = DateTime.Now,
                        Active = Convert.ToBoolean(active)
                    };
                    _db.Dish_Attributes.InsertOnSubmit(d);
                    _db.SubmitChanges();

                }
            }
            else
            {
                notice = "Check data again";
            }
            return RedirectToAction("Index", new { message = notice, id = (int)Session["IDDish"] });
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                Dish_Attribute d = _db.Dish_Attributes.Where(x => x.IDAttribute == id).SingleOrDefault();
                Session["dishattr"] = d;
            }
            return RedirectToAction("Index", new { id = (int)Session["IDDish"] });
        }

        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                    _db.Dish_Attributes.Where(x => x.IDAttribute == id).SingleOrDefault().Active = false;
                    _db.SubmitChanges();
                    return RedirectToAction("Index", new { id = (int)Session["IDDish"] });
                
            }
            return RedirectToAction("Index", new { message = "Can not delete" });
        }

        public ActionResult Cancel()
        {
            Session.Remove("dishattr");
            return RedirectToAction("Index", new { id = (int)Session["IDDish"] });
        }
    }
}