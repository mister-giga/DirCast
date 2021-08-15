using System;
using System.Collections.Generic;
using System.Text;

namespace DirCastCommon.Models
{

    public record DirFileInfo(string Name, string Path)
    {
        public string GetExtension() => System.IO.Path.GetExtension(Name);
    }
}
