using DeliveryApi.Infrastructure.Configurations;
using DeliveryApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Infrastructure
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions options) : base(options)
        {
        }

        
        public DbSet<Order> Orders { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<OrderArticle> OrderArticles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ArticleConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderArticleConfiguration());
        }
    }
}
