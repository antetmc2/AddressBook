//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AddressBook.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserContactTag
    {
        public int IDuser { get; set; }
        public int IDcontact { get; set; }
        public int IDtag { get; set; }
    
        public virtual Contact Contact { get; set; }
        public virtual Tag Tag { get; set; }
        public virtual User User { get; set; }
    }
}
