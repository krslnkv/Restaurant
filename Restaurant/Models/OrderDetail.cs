﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Models
{
    public class OrderDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        public DateTime OrderTime { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}