using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorkPassServer.Domain.Branches;
using NetWorkPassServer.Domain.Devices;
using SharedLibrary.Abstractions.Entity;
using SharedLibrary.Consts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Configurations;
internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");
        //Pk
        builder.HasKey(x => x.Id);
       
        
        // DeviceName (ValueObject)
        builder.OwnsOne(x => x.Name, fn =>
        {
            fn.Property(p => p.Value)
            .HasColumnType("nvarchar")
                .HasColumnName("Name")
                .HasMaxLength(EntityConsts.MaxNameLength)
                .IsRequired();
        });
      
        
        
        // IpAddress (ValueObject)
        builder.OwnsOne(x => x.IpAddress, fn => {
            fn.Property(p => p.Value)
            .HasColumnType(SqlDbType.NVarChar.ToString())

            .HasColumnName("IpAddress")
            .HasMaxLength(EntityConsts.MaxNameLength).IsRequired();
            fn.HasIndex(p => p.Value);

        });
       
        
        // Branch relation (FK)
        builder.Property(x => x.BranchId)      
               .IsRequired();
        builder.HasIndex(x => x.BranchId);
        //Enum
     
        
        builder.Property(x => x.Type)
              .HasConversion<string>() // və ya string
              .IsRequired();
        // Description
        builder.Property(x => x.Description)
               .HasMaxLength(EntityConsts.MaxDesrictionLength);
       

        // Audit (Entity-dən gəlir)
        builder.Property(x => x.CreationTime).IsRequired();

        builder.HasOne(x=>x.Branch)
       .WithMany(x=>x.Devices)
       .HasForeignKey(x => x.BranchId)
       .OnDelete(DeleteBehavior.Restrict);

     
    }
}
