using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Controllers
{
    
    public class UsersController:BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext Context){
             _context=Context;
       }

       [HttpGet]
       public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        var Users = await _context.Users.ToListAsync();
        return Users;
       }
       
       [Authorize]
       [HttpGet("{id}")]
       public async Task<ActionResult<AppUser>> GetUser( int id){
        var User = await _context.Users.FindAsync(id);
        return User;
       }
    }
}