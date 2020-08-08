using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    public partial class MedicalProcedures
    {
        public MedicalProcedures()
        {
            MedicalProcedureRecord = new HashSet<MedicalProcedureRecord>();
            MedicalProcedureReservation = new HashSet<MedicalProcedureReservation>();
        }

        public string Name { get; set; }
        public short RecoveringDays { get; set; }

        public virtual ICollection<MedicalProcedureRecord> MedicalProcedureRecord { get; set; }
        public virtual ICollection<MedicalProcedureReservation> MedicalProcedureReservation { get; set; }
    }
}
