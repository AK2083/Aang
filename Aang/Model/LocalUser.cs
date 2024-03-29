﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Aang.Model
{
    [Table("User")]
    public class LocalUser
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
