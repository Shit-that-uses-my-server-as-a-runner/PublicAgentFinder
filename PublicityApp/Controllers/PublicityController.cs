using Microsoft.AspNetCore.Mvc;
using PublicityApp.Abstractions;

namespace PublicityApp.Controllers;

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
    public async Task<ActionResult<IEnumerable<string>>> GetAgents([FromQuery]string location)
    {
        //model checking: first symbol must be '/'
        //  - otherwise exception will be thrown at line 58
        if (location[0] != '/')
        {
            return BadRequest("Invalid location format");
        }

        var result = new List<string>();

        int posSlash = 1;
        while (posSlash > 0)
        {
            posSlash = location.LastIndexOf('/');

            var buf = await db.GetByKeyAsync(location);
            logger.LogInformation($"Searching for {location}");

            if (buf is not null)
            {
                result.AddRange(buf);
            }
            
            location = location.Substring(0, posSlash);
        }
        
        if(result.Count == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }
}
