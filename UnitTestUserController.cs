using InsuranceManagement.Controllers;
using InsuranceManagement.Data;
using InsuranceManagement.Domain;
using InsuranceManagement.DTOs;
using InsuranceManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InsuranceManagement.Tests
{
	[TestClass]
	public class UserControllerTests
	{
		private UserDbContext _userDbContext;
		private UserController _userController;
		private Mock<ITokenRepository> _tokenRepositoryMock;
		private Mock<IPasswordHasher> _passwordHasherMock;

		[TestInitialize]
		public void Initialize()
		{
			var options = new DbContextOptionsBuilder<UserDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			_userDbContext = new UserDbContext(options);
			_tokenRepositoryMock = new Mock<ITokenRepository>();
			_passwordHasherMock = new Mock<IPasswordHasher>();
			_userController = new UserController(_userDbContext, _tokenRepositoryMock.Object, _passwordHasherMock.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_userDbContext.Database.EnsureDeleted();
		}

		[TestMethod]
		public void GetByEmailAndPassword_ExistingUser_ReturnsOkResultWithToken()
		{
			// Arrange
			var email = "test@example.com";
			var password = "password";
			var hashedPassword = "hashedPassword";
			var user = new User()
			{
				email = email,
				password = hashedPassword,
				// Set other properties as needed
			};
			_userDbContext.users.Add(user);
			_userDbContext.SaveChanges();
			_passwordHasherMock.Setup(p => p.HashPassword(password)).Returns(hashedPassword);
			_tokenRepositoryMock.Setup(t => t.CreateJWTToken(It.IsAny<UserDTO>())).Returns("jwtToken");

			// Act
			var result = _userController.GetByEmailAndPassword(email, password) as OkObjectResult;
			var loginResponse = result.Value as LoginResponseDto;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(200, result.StatusCode);
			Assert.IsNotNull(loginResponse);
			Assert.AreEqual("jwtToken", loginResponse.token);
		}

		[TestMethod]
		public void GetByEmailAndPassword_NonExistingUser_ReturnsNotFoundResult()
		{
			// Arrange
			var email = "nonexisting@example.com";
			var password = "password";

			// Act
			var result = _userController.GetByEmailAndPassword(email, password) as NotFoundObjectResult;
			var errorResponse = result.Value as string;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(404, result.StatusCode);
			//Assert.IsNotNull(errorResponse);
			//Assert.AreEqual(1, errorResponse.ErrorCode);
			//Assert.AreEqual("Tài khoản không tồn tại", errorResponse.ErrorMessage);
		}

		// Write more test methods for other actions in UserController as needed
	}
}