using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class ExportController : Controller
    {
        dbFoodExpressDataContext _db;
        public ExportController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Export
        public ActionResult Index(DateTime? StartDate, DateTime? EndDate, DateTime? currentStartDateFilter, DateTime? currentEndDateFilter, int? page)
        {
            if (Session["user"] != null)
            {
                Customer cus = Session["user"] as Customer;
                var ls = _db.sp_Export_GetAllExportByCustomer(cus.IDRole, cus.IDCustomer).ToList();
                List<Models.Export> lsExport = ls.Select(x => new Models.Export
                {
                    Active = x.Active.Value,
                    CreatedBy = x.CreatedBy.Value,
                    CreatedOn = x.CreatedOn.Value,
                    IDExport = x.IDExport,
                    ExportDate = x.ExportDate.Value,
                    Title = x.Title,
                    TotalPrice = x.TotalPrice.Value,
                    Exporter = x.NameCustomer
                }).ToList();
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
                if (StartDate != null)
                {
                    lsExport = lsExport.Where(x => x.CreatedOn >= StartDate).ToList();
                }
                if (EndDate != null)
                {
                    lsExport = lsExport.Where(x => x.CreatedOn <= EndDate).ToList();
                }
                int pagesize = 20;
                int pagenumber = page ?? 1;
                return View(lsExport.ToPagedList(pagenumber, pagesize));
            }
            return RedirectToAction("Login", "User");
        }
        [HttpPost]
        public ActionResult UnactiveExport(bool value, int idExport)
        {
            if (idExport > 0)
            {
                var ip = _db.WarehouseExports.Where(x => x.IDExport == idExport).SingleOrDefault();
                if (ip != null)
                {
                    ip.Active = value;
                    _db.SubmitChanges();
                    return Json("OK");
                }
            }
            return Json("Can not unactive this item");
        }
        [HttpPost]
        public ActionResult ChangeDateExport(string value, int idExport)
        {
            DateTime date;
            if (!DateTime.TryParse(value, out date))
            {
                return Json("Enter correct date format : yyyy/MM/dd");
            }
            if (idExport > 0 && date != null)
            {
                var ip = _db.WarehouseExports.Where(x => x.IDExport == idExport).SingleOrDefault();
                if (ip != null)
                {
                    ip.ExportDate = date;
                    _db.SubmitChanges();
                    return Json("OK");
                }
            }
            return Json("Can not change Export date of this item");
        }
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _db.ExportDetails.DeleteAllOnSubmit(_db.ExportDetails.Where(x => x.IDExport == id).ToList());
                _db.SubmitChanges();
                _db.WarehouseExports.DeleteOnSubmit(_db.WarehouseExports.Where(x => x.IDExport == id).SingleOrDefault());
                _db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View("PageNotFound");
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                List<ExportDetail> ls = _db.ExportDetails.Where(x => x.IDExport == id && x.Active == true).ToList();
                List<Models.ExportDetail> lsID = ls.Select(x => new Models.ExportDetail
                {
                    Active = x.Active.Value,
                    IDEDetail = x.IDEDetail,
                    Export = x.WarehouseExport,
                    Ingredient = x.Ingredient,
                    Quantity = x.Quantity.Value,
                    Unit = x.Unit.Value,
                    TotalPrice = x.TotalPrice.Value,
                    Price = x.Price.Value
                }).ToList();
                ViewBag.Ingredients = _db.Ingredients.Where(x => x.IDRes == lsID[0].Ingredient.IDRes).ToList();
                Session["ExportDetail"] = lsID;
                return View(lsID);
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult ChangeIngredientInformation(string text, int idDetail, int idIngredient, decimal value)
        {
            if (idIngredient > 0)
            {
                if (value < 0)
                {
                    return Json("Value must be greater than 0");
                }
                List<Models.ExportDetail> lsID = Session["ExportDetail"] as List<Models.ExportDetail>;
                if (lsID == null)
                {
                    lsID = new List<Models.ExportDetail>();
                }
                switch (text)
                {
                    case "Quantity":
                        if (idDetail > 0)
                        {
                            var id = lsID.Where(x => x.IDEDetail == idDetail).SingleOrDefault();
                            id.Quantity = (int)value;
                        }
                        else
                        {
                            var detail = lsID.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                            if (detail != null)
                            {
                                detail.Quantity = (int)value;
                            }
                            else
                            {
                                Models.ExportDetail id = new Models.ExportDetail
                                {
                                    Active = true,
                                    Quantity = (int)value,
                                    Ingredient = new Ingredient { IDIngredient = idIngredient },
                                    Price = 0,
                                    Unit = 0
                                };
                                lsID.Add(id);
                            }

                        }
                        break;
                    case "Price":
                        if (idDetail > 0)
                        {
                            var id = lsID.Where(x => x.IDEDetail == idDetail).SingleOrDefault();
                            id.Price = value;
                        }
                        else
                        {
                            var detail = lsID.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                            if (detail != null)
                            {
                                detail.Price = value;
                            }
                            else
                            {
                                Models.ExportDetail id = new Models.ExportDetail
                                {
                                    Active = true,
                                    Price = value,
                                    Ingredient = new Ingredient { IDIngredient = idIngredient },
                                    Quantity = 0,
                                    Unit = 0
                                };
                                lsID.Add(id);
                            }
                        }
                        break;
                    default:
                        break;
                }
                Session["ExportDetail"] = lsID;
                return Json("OK");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult ChangeUnitInformation(int idDetail, int idIngredient, int? value)
        {
            if (idIngredient > 0)
            {
                if (value.HasValue)
                {
                    List<Models.ExportDetail> lsID = Session["ExportDetail"] as List<Models.ExportDetail>;
                    if (lsID == null)
                    {
                        lsID = new List<Models.ExportDetail>();
                    }
                    if (idDetail > 0)
                    {
                        var id = lsID.Where(x => x.IDEDetail == idDetail).SingleOrDefault();
                        id.Unit = value.Value;
                    }
                    else
                    {
                        var detail = lsID.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                        if (detail != null)
                        {
                            detail.Unit = value.Value;
                        }
                        else
                        {
                            Models.ExportDetail id = new Models.ExportDetail
                            {
                                Active = true,
                                Unit = value.Value,
                                Ingredient = new Ingredient { IDIngredient = idIngredient },
                                Price = 0,
                                Quantity = 0
                            };
                            lsID.Add(id);
                        }
                    }
                    Session["ExportDetail"] = lsID;
                    return Json("OK");
                }
                return Json("Select other kind of unit");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult UpdateExport(string Title, DateTime? ExportDate)
        {
            //if (string.IsNullOrEmpty(ExportDate) && string.IsNullOrWhiteSpace(ExportDate))
            List<Models.ExportDetail> lsID = Session["ExportDetail"] as List<Models.ExportDetail>;
            if (!ExportDate.HasValue)
            {
                ViewBag.ExportDate = "Enter Export Date";
                return View("Edit", lsID);
            }
            lsID = lsID.Where(x => x.Quantity > 0 && x.Price > 0 && x.Unit > 0 && x.Active == true).ToList();
            List<ExportDetail> ls = new List<ExportDetail>();
            lsID.ForEach(x =>
            {
                ExportDetail id = new ExportDetail
                {
                    IDExport = lsID[0].Export.IDExport,
                    Active = x.Active,
                    IDIngredient = x.Ingredient.IDIngredient,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Unit = x.Unit,
                    TotalPrice = x.Price * x.Quantity
                };
                ls.Add(id);
            });

            // delete all ingredient
            _db.ExportDetails.DeleteAllOnSubmit(_db.ExportDetails.Where(x => x.IDExport == lsID[0].Export.IDExport).ToList());
            _db.SubmitChanges();

            // insert again ingredient
            _db.ExportDetails.InsertAllOnSubmit(ls);
            _db.SubmitChanges();

            // update Export information
            var ip = _db.WarehouseExports.Where(x => x.IDExport == lsID[0].Export.IDExport).SingleOrDefault();
            if (ip != null)
            {
                ip.Title = Title;
                ip.ExportDate = ExportDate;
                ip.TotalPrice = ls.Select(x => x.TotalPrice).Sum();
            }
            _db.SubmitChanges();
            Session.Remove("ExportDetail");
            return RedirectToAction("Edit", new { id = lsID[0].Export.IDExport });
        }

        public ActionResult Details(int id)
        {
            if (id != 0)
            {
                List<ExportDetail> ls = _db.ExportDetails.Where(x => x.IDExport == id && x.Active == true).ToList();
                List<Models.ExportDetail> lsID = ls.Select(x => new Models.ExportDetail
                {
                    Active = x.Active.Value,
                    IDEDetail = x.IDEDetail,
                    Export = x.WarehouseExport,
                    Ingredient = x.Ingredient,
                    Quantity = x.Quantity.Value,
                    Unit = x.Unit.Value,
                    TotalPrice = x.TotalPrice.Value,
                    Price = x.Price.Value
                }).ToList();
                return View(lsID);
            }
            return View("PageNotFound");
        }

        public ActionResult Create(int id)
        {
            if (id > 0)
            {
                ViewBag.Ingredients = _db.Ingredients.Where(x => x.IDRes == id).ToList();
                Models.Export ip = new Models.Export
                {
                    lsDetail = new List<Models.ExportDetail>(),
                    IdRes = id
                };
                Session["NewExport"] = ip;
                return View();
            }
            return View("PageNotFound");
        }
        [HttpPost]
        public ActionResult CreateNewExportInformation(string text, int idIngredient, decimal value)
        {
            if (idIngredient > 0)
            {
                if (value < 0)
                {
                    return Json("Value must be greater than 0");
                }
                Models.Export ip = Session["NewExport"] as Models.Export;
                switch (text)
                {
                    case "Quantity":
                        if (ip.lsDetail.Count > 0)
                        {
                            var detail = ip.lsDetail.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                            if (detail != null)
                            {
                                detail.Quantity = (int)value;
                            }
                        }
                        else
                        {
                            Models.ExportDetail id = new Models.ExportDetail
                            {
                                Active = true,
                                Quantity = (int)value,
                                Ingredient = new Ingredient { IDIngredient = idIngredient },
                                Price = 0,
                                Unit = 0
                            };
                            ip.lsDetail.Add(id);
                        }
                        break;
                    case "Price":
                        if (ip.lsDetail.Count > 0)
                        {
                            var detail = ip.lsDetail.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                            if (detail != null)
                            {
                                detail.Price = value;
                            }
                        }
                        else
                        {
                            Models.ExportDetail id = new Models.ExportDetail
                            {
                                Active = true,
                                Quantity = 0,
                                Ingredient = new Ingredient { IDIngredient = idIngredient },
                                Price = value,
                                Unit = 0
                            };
                            ip.lsDetail.Add(id);
                        }
                        break;
                    default:
                        break;
                }
                Session["NewExport"] = ip;
                return Json("OK");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult CreateNewExportUnit(int idIngredient, int? value)
        {
            if (idIngredient > 0)
            {
                if (value.HasValue)
                {
                    Models.Export ip = Session["NewExport"] as Models.Export;
                    if (ip.lsDetail.Count > 0)
                    {
                        var detail = ip.lsDetail.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                        if (detail != null)
                        {
                            detail.Unit = value.Value;
                        }
                    }
                    else
                    {
                        Models.ExportDetail id = new Models.ExportDetail
                        {
                            Active = true,
                            Unit = value.Value,
                            Ingredient = new Ingredient { IDIngredient = idIngredient },
                            Price = 0,
                            Quantity = 0
                        };
                        ip.lsDetail.Add(id);
                    }

                    Session["NewExport"] = ip;
                    return Json("OK");
                }
                return Json("Select other kind of unit");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult CreateExport(string Title, DateTime? ExportDate)
        {
            //if (string.IsNullOrEmpty(ExportDate) && string.IsNullOrWhiteSpace(ExportDate))
            Customer c = Session["user"] as Customer;
            Models.Export ip = Session["NewExport"] as Models.Export;
            if (!ExportDate.HasValue)
            {
                ViewBag.ExportDate = "Enter Export Date";
                ViewBag.Ingredients = _db.Ingredients.Where(x => x.IDRes == ip.IdRes).ToList();
                return View("Create");
            }
            ip.lsDetail = ip.lsDetail.Where(x => x.Quantity > 0 && x.Price > 0 && x.Unit > 0 && x.Active == true).ToList();
            List<ExportDetail> ls = new List<ExportDetail>();
      
            int idExport = (int)_db.sp_Export_CreateNewExport(Title, ExportDate, ip.lsDetail.Select(x => x.Quantity * x.Price).Sum(), c.IDCustomer).FirstOrDefault().IdExport;
            if (idExport > 0)
            {
                List<ExportDetail> lsID = ip.lsDetail.Select(x => new ExportDetail
                {
                    Active = true,
                    IDExport = idExport,
                    IDIngredient = x.Ingredient.IDIngredient,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    TotalPrice = x.Price * x.Quantity,
                    Unit = x.Unit
                }).ToList();
                _db.ExportDetails.InsertAllOnSubmit(lsID);
                _db.SubmitChanges();
            }
            Session.Remove("NewExport");
            return RedirectToAction("Index");
        }
    }
}