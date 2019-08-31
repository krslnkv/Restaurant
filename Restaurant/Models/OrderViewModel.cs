using System.Collections.Generic;

namespace Restaurant.Models
{
    public class NewOrderModel
    {
        public List<OrderDetail> OrderDetails { get; set; }
        public Order Order { get; set; }

    }
}