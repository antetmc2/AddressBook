using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBook.DB;

namespace AddressBook.DAL
{
    public class ContactRepository
    {
        public Contact SetBasicParams(Contact newContact, ContactInformation existingContact)
        {
            newContact.FirstName = existingContact.FirstName;
            newContact.LastName = existingContact.LastName;
            newContact.Address = existingContact.Address;
            newContact.City = existingContact.City;
            newContact.OIB = existingContact.OIB;
            return newContact;
        }

        public void AddTag(int ID, string tagString, int userId)
        {
            using (var db = new AddressBookEntities())
            {
                var tags = db.Tag.Where(x => x.TagName == tagString.Trim().ToLower() && x.TagOwner == userId);
                if (!tags.Any())
                {
                    var tag = db.Tag.Create();
                    tag.TagName = tagString.Trim().ToLower();
                    tag.TagOwner = userId;
                    db.Tag.Add(tag);
                    db.SaveChanges();
                    tag = db.Tag.Find(tag.ID);
                    tag.Contact.Add(db.Contact.Find(ID));
                    db.SaveChanges();
                }
                else
                {
                    var chosenTag = db.Tag.Where(x => x.TagName == tagString.Trim().ToLower() && x.TagOwner == userId).SingleOrDefault();
                    if (!chosenTag.Contact.Where(x => x.ID == ID).Any())
                    {
                        chosenTag.Contact.Add(db.Contact.Find(ID));
                        db.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Metoda za stvaranje i dodavanje broja / e-maila za pojedinog kontakta.
        /// </summary>
        /// <param name="ID">ID kontakta kojem je potrebno dodati e-mail / broj.</param>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public void CreateAddress(int ID, int type, string info)
        {
            using (var db = new AddressBookEntities())
            {
                var newInfo = db.ContactInfo.Create();
                newInfo.IDcontact = ID;
                newInfo.IDtype = type;
                newInfo.Info = info;

                db.ContactInfo.Add(newInfo);
                db.SaveChanges();
            }
        }
    }
}
