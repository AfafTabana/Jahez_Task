using FluentAssertions;
using JahezTask.Application.DTOs.Book.Commands.AddBook;
using JahezTask.Application.Features.Account.Commands.Login;
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
    public class LoginHandlerTest
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LoginCommandHandler _handler;

        public LoginHandlerTest()
        {
            var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                userStore,
                null, null, null, null, null, null, null, null);
            var roleStore = NSubstitute.Substitute.For<IRoleStore<IdentityRole<int>>>();
            _roleManager = NSubstitute.Substitute.For<RoleManager<IdentityRole<int>>>(
                roleStore, null, null, null, null);
            _signInManager = NSubstitute.Substitute.For<SignInManager<ApplicationUser>>(
                _userManager,
                NSubstitute.Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                NSubstitute.Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _handler = new LoginCommandHandler(_signInManager, _userManager, _roleManager);
        }

        #region Login Test
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenNonExistentEmail()
        {
            // Arrange
            var loginDTO = new LoginCommand
            {
                Email = "nonexistent@test.com",
                Password = "Password123!"
            };

            _userManager.FindByEmailAsync(loginDTO.Email)
                .Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _handler.Handle(loginDTO, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid login attempt ..");

            await _signInManager.DidNotReceive().CheckPasswordSignInAsync(
                Arg.Any<ApplicationUser>(),
                Arg.Any<string>(),
                Arg.Any<bool>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure__WhenInvalidPassword()
        {
            // Arrange
            var loginDTO = new LoginCommand
            {
                Email = "user@test.com",
                Password = "WrongPassword!"
            };

            var user = new ApplicationUser
            {
                Email = loginDTO.Email,
                UserName = "testuser"
            };

            _userManager.FindByEmailAsync(loginDTO.Email)
                .Returns(Task.FromResult(user));

            _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false)
                .Returns(SignInResult.Failed);

            // Act
            var result = await _handler.Handle(loginDTO, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid login attempt");
        }
        #endregion
    }
}

