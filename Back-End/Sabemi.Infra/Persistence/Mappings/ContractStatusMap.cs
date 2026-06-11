using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sabemi.Domain.Entities;
namespace Sabemi.Infra.Persistence.Mappings;

internal class ContractStatusMap : IEntityTypeConfiguration<ContractStatus>
{
    public void Configure(EntityTypeBuilder<ContractStatus> builder)
    {
        builder.SetDefaultMap();

        builder
            .Property(_ => _.ContractId)
            .IsRequired();

        builder
            .Property(_ => _.Value)
            .IsRequired();

        builder
            .HasOne(_ => _.Contract)
            .WithOne(contract => contract.Status)
            .HasForeignKey<ContractStatus>(_ => _.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
