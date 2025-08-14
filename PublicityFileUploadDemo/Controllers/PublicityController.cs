using Microsoft.AspNetCore.Mvc;

namespace PublicityFileUploadDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicityController : ControllerBase
{

    private readonly ILogger<PublicityController> _logger;

    public PublicityController(ILogger<PublicityController> logger)
    {
        _logger = logger;
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadFile(IFormFile file)
    {
        var stream = file.OpenReadStream();
        var reader = new StreamReader(stream);

        string? line;
        while ((line = reader.ReadLine()) != null)
            _logger.Log(LogLevel.Error, line);

        reader.Close();
        
        return Ok();
    }

    [HttpGet("hello")]
    public async Task<IActionResult> Hello()
    {
        return Ok();
    }
}
