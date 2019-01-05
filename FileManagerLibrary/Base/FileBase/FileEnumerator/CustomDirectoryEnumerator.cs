using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

/*
 * This class file is developed as a backup for performance issue with file access. Not used as of now.
 */
namespace FileManagerLibrary.Base.FileBase.FileEnumerator
{

   /// <summary>
    /// A fast enumerator of files in a directory.  Use this if you need to get attributes for 
    /// all files in a directory.
    /// </summary>
    /// <remarks>
    /// This enumerator is substantially faster than using <see cref="Directory.GetFiles(string)"/>
    /// and then creating a new FileInfo object for each path.  Use this version when you 
    /// will need to look at the attibutes of each file returned (for example, you need
    /// to check each file in a directory to see if it was modified after a specific date).
    /// </remarks>
    public static class FastDirectoryEnumerator
    {
        /// <summary>
        /// Gets <see cref="CustomFileInfo"/> for all the files in a directory.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference (Nothing in VB)
        /// </exception>
        public static IEnumerable<CustomFileInfo> EnumerateFiles(string path)
        {
            return FastDirectoryEnumerator.EnumerateFiles(path, "*");
        }

        /// <summary>
        /// Gets <see cref="CustomFileInfo"/> for all the files in a directory that match a 
        /// specific filter.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is a null reference (Nothing in VB)
        /// </exception>
        public static IEnumerable<CustomFileInfo> EnumerateFiles(string path, string searchPattern)
        {
            return FastDirectoryEnumerator.EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }
        
        /// <summary>
        /// Gets <see cref="CustomFileInfo"/> for all the files in a directory that 
        /// match a specific filter, optionally including all sub directories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <param name="searchOption">
        /// One of the SearchOption values that specifies whether the search 
        /// operation should include all subdirectories or only the current directory.
        /// </param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="searchOption"/> is not one of the valid values of the
        /// <see cref="System.IO.SearchOption"/> enumeration.
        /// </exception>
        public static IEnumerable<CustomFileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return FastDirectoryEnumerator.EnumerateFiles(path, searchPattern, searchOption, true);
        }

        /// <summary>
        /// Gets <see cref="CustomFileInfo"/> for all the files in a directory that 
        /// match a specific filter, optionally including all sub directories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <param name="searchOption">
        /// One of the SearchOption values that specifies whether the search 
        /// operation should include all subdirectories or only the current directory.
        /// </param>
        /// <param name="excludeFolders">
        /// Specifies need to exclude folders from return list. Default is True
        /// </param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="searchOption"/> is not one of the valid values of the
        /// <see cref="System.IO.SearchOption"/> enumeration.
        /// </exception>
        public static IEnumerable<CustomFileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption, bool excludeFolders)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
            {
                throw new ArgumentOutOfRangeException("searchOption");
            }

            string fullPath = Path.GetFullPath(path);

            return new FileEnumerable(fullPath, searchPattern, searchOption,excludeFolders);
        }

        /// <summary>
        /// Gets <see cref="CustomFileInfo"/> for all the files in a directory that match a 
        /// specific filter.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">The search string to match against files in the path.</param>
        /// <returns>An object that implements <see cref="IEnumerable{FileData}"/> and 
        /// allows you to enumerate the files in the given directory.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is a null reference (Nothing in VB)
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is a null reference (Nothing in VB)
        /// </exception>
        public static CustomFileInfo[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            IEnumerable<CustomFileInfo> e = FastDirectoryEnumerator.EnumerateFiles(path, searchPattern, searchOption);
            List<CustomFileInfo> list = new List<CustomFileInfo>(e);

            CustomFileInfo[] retval = new CustomFileInfo[list.Count];
            list.CopyTo(retval);

            return retval;
        }

        /// <summary>
        /// Provides the implementation of the 
        /// <see cref="T:System.Collections.Generic.IEnumerable`1"/> interface
        /// </summary>
        private class FileEnumerable : IEnumerable<CustomFileInfo>
        {
            private readonly string m_path;
            private readonly string m_filter;
            private readonly SearchOption m_searchOption;
            private readonly bool m_IncludeFolders;

            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerable"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerable(string path, string filter, SearchOption searchOption)
            {
                m_path = path;
                m_filter = filter;
                m_searchOption = searchOption;
                m_IncludeFolders = true;
            }


            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerable"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// <param name="includeFolders">Specifies need to exclude folders from return list. Default is True</param>
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerable(string path, string filter, SearchOption searchOption,bool excludeFolders)
            {
                m_path = path;
                m_filter = filter;
                m_searchOption = searchOption;
                m_IncludeFolders = !excludeFolders;
            }


            #region IEnumerable<FileData> Members

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can 
            /// be used to iterate through the collection.
            /// </returns>
            public IEnumerator<CustomFileInfo> GetEnumerator()
            {
                return new FileEnumerator(m_path, m_filter, m_searchOption, m_IncludeFolders);
            }

            #endregion

            #region IEnumerable Members

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be 
            /// used to iterate through the collection.
            /// </returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new FileEnumerator(m_path, m_filter, m_searchOption, m_IncludeFolders);
            }

            #endregion
        }

        /// <summary>
        /// Wraps a FindFirstFile handle.
        /// </summary>
        private sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [DllImport("kernel32.dll")]
            private static extern bool FindClose(IntPtr handle);

            /// <summary>
            /// Initializes a new instance of the <see cref="SafeFindHandle"/> class.
            /// </summary>
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal SafeFindHandle()
                : base(true)
            {
            }

            /// <summary>
            /// When overridden in a derived class, executes the code required to free the handle.
            /// </summary>
            /// <returns>
            /// true if the handle is released successfully; otherwise, in the 
            /// event of a catastrophic failure, false. In this case, it 
            /// generates a releaseHandleFailed MDA Managed Debugging Assistant.
            /// </returns>
            protected override bool ReleaseHandle()
            {
                return FindClose(base.handle);
            }
        }

        /// <summary>
        /// Provides the implementation of the 
        /// <see cref="T:System.Collections.Generic.IEnumerator`1"/> interface
        /// </summary>
        [System.Security.SuppressUnmanagedCodeSecurity]
        private class FileEnumerator : IEnumerator<CustomFileInfo>
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern SafeFindHandle FindFirstFile(string fileName,
                [In, Out] WIN32_FIND_DATA data);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool FindNextFile(SafeFindHandle hndFindFile,
                    [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);

            /// <summary>
            /// Hold context information about where we current are in the directory search.
            /// </summary>
            private class SearchContext
            {
                public readonly string Path;
                public Stack<string> SubdirectoriesToProcess;

                public SearchContext(string path)
                {
                    this.Path = path;
                }
            }

            private string m_path;
            private string m_filter;
            private SearchOption m_searchOption;
            private Stack<SearchContext> m_contextStack;
            private SearchContext m_currentContext;
            private bool m_includeFolders;
            private SafeFindHandle m_hndFindFile;
            private WIN32_FIND_DATA m_win_find_data = new WIN32_FIND_DATA();

            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerator"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerator(string path, string filter, SearchOption searchOption)
            {
                GetFileEnumerator(path, filter, searchOption, false);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="FileEnumerator"/> class.
            /// </summary>
            /// <param name="path">The path to search.</param>
            /// <param name="filter">The search string to match against files in the path.</param>
            /// <param name="searchOption">
            /// <param name="IncludeFolder">Specifies the enumeration should consider folders</param>
            /// One of the SearchOption values that specifies whether the search 
            /// operation should include all subdirectories or only the current directory.
            /// </param>
            public FileEnumerator(string path, string filter, SearchOption searchOption, bool IncludeFolder)
            {
                GetFileEnumerator(path, filter, searchOption, IncludeFolder);
            }

            private void GetFileEnumerator(string path, string filter, SearchOption searchOption, bool IncludeFolders)
            {
                m_path = path;
                m_filter = filter;
                m_searchOption = searchOption;
                m_currentContext = new SearchContext(path);
                m_includeFolders = IncludeFolders;
                if (m_searchOption == SearchOption.AllDirectories)
                {
                    m_contextStack = new Stack<SearchContext>();
                }
            }

            #region IEnumerator<FileData> Members

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            public CustomFileInfo Current
            {
                get { return new CustomFileInfo(m_path, m_win_find_data); }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, 
            /// or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (m_hndFindFile != null)
                {
                    m_hndFindFile.Dispose();
                }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// The element in the collection at the current position of the enumerator.
            /// </returns>
            object System.Collections.IEnumerator.Current
            {
                get { return new CustomFileInfo(m_path, m_win_find_data); }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; 
            /// false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                bool retval = false;

                //If the handle is null, this is first call to MoveNext in the current 
                // directory.  In that case, start a new search.
                if (m_currentContext.SubdirectoriesToProcess == null)
                {
                    if (m_hndFindFile == null)
                    {
                        new FileIOPermission(FileIOPermissionAccess.PathDiscovery, m_path).Demand();

                        string searchPath = Path.Combine(m_path, m_filter);
                        m_hndFindFile = FindFirstFile(searchPath, m_win_find_data);
                        retval = !m_hndFindFile.IsInvalid;
                    }
                    else
                    {
                        //Otherwise, find the next item.
                        retval = FindNextFile(m_hndFindFile, m_win_find_data);
                    }
                }

                //If the call to FindNextFile or FindFirstFile succeeded...
                if (retval)
                {
                    if (m_includeFolders == false)
                    {
                        if (((FileAttributes)m_win_find_data.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            //Ignore folders
                            return MoveNext();
                        }
                    }
                    else
                    {
                        if (((FileAttributes)m_win_find_data.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory && (m_win_find_data.cFileName == "." || m_win_find_data.cFileName == ".."))
                        {
                            return MoveNext();
                        }
                        else
                            return retval; //returns the current Folder location
                    }
                }
                else if (m_searchOption == SearchOption.AllDirectories)
                {
                    if (m_currentContext.SubdirectoriesToProcess == null)
                    {
                        string[] subDirectories = Directory.GetDirectories(m_path);
                        m_currentContext.SubdirectoriesToProcess = new Stack<string>(subDirectories);
                    }

                    if (m_currentContext.SubdirectoriesToProcess.Count > 0)
                    {
                        string subDir = m_currentContext.SubdirectoriesToProcess.Pop();

                        m_contextStack.Push(m_currentContext);
                        m_path = subDir;
                        m_hndFindFile = null;
                        m_currentContext = new SearchContext(m_path);
                        return MoveNext();
                    }

                    //If there are no more files in this directory and we are 
                    // in a sub directory, pop back up to the parent directory and
                    // continue the search from there.
                    if (m_contextStack.Count > 0)
                    {
                        m_currentContext = m_contextStack.Pop();
                        m_path = m_currentContext.Path;
                        if (m_hndFindFile != null)
                        {
                            m_hndFindFile.Close();
                            m_hndFindFile = null;
                        }

                        return MoveNext();
                    }
                }
                return retval;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public void Reset()
            {
                m_hndFindFile = null;
            }

            #endregion
        }
    }
    
    /// <summary>
    /// Contains information about a file returned by the 
    /// <see cref="FastDirectoryEnumerator"/> class.
    /// </summary>
    [Serializable]
    public class CustomFileInfo
    {
        /// <summary>
        /// Attributes of the file.
        /// </summary>
        public FileAttributes Attributes { get; private set; }

        public DateTime CreationTime
        {
            get { return this.CreationTimeUtc.ToLocalTime(); }
        }

        /// <summary>
        /// File creation time in UTC
        /// </summary>
        public DateTime CreationTimeUtc { get; private set; }

        /// <summary>
        /// Gets the last access time in local time.
        /// </summary>
        public DateTime LastAccesTime
        {
            get { return this.LastAccessTimeUtc.ToLocalTime(); }
        }

        /// <summary>
        /// File last access time in UTC
        /// </summary>
        public DateTime LastAccessTimeUtc { get; private set; }

        /// <summary>
        /// Gets the last access time in local time.
        /// </summary>
        public DateTime LastWriteTime
        {
            get { return this.LastWriteTimeUtc.ToLocalTime(); }
        }

        /// <summary>
        /// File last write time in UTC
        /// </summary>
        public DateTime LastWriteTimeUtc { get; private set; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Full path to the file.
        /// </summary>
        public string FullFileName { get; private set; }

        /// <summary>
        /// Full directory Path of the file.
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.FileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFileInfo"/> class.
        /// </summary>
        /// <param name="dir">The directory that the file is stored at</param>
        /// <param name="findData">WIN32_FIND_DATA structure that this
        /// object wraps.</param>
        internal CustomFileInfo(string dir, WIN32_FIND_DATA findData)
        {
            this.Attributes = findData.dwFileAttributes;


            this.CreationTimeUtc = ConvertDateTime(findData.ftCreationTime_dwHighDateTime,
                                                findData.ftCreationTime_dwLowDateTime);

            this.LastAccessTimeUtc = ConvertDateTime(findData.ftLastAccessTime_dwHighDateTime,
                                                findData.ftLastAccessTime_dwLowDateTime);

            this.LastWriteTimeUtc = ConvertDateTime(findData.ftLastWriteTime_dwHighDateTime,
                                                findData.ftLastWriteTime_dwLowDateTime);

            this.Size = CombineHighLowInts(findData.nFileSizeHigh, findData.nFileSizeLow);

            this.FileName = findData.cFileName;

            this.Directory = dir;

            this.FullFileName = System.IO.Path.Combine(dir, findData.cFileName);
        }

        private static long CombineHighLowInts(uint high, uint low)
        {
            return (((long)high) << 0x20) | low;
        }

        private static DateTime ConvertDateTime(uint high, uint low)
        {
            long fileTime = CombineHighLowInts(high, low);
            return DateTime.FromFileTimeUtc(fileTime);
        }
    }

    /// <summary>
    /// Contains information about the file that is found 
    /// by the FindFirstFile or FindNextFile functions.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), BestFitMapping(false)]
    internal class WIN32_FIND_DATA
    {
        public FileAttributes dwFileAttributes;
        public uint ftCreationTime_dwLowDateTime;
        public uint ftCreationTime_dwHighDateTime;
        public uint ftLastAccessTime_dwLowDateTime;
        public uint ftLastAccessTime_dwHighDateTime;
        public uint ftLastWriteTime_dwLowDateTime;
        public uint ftLastWriteTime_dwHighDateTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public int dwReserved0;
        public int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return "File name=" + cFileName;
        }
    }

}
