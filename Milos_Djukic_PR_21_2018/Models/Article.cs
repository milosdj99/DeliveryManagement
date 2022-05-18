using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Models
{
    public class Article
    {
        public Guid Id;
        public string Name { get; set; }
        public int Price { get; set; }
        public string Ingredients { get; set; }
    }
}
