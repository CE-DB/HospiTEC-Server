using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class Region
    {
        public Region()
        {
            Contact = new HashSet<Contact>();
            Hospital = new HashSet<Hospital>();
            Patient = new HashSet<Patient>();
        }

        public string Name { get; set; }
        public string Country { get; set; }

        public virtual Country CountryNavigation { get; set; }
        public virtual ICollection<Contact> Contact { get; set; }
        public virtual ICollection<Hospital> Hospital { get; set; }
        public virtual ICollection<Patient> Patient { get; set; }
    }
}
