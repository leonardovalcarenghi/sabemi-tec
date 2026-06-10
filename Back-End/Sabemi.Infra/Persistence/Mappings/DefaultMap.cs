using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sabemi.Domain.Interfaces;

namespace Sabemi.Infra.Persistence.Mappings;

public static class DefaultMap
{
    public static void SetDefaultMap<T>(this EntityTypeBuilder<T> builder) where T : class, IEntity
    {
        Type entityType = typeof(T);
        builder.ToTable(entityType.Name);

        builder
            .HasKey(_ => _.Id);

        builder
            .Property(_ => _.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

    }
}
