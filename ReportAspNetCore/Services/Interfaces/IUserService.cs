using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportAspNetCore.DTOs;
using ReportAspNetCore.Models;

namespace ReportAspNetCore.Services.Interfaces
{
   public interface IUserService
    {
        bool IsExistEmail(string email);

        int addUser(User user);

        User LoginUser(LoginViewModel login);
    }
}
