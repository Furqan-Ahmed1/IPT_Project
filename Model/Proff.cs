using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class Proff
    {
        public string Recipient_id { get; set; }
        public string Blood_Group { get; set; }
        public string Quantity { get; set; }
        public string Medical_report { get; set; }
        public string Conclusion_Rating { get; set; }
        public string Approval { get; set; }

        public List<Proff> proffs { get; set; }
    }
}