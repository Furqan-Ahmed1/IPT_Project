using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class Inventory
    {
        public int blood_id { get; set; }
        public int donor_id { get; set; }
        public int report_id { get; set; }
        public int quantity { get; set; }

        public List<Inventory> Inv { get; set; }
    }
}