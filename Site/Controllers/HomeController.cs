using CF.Models;
using DataAccessor;
using Models;
using Models.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using Utility.Models;

namespace Site.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application Name page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        public ActionResult Index()
        {
            if (Session["ReturnUrl"] != null)
            {
                ViewBag.ReturnUrl = Session["ReturnUrl"].ToString();

                Session.Remove("ReturnUrl");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginFormModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginFormModel loginFormModel)
        {
            RequestResult requestResult = AccountDataAccessor.GetAccount(loginFormModel);

            if (requestResult.IsSuccess)
            {
                var person = requestResult.Data as Person;

                if (Config.HaveLDAPSettings)
                {
                    return View();
                }
                else
                {
                    if (string.Compare(person.Password, loginFormModel.Password) == 0)
                    {
                        var organizations = HttpRuntime.Cache.GetOrInsert("organizations", () => OrganizationDataAccessor.GetAllOrganizations());

                        requestResult = AccountDataAccessor.GetAccount(organizations, person);

                        if (requestResult.IsSuccess)
                        {
                            var account = requestResult.Data as Account;
                            var ticket = new FormsAuthenticationTicket(1, account.Id, DateTime.Now, DateTime.Now.AddHours(24), true, account.Id, FormsAuthentication.FormsCookiePath);
                            string encTicket = FormsAuthentication.Encrypt(ticket);
                            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                            Session["Account"] = account;

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("LoginId", requestResult.Message);
                            return View();
                        }
                        
                    }
                    else
                    {
                        ModelState.AddModelError("Password",Resources.Resource.WrongPassword);
                        return View();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("LoginId", requestResult.Message);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult SignOut()
        {
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new PasswordFormModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordFormModel passwordFormModel)
        {
            return Content(JsonConvert.SerializeObject(AccountDataAccessor.ChangePassword(passwordFormModel, Session["Account"] as Account)));
        }
    }
}