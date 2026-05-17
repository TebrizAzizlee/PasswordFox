using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorkPassServer.Domain.Branches;
using SharedLibrary.Consts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorkPassServer.Infrastructure.Configurations;
internal class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        // builder.ToTable("Branches");
       
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Name, fn =>
        {
            fn.Property(p => p.Value)
                .HasColumnName("Name")
                .HasMaxLength(EntityConsts.MaxNameLength)
                .IsRequired();
            fn.HasIndex(p => p.Value).IsUnique();
        });
        

        //Address
        builder.OwnsOne(x => x.Address, fn =>
        {
            fn.Property(p => p.City)
                .HasColumnName("City")
                .HasMaxLength(EntityConsts.MaxNameLength)
                .IsRequired();
            fn.Property(p => p.District)

               .HasColumnName("District")
               .HasMaxLength(EntityConsts.MaxNameLength)
               .IsRequired();

            fn.Property(p => p.FullAddress)
              .HasColumnName("FullAddress")
              .HasMaxLength(EntityConsts.MaxFullNameLength)
              .IsRequired();

         
           
        });

        //ContactInfo
        builder.OwnsOne(x => x.ContactInfo, fn =>
        {
            fn.Property(p => p.PhoneNumber1)
            .HasColumnName("PhoneNumber1")
              .HasMaxLength(20)
              .IsRequired();

            fn.Property(p => p.PhoneNumber2)
            .HasColumnName("PhoneNumber2")
                .HasMaxLength(20);

            fn.Property(p => p.Email)
               .HasColumnName("Email")
               .HasMaxLength(EntityConsts.MaxEmailLength)
               .IsRequired();

        });

        //NetworkInfo
        builder.OwnsOne(x => x.NetworkInfo, fn =>
        {
            fn.Property(p => p.WanIp)
                .HasColumnName("WanIp")
                .HasMaxLength(EntityConsts.MaxNameLength)
                .IsRequired();
            fn.Property(p => p.Gateway)

               .HasColumnName("Gateway")
               .HasMaxLength(EntityConsts.MaxNameLength)
               .IsRequired();

            fn.Property(p => p.Subnet)
              .HasColumnName("Subnet")
              .HasMaxLength(EntityConsts.MaxFullNameLength)
              .IsRequired();

            fn.Property(p => p.DnsServer)
               .HasColumnName("DnsServer")
               .HasMaxLength(EntityConsts.MaxFullNameLength)
               .IsRequired();

        });
    }
}
