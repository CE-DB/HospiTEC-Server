using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public partial class Bed
    {
        public Bed()
        {
            MedicalEquipmentBed = new HashSet<MedicalEquipmentBed>();
            ReservationBed = new HashSet<ReservationBed>();
        }

        public int IdBed { get; set; }
        public bool IsIcu { get; set; }
        public int IdRoom { get; set; }

        public virtual MedicalRoom IdRoomNavigation { get; set; }
        public virtual ICollection<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
        public virtual ICollection<ReservationBed> ReservationBed { get; set; }
    }
}
