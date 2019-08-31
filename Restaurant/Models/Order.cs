using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public Shift Shift { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
        public DateTime OrderTime { get; set; }
        public int WaiterId { get; set; }
        public Waiter Waiter { get; set; }
        public int FinalPrice { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
    }
}