using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuickNote_Data;
using QuickNote_Data.Entities;
using QuickNote_Models.User;

namespace QuickNote_Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RegisterUserAsync(UserRegister model)
        {
            // If the email or username is already in use return false
            if(await GetUserByEmailAsync(model.Email) != null || await GetUserByUsernameAsync(model.Username) != null)
                return false;

            var entity = new UserEntity
            {
                Email = model.Email,
                Username = model.Username,
                Password = model.Password,
                DateCreated = DateTime.Now
            };

            _db.Users.Add(entity);
            var numOfChanges = await _db.SaveChangesAsync();

            return numOfChanges == 1;
        }



        // FirstOrDefaultAsync() will return either a User if there is a match or null, if null then the email is not in use
        private async Task<UserEntity> GetUserByEmailAsync(string email) => await _db.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());

        // FirstOrDefaultAsync() will return null if no object is found, so we can check that to see whether or not the given username is already in use
        private async Task<UserEntity> GetUserByUsernameAsync(string username) => await _db.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }
}