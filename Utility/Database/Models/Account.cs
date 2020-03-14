using System;
using System.Collections.Generic;

namespace Utility.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
