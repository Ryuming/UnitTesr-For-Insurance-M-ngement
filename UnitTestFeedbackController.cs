using AutoMapper;
using InsuranceManagement.Controllers;
using InsuranceManagement.Data;
using InsuranceManagement.Domain;
using InsuranceManagement.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InsuranceManagement.Services;
using NSubstitute;
using Microsoft.AspNetCore.Mvc.Core;

namespace InsuranceManagement.Tests
{
	[TestClass]
	public class FeedbackControllerTests
	{
		private Mock<IRepository<Feedback>> _mockFeedbackRepository;
		private Mock<IMapper> _mockMapper;
		private FeedbackController _feedbackController;

		[TestInitialize]
		public void Setup()
		{
			_mockFeedbackRepository = new Mock<IRepository<Feedback>>();
			_mockMapper = new Mock<IMapper>();
			_feedbackController = new FeedbackController(_mockFeedbackRepository.Object, _mockMapper.Object);
		}

		[TestMethod]
		public void GetAll_ReturnsOkResult()
		{
			// Arrange
			var feedbackList = new List<Feedback>();
			_mockFeedbackRepository.Setup(repo => repo.GetAll()).Returns(feedbackList);

			// Act
			var result = _feedbackController.GetAll();

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
		}

		[TestMethod]
		public void GetById_WithValidId_ReturnsOkResult()
		{
			// Arrange
			var feedbackId = Guid.NewGuid();
			var feedback = new Feedback { id = feedbackId };
			//var feedbackDTO = new InsertFeedbackDTO { id = feedbackId };
			_mockFeedbackRepository.Setup(repo => repo.GetById(feedbackId, default)).Returns(feedback);
			_mockMapper.Setup(mapper => mapper.Map<Feedback>(It.IsAny<Feedback>())).Returns(feedback);

			// Act
			var result = _feedbackController.GetById(feedbackId);

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
		}

		[TestMethod]
		public void GetById_WithInvalidId_ReturnsNotFoundResult()
		{
			// Arrange
			var feedbackId = Guid.NewGuid();
			_mockFeedbackRepository.Setup(repo => repo.GetById(feedbackId, default)).Returns((Feedback)null);

			// Act
			var result = _feedbackController.GetById(feedbackId);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}

		[TestMethod]
		public void Create_WithValidModel_ReturnsOkResult()
		{
			// Arrange
			var insertFeedbackDTO = new InsertFeedbackDTO();
			var feedbackDomain = new Feedback();
			var createdFeedback = new Feedback();
			var feedbackDTO = new InsertFeedbackDTO();
			_mockMapper.Setup(mapper => mapper.Map<Feedback>(insertFeedbackDTO)).Returns(feedbackDomain);
			_mockFeedbackRepository.Setup(repo => repo.Create(feedbackDomain)).Returns(createdFeedback);
			_mockMapper.Setup(mapper => mapper.Map<InsertFeedbackDTO, Feedback>(feedbackDTO)).Returns(feedbackDomain);

			// Act
			var result = _feedbackController.Create(insertFeedbackDTO);

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
			//var response = result as Dictionary<string, object>;
			//Assert.IsTrue((bool)response["Success"]);
			//Assert.AreEqual(feedbackDTO, response["Data"]);
		}

		[TestMethod]
		public void Create_WithInvalidModel_ReturnsBadRequestResult()
		{
			// Arrange
			_feedbackController.ModelState.AddModelError("email", "Email không hợp lệ");
			var insertFeedbackDTO = new InsertFeedbackDTO();

			// Act
			var result = _feedbackController.Create(insertFeedbackDTO);

			// Assert
			Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
		}

		[TestMethod]
		public void UpdatePurchase_WithValidId_ReturnsOkResult()
		{
			// Arrange
			var feedbackId = Guid.NewGuid();
			var feedbackToUpdate = new Feedback { id = feedbackId };
			_mockFeedbackRepository.Setup(repo => repo.GetById(feedbackId, default)).Returns(feedbackToUpdate);

			// Act
			var result = _feedbackController.UpdatePurchase(feedbackId);

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkObjectResult));
		}

		[TestMethod]
		public void UpdatePurchase_WithInvalidId_ReturnsNotFoundResult()
		{
			// Arrange
			var feedbackId = Guid.NewGuid();
			_mockFeedbackRepository.Setup(repo => repo.GetById(feedbackId, default)).Returns((Feedback)null);

			// Act
			var result = _feedbackController.UpdatePurchase(feedbackId);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}
	}
}