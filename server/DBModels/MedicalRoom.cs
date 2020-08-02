using System;
using System.Collections.Generic;

namespace HospiTec_Server.DBModels
{
    public  class MedicalRoom
    {
        public MedicalRoom()
        {
            Bed = new HashSet<Bed>();
        }

        public int IdRoom { get; set; }
        public short FloorNumber { get; set; }
        public string Name { get; set; }
        public short Capacity { get; set; }
        public string CareType { get; set; }

        public  ICollection<Bed> Bed { get; set; }
    }
}
