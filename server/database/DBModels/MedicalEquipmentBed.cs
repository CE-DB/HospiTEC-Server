using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity Medical equipment Bed
    /// </summary>
    public partial class MedicalEquipmentBed
    {
        public int IdBed { get; set; }
        public string SerialNumber { get; set; }

        public virtual Bed IdBedNavigation { get; set; }
        public virtual MedicalEquipment SerialNumberNavigation { get; set; }
    }
}
