﻿using AddressBook.App.Models;
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
            return User.Identity.GetUserName();
        }

        public int CurrentUserID()
        {
            using(var db = new AddressBookEntities())
            {
                var user = CurrentUser();
                return db.User.Where(x => x.Username == user).SingleOrDefault().ID;
            }
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
                if (contact.Numbers != null && contact.Emails != null)
                {
                    var numb = contact.Numbers.Distinct();
                    var emails = contact.Emails.Distinct();

                    try
                    {
                        foreach (var no in numb)
                        {
                            contactRepo.CreateAddress(item.ID, 1, no.PhoneNumber);
                        }
                        foreach (var no in emails)
                        {
                            contactRepo.CreateAddress(item.ID, 2, no.EmailAddress);
                        }
                        foreach (var tag in contact.Tags)
                        {
                            contactRepo.AddTag(item.ID, tag, CurrentUserID());
                        }
                    }
                    catch (NullReferenceException exc) { }
                }
            }
        }
    }
}