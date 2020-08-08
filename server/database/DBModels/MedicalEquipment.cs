using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity Medical equipment
    /// </summary>
    public partial class MedicalEquipment
    {
        public MedicalEquipment()
        {
            MedicalEquipmentBed = new HashSet<MedicalEquipmentBed>();
        }

        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public string Provider { get; set; }

        public virtual ICollection<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
    }
}
