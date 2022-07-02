using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Models
{
    public class Order
    {
        public Guid Id;
        public List<OrderArticle> OrderArticles { get; set; }
        public Guid CustomerId { get; set; }
        public Guid DelivererId { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public int Price { get; set; }
        public DateTime Time { get; set; }
        public bool Accepted { get; set; }
    }
}
