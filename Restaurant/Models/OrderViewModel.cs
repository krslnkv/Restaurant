namespace Restaurant.Models
{
    public class NewOrderModel
    {
        public string GuestEmail { get; set; }
        public string GuestName { get; set; }
        public int TableId { get; set; }
        public int WaiterId { get; set; }

    }
}