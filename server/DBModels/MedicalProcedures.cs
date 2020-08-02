using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalProcedures
    {
        public MedicalProcedures()
        {
            MedicalProcedureRecord = new HashSet<MedicalProcedureRecord>();
        }

        public string Name { get; set; }
        public short RecoveringDays { get; set; }

        public  ICollection<MedicalProcedureRecord> MedicalProcedureRecord { get; set; }
    }
}
