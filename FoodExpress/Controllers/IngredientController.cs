using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class IngredientController : Controller
    {
        dbFoodExpressDataContext _db;
        public IngredientController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Ingredient
        public ActionResult Index(string message, string keyword, string currentFilter, int? currentRestaurantFilter, int? page)
        {
            Customer cus = Session["User"] as Customer;
            List<Models.Ingredient> lsIngre = new List<Models.Ingredient>();
            
            if (cus != null)
            {
                if (cus.IDRole == 4)
                {
                    List<Res_Restaurant> lsRestaurant = new List<Res_Restaurant>();
                    lsRestaurant = _db.Res_Restaurants.Where(x => x.OwnerId == cus.IDCustomer).ToList();
                    if (lsRestaurant != null)
                    {
                        lsRestaurant.ForEach(x =>
                        {
                            lsIngre = lsIngre.Concat(_db.Ingredients.Where(y => y.IDRes == x.IDRes && y.Active == true).Select(z => new Models.Ingredient
                            {
                                IDIngrdient = z.IDIngredient,
                                NameIngredient = z.NameIngredient,
                                IDRes = z.IDRes.Value,
                                Active = z.Active.Value,
                                NameRes = z.Res_Restaurant.NameRes
                            }).ToList()).ToList();
                        });
                    }
                    Session["lsRes"] = lsRestaurant;
                }
                if (cus.IDRole == 5)
                {
                    lsIngre = _db.Ingredients.Where(y => y.IDRes == cus.IDRes && y.Active == true).Select(z => new Models.Ingredient
                    {
                        IDIngrdient = z.IDIngredient,
                        NameIngredient = z.NameIngredient,
                        IDRes = z.IDRes.Value,
                        Active = z.Active.Value,
                        NameRes = z.Res_Restaurant.NameRes
                    }).ToList();
                }
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
                ViewBag.CurrentRestaurantFilter = currentRestaurantFilter;
                if (keyword != null)
                {
                    lsIngre = lsIngre.Where(x => x.NameIngredient.ToLower().Contains(keyword.ToLower())).ToList();

                }
                if (currentRestaurantFilter != null)
                {
                    lsIngre = lsIngre.Where(x => x.IDRes == currentRestaurantFilter.Value).ToList();
                }
                int pagesize = 20;
                int pagenumber = page ?? 1;
                return View(lsIngre.ToPagedList(pagenumber, pagesize));
            }
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public ActionResult CreateIngredient(string ingName, int? restaurant, bool active = false)
        {
            Customer cus = Session["User"] as Customer;
            int idres = 0;
            if (cus.IDRole == 4)
            {
                if (!restaurant.HasValue)
                {
                    return RedirectToAction("Index", new { message = "Enter ingredient name or choose restaurant"});
                }
                idres = restaurant.Value;
            }
            if (cus.IDRole == 5)
            {
                idres = cus.IDRes.Value;
            }
            if (Session["Ingredient"] != null)
            {
                Ingredient i = Session["Ingredient"] as Ingredient;
                Ingredient ingre = _db.Ingredients.Where(x => x.IDIngredient == i.IDIngredient).SingleOrDefault();
                ingre.NameIngredient = ingName;
                ingre.IDRes = idres;
                ingre.Active = active;
                _db.SubmitChanges();
                Session.Remove("Ingredient");
            }
            else
            {
                Ingredient ingre = new Ingredient
                {
                    NameIngredient = ingName,
                    IDRes = idres,
                    Active = active,
                    CreatedOn = DateTime.Now
                };
                _db.Ingredients.InsertOnSubmit(ingre);
                _db.SubmitChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Cancel()
        {
            Session.Remove("Ingredient");
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                Ingredient ingre = _db.Ingredients.Where(x => x.IDIngredient == id).SingleOrDefault();
                if (ingre != null)
                {
                    Session["Ingredient"] = ingre;
                }
                return RedirectToAction("Index");
            }
            return View("PageNotFound");
        }

        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                Ingredient ingre = _db.Ingredients.Where(x => x.IDIngredient == id).SingleOrDefault();
                ingre.Active = false;
                _db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View("PageNotFound");
        }
    }
}