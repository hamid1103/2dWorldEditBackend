using Microsoft.AspNetCore.Mvc;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.Tests
{
    [TestClass]
    public sealed class ExampleObjectsControllerTests
    {
        private ExampleObjectsController controller;
        private Mock<IExampleObjectRepository> exampleObjectRepository;
        private Mock<IAuthenticationService> authenticationService;

        [TestInitialize]
        public void Setup()
        {
            exampleObjectRepository = new Mock<IExampleObjectRepository>();
            authenticationService = new Mock<IAuthenticationService>();

            controller = new ExampleObjectsController(exampleObjectRepository.Object, authenticationService.Object);
        }

        [TestMethod]
        public async Task Get_ExampleObjectThatDoesNotExist_Returns404NotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            exampleObjectRepository.Setup(x => x.SelectAsync(id)).ReturnsAsync(null as ExampleObject);

            // Act
            var response = await controller.GetByIdAsync(id);

            // Assert
            Assert.IsInstanceOfType<NotFoundObjectResult>(response.Result);
        }
    }
}