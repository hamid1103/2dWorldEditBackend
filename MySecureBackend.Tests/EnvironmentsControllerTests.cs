using Microsoft.AspNetCore.Mvc;
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
}