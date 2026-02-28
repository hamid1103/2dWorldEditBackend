using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json","multipart/form-data")]
[Produces("application/json")]
public class PreviewController : ControllerBase
{
    private IHttpContextAccessor _httpContextAccessor;
    
    public PreviewController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    [HttpGet(Name = "GetPreview")]
    public async Task<ActionResult> GetById(String path)
    {
        return Ok();
    }

    [HttpPost(Name = "AddPreview")]
    public async Task<ActionResult> AddAsync(IFormFile file)

    {
        Guid Id = Guid.NewGuid();
        
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var fileName = $"{Id}_{file.FileName}";
        var filePath = Path.Combine(uploadPath, fileName);

        if (file != null && file.Length > 0)
        {
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        var host = string.Format("{0}{1}",
            _httpContextAccessor.HttpContext!.Request.IsHttps ? "https://" : "http://",
            _httpContextAccessor.HttpContext.Request.Host);

        PreviewImage image = new PreviewImage();
        image.Url = $"{host}/uploads/{fileName}";
        return Ok(image);
    }

}