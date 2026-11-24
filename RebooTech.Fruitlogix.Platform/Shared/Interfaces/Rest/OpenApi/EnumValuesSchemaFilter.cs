using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RebooTech.Fruitlogix.Platform.Shared.Interfaces.Rest.OpenApi;

/// <summary>
///     Marca una propiedad <c>string</c> de un resource cuyos valores válidos son los nombres de un enum.
///     Swagger la publica con <c>enum</c> y la lista de valores en la descripción.
///     En records posicionales se usa como <c>[property: SwaggerEnumValues(typeof(X))]</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class SwaggerEnumValuesAttribute(Type enumType, params string[] exclude) : Attribute
{
    public Type EnumType { get; } = enumType.IsEnum
        ? enumType
        : throw new ArgumentException($"{enumType.Name} is not an enum.", nameof(enumType));

    /// <summary>Valores del enum que no se aceptan en este campo (p. ej. Admin en el sign-up).</summary>
    public string[] Exclude { get; } = exclude;

    public IReadOnlyList<string> Values => Enum.GetNames(EnumType).Except(Exclude).ToList();
}

public class EnumValuesSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var attribute = context.MemberInfo?.GetCustomAttribute<SwaggerEnumValuesAttribute>()
                        ?? context.ParameterInfo?.GetCustomAttribute<SwaggerEnumValuesAttribute>();
        if (attribute is null || schema is not OpenApiSchema openApiSchema) return;

        var values = attribute.Values;
        openApiSchema.Enum = values.Select(v => (JsonNode)JsonValue.Create(v)).ToList();

        var valuesText = $"Values: {string.Join(", ", values)}.";
        openApiSchema.Description = string.IsNullOrWhiteSpace(openApiSchema.Description)
            ? valuesText
            : $"{openApiSchema.Description} {valuesText}";
    }
}
