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
                //var user = CurrentUserID();
                var kontakt = db.Contact.Where(x => x.IDuser == 1).OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
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
                item.IDuser = CurrentUserID();
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

        /// <summary>
        /// POST metoda za uklanjanje taga.
        /// </summary>
        /// <param name="idUser">ID kontakta kojem treba ukloniti tag</param>
        /// <param name="chosenTag">Tag koji je potrebno ukloniti</param>
        [HttpPost]
        public void RemoveTag(int idUser, string chosenTag)
        {
            using (var db = new AddressBookEntities())
            {
                var user = CurrentUserID();
                var tag = contactRepo.GetSingleTagInfo(chosenTag, user, db);
                var contact = contactRepo.GetContactInfoFromTag(tag, idUser);
                tag.Contact.Remove(contact);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// POST metoda koja uklanja broj ili e-mail prema ID-u informacije (autoinkrement primarnog ključa iz relacije Addresses)
        /// </summary>
        /// <param name="id">ID e-maila ili broja</param>
        [HttpPost]
        public void RemoveNumberEmail(int id)
        {
            using (var db = new AddressBookEntities())
            {
                var numb = db.ContactInfo.Find(id);
                db.ContactInfo.Remove(numb);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// GET metoda za dohvaćanje informacija o izabranom kontaktu (prozor za uređivanje podataka).
        /// </summary>
        /// <param name="id">ID kontakta</param>
        /// <returns>Informacije o kontaktu.</returns>
        public JsonResult GetContactById(string id)
        {
            int idContact = Convert.ToInt32(id);
            ContactInformation contact = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var item = db.Contact.Find(idContact);
                contact = contactRepo.Set(item);
            }

            return new JsonResult { Data = contact, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// POST metoda za brisanje odabranog kontakta.
        /// </summary>
        /// <param name="id">ID odabranog kontakta</param>
        [HttpPost]
        public void Delete(int id)
        {
            using (var db = new AddressBookEntities())
            {
                var contact = db.Contact.Find(id);
                db.Contact.Remove(contact);
                db.SaveChanges();
            }
        }

    }
}