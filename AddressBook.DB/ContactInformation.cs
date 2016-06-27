using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBook.DB
{
    public class Number
    {
        public int? ID { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class Email
    {
        public int? ID { get; set; }
        public string EmailAddress { get; set; }
    }

    public class Tags
    {
        public int? ID { get; set; }
        public string TagName { get; set; }
    }

    /// <summary>
    /// Posebna klasa kreirana za podatke o kontaktu koja sadrži podatke iz svih drugih relacija i omogućuje lakše baratanje podatcima.
    /// </summary>
    public class ContactInformation
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string OIB { get; set; }
        public List<Number> Numbers { get; set; }
        public List<Email> Emails { get; set; }
        public List<string> Tags { get; set; }
    }
}
