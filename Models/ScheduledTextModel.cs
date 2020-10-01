using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageScheduler.Models
{
    public class ScheduledTextModel
    {
        public int Id { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public TimeSpan UtcTime { get; set; }
    }
}
