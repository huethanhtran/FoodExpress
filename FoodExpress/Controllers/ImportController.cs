using FoodExpress.Models.Element;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class ImportController : Controller
    {
        dbFoodExpressDataContext _db;
        public ImportController()
        {
            _db = new dbFoodExpressDataContext();
        }
        // GET: Import
        public ActionResult Index(DateTime? StartDate, DateTime? EndDate, DateTime? currentStartDateFilter, DateTime? currentEndDateFilter, int? page)
        {
            if (Session["user"] != null)
            {
                Customer cus = Session["user"] as Customer;
                var ls = _db.sp_Import_GetAllImportByCustomer(cus.IDRole, cus.IDCustomer).ToList();
                List<Models.Import> lsImport = ls.Select(x => new Models.Import
                {
                    Active = x.Active.Value,
                    CreatedBy = x.CreatedBy.Value,
                    CreatedOn = x.CreatedOn.Value,
                    IDImport = x.IDImport,
                    ImportDate = x.ImportDate.Value,
                    Title = x.Title,
                    TotalPrice = x.TotalPrice.Value,
                    Importer = x.NameCustomer
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
                    lsImport = lsImport.Where(x => x.CreatedOn >= StartDate).ToList();
                }
                if (EndDate != null)
                {
                    lsImport = lsImport.Where(x => x.CreatedOn <= EndDate).ToList();
                }
                int pagesize = 20;
                int pagenumber = page ?? 1;
                return View(lsImport.ToPagedList(pagenumber, pagesize));
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult UnactiveImport(bool value, int idImport)
        {
            if (idImport > 0)
            {
                var ip = _db.WarehouseImports.Where(x => x.IDImport == idImport).SingleOrDefault();
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
        public ActionResult ChangeDateImport(string value, int idImport)
        {
            DateTime date;
            if (!DateTime.TryParse(value, out date))
            {
                return Json("Enter correct date format : yyyy/MM/dd");
            }
            if (idImport > 0 && date != null)
            {
                var ip = _db.WarehouseImports.Where(x => x.IDImport == idImport).SingleOrDefault();
                if (ip != null)
                {
                    ip.ImportDate = date;
                    _db.SubmitChanges();
                    return Json("OK");
                }
            }
            return Json("Can not change import date of this item");
        }

        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _db.ImportDetails.DeleteAllOnSubmit(_db.ImportDetails.Where(x => x.IDImport == id).ToList());
                _db.SubmitChanges();
                _db.WarehouseImports.DeleteOnSubmit(_db.WarehouseImports.Where(x => x.IDImport == id).SingleOrDefault());
                _db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View("PageNotFound");
        }

        public ActionResult Edit(int id)
        {
            if (id != 0)
            {
                List<ImportDetail> ls = _db.ImportDetails.Where(x => x.IDImport == id && x.Active == true).ToList();
                List<Models.ImportDetail> lsID = ls.Select(x => new Models.ImportDetail
                {
                    Active = x.Active.Value,
                    IDIDetail = x.IDIDetail,
                    Import = x.WarehouseImport,
                    Ingredient = x.Ingredient,
                    Quantity = x.Quantity.Value,
                    Unit = x.Unit.Value,
                    TotalPrice = x.TotalPrice.Value,
                    Price = x.Price.Value
                }).ToList();
                ViewBag.Ingredients = _db.Ingredients.Where(x => x.IDRes == lsID[0].Ingredient.IDRes).ToList();
                Session["ImportDetail"] = lsID;
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
                List<Models.ImportDetail> lsID = Session["ImportDetail"] as List<Models.ImportDetail>;
                if (lsID == null)
                {
                    lsID = new List<Models.ImportDetail>();
                }
                switch (text)
                {
                    case "Quantity":
                        if (idDetail > 0)
                        {
                            var id = lsID.Where(x => x.IDIDetail == idDetail).SingleOrDefault();
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
                                Models.ImportDetail id = new Models.ImportDetail
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
                            var id = lsID.Where(x => x.IDIDetail == idDetail).SingleOrDefault();
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
                                Models.ImportDetail id = new Models.ImportDetail
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
                Session["ImportDetail"] = lsID;
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
                    List<Models.ImportDetail> lsID = Session["ImportDetail"] as List<Models.ImportDetail>;
                    if (lsID == null)
                    {
                        lsID = new List<Models.ImportDetail>();
                    }
                    if (idDetail > 0)
                    {
                        var id = lsID.Where(x => x.IDIDetail == idDetail).SingleOrDefault();
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
                            Models.ImportDetail id = new Models.ImportDetail
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
                    Session["ImportDetail"] = lsID;
                    return Json("OK");
                }
                return Json("Select other kind of unit");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult UpdateImport(string Title, DateTime? ImportDate)
        {
            //if (string.IsNullOrEmpty(ImportDate) && string.IsNullOrWhiteSpace(ImportDate))
            List<Models.ImportDetail> lsID = Session["ImportDetail"] as List<Models.ImportDetail>;
            if (!ImportDate.HasValue)
            {
                ViewBag.ImportDate = "Enter Import Date";
                return View("Edit", lsID);
            }
            lsID = lsID.Where(x => x.Quantity > 0 && x.Price > 0 && x.Unit > 0 && x.Active == true).ToList();
            List<ImportDetail> ls = new List<ImportDetail>();
            lsID.ForEach(x =>
            {
                ImportDetail id = new ImportDetail
                {
                    IDImport = lsID[0].Import.IDImport,
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
            _db.ImportDetails.DeleteAllOnSubmit(_db.ImportDetails.Where(x => x.IDImport == lsID[0].Import.IDImport).ToList());
            _db.SubmitChanges();

            // insert again ingredient
            _db.ImportDetails.InsertAllOnSubmit(ls);
            _db.SubmitChanges();

            // update import information
            var ip = _db.WarehouseImports.Where(x => x.IDImport == lsID[0].Import.IDImport).SingleOrDefault();
            if (ip != null)
            {
                ip.Title = Title;
                ip.ImportDate = ImportDate;
                ip.TotalPrice = ls.Select(x => x.TotalPrice).Sum();
            }
            _db.SubmitChanges();
            Session.Remove("ImportDetail");
            return RedirectToAction("Edit", new { id = lsID[0].Import.IDImport });
        }

        public ActionResult Details(int id)
        {
            if (id != 0)
            {
                List<ImportDetail> ls = _db.ImportDetails.Where(x => x.IDImport == id && x.Active == true).ToList();
                List<Models.ImportDetail> lsID = ls.Select(x => new Models.ImportDetail
                {
                    Active = x.Active.Value,
                    IDIDetail = x.IDIDetail,
                    Import = x.WarehouseImport,
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
                Models.Import ip = new Models.Import
                {
                    lsDetail = new List<Models.ImportDetail>(),
                    IdRes = id
                };
                Session["NewImport"] = ip;
                return View();
            }
            return View("PageNotFound");
        }
        [HttpPost]
        public ActionResult CreateNewImportInformation(string text, int idIngredient, decimal value)
        {
            if (idIngredient > 0)
            {
                if (value < 0)
                {
                    return Json("Value must be greater than 0");
                }
                Models.Import ip = Session["NewImport"] as Models.Import;
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
                            Models.ImportDetail id = new Models.ImportDetail
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
                            Models.ImportDetail id = new Models.ImportDetail
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
                Session["NewImport"] = ip;
                return Json("OK");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult CreateNewImportUnit(int idIngredient, int? value)
        {
            if (idIngredient > 0)
            {
                if (value.HasValue)
                {
                    Models.Import ip = Session["NewImport"] as Models.Import;
                    if (ip.lsDetail.Count>0)
                    {
                        var detail = ip.lsDetail.Where(x => x.Ingredient.IDIngredient == idIngredient).SingleOrDefault();
                        if (detail != null)
                        {
                            detail.Unit = value.Value;
                        }
                    }
                    else
                    {
                        Models.ImportDetail id = new Models.ImportDetail
                        {
                            Active = true,
                            Unit = value.Value,
                            Ingredient = new Ingredient { IDIngredient = idIngredient },
                            Price = 0,
                            Quantity = 0
                        };
                        ip.lsDetail.Add(id);
                    }

                    Session["NewImport"] = ip;
                    return Json("OK");
                }
                return Json("Select other kind of unit");
            }
            return View("PageNotFound");
        }

        [HttpPost]
        public ActionResult CreateImport(string Title, DateTime? ImportDate)
        {
            //if (string.IsNullOrEmpty(ImportDate) && string.IsNullOrWhiteSpace(ImportDate))
            Customer c = Session["user"] as Customer;
            Models.Import ip = Session["NewImport"] as Models.Import;
            if (!ImportDate.HasValue)
            {
                ViewBag.ImportDate = "Enter Import Date";
                ViewBag.Ingredients = _db.Ingredients.Where(x => x.IDRes == ip.IdRes).ToList();
                return View("Create");
            }
            ip.lsDetail = ip.lsDetail.Where(x => x.Quantity > 0 && x.Price > 0 && x.Unit > 0 && x.Active == true).ToList();
            List<ImportDetail> ls = new List<ImportDetail>();
            int idImport = (int)_db.sp_Import_CreateNewImport(Title, ImportDate, ip.lsDetail.Select(x => x.Quantity * x.Price).Sum(), c.IDCustomer).FirstOrDefault().IdImport;
            if (idImport > 0)
            {
                List<ImportDetail> lsID = ip.lsDetail.Select(x => new ImportDetail
                {
                    Active = true,
                    IDImport = idImport,
                    IDIngredient = x.Ingredient.IDIngredient,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    TotalPrice = x.Price * x.Quantity,
                    Unit = x.Unit
                }).ToList();
                _db.ImportDetails.InsertAllOnSubmit(lsID);
                _db.SubmitChanges();
            }
            Session.Remove("NewImport");
            return RedirectToAction("Index");
        }
    }
}