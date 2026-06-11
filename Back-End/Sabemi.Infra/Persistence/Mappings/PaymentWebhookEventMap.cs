using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sabemi.Domain.Entities;

namespace Sabemi.Infra.Persistence.Mappings;

internal class PaymentWebhookEventMap : IEntityTypeConfiguration<PaymentWebhookEvent>
{
    public void Configure(EntityTypeBuilder<PaymentWebhookEvent> builder)
    {
        builder.SetDefaultMap();

        builder
            .Property(_ => _.ContractId)
            .IsRequired();

        builder
            .Property(_ => _.TransactionId)
            .IsRequired();

        builder
            .Property(_ => _.Payload)
            .IsRequired();

        builder
            .Property(_ => _.Status)
            .IsRequired();

        builder
            .Property(_ => _.RetryCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(_ => _.ErrorMessage)
            .HasMaxLength(1_000);

        builder
            .Property(_ => _.ProcessedAt)
            .IsRequired(false);

        builder
            .Property(_ => _.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder
            .HasOne(_ => _.Contract)
            .WithMany(contract => contract.WebhookEvents)
            .HasForeignKey(_ => _.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
