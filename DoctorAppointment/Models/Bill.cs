using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoctorAppointment.Models
{
    [Table("Bill")]
    public class Bill
    {
        [Key]
        public int BillId { get; set; }

        // Foreign Keys
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorId { get; set; }

        // Bill Info
        [DataType(DataType.Date)]
        public DateTime BillDate { get; set; } = DateTime.Now;

        public string ConsultationFee { get; set; }

        public string OtherCharges { get; set; }

        public string Discount { get; set; }
   
        public string TotalAmount { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid"; // Paid, Unpaid, Partial

        [StringLength(50)]
        public string PaymentMethod { get; set; } // Cash, Card, Mobile Banking

        // Navigation Properties
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
    }
}