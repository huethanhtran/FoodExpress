using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class OrderController : Controller
    {
        dbFoodExpressDataContext _db;
        public OrderController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Order
        public ActionResult Index(DateTime? StartDate, DateTime? EndDate, DateTime? currentStartDateFilter, DateTime? currentEndDateFilter, int? page, int? id, int? IDRes)
        {
            if (Session["user"] != null)
            {
                Customer cus = Session["user"] as Customer;
                List<Models.Order> lsOrder = new List<Models.Order>();
                if (IDRes.HasValue)
                {
                    id = IDRes.Value;
                }
                if (cus.IDRole == 2 || cus.IDRole == 3)
                {
                    if (id.HasValue)
                    {
                        lsOrder = _db.Orders.Where(x => x.IDRes == id).Select(x => new Models.Order
                        {
                            OrderId = x.IDOrder,
                            Active = x.Active.Value,
                            AddressOrder = x.AddressOrder,
                            Customer = x.Customer,
                            Restaurant = x.Res_Restaurant,
                            IsPay = x.IsPay.Value,
                            OrderPrice = x.OrderPrice.Value,
                            TotalPrice = x.TotalPrice.Value,
                            ReturnBack = x.ReturnBack.Value,
                            ServiceFee = x.ServiceFee.Value,
                            ShippingFee = x.ShippingFee.Value,
                            UpdatedBy = x.UpdatedBy.Value,
                            CreatedOn = x.CreatedOn.Value
                        }).ToList();
                    }
                    else
                    {
                        lsOrder = _db.Orders.Select(x => new Models.Order
                        {
                            OrderId = x.IDOrder,
                            Active = x.Active.Value,
                            AddressOrder = x.AddressOrder,
                            Customer = x.Customer,
                            Restaurant = x.Res_Restaurant,
                            IsPay = x.IsPay.Value,
                            OrderPrice = x.OrderPrice.Value,
                            TotalPrice = x.TotalPrice.Value,
                            ReturnBack = x.ReturnBack.Value,
                            ServiceFee = x.ServiceFee.Value,
                            ShippingFee = x.ShippingFee.Value,
                            UpdatedBy = x.UpdatedBy.Value,
                            CreatedOn = x.CreatedOn.Value
                        }).ToList();
                    }
                }
                else if (cus.IDRole == 4)
                {
                    List<int> lsRes = _db.Res_Restaurants.Where(x => x.OwnerId == cus.IDCustomer).Select(x => x.IDRes).ToList();
                    lsRes.ForEach(x =>
                    {
                        lsOrder = lsOrder.Concat(_db.Orders.Where(y => y.IDRes == x).Select(y => new Models.Order
                        {
                            OrderId = y.IDOrder,
                            Active = y.Active.Value,
                            AddressOrder = y.AddressOrder,
                            Customer = y.Customer,
                            Restaurant = y.Res_Restaurant,
                            IsPay = y.IsPay.Value,
                            OrderPrice = y.OrderPrice.Value,
                            TotalPrice = y.TotalPrice.Value,
                            ReturnBack = y.ReturnBack.Value,
                            ServiceFee = y.ServiceFee.Value,
                            ShippingFee = y.ShippingFee.Value,
                            UpdatedBy = y.UpdatedBy.Value,
                            CreatedOn = y.CreatedOn.Value
                        }).ToList()).ToList();
                    });
                }
                else if (cus.IDRole == 5 || cus.IDRole == 6)
                {
                    lsOrder = _db.Orders.Where(x => x.IDRes == cus.IDRes).Select(x => new Models.Order
                    {
                        OrderId = x.IDOrder,
                        Active = x.Active.Value,
                        AddressOrder = x.AddressOrder,
                        Customer = x.Customer,
                        Restaurant = x.Res_Restaurant,
                        IsPay = x.IsPay.Value,
                        OrderPrice = x.OrderPrice.Value,
                        TotalPrice = x.TotalPrice.Value,
                        ReturnBack = x.ReturnBack.Value,
                        ServiceFee = x.ServiceFee.Value,
                        ShippingFee = x.ShippingFee.Value,
                        UpdatedBy = x.UpdatedBy.Value,
                        CreatedOn = x.CreatedOn.Value
                    }).ToList();
                }
                if (StartDate != null || EndDate != null)
                {
                    page = 1;
                }
                else
                {

                    StartDate = currentStartDateFilter;
                    EndDate = currentEndDateFilter;
                }
                ViewBag.CurrentStartDateFilter = StartDate;
                ViewBag.CurrentEndDateFilter = EndDate;
                ViewBag.IDRes = id;
                if (StartDate != null)
                {
                    lsOrder = lsOrder.Where(x => x.CreatedOn >= StartDate).ToList();
                }
                if (EndDate != null)
                {
                    lsOrder = lsOrder.Where(x => x.CreatedOn <= EndDate).ToList();
                }
                int pagesize = 20;
                int pagenumber = page ?? 1;
                ViewBag.Total = Convert.ToDouble(lsOrder.Select(x => x.ReturnBack).Sum());
                return View(lsOrder.ToPagedList(pagenumber, pagesize));
            }
            return RedirectToAction("Login", "User");
        }

        public ActionResult Details(int id)
        {
            if (id != 0)
            {
                List<Models.OrderDetail> od = new List<Models.OrderDetail>();
                if (Session["OrderDetail"] != null)
                {
                    od = Session["OrderDetail"] as List<Models.OrderDetail>;
                }
                else
                {
                    od = _db.OrderDetails.Where(x => x.IDOrder == id).Select(x => new Models.OrderDetail
                    {
                        Active = x.Active.Value,
                        Dish = x.Dish,
                        IDODetail = x.IDDetail,
                        Order = x.Order,
                        Price = x.Price.Value,
                        Quantity = x.Quantity.Value,
                        Attributes = JsonConvert.DeserializeObject<List<Models.Attribute>>(x.Attributes),

                    }).ToList();
                    Session["OrderDetail"] = od;
                }


                return View(od);
            }
            return View("PageNotFound");
        }

        public ActionResult DeleteTopping(int id, int idDetail, int idOrder)
        {
            if (id != 0 && idDetail != 0 && idOrder != 0)
            {
                List<Models.OrderDetail> ls = Session["OrderDetail"] as List<Models.OrderDetail>;
                if (ls != null)
                {
                    Models.OrderDetail od = ls.Where(x => x.IDODetail == idDetail).SingleOrDefault();
                    if (od != null)
                    {
                        od.Attributes.Remove(od.Attributes.Where(x => x.Id == id).SingleOrDefault());
                        Session["OrderDetail"] = ls;
                        return RedirectToAction("Details", new { id = idOrder });
                    }
                }
            }
            return View("PageNotFound");
        }
        [HttpPost]
        public ActionResult UpdateDish(int newquantity, int idDish, int idDetail)
        {
            if (idDish != 0 && idDetail != 0)
            {
                if (newquantity > 0)
                {
                    List<Models.OrderDetail> ls = Session["OrderDetail"] as List<Models.OrderDetail>;
                    if (ls != null)
                    {
                        Models.OrderDetail od = ls.Where(x => x.IDODetail == idDetail).SingleOrDefault();
                        if (od != null)
                        {
                            od.Quantity = newquantity;
                            Session["OrderDetail"] = ls;
                        }
                    }
                    return Json("OK");
                }
                else
                {
                    return Json("Quantity must be greater than 0");
                }
            }
            return View("PageNotFound");
        }

        public ActionResult DeleteDish(int id, int idOrder)
        {
            if (id > 0)
            {
                List<Models.OrderDetail> ls = Session["OrderDetail"] as List<Models.OrderDetail>;
                if (ls != null)
                {
                    Models.OrderDetail od = ls.Where(x => x.IDODetail == id).SingleOrDefault();
                    ls.Remove(od);
                    Session["OrderDetail"] = ls;
                    return RedirectToAction("Details", new { id = idOrder });
                }
            }
            return View("PageNotFound");
        }
        [HttpPost]
        public ActionResult UpdateOrder(decimal ShippingFee, decimal ServiceFee, string AddressOrder)
        {

            List<Models.OrderDetail> ls = Session["OrderDetail"] as List<Models.OrderDetail>;
            if (ls != null)
            {
                // delete all detail items
                List<OrderDetail> lsDetail = _db.OrderDetails.Where(x => x.IDOrder == ls[0].Order.IDOrder).ToList();
                _db.OrderDetails.DeleteAllOnSubmit(lsDetail);
                _db.SubmitChanges();
                // insert new detail items
                List<OrderDetail> lsDetail2 = ls.Select(x => new OrderDetail
                {
                    Active = x.Active,
                    IDDish = x.Dish.IDDish,
                    IDOrder = x.Order.IDOrder,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Attributes = JsonConvert.SerializeObject(x.Attributes)
                }).ToList();
                _db.OrderDetails.InsertAllOnSubmit(lsDetail2);
                _db.SubmitChanges();

                // change order information
                Order o = _db.Orders.Where(x => x.IDOrder == ls[0].Order.IDOrder).SingleOrDefault();
                o.ServiceFee = ServiceFee;
                o.ShippingFee = ShippingFee;
                o.AddressOrder = AddressOrder;

                decimal orderPrice = ls.Select(x => (x.Price + x.Attributes.Select(y => y.Price).Sum())*x.Quantity).Sum();
                decimal totalPrice = orderPrice + ServiceFee + ShippingFee;
                o.OrderPrice = orderPrice;
                o.TotalPrice = totalPrice;
                o.ReturnBack = (totalPrice * (_db.Res_Restaurants.Where(x => x.IDRes == o.IDRes).SingleOrDefault().Commission)) / 100;
                _db.SubmitChanges();
                Session.Remove("OrderDetail");
            }
            return RedirectToAction("Details", new { id = ls[0].Order.IDOrder });
        }
        [HttpPost]
        public ActionResult ChangeNormalInformation(string value, string text)
        {
            List<Models.OrderDetail> ls = Session["OrderDetail"] as List<Models.OrderDetail>;
            switch (text)
            {
                case "ServiceFee":
                    ls[0].Order.ServiceFee = decimal.Parse(value);
                    break;
                case "ShippingFee":
                    ls[0].Order.ShippingFee = decimal.Parse(value);
                    break;
                case "AddressOrder":
                    ls[0].Order.AddressOrder = value;
                    break;
                default:
                    break;
            }
            Session["OrderDetail"] = ls;
            return Json("OK");
        }

        [HttpPost]
        public ActionResult ChangeStatusOrder(string text, bool value, int idOrder)
        {
            Order o = _db.Orders.Where(x => x.IDOrder == idOrder).SingleOrDefault();
            switch (text)
            {
                case "IsPay":
                    o.IsPay = value;
                    break;
                case "Active":
                    o.Active = value;
                    break;
                default:
                    break;
            }
            _db.SubmitChanges();
            return Json("OK");
        }
    }
}