using System;
using System.Collections.Generic;

namespace HospiTec_Server.database.DBModels
{
    /// <summary>
    /// This class maps the entity Medical room
    /// </summary>
    public partial class MedicalRoom
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

        public virtual ICollection<Bed> Bed { get; set; }
    }
}
