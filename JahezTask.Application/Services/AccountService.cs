using AutoMapper;
using JahezTask.Application.DTOs.Account;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace Jahez_Task.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AccountService(SignInManager<ApplicationUser> _signinmanager , UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager , IUnitOfWork _unitofwork , IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            unitOfWork = _unitofwork;
            this.mapper = mapper;
            signInManager = _signinmanager;
        }

        public async Task<(bool Success, string Message)> Register(RegisterDTO registerDTO, CancellationToken cancellationToken = default)
        {
            if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
                return (false, "Email is already Exist.");

            ApplicationUser Member = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,

            };
            var result = await userManager.CreateAsync(Member , registerDTO.Password );

            if( !result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            if (!await roleManager.RoleExistsAsync("member"))
                await roleManager.CreateAsync(new IdentityRole<int>("member"));
            await userManager.AddToRoleAsync(Member, "member");

            return (true, "customer registerd succesfully");

        }

        public async Task<(bool Success, string Message)> Login(LoginDTO loginDTO, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return (false, "Invalid login attempt ..");
            var passwordcheck = await signInManager.CheckPasswordSignInAsync(user , loginDTO.Password , false);
            if (!passwordcheck.Succeeded)
                return (false, "Invalid login attempt ..");

            return (true, "Login success");
        }



    }
}
