using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class MedicalReport
    {
        public int donor_id { get; set; }
        public int report_id { get; set; }
        [Required]
        public string Blood_Group { get; set; }

        public string Compatibility { get; set; }

        public List<MedicalReport> MR { get; set; }
    }
}