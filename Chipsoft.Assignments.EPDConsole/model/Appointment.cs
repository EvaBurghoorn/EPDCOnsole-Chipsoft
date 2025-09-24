using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipsoft.Assignments.EPDConsole.model
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int AppointmentId { get; set; }
        public required DateTime Date { get; set; }

        public required int PatientId { get; set; }
        public required int PhysicianId { get; set; }


        public  Patient? Patient { get; set; }
        public  Physician? Physician { get; set; }

       
    }
}
