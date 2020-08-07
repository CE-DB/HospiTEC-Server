using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Contact
    {
        public Contact()
        {
            ContactPathology = new HashSet<ContactPathology>();
            Hospital = new HashSet<Hospital>();
            PatientContact = new HashSet<PatientContact>();
        }

        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Region { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public byte? Age { get; set; }

        public virtual Region RegionNavigation { get; set; }
        public virtual ICollection<ContactPathology> ContactPathology { get; set; }
        public virtual ICollection<Hospital> Hospital { get; set; }
        public virtual ICollection<PatientContact> PatientContact { get; set; }
    }
}
