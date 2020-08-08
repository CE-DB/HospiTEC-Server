using HotChocolate;
using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    public partial class Reservation
    {
        public Reservation()
        {
            MedicalProcedureReservation = new HashSet<MedicalProcedureReservation>();
        }

        public string Identification { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int? IdBed { get; set; }
        public virtual Bed IdBedNavigation { get; set; }
        public virtual Patient IdentificationNavigation { get; set; }
        public virtual ICollection<MedicalProcedureReservation> MedicalProcedureReservation { get; set; }
    }
}
