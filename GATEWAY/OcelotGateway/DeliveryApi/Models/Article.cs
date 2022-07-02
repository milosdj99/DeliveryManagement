using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Models
{
    public class Article
    {
        public Guid Id;
        public string Name { get; set; }
        public int Price { get; set; }
        public string Ingredients { get; set; }
        public List<OrderArticle> OrderArticles { get; set; }
    }
}
