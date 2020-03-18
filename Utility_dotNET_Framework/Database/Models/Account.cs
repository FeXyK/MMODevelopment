using System;
using System.Collections.Generic;

namespace Utility_dotNET_Framework.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool Admin { get; set; }

        }
}
