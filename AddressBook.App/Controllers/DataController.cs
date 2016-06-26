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
        public JsonResult Index()
        {
            List<ContactInformation> kontakti = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var kontakt = db.Contact.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
                foreach (var item in kontakt)
                {
                    k = new ContactInformation();
                    k = contactRepo.Set(item);
                    kontakti.Add(k);
                }
            }
            //var json = JsonConvert.SerializeObject(kontakti);
            return new JsonResult { Data = kontakti, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        /// <summary>
        /// POST metoda za nadograđivanje osnovnih informacija o kontaktu (ime, prezime, adresa, grad, OIB)
        /// </summary>
        /// <param name="contact">Kontakt koji se uređuje</param>
        [HttpPost]
        public void Update(ContactInformation contact)
        {
            using (var db = new AddressBookEntities())
            {
                var item = db.Contact.Find(contact.ID);
                item = contactRepo.SetBasicParams(item, contact);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// POST metoda koja ovisno o postojanosti broja / e-maila /taga obavlja radnju uređivanja ili dodavanja broja / e-maila / taga.
        /// </summary>
        /// <param name="ID">ID kontakta kojem treba spremiti informaciju</param>
        /// <param name="IDinfo">ID informacije iz relacije Addresses</param>
        /// <param name="text">Informacija koju treba spremiti</param>
        /// <param name="type">Vrsta informacije (u obliku integera slično kao u relaciji)</param>
        [HttpPost]
        public void AddUpdateEmailNumber(int ID, int IDinfo, string text, int type)
        {
            using (var db = new AddressBookEntities())
            {
                var contact = db.Contact.Find(ID);

                if (type == 3)
                {
                    contactRepo.AddTag(ID, text, CurrentUserID());
                    return;
                }

                var upit = contact.ContactInfo.Where(x => x.ID == IDinfo);

                if (!upit.Any())
                {
                    contactRepo.CreateAddress(ID, type, text.Trim());
                }

                else if (upit.Count() == 1)
                {
                    var infoUpd = db.ContactInfo.Find(IDinfo);
                    infoUpd.Info = text;
                    db.SaveChanges();
                }
            }
        }
    }
}