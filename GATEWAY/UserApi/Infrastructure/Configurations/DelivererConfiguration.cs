using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Models;

namespace UserApi.Infrastructure.Configurations
{
    public class DelivererConfiguration : IEntityTypeConfiguration<Deliverer>
    {
        public void Configure(EntityTypeBuilder<Deliverer> builder)
        {
            builder.HasKey(x => x.Id);

        }

    }
}
