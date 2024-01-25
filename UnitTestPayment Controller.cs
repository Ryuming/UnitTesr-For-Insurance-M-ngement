using AutoMapper;
using InsuranceManagement.Controllers;
using InsuranceManagement.Data;
using InsuranceManagement.Domain;
using InsuranceManagement.DTOs;
using InsuranceManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace InsuranceManagement.Tests
{
	[TestClass]
	public class PaymentControllerTests
	{
		private PaymentController _paymentController;
		private Mock<IRepository<Payment>> _mockPaymentRepository;
		private Mock<IMapper> _mockMapper;

		[TestInitialize]
		public void TestInitialize()
		{
			_mockPaymentRepository = new Mock<IRepository<Payment>>();
			_mockMapper = new Mock<IMapper>();

			_paymentController = new PaymentController(
				_mockPaymentRepository.Object,
				_mockMapper.Object
			);
		}

		[TestMethod]
		public void GetAll_ReturnsOkResultWithPayments()
		{
			// Arrange
			var payments = new List<Payment>
			{
				new Payment { id = Guid.NewGuid(), name = "Payment 1", status = "Paid" },
				new Payment { id = Guid.NewGuid(), name = "Payment 2", status = "Pending" }
			};

			_mockPaymentRepository.Setup(repo => repo.GetAll()).Returns(payments);

			// Act
			var result = _paymentController.GetAll() as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var returnedPayments = result.Value as List<Payment>;
			Assert.IsNotNull(returnedPayments);
			CollectionAssert.AreEqual(payments, returnedPayments);
		}

		[TestMethod]
		public void GetById_WhenPaymentExists_ReturnsOkResultWithPayment()
		{
			// Arrange
			var paymentId = Guid.NewGuid();
			var payment = new Payment { id = paymentId, name = "Payment 1", status = "Paid" };

			_mockPaymentRepository.Setup(repo => repo.GetById(paymentId, default)).Returns(payment);
			_mockMapper.Setup(mapper => mapper.Map<Payment>(payment)).Returns(payment);

			// Act
			var result = _paymentController.GetById(paymentId) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var returnedPayment = result.Value as Payment;
			Assert.IsNotNull(returnedPayment);
			Assert.AreEqual(payment, returnedPayment);
		}

		[TestMethod]
		public void GetById_WhenPaymentDoesNotExist_ReturnsNotFoundResult()
		{
			// Arrange
			var paymentId = Guid.NewGuid();
			Payment payment = null;

			_mockPaymentRepository.Setup(repo => repo.GetById(paymentId, default)).Returns(payment);

			// Act
			var result = _paymentController.GetById(paymentId) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}

		[TestMethod]
		public async Task Create_WithValidData_ReturnsOkResultWithPayment()
		{
			// Arrange
			var dto = new InsertPaymentDTO { name = "Payment 1"};
			var paymentDomain = new Payment { id = Guid.NewGuid(), name = dto.name };

			_mockMapper.Setup(mapper => mapper.Map<Payment>(dto)).Returns(paymentDomain);
			_mockPaymentRepository.Setup(repo => repo.Create(paymentDomain)).Returns(paymentDomain);
			_mockMapper.Setup(mapper => mapper.Map<Payment>(paymentDomain)).Returns(paymentDomain);

			// Act
			var result = await _paymentController.Create(dto) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			//var responseData = result.Value as dynamic;
			//Assert.IsNotNull(responseData);
			//Assert.IsTrue(responseData.Success);
			//Assert.AreEqual(paymentDomain, responseData.Data);
		}

		[TestMethod]
		public async Task Create_WithInvalidData_ReturnsBadRequestWithPayment()
		{
			// Arrange
			var dto = new InsertPaymentDTO { name = "Payment 1"};
			var validationResults = new List<ValidationResult>
	{
		new ValidationResult("Email không hợp lệ", new[] { "email" }),
		new ValidationResult("Số điện thoại phải có 10 chữ số", new[] { "phone" }),
		new ValidationResult("Vui lòng nhập số tài khoản hợp lệ (9-14 số)", new[] { "bankAccount" })
	};

			_paymentController.ModelState.AddModelError("email", "Email không hợp lệ");
			_paymentController.ModelState.AddModelError("phone", "Số điện thoại phải có 10 chữ số");
			_paymentController.ModelState.AddModelError("bankAccount", "Vui lòng nhập số tài khoản hợp lệ (9-14 số)");

			// Act
			var result = await _paymentController.Create(dto) as BadRequestObjectResult;

			// Assert
			//Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

			//var responseData = result.Value as dynamic;
			//Assert.IsNotNull(responseData);
			

		
		}

		[TestMethod]
		public void Update_WhenPaymentExists_ReturnsOkResultWithUpdatedPayment()
		{
			// Arrange
			var paymentId = Guid.NewGuid();
			var dto = new UpdatePaymentDTO { status = "Paid", reason = "Completed" };
			var paymentDomain = new Payment { id = paymentId, name = "Payment 1", status = "Pending" };

			_mockPaymentRepository.Setup(repo => repo.GetById(paymentId, default)).Returns(paymentDomain);
			_mockMapper.Setup(mapper => mapper.Map<Payment>(paymentDomain)).Returns(paymentDomain);

			// Act
			var result = _paymentController.Update(paymentId, dto) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var updatedPayment = result.Value as Payment;
			Assert.IsNotNull(updatedPayment);
			Assert.AreEqual(dto.status, updatedPayment.status);
			Assert.AreEqual(dto.reason, updatedPayment.reason);
		}

		[TestMethod]
		public void Update_WhenPaymentDoesNotExist_ReturnsNotFoundResult()
		{
			// Arrange
			var paymentId = Guid.NewGuid();
			Payment payment = null;
			var dto = new UpdatePaymentDTO { status = "Paid", reason = "Completed" };

			_mockPaymentRepository.Setup(repo => repo.GetById(paymentId, default)).Returns(payment);

			// Act
			var result = _paymentController.Update(paymentId, dto) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}

		
	}
}