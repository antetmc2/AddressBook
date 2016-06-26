using AddressBook.App.Models;
using AddressBook.DAL;
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
        ContactRepository contactRepo = new ContactRepository();
        // GET: Data
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel model)
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
                        return Json(new { success = true });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }
            return Json(new
            {
                success = false,
                errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                    .Select(m => m.ErrorMessage).ToArray()
            });
        }

        [HttpPost]
        public JsonResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (AddressBookEntities db = new AddressBookEntities())
                {
                    try
                    {
                        var newUser = db.User.Create();
                        newUser = new User { Username = model.Username, Password = model.Password, Email = model.Email };
                        db.User.Add(newUser);
                        db.SaveChanges();

                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        var FormsAuthCookie = Response.Cookies[FormsAuthentication.FormsCookieName];
                        var ExistingTicket = FormsAuthentication.Decrypt(FormsAuthCookie.Value).Name;
                        return Json(new { success = true });
                    }
                    catch (Exception exc)
                    {
                        ModelState.AddModelError("", exc.Message);
                        return Json(new
                        {
                            success = false,
                            errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray()
                        });
                    }
                }
            }
            else return Json(new
            {
                success = false,
                errors = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                .Select(m => m.ErrorMessage).ToArray()
            });
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

        [HttpPost]
        public void Create(ContactInformation contact)
        {
            using(AddressBookEntities db = new AddressBookEntities())
            {
                var item = db.Contact.Create();
                item = contactRepo.SetBasicParams(item, contact);
                db.Contact.Add(item);
                db.SaveChanges();
            }
            int a = 5;
        }
    }
}