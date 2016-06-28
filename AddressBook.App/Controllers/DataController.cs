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
        UserController userController = new UserController();
        // GET: Data
        public JsonResult Index()
        {
            List<ContactInformation> contacts = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var contactsFromDB = db.Contact.Where(x => x.IDuser == user).OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
                foreach (var item in contactsFromDB)
                {
                    k = new ContactInformation();
                    k = contactRepo.Set(item);
                    contacts.Add(k);
                }
            }
            //var json = JsonConvert.SerializeObject(kontakti);
            return new JsonResult { Data = contacts, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SearchByFirstName(string term = "")
        {
            List<ContactInformation> contacts = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var searchResults = db.Contact.Where(x => x.FirstName.ToLower().Contains(term.ToLower()) && x.IDuser == user);

                foreach(var cont in searchResults)
                {
                    k = contactRepo.Set(cont);
                    contacts.Add(k);
                }
            }
            return new JsonResult { Data = contacts, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SearchByLastName(string term = "")
        {
            List<ContactInformation> contacts = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var searchResults = db.Contact.Where(x => x.LastName.ToLower().Contains(term.ToLower()) && x.IDuser == user);

                foreach (var cont in searchResults)
                {
                    k = contactRepo.Set(cont);
                    contacts.Add(k);
                }
            }
            return new JsonResult { Data = contacts, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SearchByTag(string term = "")
        {
            List<ContactInformation> contacts = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();
            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var tags = db.Tag.Where(x => x.TagName.ToLower().Contains(term.ToLower()) && x.TagOwner == user);

                foreach(var tag in tags)
                {
                    var searchResults = tag.Contact;
                    foreach (var cont in searchResults)
                    {
                        k = contactRepo.Set(cont);
                        contacts.Add(k);
                    }
                }
            }
            return new JsonResult { Data = contacts, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [HttpPost]
        public void Create(ContactInformation contact)
        {
            using(AddressBookEntities db = new AddressBookEntities())
            {
                var item = db.Contact.Create();
                item = contactRepo.SetBasicParams(item, contact);
                item.IDuser = userController.CurrentUserID();
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
                            contactRepo.AddTag(item.ID, tag, userController.CurrentUserID());
                        }
                    }
                    catch (NullReferenceException exc) { }
                }
            }
        }

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

        [HttpPost]
        public void AddUpdateEmailNumber(int ID, int IDinfo, string text, int type)
        {
            using (var db = new AddressBookEntities())
            {
                var contact = db.Contact.Find(ID);

                if (type == 3)
                {
                    contactRepo.AddTag(ID, text, userController.CurrentUserID());
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

        [HttpPost]
        public void RemoveTag(int idUser, string chosenTag)
        {
            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var tag = contactRepo.GetSingleTagInfo(chosenTag, user, db);
                var contact = contactRepo.GetContactInfoFromTag(tag, idUser);
                tag.Contact.Remove(contact);
                db.SaveChanges();
            }
        }

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

        public JsonResult GetUsedTags()
        {
            using(var db = new AddressBookEntities())
            {
                List<Tags> tagsList = new List<Tags>();
                var user = userController.CurrentUserID();
                var tags = db.Tag.Where(x => x.TagOwner == user).OrderBy(x => x.TagName);
                foreach(var tag in tags)
                {
                    if (tag.Contact.Count() > 0) tagsList.Add(new Tags { ID = tag.ID, TagName = tag.TagName });
                }
                return new JsonResult { Data = tagsList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            }
        }

        public JsonResult Filter(int tagID)
        {
            List<ContactInformation> contacts = new List<ContactInformation>();
            ContactInformation k = new ContactInformation();

            using (var db = new AddressBookEntities())
            {
                var user = userController.CurrentUserID();
                var tag = db.Tag.Where(x => x.ID == tagID).SingleOrDefault();

                foreach (var item in tag.Contact)
                {
                    k = new ContactInformation();
                    k = contactRepo.Set(item);
                    contacts.Add(k);
                }

                return new JsonResult { Data = contacts, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
    }
}