using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class Person
    {
        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Province { get; set; }
        public string Canton { get; set; }
        public string ExactAddress { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
