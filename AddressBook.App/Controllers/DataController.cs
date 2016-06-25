using AddressBook.App.Models;
using AddressBook.DB;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AddressBook.App.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public void Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (AddressBookEntities db = new AddressBookEntities())
                {
                    bool userValid = db.User.Any(user => user.Username == model.Username && user.Password == model.Password);

                    if (userValid)
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        var FormsAuthCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                        var ExistingTicket = FormsAuthentication.Decrypt(FormsAuthCookie.Value).Name;
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }

            }
        }

        [HttpPost]
        public void Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (AddressBookEntities db = new AddressBookEntities())
                {
                    var newUser = db.User.Create();
                    newUser = new User { Username = model.Username, Password = model.Password, Email = model.Email };
                    db.User.Add(newUser);
                    db.SaveChanges();

                    FormsAuthentication.SetAuthCookie(model.Username, false);
                    var FormsAuthCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                    var ExistingTicket = FormsAuthentication.Decrypt(FormsAuthCookie.Value).Name;
                }
            }
        }

        [HttpPost]
        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }

        public string CurrentUser()
        {
            var user = User.Identity.GetUserName();
            return user;
        }
    }
}