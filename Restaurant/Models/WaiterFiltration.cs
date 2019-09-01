using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant.Models
{
    public class WaiterFiltration
    {
        public int IsWork { get; set; }
        public int IsWorkingNow { get; set; }
        public string WaiterNameAndLastName { get; set; }
    }
}