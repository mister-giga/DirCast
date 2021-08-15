using System;
using System.Collections.Generic;
using System.Text;

namespace DirCastCommon.Models
{
    public record DirInfo(string Path, DirInfo[] SubDirs, DirFileInfo[] Files)
    {
        public string GetName() => Path;// System.IO.Path.GetDirectoryName(Path);
    }
}
