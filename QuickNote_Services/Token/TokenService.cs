using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuickNote_Data;
using QuickNote_Data.Entities;
using QuickNote_Models.Token;

namespace QuickNote_Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _db;

        public TokenService(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<TokenResponse> GetTokenAsync(TokenRequest model)
        {
            var userEntity = await GetValidUserAsync(model);

            if(userEntity is null)
                return null;
            
            return GenerateToken(userEntity);
        }

        private async Task<UserEntity> GetValidUserAsync(TokenRequest model)
        {
            // Finding user by username matching the model username
            var userEntity = await _db.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == model.Username.ToLower());

            if(userEntity is null)
                return null;

            var passwordHasher = new PasswordHasher<UserEntity>();

            // Validating that the password on the user and model are the same
            var verifyPasswordResult = passwordHasher.VerifyHashedPassword(userEntity, userEntity.Password, model.Password);

            // If the passwords don't match, return null
            if(verifyPasswordResult == PasswordVerificationResult.Failed)
                return null;

            return userEntity;
        }

        private TokenResponse GenerateToken(UserEntity entity)
        {
            var claims = GetClaims(entity);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            {
                Token = tokenHandler.WriteToken(token),
                IssuedAt = token.ValidFrom,
                ExiresAt = token.ValidTo
            };

            return tokenResponse;
        }

        private Claim[] GetClaims(UserEntity user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";

            // If the user doesn't have a saved first or last name (not required) set the name as the username (required)
            var name = !string.IsNullOrWhiteSpace(fullName) ? fullName : user.Username;

            var claims = new Claim[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username),
                new Claim("Email", user.Email),
                new Claim("Name", name)
            };

            return claims;
        }
    }
}