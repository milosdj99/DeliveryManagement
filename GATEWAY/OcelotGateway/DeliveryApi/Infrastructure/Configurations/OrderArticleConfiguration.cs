using DeliveryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Infrastructure.Configurations
{
    public class OrderArticleConfiguration : IEntityTypeConfiguration<OrderArticle>
    {
        public void Configure(EntityTypeBuilder<OrderArticle> builder)
        {
            builder.HasKey(sc => sc.Id);

            builder.HasOne<Article>(sc => sc.Article)
            .WithMany(s => s.OrderArticles)
            .HasForeignKey(sc => sc.ArticleId);

            builder.HasOne<Order>(sc => sc.Order)
            .WithMany(s => s.OrderArticles)
            .HasForeignKey(sc => sc.OrderId);


        }
    }
}
