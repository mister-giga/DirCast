using DirCastWebServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer.Controllers
{
    [Route("")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoot() => new ContentResult
        {
            StatusCode = 200,
            Content = System.Text.Json.JsonSerializer.Serialize(new
            {
                WebServerUrl = InitializationService.Url,
                RootBrowingDirectory = InitializationService.Dir,
                ApiDocUrl = Path.Combine(InitializationService.Url, "swagger")
            }, options: new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            }),
            ContentType = "application/json"
        };
    }
}
