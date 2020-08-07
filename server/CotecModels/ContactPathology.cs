using System;
using System.Collections.Generic;

namespace HospiTec_Server.CotecModels
{
    public partial class ContactPathology
    {
        public string PathologyName { get; set; }
        public string ContactId { get; set; }

        public virtual Contact Contact { get; set; }
        public virtual Pathology PathologyNameNavigation { get; set; }
    }
}
