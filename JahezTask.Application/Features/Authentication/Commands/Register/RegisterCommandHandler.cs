using AutoMapper;
using JahezTask.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.Features.Account.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, (bool Success, string Message)>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IMapper mapper;
        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }
        public async Task<(bool Success, string Message)> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await userManager.FindByEmailAsync(request.Email) != null)
                return (false, "Email is already Exist.");

            ApplicationUser Member = new ApplicationUser()
            {
                Email = request.Email,
                UserName = request.UserName,

            };
            var result = await userManager.CreateAsync(Member, request.Password);

            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            if (!await roleManager.RoleExistsAsync("member"))
                await roleManager.CreateAsync(new IdentityRole<int>("member"));
            await userManager.AddToRoleAsync(Member, "member");

            return (true, "customer registerd succesfully");
        }
    }
}
