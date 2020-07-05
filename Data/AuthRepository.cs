using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _datacontext;
        public AuthRepository(DataContext datacontext)
        {
           _datacontext = datacontext;

        }
        public async Task<User> Login(string username, string password)
        {
           var user =await _datacontext.Users.FirstOrDefaultAsync(x=>x.UserName==username);
           if(user == null)
           return null;

           if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
           return null;

           return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using( var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt))
         {
            
             var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

             for(int i=0;i<computedHash.Length;i++)
             {
                 if(computedHash[i]!=passwordHash[i]) return false;
             }

         }
         return true;
           
        }

        public async Task<User> Register(User user, string password)
        {
          byte[] passwordHash, passwordSalt;
          CreatePasswordHash(password,out passwordHash,out passwordSalt );
          user.PasswordHash=passwordHash;
          user.PasswordSalt=passwordSalt;
          
          await _datacontext.Users.AddAsync(user);
          await _datacontext.SaveChangesAsync();

          return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
         using( var hmac= new System.Security.Cryptography.HMACSHA512())
         {
             passwordSalt=hmac.Key;
             passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
         }
        }

        public async Task<bool> userExists(string username)
        {
          if(await _datacontext.Users.AnyAsync(x=>x.UserName==username))
            return true;

          return false;
        }
    }
}