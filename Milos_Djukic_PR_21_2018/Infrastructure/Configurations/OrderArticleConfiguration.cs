using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Infrastructure.Configurations
{
    
        public class OrderArticleConfiguration : IEntityTypeConfiguration<OrderArticle>
        {
            

            public void Configure(EntityTypeBuilder<OrderArticle> builder)
            {
                builder.HasKey(sc => new { sc.OrderId, sc.ArticleId });

                builder.HasOne<Article>(sc => sc.Article)
                .WithMany(s => s.OrderArticles)
                .HasForeignKey(sc => sc.ArticleId);

                builder.HasOne<Order>(sc => sc.Order)
                .WithMany(s => s.OrderArticles)
                .HasForeignKey(sc => sc.OrderId);


        }
        }
    
}
