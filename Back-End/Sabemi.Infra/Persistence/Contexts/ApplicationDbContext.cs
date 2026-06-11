using Microsoft.EntityFrameworkCore;
using Sabemi.Domain.Entities;


namespace Sabemi.Infra.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractStatus> ContractStatuses => Set<ContractStatus>();
    public DbSet<ContractPayment> ContractPayments => Set<ContractPayment>();
    public DbSet<PaymentWebhookEvent> PaymentWebhookEvents => Set<PaymentWebhookEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
