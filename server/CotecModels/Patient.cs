using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public class Patient
    {
        public Patient()
        {
        }

        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }
}
