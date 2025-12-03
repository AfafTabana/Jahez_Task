using AutoMapper;
using Azure.Core;
using JahezTask.Application.Interfaces;
using JahezTask.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Features.Account.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand , (bool Success, string Message)>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        

        public LoginCommandHandler(SignInManager<ApplicationUser> _signinmanager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
          
            signInManager = _signinmanager;
        }
        public async Task<(bool Success, string Message)> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (false, "Invalid login attempt ..");
            var passwordcheck = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!passwordcheck.Succeeded)
                return (false, "Invalid login attempt ..");

            return (true, "Login success");
        }
    }
}
