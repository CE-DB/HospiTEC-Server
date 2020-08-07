using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class SanitaryMeasuresChanges
    {
        public string CountryName { get; set; }
        public string MeasureName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Country CountryNameNavigation { get; set; }
        public virtual SanitaryMeasure MeasureNameNavigation { get; set; }
    }
}
