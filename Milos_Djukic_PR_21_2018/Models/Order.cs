﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Models
{
    public class Order
    {
        public Guid Id;
        public List<OrderArticle> OrderArticles { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid DelivererId { get; set; }
        public Deliverer Deliverer { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public int Price { get; set; }
        public DateTime Time { get; set; }
        public bool Accepted { get; set; }
    }
}
