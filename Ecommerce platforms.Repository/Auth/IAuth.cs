using Ecommerce_platforms.Repository.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Repository.Auth
{
    public interface IAuth
    {
        Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager);
    }
}
