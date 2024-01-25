using AutoMapper;
using InsuranceManagement.Controllers;
using InsuranceManagement.Data;
using InsuranceManagement.Domain;
using InsuranceManagement.Services;
using InsuranceManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace InsuranceManagement.Tests
{
	[TestClass]
	public class UnitTestInsuranceController
	{
		private InsuranceController _insuranceController;
		private Mock<IRepository<Insurance>> _mockInsuranceRepository;
		private Mock<IMapper> _mockMapper;
		private Mock<FirebaseService> _mockFirebaseService;

		[TestInitialize]
		public void Setup()
		{
			_mockInsuranceRepository = new Mock<IRepository<Insurance>>();
			_mockMapper = new Mock<IMapper>();
			_mockFirebaseService = new Mock<FirebaseService>();
			_insuranceController = new InsuranceController(_mockInsuranceRepository.Object, _mockMapper.Object);
		}

		[TestMethod]

		public void GetAll_ReturnsOkResult()
		{
			//Arrange
			var insuranceList = new List<Insurance>();
			_mockInsuranceRepository.Setup(repo => repo.GetAll()).Returns(insuranceList);

			//Act
			var result = _insuranceController.GetAll();

			//Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
		}


		[TestMethod]
		public void GetById_WithValidId_ReturnsOkResult()
		{
			//Arrange 
			var insuranceId = Guid.NewGuid();
			var insurance = new Insurance { id = insuranceId };

			_mockInsuranceRepository.Setup(repo=>repo.GetById(insuranceId, default)).Returns(insurance);
			_mockMapper.Setup(mapper => mapper.Map<Insurance>(It.IsAny<Insurance>())).Returns(insurance);

			//Act
			var result = _insuranceController.GetById(insuranceId);

			//Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
		}

		[TestMethod]

		public void GetById_WithInvalidId_ReturnsNotFoundResult()
		{
			var insuranceId = Guid.NewGuid();
			_mockInsuranceRepository.Setup(repo => repo.GetById(insuranceId, default)).Returns((Insurance)null);

			//Act
			var result = _insuranceController.GetById(insuranceId);

			//Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));

		}

		[TestMethod]

		public async Task Create_WithValidModel_ReturnsOkResult()
		{
			//Arrange 
			var insertInsuranceDTO = new InsertInsuranceDTO();
			var insuranceDomain = new Insurance();
			var createdInsurance = new Insurance();
			var insuranceDTO = new InsertInsuranceDTO();

			_mockMapper.Setup(mapper => mapper.Map<Insurance>(insertInsuranceDTO)).Returns(insuranceDomain);
			_mockInsuranceRepository.Setup(repo => repo.Create(insuranceDomain)).Returns(createdInsurance);
			_mockMapper.Setup(mapper => mapper.Map<InsertInsuranceDTO, Insurance>(insuranceDTO)).Returns(insuranceDomain);

			//Act
			var result = _insuranceController.Create(insertInsuranceDTO);

			//Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));

		}

		[TestMethod]

		public async Task Create_WithInvalidModel_ReturnsBadRequestResult()
		{
			// Arrange
			_insuranceController.ModelState.AddModelError("email", "Email không hợp lệ");
			var insertInsuranceDTO = new InsertInsuranceDTO();

			// Act
			var result = _insuranceController.Create(insertInsuranceDTO);

			// Assert
			Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
		}

		[TestMethod]
		public async Task  Update_WithValidId_ReturnsOkResult()
		{
			var insuranceId = Guid.NewGuid();
			var updateInsuranceDto = new UpdateInsuranceDTO { image = new FormFile(null, 0, 0, "testImage", "testImage.jpg") };
			var insuranceEntity = new Insurance { id = insuranceId };

			_mockInsuranceRepository.Setup(repo => repo.GetById(insuranceId, default)).Returns(insuranceEntity);
			_mockFirebaseService.Setup(service => FirebaseService.UploadToFirebase(updateInsuranceDto.image)).ReturnsAsync("uploadedImageUrl");
			_mockMapper.Setup(mapper => mapper.Map<Insurance>(updateInsuranceDto)).Returns(insuranceEntity);

			// Act
			var result = await _insuranceController.Update(insuranceId, updateInsuranceDto) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var updatedInsuranceDto = result.Value as InsuranceDTO;
			Assert.IsNotNull(updatedInsuranceDto);
			Assert.AreEqual(insuranceEntity.id, updatedInsuranceDto.id);
		}

		[TestMethod]
		public async Task Update_WhenInsuranceDoesNotExist_ReturnsNotFoundResult()
		{
			// Arrange
			var insuranceId = Guid.NewGuid();
			Insurance insuranceEntity = null;
			var updateInsuranceDto = new UpdateInsuranceDTO { image = new FormFile(null, 0, 0, "testImage", "testImage.jpg") };

			_mockInsuranceRepository.Setup(repo => repo.GetById(insuranceId, default)).Returns(insuranceEntity);

			// Act
			var result = await _insuranceController.Update(insuranceId, updateInsuranceDto) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}

		[TestMethod]
		public void Delete_WhenInsuranceExists_ReturnsOkResult()
		{
			// Arrange
			var insuranceId = Guid.NewGuid();
			var insuranceEntity = new Insurance { id = insuranceId };

			_mockInsuranceRepository.Setup(repo => repo.GetById(insuranceId, default)).Returns(insuranceEntity);

			// Act
			var result = _insuranceController.Delete(insuranceId) as OkObjectResult;

			// Assert
			//Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			//var deletedInsuranceDto = result.Value as InsuranceDTO;
			//Assert.IsNotNull(deletedInsuranceDto);
			//Assert.AreEqual(insuranceEntity.id, deletedInsuranceDto.id);

			//_mockInsuranceRepository.Verify(repo => repo.Delete(insuranceId), Times.Once);
		}

		[TestMethod]
		public void Delete_WhenInsuranceDoesNotExist_ReturnsNotFoundResult()
		{
			// Arrange
			var insuranceId = Guid.NewGuid();
			Insurance insuranceEntity = null;

			_mockInsuranceRepository.Setup(repo => repo.GetById(insuranceId, default)).Returns(insuranceEntity);

			// Act
			var result = _insuranceController.Delete(insuranceId) as NotFoundResult;

			// Assert
			//Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);

			_mockInsuranceRepository.Verify(repo => repo.Delete(insuranceId), Times.Never);
		}
	}

}
