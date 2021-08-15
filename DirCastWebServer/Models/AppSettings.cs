using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer.Models
{
    public class AppSettings
    {
        public string Dir { get; }

        public AppSettings(string dir)
        {
            Dir = dir;
        }

        public string GetDir()
        {
            if (Dir.IsNullOrWhiteSpace())
                throw new ApplicationException("Value for \"dir\" is not set!");

            if (!IsPathValidRootedLocal(Dir))
                throw new ApplicationException($"Value for \"dir\" specified as \"{Dir}\" is not valid path!");

            if (!Directory.Exists(Dir))
                throw new ApplicationException($"Directory \"{Dir}\" specified by \"dir\" does not exist");


            return Dir;

            static bool IsPathValidRootedLocal(string pathString) => Uri.TryCreate(pathString, UriKind.Absolute, out var pathUri) && pathUri != null && pathUri.IsLoopback;
        }
    }
}
