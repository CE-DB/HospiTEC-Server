using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity Medical procedure record.
    /// </summary>
    public partial class MedicalProcedureRecord
    {
        public string Identification { get; set; }
        public string PathologyName { get; set; }
        public DateTime DiagnosticDate { get; set; }
        public string ProcedureName { get; set; }
        public DateTime OperationExecutionDate { get; set; }

        public virtual ClinicRecord ClinicRecord { get; set; }
        public virtual MedicalProcedures ProcedureNameNavigation { get; set; }
    }
}
