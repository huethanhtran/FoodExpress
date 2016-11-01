using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class UserController : Controller
    {
        dbFoodExpressDataContext _db;
        public UserController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: User
        public ActionResult Index(string keyword, string currentFilter, int? page)
        {
            List<Models.Customer> lsCustomer = _db.Customers.Select(x =>
            new Models.Customer {
                IDCustomer = x.IDCustomer,
                NameCustomer = x.NameCustomer,
                Email = x.Email,
                Phone = x.Phone,
                Fax = x.Fax,
                Active = x.Active.Value,
                CreatedOn = x.CreatedOn.Value,
                Avatar = x.Avatar,
                CustomerAddress = x.CustomerAddress,
                DateOfBirth = x.DateOfBirth.Value,
                FirstName = x.FirstName,
                IDRole = x.IDRole.Value,
                UserName = x.UserName }).ToList();
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
                lsCustomer = lsCustomer.Where(x => x.NameCustomer.ToLower().Contains(keyword.ToLower()) || x.UserName.ToLower().Contains(keyword.ToLower()) || x.FirstName.ToLower().Contains(keyword.ToLower()) || x.Email.ToLower().Contains(keyword.ToLower()) || x.Phone.ToLower().Contains(keyword.ToLower())).ToList();
            }
            int pagesize = 20;
            int pagenumber = page ?? 1;
            return View(lsCustomer.ToPagedList(pagenumber, pagesize));

        }
        [HttpPost]
        public ActionResult UnactiveUser(int userId, bool value)
        {
            Customer c = _db.Customers.Where(x => x.IDCustomer == userId).SingleOrDefault();
            if (c !=  null)
            {
                c.Active = value;
                _db.SubmitChanges();
                return Json("OK");
            }
            else
            {
                return Json("unsuccess");
            }
        }

        public ActionResult Details(int id)
        {
            if (id != 0)
            {
                Customer cus = _db.Customers.Where(x => x.IDCustomer == id).SingleOrDefault();
                if (cus != null)
                {
                    Models.Customer c = new Models.Customer
                    {
                        IDCustomer = cus.IDCustomer,
                        NameCustomer = cus.NameCustomer,
                        FirstName = cus.FirstName,
                        UserName = cus.UserName,
                        Email = cus.Email,
                        Phone = cus.Phone,
                        Fax = cus.Fax,
                        DateOfBirth = cus.DateOfBirth.Value,
                        IDRole = cus.IDRole.Value,
                        CustomerAddress = cus.CustomerAddress,
                        Active = cus.Active.Value,
                        Avatar = cus.Avatar,
                        CreatedOn = cus.CreatedOn.Value,
                        NameRole = cus.CustomerRole.NameRole
                    };
                    return View(c);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult GrantRoleUser(int userId, int value)
        {
            Customer c = _db.Customers.Where(x => x.IDCustomer == userId).SingleOrDefault();
            if (c != null)
            {
                c.IDRole = value;
                _db.SubmitChanges();
                return Json("OK");
            }
            else
            {
                return Json("unsuccess");
            }
        }
    }
}