using DirCastCommon.Models;
using DirCastWebServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer.Controllers
{
    [Route("api/dirs")]
    [ApiController]
    public class DirsController : ControllerBase
    {
        private readonly IDirBrowserService dirBrowserService;

        public DirsController(IDirBrowserService dirBrowserService)
        {
            this.dirBrowserService = dirBrowserService;
        }

        [HttpGet("explore")]
        public DirInfo Explore(string path) => dirBrowserService.GetInfo(path);

        [HttpGet("view")]
        public FileResult View(string path, string name)
        {
            var (fullPath, fileName, contentType) = dirBrowserService.GetFileInfo(new DirFileInfo(name, path));
            return PhysicalFile(fullPath, contentType, fileName, null, new EntityTagHeaderValue($"\"{fullPath.GetHashCode():x}\""), true);
        }
    }
}
