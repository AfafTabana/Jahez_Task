using AutoMapper;
using Jahez_Task.DTOs.Account;
using Jahez_Task.Models;
using Jahez_Task.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System;

namespace Jahez_Task.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private unitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AccountService(SignInManager<ApplicationUser> _signinmanager , UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager , unitOfWork _unitofwork , IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            unitOfWork = _unitofwork;
            this.mapper = mapper;
            signInManager = _signinmanager;
        }

        public async Task<(bool Success, string Message)> Register(RegisterDTO registerDTO)
        {
            if (await _userManager.FindByEmailAsync(registerDTO.Email)!= null)
                return (false, "Email is already Exist.");

            ApplicationUser Member = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,

            };
            var result = await _userManager.CreateAsync(Member , registerDTO.Password);

            if( !result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            if (!await _roleManager.RoleExistsAsync("Member"))
                await _roleManager.CreateAsync(new IdentityRole<int>("Member"));
            await _userManager.AddToRoleAsync(Member, "Member");

            return (true, "customer registerd succesfully");

        }

        public async Task<(bool Success, string Message)> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return (false, "Invalid login attempt ..");
            var passwordcheck = await signInManager.CheckPasswordSignInAsync(user , loginDTO.Password , false);
            if (!passwordcheck.Succeeded)
                return (false, "Invalid login attempt ..");

            return (true, "Login success");
        }



    }
}
