using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authrepo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository authrepo, IConfiguration config)
        {
           _config = config;
            _authrepo = authrepo;

        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
        {
            //validate Request
            userForRegister.UserName = userForRegister.UserName.ToLower();

            if (await _authrepo.userExists(userForRegister.UserName))
            {
                return BadRequest("User Already Exists");

            }

            var userToCreate = new User
            {
                UserName = userForRegister.UserName
            };
            var createdUser = await _authrepo.Register(userToCreate, userForRegister.Password);

            return StatusCode(201);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userLoginDto)
        {
            var userFromRepo = await _authrepo.Login(userLoginDto.UserName.ToLower(), userLoginDto.Password);
            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()) ,
            new Claim(ClaimTypes.Name,userFromRepo.UserName.ToString())
         };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject= new ClaimsIdentity(claims),
                Expires= DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();

            var token=tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });
    

    }



    }


}