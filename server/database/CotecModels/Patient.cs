using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    /// <summary>
    /// This class maps the patient entitiy in the database of CoTEC-2020
    /// </summary>
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
