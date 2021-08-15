using DirCastCommon.Models;
using DirCastWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer.Services
{
    public interface IDirBrowserService
    {
        DirInfo GetInfo(string path);
        (string fullPath, string fileName, string contentType) GetFileInfo(DirFileInfo dirFileInfo);
    }
}
