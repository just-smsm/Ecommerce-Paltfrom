
using Ecommerce_platforms.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Ecommerce_platforms.Repository.Data.Config
{
    internal class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(b => b.Name).IsRequired()
                .HasMaxLength(50);
           // builder.HasMany(b=>b.Products).WithOne(b=>b.Brand).HasForeignKey(b=>b.BrandId);
        }
    }
}
