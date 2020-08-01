using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class ReservationBed
    {
        public string Identification { get; set; }
        public int IdBed { get; set; }
        public DateTime CheckInDate { get; set; }

        public virtual Bed IdBedNavigation { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
