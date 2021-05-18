using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Queue.Models
{
    public class QRCode
    {
        [Display(Name = "QRCode Text")]
        public string QRCodeText { get; set; }
        [Display(Name = "QRCode Image")]
        public string QRCodeImagePath { get; set; }
    }

    public class User
    {
        public string NameMain { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Validated { get; set; }
        public bool Registered { get; set; }
        public string GuestName { get; set; }
        public DateTime DateRegistered { get; set; }
        public DateTime DateEntered { get; set; }
    }
}