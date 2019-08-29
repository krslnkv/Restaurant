using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Manager
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User {get; set;}
        public bool IsWorkingNow { get; set; }
        public ICollection<Shift> Shifts { get; set; }
        public Manager()
        {
            Shifts = new HashSet<Shift>();
        }
    }
}