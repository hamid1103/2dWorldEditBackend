using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories.Environment2D;
using MySecureBackend.WebApi.Repositories.Object2D;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class Object2DController : ControllerBase
{
    private readonly IObjectRepository object2DRepo;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEnvironmentRepository _environmentRepository;

    public Object2DController(IObjectRepository object2DRepo, IEnvironmentRepository environmentRepository, IAuthenticationService authSer)
    {
        this.object2DRepo = object2DRepo;
        _authenticationService = authSer;
        _environmentRepository = environmentRepository;
    }
    
    [HttpGet(Name = "Get2DObjects")]
    public async Task<ActionResult<List<Object2D>>> GetAsync()
    {
        var exampleObjects = await object2DRepo.SelectAsync();
        return Ok(exampleObjects);
    }
    
    [HttpGet("{exampleObjectId}", Name = "GetObject2DById")]
    public async Task<ActionResult<Object2D>> GetByIdAsync(Guid exampleObjectId)
    {
        var exampleObject = await object2DRepo.SelectAsync(exampleObjectId);

        if (exampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"2D Object {exampleObjectId} not found" });

        return Ok(exampleObject);
    }
    
    [HttpPost("Bulk",Name = "Add2DObjects")]
    public async Task<ActionResult> AddMultipleAsync([FromBody] CreateObject2DBatchRequest batchRequest)
    {
        foreach (var object2D in batchRequest.Objects)
        {
            //Check if object exists already.
            if (await object2DRepo.ObjectExistsAsync(batchRequest.EnvironmentId, (int)object2D.PosX, (int)object2D.PosY))
            {
                await object2DRepo.UpdateAsync(object2D);
            }
            else
            {
                object2D.Id = Guid.NewGuid();
                object2D.EnvironmentId = batchRequest.EnvironmentId;
                await object2DRepo.InsertAsync(object2D);
            }
        }
        
        return Ok();
    }
    
    [HttpPost(Name = "Add2DObject")]
    public async Task<ActionResult<Object2D>> AddAsync(Object2D exampleObject, Guid environmentId)
    {
        exampleObject.Id = Guid.NewGuid();
        exampleObject.EnvironmentId = environmentId;

        await object2DRepo.InsertAsync(exampleObject);

        return CreatedAtRoute("GetObject2DById", new { exampleObjectId = exampleObject.Id }, exampleObject);
    }
    
    [HttpPut("{exampleObjectId}", Name = "UpdateObject2D")]
    public async Task<ActionResult<Object2D>> UpdateAsync(Guid exampleObjectId, Object2D exampleObject)
    {
        var existingExampleObject = await object2DRepo.SelectAsync(exampleObjectId);
        
        if (existingExampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"2D Object {exampleObjectId} not found" });

        if (exampleObject.Id != exampleObjectId)
            return Conflict(new ProblemDetails { Detail = "The id of the 2D Object in the route does not match the id of the 2D Object in the body" });

        await object2DRepo.UpdateAsync(exampleObject);

        return Ok(exampleObject);
    }
    
    [HttpDelete("{exampleObjectId}", Name = "DeleteObject2D")]
    public async Task<ActionResult> DeleteAsync(Guid exampleObjectId)
    {
        var exampleObject = await object2DRepo.SelectAsync(exampleObjectId);
        if (exampleObject.EnvironmentId != null)
        {
            Environment2D env = await _environmentRepository.SelectAsync((Guid)exampleObject.EnvironmentId);
            if (!IsAuthorized(env))
                return Unauthorized();
        }
        
        if (exampleObject == null)
            return NotFound(new ProblemDetails { Detail = $"Object 2d {exampleObjectId} not found" });

        await object2DRepo.DeleteAsync(exampleObjectId);

        return Ok();
    }
    
    private bool IsAuthorized(string? toCompareUserid)
    {
        if (toCompareUserid.IsNullOrEmpty())
            return true;
        return toCompareUserid == _authenticationService.GetCurrentAuthenticatedUserId();
    }
    
    private bool IsAuthorized(Environment2D environment2D)
    {
        if (environment2D.UserId.IsNullOrEmpty())
            return true;
        return environment2D.UserId == _authenticationService.GetCurrentAuthenticatedUserId();
    }
    
}

public class CreateObject2DBatchRequest
{
    public Guid EnvironmentId { get; set; }
    public List<Object2D> Objects { get; set; }
}