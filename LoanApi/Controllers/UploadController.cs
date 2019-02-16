using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using LoanApi.Models;
using Microsoft.AspNetCore.Hosting;

namespace LoanApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public UploadController(AppDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        
        [HttpPost("{filetype}"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFiles([FromRoute] string filetype)
        {
            try
            {
                var file = Request.Form.Files[0];
                //string folderName = $"Files/{filetype}";
                var newPath = Path.Combine(Directory.GetCurrentDirectory(), $"Files/{filetype}");
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string[] filedetails = file.FileName.Split('.');
                    filedetails[0] = DateTime.Now.ToString(@"yyyy-MM-dd ") + "T" + DateTime.Now.ToString(@" hh mm ss");
                    string filename = string.Concat(filedetails[0], ".", filedetails[1]);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), $"Files/{filetype}", filename);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return Ok($"{filename}");

                }
                return Ok($"Upload Successful.");
            }
            catch (System.Exception ex)
            {
                return BadRequest("Upload Failed: " + ex.Message);
            }
        }

        [HttpPost("Image/{filetype}")]
        public async Task<IActionResult> UploadFile([FromRoute] string filetype, [FromBody] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("File not selected");
            if (string.IsNullOrEmpty(filetype))
                return Content("File type required");
            string[] filedetails = file.FileName.Split('.');
            filedetails[0] = DateTime.Now.ToString(@"yyyy-MM-dd ") + "T" + DateTime.Now.ToString(@" hh mm ss");
            string filename = string.Concat(filedetails[0], ".", filedetails[1]);
            var path = Path.Combine(
                Directory.GetCurrentDirectory(), $"Files/{filetype}",
                filename);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok($"{filename}");
        }
    }
}