using AutoMapper;
using FluentAssertions;
using JahezTask.Application.Features.Account.Commands.Login;
using JahezTask.Application.Features.Account.Commands.Register;
using JahezTask.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BookLendingTest.Features.Authentication
{
    public class RegisterHandlerTest
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
       
        private readonly IMapper mapper;
        private readonly RegisterCommandHandler _handler;

        public RegisterHandlerTest()
        {
            var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                userStore,
                null, null, null, null, null, null, null, null);
            var roleStore = NSubstitute.Substitute.For<IRoleStore<IdentityRole<int>>>();
            _roleManager = NSubstitute.Substitute.For<RoleManager<IdentityRole<int>>>(
                roleStore, null, null, null, null);
            mapper = Substitute.For<IMapper>();

            _handler = new RegisterCommandHandler(_userManager, _roleManager , mapper );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WithNewEmail()
        {
            //Arange 
            RegisterCommand registerDTO = new RegisterCommand
            {
                Email = "newuser@test.com",
                UserName = "newuser",
                Password = "Password123!"
            };

            _userManager.FindByEmailAsync(registerDTO.Email)
                    .Returns(Task.FromResult<ApplicationUser>(null));

            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), registerDTO.Password)
                .Returns(IdentityResult.Success);

            _roleManager.RoleExistsAsync("member")
                    .Returns(true);

            _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), "member")
                    .Returns(IdentityResult.Success);

            //Act 
            
            var result = await _handler.Handle(registerDTO, CancellationToken.None);

            //Assert
            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("customer registerd succesfully");
            

            await _userManager.Received(1).CreateAsync(
                Arg.Is<ApplicationUser>(u => u.Email == registerDTO.Email),
                registerDTO.Password);


        }

        [Fact]
        public async Task Register_WithExistingEmail_ShouldReturnFailure()
        {
            // Arrange
            var registerDTO = new RegisterCommand
            {
                Email = "existing@test.com",
                UserName = "existinguser",
                Password = "Password123!"
            };

            var existingUser = new ApplicationUser
            {
                Email = registerDTO.Email,
                UserName = "existing"
            };

            _userManager.FindByEmailAsync(registerDTO.Email)
                .Returns(Task.FromResult(existingUser));

            // Act
            var result = await _handler.Handle(registerDTO, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Email is already Exist.");

            await _userManager.DidNotReceive().CreateAsync(
                Arg.Any<ApplicationUser>(),
                Arg.Any<string>());
        }

        [Fact]

        public async Task Register_WithPasswordLessThan6Caracters_ShoulsReturnFailure()
        {
            // Arrange
            var registerDTO = new RegisterCommand
            {
                Email = "newemail@gmail.com",
                UserName = "newuser",
                Password = "123"
            };

            _userManager.FindByEmailAsync(registerDTO.Email)
                .Returns(Task.FromResult<ApplicationUser>(null));

            _userManager.CreateAsync(Arg.Any<ApplicationUser>(), registerDTO.Password)
              .Returns(Task.FromResult(IdentityResult.Failed(
            new IdentityError { Description = "Password must be at least 6 characters." }
        )));

            // Act
            var result = await _handler.Handle(registerDTO, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Password must be at least 6 characters.");
        }


    }
}
