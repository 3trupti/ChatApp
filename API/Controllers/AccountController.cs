using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using API.Data;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenservice;
        public AccountController(DataContext Context, ITokenService TokenService){
           _context=Context;
           _tokenservice=TokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDtos registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName= registerDto.Username.ToLower(),
                PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                Username=user.UserName,
                Token= _tokenservice.CreateToken(user)
            };
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDtos Logindto){

            var user= await _context.Users.SingleOrDefaultAsync(x=>x.UserName==Logindto.Username);
            if(user==null)return Unauthorized("Invalid Username");

            using var hmac= new HMACSHA512(user.PasswordSalt);
            var computedHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(Logindto.Password));

            for(int i=0;i<computedHash.Length;i++){
                if(computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return new UserDto{
                Username=user.UserName,
                Token= _tokenservice.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}                                                                                                                                    
