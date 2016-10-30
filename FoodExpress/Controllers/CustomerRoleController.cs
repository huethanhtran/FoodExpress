using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class CustomerRoleController : Controller
    {
        dbFoodExpressDataContext _db;
        public CustomerRoleController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: CustomerRole
        public ActionResult Index(string message, string keyword, string currentFilter, int? page)
        {
            List<Models.CustomerRole> lsRole = _db.CustomerRoles.Select(x=> new Models.CustomerRole { IDRole = x.IDRole, NameRole = x.NameRole, Active = x.Active.Value }).ToList();
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
                lsRole = lsRole.Where(x => x.NameRole.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            return View(lsRole.ToPagedList(pagenumber, pagesize));
        }

        [HttpPost]
        public ActionResult CreateRole(string rolename, bool active = false)
        {
            CustomerRole role = null;
            string notice = null;
            if (!string.IsNullOrEmpty(rolename) && !string.IsNullOrWhiteSpace(rolename))
            {
                if (Session["role"] != null)
                {
                    role = Session["role"] as CustomerRole;
                    CustomerRole cr = _db.CustomerRoles.Where(x => x.IDRole == role.IDRole).SingleOrDefault();
                    cr.NameRole = rolename;
                    cr.Active = Convert.ToBoolean(active);
                    _db.SubmitChanges();
                    Session.Remove("role");
                }
                else
                {
                    role = new CustomerRole
                    {
                        NameRole = rolename,
                        Active = Convert.ToBoolean(active)
                    };
                    _db.CustomerRoles.InsertOnSubmit(role);
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
                CustomerRole role = _db.CustomerRoles.Where(x => x.IDRole == id).SingleOrDefault();
                Session["role"] = role;
            }
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                if (!_db.Customers.Any(x=>x.IDRole == id))
                {
                    _db.CustomerRoles.DeleteOnSubmit(_db.CustomerRoles.Where(x => x.IDRole == id).SingleOrDefault());
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