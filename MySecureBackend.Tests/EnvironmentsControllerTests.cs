using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MySecureBackend.WebApi.Controllers;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories.Environment2D;
using MySecureBackend.WebApi.Repositories.Object2D;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.Tests;

[TestClass]
public class EnvironmentsControllerTests
{
    private readonly ILogger _logger;
    private EnvironmentsController _controller;
    private Mock<IEnvironmentRepository> _environmentRepo;
    private Mock<IObjectRepository> _objectRepo;
    private Mock<IAuthenticationService> _authService;

    [TestInitialize]
    public void Setup()
    {
        _environmentRepo = new Mock<IEnvironmentRepository>();
        _authService = new Mock<IAuthenticationService>();
        _objectRepo = new Mock<IObjectRepository>();

        _controller = new EnvironmentsController(_environmentRepo.Object, _authService.Object, _objectRepo.Object);
    }

    [TestMethod]
    public async Task Get_EnvironmentThatDoesNotExist_Returns404NotFound()
    {
        Guid id = Guid.NewGuid();
        _environmentRepo.Setup(x => x.SelectAsync(id)).ReturnsAsync(null as Environment2D);

        var response = await _controller.GetByIdAsync(id);
        
        Assert.IsInstanceOfType<NotFoundObjectResult>(response.Result);
    }

    [TestMethod]
    public async Task Add_MoreThanFiveEnvironment_ReturnsConflictResponse()
    {
        string userId = Guid.NewGuid().ToString();
        List<Environment2D> environment2Ds = new List<Environment2D>()
        {
            new Environment2D { Id = Guid.NewGuid(), Name = "First", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Second", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Third", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Fourth", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Fifth", PreviewURL = "", UserId = userId },
        };
        Environment2D newEnv = new Environment2D{Name = "Sixth", PreviewURL = "", UserId = userId};
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
        _environmentRepo.Setup(x => x.SelectAsync(It.IsAny<string>())).ReturnsAsync(environment2Ds);
        _controller = new EnvironmentsController(_environmentRepo.Object, _authService.Object, _objectRepo.Object);
        
        //Act
        var response = await _controller.AddAsync(newEnv);
        //Assert
        var objectResult = response.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(409, objectResult.StatusCode);
    }

    [TestMethod]
    public async Task Add_EnvironmentWithNonUniqueName_ReturnsConflictResponse()
    {
        string userId = Guid.NewGuid().ToString();
        List<Environment2D> environment2Ds = new List<Environment2D>()
        {
            new Environment2D { Id = Guid.NewGuid(), Name = "First", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Second", PreviewURL = "", UserId = userId },
            new Environment2D { Id = Guid.NewGuid(), Name = "Third", PreviewURL = "", UserId = userId }
        };
        Environment2D newEnv = new Environment2D{Name = "Third", PreviewURL = "", UserId = userId};
        _authService.Setup(x => x.GetCurrentAuthenticatedUserId()).Returns(userId);
        _environmentRepo.Setup(x => x.SelectAsync(It.IsAny<string>())).ReturnsAsync(environment2Ds);
        _controller = new EnvironmentsController(_environmentRepo.Object, _authService.Object, _objectRepo.Object);
        
        //Act
        var response = await _controller.AddAsync(newEnv);
        //Assert
        var objectResult = response.Result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(409, objectResult.StatusCode);
    }
}