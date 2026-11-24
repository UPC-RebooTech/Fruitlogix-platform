using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Commands;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;
using RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Resources;

// Added for ArgumentNullException

namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="SignUpResource" /> into a <see cref="SignUpCommand" />.
/// </summary>
public static class SignUpCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="SignUpResource" /> to a <see cref="SignUpCommand" />.
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="SignUpResource" /> containing the sign-up data. Must not be null.
    /// </param>
    /// <returns>
    ///     A new <see cref="SignUpCommand" /> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the input <paramref name="resource" /> is null.</exception>
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        if (resource == null)
            throw new ArgumentNullException(nameof(resource),
                "SignUpResource cannot be null when converting to command.");
        return new SignUpCommand(resource.Username, resource.Password,
            TryParseRole(resource.Role, out var role) ? role : UserRole.CommercialClient);
    }

    /// <summary>
    ///     Roles que se pueden elegir al registrarse. Admin queda fuera: solo se asigna en la base de datos.
    /// </summary>
    public static readonly UserRole[] SelfAssignableRoles =
        [UserRole.CommercialClient, UserRole.Producer, UserRole.Distributor];

    /// <summary>
    ///     true si <paramref name="value" /> es null/vacío (rol por defecto) o un rol permitido al registrarse.
    /// </summary>
    public static bool TryParseRole(string? value, out UserRole role)
    {
        role = UserRole.CommercialClient;
        if (string.IsNullOrWhiteSpace(value)) return true;
        return Enum.TryParse(value.Trim(), ignoreCase: true, out role)
               && !int.TryParse(value.Trim(), out _)
               && SelfAssignableRoles.Contains(role);
    }
}