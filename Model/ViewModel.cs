using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class ViewModel
    {
        public MedicalReport medicalReport { get; set; }

        public History history { get; set; }
        public Inventory inventory { get; set; }
    }
}