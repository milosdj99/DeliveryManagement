﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Infrastructure.Configurations
{
    public class DelivererConfiguration : IEntityTypeConfiguration<Deliverer>
    {
        public void Configure(EntityTypeBuilder<Deliverer> builder)
        {
            builder.HasKey(x => x.Id);

        }

    }
}