namespace RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Rol del usuario. Admin es interno: solo se asigna a mano en la base de datos.
/// </summary>
public enum UserRole
{
    CommercialClient,
    Producer,
    Distributor,
    Admin
}
