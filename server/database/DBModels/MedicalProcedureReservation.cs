using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity Medical procedure reservation
    /// </summary>
    public partial class MedicalProcedureReservation
    {
        public string Identification { get; set; }
        public DateTime CheckInDate { get; set; }
        public string Name { get; set; }

        public virtual MedicalProcedures NameNavigation { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
