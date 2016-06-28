using AddressBook.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressBook.DAL
{
    public class UserRepository
    {
        public bool UserExists(string username, string password, AddressBookEntities db)
        {
            return db.User.Any(user => user.Username == username && user.Password == password);
        }
    }
}
