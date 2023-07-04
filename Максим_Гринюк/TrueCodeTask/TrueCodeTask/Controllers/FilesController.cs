using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TrueCodeTask.Helpers;

namespace TrueCodeTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private FileParserFactory _fileParserFactory;

        public FilesController(ILogger<FilesController> logger,
             FileParserFactory fileParserFactory)
        {
            _logger = logger;
            _fileParserFactory = fileParserFactory;
        }

     
        [SwaggerOperation(
            Summary = "Reads a message/text from input file",
            Description = "Returns a message/text from attached file with configured last symbol and message buffer size",
            OperationId = "read-file")]
        [HttpPost("read-attached-file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        public async Task<IActionResult> ReadAttachedFile(IFormFile file)
        {
            if (file.FileName == null)
            {
                return BadRequest();
            }
            var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;
            var service = _fileParserFactory.GetFileParser(stream, file.FileName);
            if (service == null)
            {
                return StatusCode(415);
            }
            var result = service.GetFileContent();
            return Ok(result);
        }


    }
}