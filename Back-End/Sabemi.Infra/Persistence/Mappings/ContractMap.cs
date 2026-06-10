using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sabemi.Domain.Common;
using Sabemi.Domain.Entities;
namespace Sabemi.Infra.Persistence.Mappings;

internal class ContractMap : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.SetDefaultMap();

        builder
            .Property(_ => _.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(_ => _.TotalAmount)
            .IsRequired()
            .HasColumnType(Constants.DEFAULT_DECIMAL_COLUMN_TYPE);

        builder
            .Property(_ => _.PaidAmount)
            .IsRequired()
            .HasColumnType(Constants.DEFAULT_DECIMAL_COLUMN_TYPE); 

        builder
            .Ignore(_ => _.PendingAmount);

        builder
            .Property(_ => _.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder
            .Property(_ => _.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
