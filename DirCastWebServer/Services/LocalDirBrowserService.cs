using DirCastCommon.Models;
using DirCastWebServer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeTypes;
using System.IO;

namespace DirCastWebServer.Services
{
    public class LocalDirBrowserService : IDirBrowserService
    {
        private readonly AppSettings appSettings;

        public LocalDirBrowserService(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public (string fullPath, string fileName, string contentType) GetFileInfo(DirFileInfo dirFileInfo)
        {
            var fileName = dirFileInfo.Name;
            if (fileName.IsNullOrWhiteSpace())
                throw new ApplicationException("File name must be provided");

            if (fileName.Contains("/"))
                throw new ApplicationException("File name must not contain \"/\"");

            var contentType = MimeTypeMap.GetMimeType(Path.GetExtension(dirFileInfo.Name));
            var dirInfo = GetDirectoryInfo(dirFileInfo.Path);
            var fullPath = Path.Combine(dirInfo.FullName, fileName);

            if (!File.Exists(fullPath))
                throw new ApplicationException($"File \"{fileName}\" does not exist at \"{dirFileInfo.Path}\"");

            return (fullPath: fullPath, fileName: fileName, contentType);
        }

        public DirInfo GetInfo(string path)
        {
            var directoryInfo = GetDirectoryInfo(path);

            return new DirInfo(path, directoryInfo.GetDirInfos(GetRootDir(), 2), directoryInfo.GetDirFileInfos(GetRootDir()));
        }

        string GetRootDir() => appSettings.GetDir();

        DirectoryInfo GetDirectoryInfo(string path)
        {
            if (path.IsNullOrWhiteSpace())
                throw new ApplicationException("Path is not provided. For root path use \".\"");
            if (path.StartsWith('/'))
                throw new ApplicationException("Path must be relative");
            if (path.Contains(".."))
                throw new ApplicationException("Path must not contain \"..\"");

            var directoryInfo = new DirectoryInfo(Path.Combine(GetRootDir(), path));
            if (!directoryInfo.Exists)
                throw new ApplicationException($"Directory \"{path}\" does not exist");

            return directoryInfo;
        }
    }
}
