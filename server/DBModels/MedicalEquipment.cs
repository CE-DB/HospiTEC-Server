using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalEquipment
    {
        public MedicalEquipment()
        {
            MedicalEquipmentBed = new HashSet<MedicalEquipmentBed>();
        }

        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public string Provider { get; set; }

        public  ICollection<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
    }
}
