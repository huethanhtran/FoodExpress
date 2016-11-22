using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace FoodExpress.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register() {
            return View();
        }
        [HttpPost]
        public JsonResult Register(string txtCustomerName, string txtUserName, string txtPassword, string txtEmail, string txtAddress) {
            if (txtCustomerName=="")
            {
                return Json("-1");
            }
            if (txtUserName=="")
            {
                return Json("-2");
            }
            if (txtPassword=="")
            {
                return Json("-3");
            }
            const string strpatternEmail = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            Regex regEmail = new Regex(strpatternEmail, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (regEmail.IsMatch(txtEmail) == false)
            {
                return Json("-4");
            }
            if (txtAddress=="")
            {
                return Json("-5");
            }
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["FoodExpressConnectionString"].ConnectionString;
            Dictionary<string, string> dc = new Dictionary<string, string>();
            dc.Add("@NameCustomer", txtCustomerName);
            dc.Add("@UserName", txtUserName);
            dc.Add("@PasswordCustomer", Helpers.DataHelper.EncryptPasswordSalt(txtPassword));
            dc.Add("@Email", txtEmail);
            dc.Add("@CustomerAddress", txtAddress);
            var kq = CLTeamSS.SQLCMDHelper.ExecuteScalaQueryPROC(conn, "sp_CreateCustomer", dc);
            if ((int)kq == 1)
            {
                return Json("index");
            }
            return Json("0");
        }
    }
}