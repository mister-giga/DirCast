using DirCastCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirCastWebServer
{
    public static class DirectoryInfoExtensions
    {
        public static DirFileInfo[] GetDirFileInfos(this DirectoryInfo directoryInfo, string rootDir) =>
            directoryInfo
            .EnumerateFiles()
            .Select(x => new DirFileInfo(x.Name, Path.GetRelativePath(rootDir, x.DirectoryName)))
            .ToArray();

        public static DirInfo[] GetDirInfos(this DirectoryInfo directoryInfo, string rootDir, int stepsUntil0)
        {
            if (stepsUntil0 <= 0)
                return null;

            return directoryInfo
                .EnumerateDirectories()
                .Select(x => new DirInfo(Path.GetRelativePath(rootDir, x.FullName), x.GetDirInfos(rootDir, stepsUntil0 - 1), x.GetDirFileInfos(rootDir)))
                .ToArray();
        }
    }
}
