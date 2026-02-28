using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class ExampleObjectsController : ControllerBase
{
    private readonly IExampleObjectRepository _exampleObjectRepository;
    private readonly IAuthenticationService _authenticationService;

    public ExampleObjectsController(IExampleObjectRepository exampleObjectRepository, IAuthenticationService authenticationService)
    {
        _exampleObjectRepository = exampleObjectRepository;
        _authenticationService = authenticationService;
    }

    [HttpGet(Name = "GetExampleObjects")]
    public async Task<ActionResult<List<ExampleObject>>> GetAsync()
    {
        var exampleObjects = await _exampleObjectRepository.SelectAsync();
        return Ok(exampleObjects);
    }

    [HttpGet("{exampleObjectId}", Name = "GetExampleObjectById")]
    public async Task<ActionResult<ExampleObject>> GetByIdAsync(Guid exampleObjectId)
    {
        var exampleObject = await _exampleObjectRepository.SelectAsync(exampleObjectId);

        if (exampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"ExampleObject {exampleObjectId} not found" });

        return Ok(exampleObject);
    }

    [HttpPost(Name = "AddExampleObject")]
    public async Task<ActionResult<ExampleObject>> AddAsync(ExampleObject exampleObject)
    {
        exampleObject.Id = Guid.NewGuid();

        await _exampleObjectRepository.InsertAsync(exampleObject);

        return CreatedAtRoute("GetExampleObjectById", new { exampleObjectId = exampleObject.Id }, exampleObject);
    }

    [HttpPut("{exampleObjectId}", Name = "UpdateExampleObject")]
    public async Task<ActionResult<ExampleObject>> UpdateAsync(Guid exampleObjectId, ExampleObject exampleObject)
    {
        var existingExampleObject = await _exampleObjectRepository.SelectAsync(exampleObjectId);

        if (existingExampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"ExampleObject {exampleObjectId} not found" });

        if (exampleObject.Id != exampleObjectId)
            return Conflict(new ProblemDetails { Detail = "The id of the ExampleObject in the route does not match the id of the ExampleObject in the body" });

        await _exampleObjectRepository.UpdateAsync(exampleObject);

        return Ok(exampleObject);
    }

    [HttpDelete("{exampleObjectId}", Name = "DeleteExampleObject")]
    public async Task<ActionResult> DeleteAsync(Guid exampleObjectId)
    {
        var exampleObject = await _exampleObjectRepository.SelectAsync(exampleObjectId);

        if (exampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"ExampleObject {exampleObjectId} not found" });

        await _exampleObjectRepository.DeleteAsync(exampleObjectId);

        return Ok();
    }
}