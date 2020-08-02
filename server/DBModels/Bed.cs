using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class Bed
    {
        public Bed()
        {
            MedicalEquipmentBed = new HashSet<MedicalEquipmentBed>();
            ReservationBed = new HashSet<ReservationBed>();
        }

        public int IdBed { get; set; }
        public bool IsIcu { get; set; }
        public int IdRoom { get; set; }

        public  MedicalRoom IdRoomNavigation { get; set; }
        public  ICollection<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
        public  ICollection<ReservationBed> ReservationBed { get; set; }
    }
}
