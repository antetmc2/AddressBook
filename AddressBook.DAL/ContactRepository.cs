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

        public ContactInformation Set(Contact item)
        {
            ContactInformation k = new ContactInformation();

            k.Numbers = new List<Number>();
            k.Emails = new List<Email>();
            k.Tags = new List<string>();
            k.ID = item.ID;
            k.FirstName = item.FirstName;
            k.LastName = item.LastName;
            k.Address = item.Address;
            k.City = item.City;
            k.OIB = item.OIB;
            foreach (var add in item.ContactInfo)
            {
                if (add.IDtype == 1) k.Numbers.Add(new Number { ID = add.ID, PhoneNumber = add.Info });
                else
                    k.Emails.Add(new Email { ID = add.ID, EmailAddress = add.Info });
            }
            foreach (var tag in item.Tag) k.Tags.Add(tag.TagName);

            k.Emails = k.Emails.OrderBy(x => x.EmailAddress).ToList();
            k.Numbers = k.Numbers.OrderBy(x => x.PhoneNumber).ToList(); ;
            k.Tags.Sort();

            return k;
        }

        public Tag GetSingleTagInfo(string chosenTag, int id, AddressBookEntities db)
        {
            return db.Tag.Where(x => x.TagName == chosenTag.Trim().ToLower() && x.TagOwner == id).SingleOrDefault();
        }

        public Contact GetContactInfoFromTag(Tag tag, int id)
        {
            return tag.Contact.Where(x => x.ID == id).SingleOrDefault();
        }

        public void UpdateEmailNumbers(int IDinfo, string text, AddressBookEntities db)
        {
            var infoUpd = db.ContactInfo.Find(IDinfo);
            infoUpd.Info = text;
            db.SaveChanges();
        }

    }
}
