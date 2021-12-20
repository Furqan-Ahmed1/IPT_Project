using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Donation_Managment_System.Model
{
    public class DonorLogin
    {
        public string id { get; set; }
        [Required(ErrorMessage = "This Field is Required.")]
        public string First_name { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        public string Last_name { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        [RegularExpression(@"^\(?([0-9]{4})\)?[-. ]?([0-9]{7})$", ErrorMessage = "Not a valid phone number")]
        public string Phone_no { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        public string Occupation { get; set; }

        [Required(ErrorMessage = "This Field is Required.")]
        public string Cnic { get; set; }

        public List<DonorLogin> donor { get; set; }
        public List<DonorLogin> recipient { get; set; }
    }
}