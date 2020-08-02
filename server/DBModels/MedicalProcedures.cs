using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalProcedures
    {
        public MedicalProcedures()
        {
            MedicalProcedureRecord = new HashSet<MedicalProcedureRecord>();
            MedicalProcedureReservation = new HashSet<MedicalProcedureReservation>();
        }

        public string Name { get; set; }
        public short RecoveringDays { get; set; }

        public  ICollection<MedicalProcedureRecord> MedicalProcedureRecord { get; set; }
        public  ICollection<MedicalProcedureReservation> MedicalProcedureReservation { get; set; }
    }
}
