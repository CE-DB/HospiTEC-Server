using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class MedicalEquipmentBed
    {
        public int IdBed { get; set; }
        public string Name { get; set; }

        public virtual Bed IdBedNavigation { get; set; }
        public virtual MedicalEquipment NameNavigation { get; set; }
    }
}
