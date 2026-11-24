namespace RebooTech.Fruitlogix.Platform.Iam.Interfaces.Rest.Resources;

/// <param name="Role">Opcional: CommercialClient (por defecto), Producer o Distributor.</param>
public record SignUpResource(string Username, string Password, string? Role = null);