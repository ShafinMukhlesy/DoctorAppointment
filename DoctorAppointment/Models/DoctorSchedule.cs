using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("DoctorSchedule")]
    public class DoctorSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        // Link to Doctor
        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        // Day of week
        [Required]
        public DayOfWeek DayOfWeek { get; set; } // Enum: Monday, Tuesday etc.

        // Time range
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Maximum patients in this slot
        public int MaxPatients { get; set; }

        // Slot duration in minutes (like 15 mins)
        public int SlotDuration { get; set; }
    }
}