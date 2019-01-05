using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class DirectoryInfoExt
    {
        /// <summary>
        /// Finds the Folder is having a subfolders or not.
        /// </summary>
        /// <returns></returns>
        public static bool HasSubDirectories(this DirectoryInfo source)
        {
            bool retval = false;
            foreach (var item in source.EnumerateFileSystemInfos())
            {
                retval = true;
                break;
            }
            return retval;
        }
    }
}
