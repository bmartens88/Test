using Microsoft.AspNetCore.Identity;

namespace Test.Idp.Data.Models;

internal sealed class ApplicationUser : IdentityUser
{
    public string FavoriteColor { get; set; } = string.Empty;
}