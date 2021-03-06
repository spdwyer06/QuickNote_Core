using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
                DateCreated = DateTime.Now
            };

            var passwordHasher = new PasswordHasher<UserEntity>();
            
            // Encrypts the password from the model and sets it as the password on the entity
            entity.Password = passwordHasher.HashPassword(entity, model.Password);

            _db.Users.Add(entity);
            var numOfChanges = await _db.SaveChangesAsync();

            return numOfChanges == 1;
        }

        public async Task<UserDetail> GetUserByIdAsync(int userId)
        {
            var entity = await _db.Users.FindAsync(userId);

            if(entity is null)
                return null;

            return new UserDetail
            {
                Id = entity.Id,
                Username = entity.Username,
                Email = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DateCreated = entity.DateCreated
            };
        }



        // FirstOrDefaultAsync() will return either a User if there is a match or null, if null then the email is not in use
        private async Task<UserEntity> GetUserByEmailAsync(string email) => await _db.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());

        // FirstOrDefaultAsync() will return null if no object is found, so we can check that to see whether or not the given username is already in use
        private async Task<UserEntity> GetUserByUsernameAsync(string username) => await _db.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }
}