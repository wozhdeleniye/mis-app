using MISBack.Data.Models;
using MISBack.Data;
using MISBack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using MISBack.Data.Enums;
using MISBack.Data.Entities;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MISBack.Configs;
using System.IdentityModel.Tokens.Jwt;

namespace MISBack.Services
{
    public class DoctorsService : IDoctorsService
    {
        private readonly AppDbContext _context;

        public DoctorsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TokenResponseModel> RegisterDoc(DoctorRegisterModel docRegisterModel)
        {
            docRegisterModel.email = NormalizeAttribute(docRegisterModel.email);

            await UniqueCheck(docRegisterModel);

            byte[] salt;
            RandomNumberGenerator.Create().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(docRegisterModel.password, salt, 100000);
            var hash = pbkdf2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var savedPasswordHash = Convert.ToBase64String(hashBytes);

            if(docRegisterModel.gender == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    $"Possible Gender values: Male, Female");
                throw ex;
            }
            CheckBirthDate(docRegisterModel.birthDate);

            await _context.Doctors.AddAsync(new Doctor
            {
                id = Guid.NewGuid(),
                createTime = DateTime.Now,
                name = docRegisterModel.name,
                birthday = docRegisterModel?.birthDate,
                gender = docRegisterModel.gender,
                email = docRegisterModel.email,
                phone = docRegisterModel.phone,
                password = savedPasswordHash,
                speciality = docRegisterModel.speciality
            });
            await _context.SaveChangesAsync();

            var credentials = new LoginCredentialsModel
            {
                email = docRegisterModel.email,
                password = docRegisterModel.password
            };

            return await LoginDoc(credentials);
        }

        public async Task<TokenResponseModel> LoginDoc(LoginCredentialsModel credentials)
        {
            credentials.email = NormalizeAttribute(credentials.email);

            var identity = await GetIdentity(credentials.email, credentials.password);

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddMinutes(JwtConfigurations.Lifetime),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var result = new TokenResponseModel()
            {
                Token = encodeJwt
            };

            return result;
        }

        public async Task EditDocProfile(Guid docId, DoctorEditModel docEditModel)
        {
            throw new NotImplementedException();
        }

        public async Task<DoctorModel> GetDocProfile(Guid docId)
        {
            var docEntity = await _context
            .Doctors
            .FirstOrDefaultAsync(x => x.id == docId);

            if (docEntity != null)
                return new DoctorModel
                {
                    id = docEntity.id,
                    createTime = docEntity.createTime,
                    name = docEntity.name,
                    birthDate = docEntity.birthday,
                    gender = docEntity.gender,
                    email = docEntity.email,
                    phone = docEntity.phone
                };

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "User not exists"
            );
            throw ex;
        }

        public async Task Logout(string token)
        {
            var alreadyExistsToken = await _context.Tokens.FirstOrDefaultAsync(x => x.InvalidToken == token);

            if (alreadyExistsToken == null)
            {
                var handler = new JwtSecurityTokenHandler();
                var expiredDate = handler.ReadJwtToken(token).ValidTo;
                _context.Tokens.Add(new Token { InvalidToken = token, ExpiredDate = expiredDate });
                await _context.SaveChangesAsync();
            }
            else
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "Token is already invalid"
                );
                throw ex;
            }
        }

        private static string NormalizeAttribute(string value)
        {
            return value.ToLower().TrimEnd();
        }

        private async Task UniqueCheck(DoctorRegisterModel docRegisterModel)
        {
            var email = await _context
                .Doctors
                .Where(x => docRegisterModel.email == x.email)
                .FirstOrDefaultAsync();

            if (email != null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    $"Account with email '{docRegisterModel.email}' already exists"
                );
                throw ex;
            }
        }

        private static void CheckBirthDate(DateTime? birthDate)
        {
            if (birthDate == null || birthDate <= DateTime.Now) return;

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                "Birth date can't be later than today");
            throw ex;
        }

        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var userEntity = await _context
                .Doctors
                .FirstOrDefaultAsync(x => x.email == email);

            if (userEntity == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "User not exists"
                );
                throw ex;
            }

            if (!CheckHashPassword(userEntity.password, password))
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "Wrong password"
                );
                throw ex;
            }

            var claims = new List<Claim>
            {
            new(ClaimsIdentity.DefaultNameClaimType, userEntity.id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity
            (
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );

            return claimsIdentity;
        }
        private static bool CheckHashPassword(string savedPasswordHash, string password)
        {
            var hashBytes = Convert.FromBase64String(savedPasswordHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            var hash = pbkdf2.GetBytes(20);
            for (var i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}
