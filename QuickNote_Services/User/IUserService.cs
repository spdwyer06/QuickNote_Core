using System.Threading.Tasks;
using QuickNote_Models.User;

namespace QuickNote_Services.User
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserRegister model);

        Task<UserDetail> GetUserByIdAsync(int userId);
    }
}