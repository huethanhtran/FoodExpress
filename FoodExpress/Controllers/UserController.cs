using FoodExpress.Helpers;
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
        public ActionResult Index(string keyword, string currentFilter, int? page, int? ownerid)
        {
            if (Session["user"] != null)
            {
                Customer c = Session["user"] as Customer;
                List<Models.Customer> lsCustomer = null;
                if (c.IDRole == 2 || c.IDRole == 3)
                {
                    lsCustomer = _db.Customers.Select(x =>
                       new Models.Customer
                       {
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
                           IDRole = x.IDRole.Value,
                           UserName = x.UserName
                       }).ToList();
                }
                else if (c.IDRole == 4)
                {
                    var lsIdRes = _db.Res_Restaurants.Where(x => x.OwnerId == ownerid).Select(x => x.IDRes).ToList();

                    lsCustomer = BuildCustomerByIdRes(lsIdRes);
                }
                else if (c.IDRole == 5)
                {
                    lsCustomer = _db.Customers.Where(x => x.IDRes == c.IDRes && x.IDRole == 6 && x.Active == true).Select(x =>
                        new Models.Customer
                        {
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
                            IDRole = x.IDRole.Value,
                            UserName = x.UserName
                        }).ToList();
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
                    lsCustomer = lsCustomer.Where(x => x.NameCustomer.ToLower().Contains(keyword.ToLower()) || x.UserName.ToLower().Contains(keyword.ToLower()) || x.Email.ToLower().Contains(keyword.ToLower()) || x.Phone.ToLower().Contains(keyword.ToLower())).ToList();
                }
                int pagesize = 20;
                int pagenumber = page ?? 1;
                return View(lsCustomer.ToPagedList(pagenumber, pagesize));
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }
        [HttpPost]
        public ActionResult UnactiveUser(int userId, bool value)
        {
            Customer c = _db.Customers.Where(x => x.IDCustomer == userId).SingleOrDefault();
            if (c != null)
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

        public ActionResult Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
            {
                password = DataHelper.EncryptPasswordSalt(password.Trim());
                Customer cus = _db.Customers.Where(x => x.UserName == username.Trim() && x.PasswordCustomer == password && x.Active == true && x.IDRole.HasValue).SingleOrDefault();
                if (cus != null)
                {
                    Session["user"] = cus;
                    if (cus.IDRole == 2 || cus.IDRole == 3)
                    {
                        return RedirectToAction("Index", "City");
                    }
                    if (cus.IDRole == 4 || cus.IDRole == 5)
                    {
                        return RedirectToAction("Index", "User");
                    }
                    if (cus.IDRole == 6)
                    {
                        return RedirectToAction("Index", "Order");
                    }
                }
                else
                {
                    ViewBag.Notice = "This account is incorrect or unactive";
                }

            }
            return View();
        }

        private List<Models.Customer> BuildCustomerByIdRes(List<int> lsIDRes)
        {
            List<Models.Customer> ls = new List<Models.Customer>();
            lsIDRes.ForEach(x =>
            {
                List<Customer> lsCus = _db.Customers.Where(y => y.IDRes == x && y.Active == true).ToList();
                if (lsCus != null)
                {
                    var lsC = lsCus.Select(z => new Models.Customer
                    {
                        IDCustomer = z.IDCustomer,
                        NameCustomer = z.NameCustomer,
                        Email = z.Email,
                        Phone = z.Phone,
                        Fax = z.Fax,
                        Active = z.Active.Value,
                        CreatedOn = z.CreatedOn.Value,
                        Avatar = z.Avatar,
                        CustomerAddress = z.CustomerAddress,
                        DateOfBirth = z.DateOfBirth.Value,
                        IDRole = z.IDRole.Value,
                        UserName = z.UserName
                    }).ToList();
                    ls = ls.Concat(lsC).ToList();
                }
            });
            return ls;
        }
    }
}