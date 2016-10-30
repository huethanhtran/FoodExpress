using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace FoodExpress.Controllers
{
    public class RestaurantCategoryController : Controller
    {
        dbFoodExpressDataContext _db;
        
        public RestaurantCategoryController()
        {
            _db = new dbFoodExpressDataContext();
            
        }
        // GET: RestaurantCategory
        public ActionResult Index(string message, string keyword,string currentFilter, int? page)
        {
            List<Res_Category> lsCate = _db.Res_Categories.ToList();
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
                lsCate = lsCate.Where(x => x.NameCate.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            return View(lsCate.ToPagedList(pagenumber, pagesize));
        }

        [HttpPost]
        public ActionResult CreateCategory(string catename, bool active = false)
        {
            Res_Category res = null;
            string notice = null;
            if (!string.IsNullOrEmpty(catename) && !string.IsNullOrWhiteSpace(catename))
            {
                if (Session["cate"] != null)
                {
                    res = Session["cate"] as Res_Category;
                    Res_Category rc = _db.Res_Categories.Where(x => x.IDCate == res.IDCate).SingleOrDefault();
                    rc.NameCate = catename;
                    rc.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("cate");
                }
                else
                {
                    res = new Res_Category
                    {
                        NameCate = catename,
                        Active = Convert.ToBoolean(active)
                    };
                    _db.Res_Categories.InsertOnSubmit(res);
                    _db.SubmitChanges();

                }
            }
            else
            {
                notice = "Check data again";
            }
            return RedirectToAction("Index", new { message = notice});
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                Res_Category res = _db.Res_Categories.Where(x => x.IDCate == id).SingleOrDefault();
                Session["cate"] = res;
            }
            return RedirectToAction("Index");
        }

       
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.Res_Categoty_Mappings.Any(x => x.IDCate == id))
                {
                    _db.Res_Categories.DeleteOnSubmit(_db.Res_Categories.Where(x => x.IDCate == id).SingleOrDefault());
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