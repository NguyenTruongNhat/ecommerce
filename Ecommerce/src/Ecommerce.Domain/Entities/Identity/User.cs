using Ecommerce.Contract.Enumerations;
using Ecommerce.Domain.Abstractions.Entities;

namespace Ecommerce.Domain.Entities.Identity;

public class User : DomainEntity<int>
{
    public User()
    {
        // Initialize collections to avoid NullReferenceException
        Devices = new HashSet<Device>();
        RefreshTokens = new HashSet<RefreshToken>();
        Carts = new HashSet<CartItem>();
        Reviews = new HashSet<Review>();
        SentMessages = new HashSet<Message>();
        ReceivedMessages = new HashSet<Message>();
    }

    // Public constructor used to create a new User in Domain Logic
    public User(
        string name,
        string email,
        string password,
        string phoneNumber,
        string avatar,
        string totpSecret,
        UserStatus status,
        int roleId) : this() // Call the default constructor to initialize Collections
    {
        // Assign required values when creating the Entity
        Name = name;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        Avatar = avatar;
        TotpSecret = totpSecret;
        Status = status;
        RoleId = roleId;
    }

    public User(
    string name,
    string email,
    string password,
    string phoneNumber,
    int roleId) : this() // Call the default constructor to initialize Collections
    {
        // Assign required values when creating the Entity
        Name = name;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        Avatar = string.Empty;
        TotpSecret = string.Empty;
        Status = UserStatus.ACTIVE;
        RoleId = roleId;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public string TotpSecret { get; set; }
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
    public virtual ICollection<Device> Devices { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<CartItem> Carts { get; set; }
    public virtual ICollection<Review> Reviews { get; set; }
    public virtual ICollection<Message> SentMessages { get; set; }
    public virtual ICollection<Message> ReceivedMessages { get; set; }

    public static User CreateUser(
    string name,
    string email,
    string hashedPassword,
    string phoneNumber,
    int roleId)
    {
        const string defaultAvatar = "";
        const string defaultTotpSecret = "";
        const UserStatus initialStatus = UserStatus.ACTIVE; 

        return new User(
            name,
            email,
            hashedPassword, 
            phoneNumber,
            defaultAvatar,
            defaultTotpSecret,
            initialStatus,
            roleId
        );
    }

}
