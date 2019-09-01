using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class Waiter
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsWorkingNow { get; set; }

        //определяет работает официант или нет,
        //так как при удалении официанта нельзя будет
        //получить доступ к его заказм
        public bool IsWork { get; set; }

        public ICollection<Shift> Shifts { get; set; }
        public ICollection<Order> Orders { get; set; }

        public Waiter()
        {
            Shifts = new HashSet<Shift>();
            Orders = new HashSet<Order>();
        }
    }
}