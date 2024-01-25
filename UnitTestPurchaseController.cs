using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using InsuranceManagement.Controllers;
using InsuranceManagement.Data;
using InsuranceManagement.Domain;
using InsuranceManagement.DTOs;
using InsuranceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NSubstitute;

namespace InsuranceManagement.Tests
{
	[TestClass]
	public class PurchaseControllerTests
	{
		private PurchaseController _purchaseController;
		private Mock<IRepository<Purchase>> _purchaseRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private Mock<UserDbContext> _userDbContextMock;

		// Helper method to create a mock DbSet from a list of objects
		private static IQueryable<T> MockDbSet<T>(IEnumerable<T> data) where T : class
		{
			var queryable = data.AsQueryable();
			var mockDbSet = new Mock<DbSet<T>>();
			mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
			mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
			mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
			mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
			return queryable;
		}

		[TestInitialize]
		public void Initialize()
		{
			_purchaseRepositoryMock = new Mock<IRepository<Purchase>>();
			_mapperMock = new Mock<IMapper>();

			// Create a real instance of UserDbContext
			var options = new DbContextOptionsBuilder<UserDbContext>()
				.UseInMemoryDatabase(databaseName: "UnitTestDb")
				.Options;

			var userDbContext = new UserDbContext(options);

			_purchaseController = new PurchaseController(
				_purchaseRepositoryMock.Object,
				_mapperMock.Object,
				userDbContext
			);
		}

		[TestMethod]
		public void GetPurchaseDetails_WithValidData_ReturnsOkResult()
		{
			// Arrange
			var purchaseDetails = new List<object>
			{
				new
				{
					Id = Guid.NewGuid(),
					UserId = Guid.NewGuid(),
					Email = "test@example.com",
					InsuranceName = "Insurance 1",
					Name = "John Doe",
					Phone = "1234567890",
					PurchaseDate = DateTime.Now,
					Status = "Pending",
					InsurancePrice = 100.0
				}
			};

			_userDbContextMock.Setup(db => db.purchases)
				.Returns((DbSet<Purchase>)MockDbSet(purchaseDetails));

			// Act
			var result = _purchaseController.GetPurchaseDetails() as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
			var responseData = result.Value as List<object>;
			Assert.IsNotNull(responseData);
			Assert.AreEqual(purchaseDetails.Count, responseData.Count);
		}

		[TestMethod]
		public void Create_WithValidData_ReturnsOkResultWithData()
		{
			// Arrange
			var dto = new PurchaseDTO
			{
				// Populate the DTO properties
			};

			var purchaseDomain = new Purchase
			{
				// Populate the Purchase domain object properties
			};

			var createdPurchase = new Purchase
			{
				// Populate the created Purchase object properties
			};

			var purchaseDTO = new PurchaseDTO
			{
				// Populate the PurchaseDTO object properties
			};

			_mapperMock.Setup(mapper => mapper.Map<Purchase>(dto))
				.Returns(purchaseDomain);

			_purchaseRepositoryMock.Setup(repo => repo.Create(purchaseDomain))
				.Returns(createdPurchase);

			_mapperMock.Setup(mapper => mapper.Map<PurchaseDTO>(createdPurchase))
				.Returns(purchaseDTO);

			// Act
			var result = _purchaseController.Create(dto) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
			var responseData = result.Value as dynamic;
			Assert.IsNotNull(responseData);
			Assert.IsTrue(responseData.Success);
			Assert.AreEqual(purchaseDTO, responseData.Data);
		}

		[TestMethod]
		public void Update_WithValidData_ReturnsOkResult()
		{
			// Arrange
			var insuranceId = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var purchaseToUpdate = new Purchase
			{
				// Populate the Purchase object properties
			};

			_purchaseRepositoryMock.Setup(repo => repo.GetById(insuranceId, userId))
				.Returns(purchaseToUpdate);

			// Act
			var result = _purchaseController.Update(insuranceId, userId) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
			Assert.AreEqual("Tình trạng đã được cập nhật thành công", result.Value);
		}

		[TestMethod]
		public void Update_WithInvalidData_ReturnsNotFoundResult()
		{
			// Arrange
			var insuranceId = Guid.NewGuid();
			var userId = Guid.NewGuid();

			_purchaseRepositoryMock.Setup(repo => repo.GetById(insuranceId, userId))
				.Returns((Purchase)null);

			// Act
			var result = _purchaseController.Update(insuranceId, userId) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}
		
	}
	
	
	}