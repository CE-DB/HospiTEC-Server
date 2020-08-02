using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class Reservation
    {
        public Reservation()
        {
            ReservationBed = new HashSet<ReservationBed>();
        }

        public string Identification { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public  Patient IdentificationNavigation { get; set; }
        public  ICollection<ReservationBed> ReservationBed { get; set; }
    }
}
