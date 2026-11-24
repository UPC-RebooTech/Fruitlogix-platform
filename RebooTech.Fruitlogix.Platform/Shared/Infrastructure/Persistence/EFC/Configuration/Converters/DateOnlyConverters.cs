using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RebooTech.Fruitlogix.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Converters;

/// <summary>
///     El proveedor MySql.EntityFrameworkCore devuelve <see cref="DateTime" /> para columnas DATE,
///     y leerlas directo como <see cref="DateOnly" /> lanza InvalidCastException.
///     Se aplica a todas las propiedades DateOnly en <c>AppDbContext.ConfigureConventions</c>.
/// </summary>
public class DateOnlyToDateTimeConverter() : ValueConverter<DateOnly, DateTime>(
    d => d.ToDateTime(TimeOnly.MinValue),
    d => DateOnly.FromDateTime(d));

/// <inheritdoc cref="DateOnlyToDateTimeConverter" />
public class NullableDateOnlyToDateTimeConverter() : ValueConverter<DateOnly?, DateTime?>(
    d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null,
    d => d.HasValue ? DateOnly.FromDateTime(d.Value) : null);
