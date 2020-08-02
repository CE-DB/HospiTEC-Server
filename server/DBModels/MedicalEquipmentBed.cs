using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalEquipmentBed
    {
        public int IdBed { get; set; }
        public string SerialNumber { get; set; }

        public  Bed IdBedNavigation { get; set; }
        public  MedicalEquipment SerialNumberNavigation { get; set; }
    }
}
