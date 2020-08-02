using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalProcedureRecord
    {
        public string Identification { get; set; }
        public string PathologyName { get; set; }
        public DateTime DiagnosticDate { get; set; }
        public string ProcedureName { get; set; }
        public DateTime OperationExecutionDate { get; set; }

        public  ClinicRecord ClinicRecord { get; set; }
        public  MedicalProcedures ProcedureNameNavigation { get; set; }
    }
}
