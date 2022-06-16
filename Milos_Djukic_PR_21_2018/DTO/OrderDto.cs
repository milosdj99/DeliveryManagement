using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.DTO
{
    public class OrderDto
    {
        public string Id { get; set; }
        public List<ArticleDTO> Articles { get; set; }
        public Guid CustomerId { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public int Price { get; set; }
        public DateTime Time { get; set; }

        public bool Accepted { get; set; }
    }
}
