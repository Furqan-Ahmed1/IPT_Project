using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class Blocked
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string Cnic { get; set; }

        public List<Blocked> blocked { get; set; }
    }
}