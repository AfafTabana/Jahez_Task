using AutoMapper;
using FluentAssertions;
using Jahez_Task.Services.AccountService;
using JahezTask.Application.DTOs.Account;
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Services
{
    public class AccountServiceTests
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountServiceTests()
        {

            var userStore = Substitute.For<IUserStore<ApplicationUser>>();
            _userManager = Substitute.For<UserManager<ApplicationUser>>(
                userStore,
                null, null, null, null, null, null, null, null);

            var roleStore = Substitute.For<IRoleStore<IdentityRole<int>>>();
            _roleManager = Substitute.For<RoleManager<IdentityRole<int>>>(
                roleStore, null, null, null, null);

            _signInManager = Substitute.For<SignInManager<ApplicationUser>>(
                _userManager,
                Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mapper = Substitute.For<IMapper>();

            _accountService = new AccountService(
                _signInManager,
                _userManager,
                _roleManager,
                _unitOfWork,
                _mapper);
        }

        #region Register Test
        [Fact]
        public async Task Register_WithNewEmail_ShouldReturnSuccess()
        {
            //Arange 
            RegisterDTO registerDTO = new RegisterDTO
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

            var result = await _accountService.Register(registerDTO);

            //Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("customer registerd succesfully");

            await _userManager.Received(1).CreateAsync(
                Arg.Is<ApplicationUser>(u => u.Email == registerDTO.Email),
                registerDTO.Password);


        }

        [Fact]
        public async Task Register_WithExistingEmail_ShouldReturnFailure()
        {
            // Arrange
            var registerDTO = new RegisterDTO
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
            var result = await _accountService.Register(registerDTO);

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
            var registerDTO = new RegisterDTO
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
            var result = await _accountService.Register(registerDTO);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Password must be at least 6 characters.");
        }

        #endregion

        #region Login Test
        [Fact]
        public async Task Login_WithNonExistentEmail_ShouldReturnFailure()
        {
            // Arrange
            var loginDTO = new LoginDTO
            {
                Email = "nonexistent@test.com",
                Password = "Password123!"
            };

            _userManager.FindByEmailAsync(loginDTO.Email)
                .Returns(Task.FromResult<ApplicationUser>(null));

            // Act
            var result = await _accountService.Login(loginDTO);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid login attempt ..");

            await _signInManager.DidNotReceive().CheckPasswordSignInAsync(
                Arg.Any<ApplicationUser>(),
                Arg.Any<string>(),
                Arg.Any<bool>());
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldReturnFailure()
        {
            // Arrange
            var loginDTO = new LoginDTO
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
            var result = await _accountService.Login(loginDTO);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid login attempt ..");
        }
        #endregion
    }
}
