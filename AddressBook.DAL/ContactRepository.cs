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
    }
}
