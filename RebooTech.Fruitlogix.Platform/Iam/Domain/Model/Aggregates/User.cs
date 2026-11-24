using System.Text.Json.Serialization;
using RebooTech.Fruitlogix.Platform.Iam.Domain.Model.ValueObjects;

namespace RebooTech.Fruitlogix.Platform.Iam.Domain.Model.Aggregates;

/**
 * <summary>
 *     The user aggregate
 * </summary>
 * <remarks>
 *     This class is used to represent a user
 * </remarks>
 */
public partial class User(string username, string passwordHash)
{
    public User() : this(string.Empty, string.Empty)
    {
    }

    public User(string username, string passwordHash, UserRole role) : this(username, passwordHash)
    {
        Role = role;
    }

    public int Id { get; }
    public string Username { get; private set; } = username;

    public UserRole Role { get; private set; } = UserRole.CommercialClient;

    [JsonIgnore] public string PasswordHash { get; private set; } = passwordHash;

    /**
     * <summary>
     *     Update the username
     * </summary>
     * <param name="username">The new username</param>
     * <returns>The updated user</returns>
     */
    public User UpdateUsername(string username)
    {
        Username = username;
        return this;
    }

    /**
     * <summary>
     *     Update the password hash
     * </summary>
     * <param name="passwordHash">The new password hash</param>
     * <returns>The updated user</returns>
     */
    public User UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }
}