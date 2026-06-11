using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sabemi.Domain.Common;
using Sabemi.Domain.Entities;
namespace Sabemi.Infra.Persistence.Mappings;

internal class ContractPaymentMap : IEntityTypeConfiguration<ContractPayment>
{
    public void Configure(EntityTypeBuilder<ContractPayment> builder)
    {
        builder.SetDefaultMap();

        builder
            .Property(_ => _.ContractId)
            .IsRequired();

        builder
            .Property(_ => _.TransactionId)
            .IsRequired();      

        builder
            .Property(_ => _.Amount)
            .IsRequired()
            .HasColumnType(Constants.DEFAULT_DECIMAL_COLUMN_TYPE);

        builder
            .Property(_ => _.PaidAt)
            .IsRequired();  

        builder
            .Property(_ => _.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder
            .HasOne(_ => _.Contract)
            .WithMany(contract => contract.Payments)
            .HasForeignKey(_ => _.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
