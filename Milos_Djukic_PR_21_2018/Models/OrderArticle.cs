using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Models
{
    public class OrderArticle
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
