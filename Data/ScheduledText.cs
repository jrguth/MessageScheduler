using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageScheduler.Data
{
    public class ScheduledText
    {
        public int Id { get; set; }
        public DateTime DateUTC { get; set; }
        public string PhoneNumber { get; set; }
    }
}
