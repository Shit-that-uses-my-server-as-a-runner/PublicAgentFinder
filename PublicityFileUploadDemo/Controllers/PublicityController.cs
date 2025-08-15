using Microsoft.AspNetCore.Mvc;
using PublicityFileUploadDemo.Abstractions;

namespace PublicityFileUploadDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class PublicityController : ControllerBase
{

    private readonly ILogger<PublicityController> logger;
    private readonly IInMemoryDb db;

    public PublicityController(ILogger<PublicityController> logger, IInMemoryDb db)
    {
        this.logger = logger;
        this.db = db;
    }

    [HttpPost("load")]
    public async Task<IActionResult> LoadAsFormFile(IFormFile file)
    {
        logger.LogInformation($"Loading a {file.FileName} through IFormFile");
        var stream = file.OpenReadStream();

        try
        {
            await db.LoadFromStreamAsync(stream);
        } catch (Exception e)
        {
            return Problem(title: "Error processing document");
        }
        
        return Ok();
    }

    [HttpGet("getAgents")]
    public async Task<ActionResult<IEnumerable<string>>> GetAgents([FromQuery]string pattern)
    {
        var result = new List<string>();

        int posSlash = 1;
        while (posSlash > 0)
        {
            posSlash = pattern.LastIndexOf('/');

            var buf = await db.GetByKeyAsync(pattern);
            logger.LogInformation($"Searching for {pattern}");

            if (buf is not null)
            {
                result.AddRange(buf);
            }
            
            pattern = pattern.Substring(0, posSlash);
        }
        
        if(result.Count == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }
}
