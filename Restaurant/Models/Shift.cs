using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Shift
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ManagerId { get; set; }
        public Manager Manager { get; set; }
        public int WaiterId { get; set; }
        public Waiter Waiter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public bool IsClosed { get; set; }
    }
}