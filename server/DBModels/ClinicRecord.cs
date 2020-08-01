using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class ClinicRecord
    {
        public ClinicRecord()
        {
            MedicalProcedureRecord = new HashSet<MedicalProcedureRecord>();
        }

        public string Identification { get; set; }
        public string PathologyName { get; set; }
        public DateTime DiagnosticDate { get; set; }
        public string Treatment { get; set; }

        public virtual Patient IdentificationNavigation { get; set; }
        public virtual ICollection<MedicalProcedureRecord> MedicalProcedureRecord { get; set; }
    }
}
