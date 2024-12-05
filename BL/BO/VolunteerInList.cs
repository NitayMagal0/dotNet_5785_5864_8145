using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class VolunteerInList
    {
        public int Id { get; init; }
        public string? FullName { get; set; }
        public bool isActive { get; set; }
        public int CallsHandled { get; set; }
        public int CallsCanceled { get; set; }
        public int CallsExpired { get; set; }
        public int CallsInProgress { get; set; }
        public double? Longitude { get; set; }
    }
}
