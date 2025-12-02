using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Contract.Enumerations;
public enum RoleType
{
    [Display(Name = "Admin")]
    Admin = 1,

    [Display(Name = "Client")]
    Client = 2,

    [Display(Name = "Seller")]
    Seller = 3
}
