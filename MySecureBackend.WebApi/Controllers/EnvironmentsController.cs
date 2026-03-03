using Dapper;
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
public class EnvironmentsController : ControllerBase
{
    private readonly IObjectRepository _objectRepository;
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IAuthenticationService _authenticationService;
    
    public EnvironmentsController(IEnvironmentRepository environmentRepo, IAuthenticationService authenticationService, IObjectRepository _objectRepository)
    {
        _environmentRepository = environmentRepo;
        _authenticationService = authenticationService;
        this._objectRepository = _objectRepository;
    }
    
    [HttpGet(Name = "GetEnvironment2DObjects")]
    public async Task<ActionResult<EnvironmentListRequest>> GetAsync()
    {
        EnvironmentListRequest result = new();
        var exampleObjects = await _environmentRepository.SelectAsync(_authenticationService.GetCurrentAuthenticatedUserId());
        result.Worlds = exampleObjects.AsList();
        return Ok(result);
    }
    
    [HttpGet("{environment2DId}", Name = "GetEnvironment2DById")]
    public async Task<ActionResult<Environment2D>> GetByIdAsync(Guid environment2DId)
    {
        var environment2D = await _environmentRepository.SelectAsync(environment2DId);

        if (environment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {environment2DId} not found" });

        return Ok(environment2D);
    }

    [HttpGet("{environment2DId}/2DObjects", Name = "GetEnvironmentByIdWithObjects")]
    public async Task<ActionResult<Environment2D>> GetEnvironmentObjects(Guid environment2DId)
    {
        var environment2D = await _environmentRepository.SelectAsync(environment2DId);
        if(IsAuthorized(environment2D.UserId))
        if (environment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {environment2DId} not found" });

        //Fetch Objects
        //ToDo: Add repository function to fetch objects by Environment Id - Done
        var object2ds = await _objectRepository.SelectAsync(environment2D);
        environment2D.objects = object2ds.AsList();
        return Ok(environment2D);
    }
    
    [HttpPost(Name = "AddEnvironment2D")]
    public async Task<ActionResult<Environment2D>> AddAsync(Environment2D environment2D)
    {
        //checks
        if (environment2D.Name.Length is > 25 or < 1)
        {
            return BadRequest("Name must be between 1 and 25 characters long.");
        }

        if ((await _environmentRepository.SelectAsync(_authenticationService.GetCurrentAuthenticatedUserId())).Any(x=>x.Name==environment2D.Name))
        {
            Console.WriteLine("World name exists for user");
            return Conflict("You already have a World with that name");
        }

        if ((await _environmentRepository.SelectAsync(_authenticationService.GetCurrentAuthenticatedUserId())).Count() >=5)
        {
            return Conflict("World limit has been reached. Max of 5.");
        }
        
        environment2D.Id = Guid.NewGuid();
        //ignore warning. Can only call if logged in.
        environment2D.UserId = _authenticationService.GetCurrentAuthenticatedUserId();
        Console.WriteLine("USERID: " + _authenticationService.GetCurrentAuthenticatedUserId());
        await _environmentRepository.InsertAsync(environment2D);

        return CreatedAtRoute("GetEnvironment2DById", new { environment2DId = environment2D.Id }, environment2D);
    }
    
    [HttpPut(Name = "UpdateEnvironment2D")]
    public async Task<ActionResult<Environment2D>> UpdateAsync(EnvironmentUpdateRequest envReq)
    {
        var existingEnvironment2D = await _environmentRepository.SelectAsync(envReq.EnvironmentID);

        if (!IsAuthorized(existingEnvironment2D.UserId))
        {
            return Unauthorized();
        }
        
        if (existingEnvironment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {envReq.EnvironmentID} not found" });

        if (envReq.Environment2D.Id != envReq.EnvironmentID)
            return Conflict(new ProblemDetails { Detail = "The id of the ExampleObject in the route does not match the id of the ExampleObject in the body" });

        await _environmentRepository.UpdateAsync(envReq.Environment2D);

        return Ok(envReq.Environment2D);
    }
    
    [HttpDelete("{environment2DId}", Name = "DeleteEnvironment")]
    public async Task<ActionResult> DeleteAsync(Guid environment2DId)
    {
        var environment2D = await _environmentRepository.SelectAsync(environment2DId);

        if (!IsAuthorized(environment2D.UserId))
            return Unauthorized();
        
        if (environment2D == null)
            return NotFound(new ProblemDetails { Detail = $"Environment {environment2DId} not found" });
        
        
        await _objectRepository.DeleteByEnvironment(environment2D);
        
        await _environmentRepository.DeleteAsync(environment2DId);

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

public class EnvironmentListRequest
{
    public List<Environment2D> Worlds { get; set; }
}

public class EnvironmentUpdateRequest
{
    public Guid EnvironmentID { get; set; }
    public Environment2D Environment2D { get; set; }
}