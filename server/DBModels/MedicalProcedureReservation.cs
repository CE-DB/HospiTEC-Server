using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalProcedureReservation
    {
        public string Identification { get; set; }
        public DateTime CheckInDate { get; set; }
        public string Name { get; set; }

        public  MedicalProcedures NameNavigation { get; set; }
        public  Reservation Reservation { get; set; }
    }
}
