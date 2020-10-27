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
        public DateTime DateUTC { get; set; }

        [Required]
        public string Email { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
