using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class ReservationBed
    {
        public string Identification { get; set; }
        public int IdBed { get; set; }
        public DateTime CheckInDate { get; set; }

        public  Bed IdBedNavigation { get; set; }
        public  Reservation Reservation { get; set; }
    }
}
