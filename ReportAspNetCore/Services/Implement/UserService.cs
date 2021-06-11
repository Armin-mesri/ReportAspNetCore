using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReportAspNetCore.Convertors;
using ReportAspNetCore.Data;
using ReportAspNetCore.DTOs;
using ReportAspNetCore.Models;
using ReportAspNetCore.Services.Interfaces;


namespace ReportAspNetCore.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly ReportAspNetCoreContext _context;

        public UserService(ReportAspNetCoreContext context)
        {
            _context = context;
        }

        public bool IsExistEmail(string email)
        {
            return _context.Users.Any(e => e.Email == email);

        }

        public int addUser(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public User LoginUser(LoginViewModel login)
        {
            string email = FixedText.FixedEmail(login.Email);

            return _context.Users.SingleOrDefault(u => u.Email == email);
        }
    }
}
