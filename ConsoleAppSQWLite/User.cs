using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSQWLite
{
    public class User
    {
        [Column("user_id")]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
