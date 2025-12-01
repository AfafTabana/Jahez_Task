using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Interfaces
{
    public interface IUserContext
    {
      
        string? GetCurrentUserId();
        string GetCurrentUserEmail();
        bool IsUserInRole(string role);
        IEnumerable<Claim> GetUserClaims();
    }
}
