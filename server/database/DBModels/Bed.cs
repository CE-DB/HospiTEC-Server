using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    public partial class Bed
    {
        public Bed()
        {
            MedicalEquipmentBed = new HashSet<MedicalEquipmentBed>();
            Reservation = new HashSet<Reservation>();
        }

        public int IdBed { get; set; }
        public bool IsIcu { get; set; }
        public int? IdRoom { get; set; }

        public virtual MedicalRoom IdRoomNavigation { get; set; }
        public virtual ICollection<MedicalEquipmentBed> MedicalEquipmentBed { get; set; }
        public virtual ICollection<Reservation> Reservation { get; set; }
    }
}
