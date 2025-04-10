using Autodesk.DesignScript.Runtime;
using Dynamo.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Path = System.IO.Path;

namespace DSCore.IO
{
    /// <summary>
    ///     Methods for working with Files.
    /// </summary>
    public static class FileSystem
    {
        #region file methods

        /// <summary>
        /// Returns absolute path from the given path. If the given path is 
        /// relative path then it is resolved with respect to the current 
        /// workspace. If file doesn't exist at the relative path but exists
        /// at the given hintPath then hintPath is returned.
        /// </summary>
        /// <param name="path">Relative path or full path</param>
        /// <param name="hintPath">Last resolved path</param>
        /// <returns>Absolute path</returns>
        [IsVisibleInDynamoLibrary(false)]
        public static string AbsolutePath(string path, string hintPath = null)
        {
            //If the path is absolute path no need to transform.
            if (Path.IsPathRooted(path)) return path;

            var session = Dynamo.Events.ExecutionEvents.ActiveSession;
            if (session != null && !string.IsNullOrEmpty(session.CurrentWorkspacePath))
            {
                var parent = Path.GetDirectoryName(session.CurrentWorkspacePath);
                var filepath = Path.Combine(parent, path);
                //If hint path is null or file exists at this location return the computed path
                //If hint path doesn't exist then the relative path might be for write operation.
                if (FileSystem.FileExists(filepath) || string.IsNullOrEmpty(hintPath) || !FileSystem.FileExists(hintPath))
                    return Path.GetFullPath(filepath);
            }

            return string.IsNullOrEmpty(hintPath) ? path : hintPath;
        }

        /// <summary>
        /// Creates File object from given file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static FileInfo FileFromPath(string path)
        {
            return new FileInfo(AbsolutePath(path));
        }

        /// <summary>
        ///     Reads a text file and returns the contents as a string.
        /// </summary>
        /// <param name="file"> File object to read text from</param>
        /// <returns name="string">Contents of the text file.</returns>
        /// <search>read file,text,file</search>
        public static string ReadText(FileInfo file)
        {
            Analytics.TrackTaskFileOperationEvent(
                                  file.Name,
                                  Actions.Read,
                                  Convert.ToInt32(file.Length));

            return System.IO.File.ReadAllText(file.FullName);
        }

        /// <summary>
        ///  Moves a specified file to a new location
        /// </summary>
        /// <param name="path">String representation of existing path</param>
        /// <param name="newPath">String representation of new path</param>
        /// <param name="overwrite">Toggle to overwrite existing files</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        public static void MoveFile(string path, string newPath, bool overwrite = false)
        {
            if (overwrite && FileExists(newPath))
                DeleteFile(newPath);
            System.IO.File.Move(path, newPath);
        }

        /// <summary>
        ///   Deletes the specified file.
        /// </summary>
        /// <param name="path">File path to delete</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        public static void DeleteFile(string path)
        {
            Analytics.TrackTaskFileOperationEvent(
                      Path.GetFileName(path),
                      Actions.Delete,
                      Convert.ToInt32(path.Length));

            System.IO.File.Delete(path);
        }

        /// <summary>
        ///     Copies a file.
        /// </summary>
        /// <param name="file">File object to copy</param>
        /// <param name="destinationPath">String representation of destination file path</param>
        /// <param name="overwrite">Toggle to overwrite existing files</param>
        /// <returns name="bool">Node performs a task, return true of copy action succeed.</returns>
        public static bool CopyFile(FileInfo file, string destinationPath, bool overwrite = false)
        {
            try
            {
                if (Path.GetDirectoryName(destinationPath) != string.Empty)
                {
                    file.CopyTo(destinationPath, overwrite);
                }
                else
                {
                    throw new FileNotFoundException(Properties.Resources.InvalidDestinationPathErrorMessage, destinationPath);
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CA2200 // Rethrow to preserve stack details
                throw ex;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }
            return true;

        }

        /// <summary>
        ///     Determines if a file exists at the given path.
        /// </summary>
        /// <param name="path">String representing a file path</param>
        /// <returns name="bool">True if file exists, false if it doesn't</returns>
        /// <search>filepath</search>
        public static bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        ///     Write the text content to a file specified by the path
        /// </summary>
        /// <param name="filePath">Path to write to</param>
        /// <param name="text">Text content</param>
        /// <returns name="void">No output</returns>
        /// <search>write file,text,file,filepath</search>
        public static void WriteText(string filePath, string text)
        {
            Analytics.TrackTaskFileOperationEvent(
                                  Path.GetFileName(filePath),
                                  Actions.Write,
                                  Convert.ToInt32(filePath.Length));

            var fullpath = AbsolutePath(filePath);
            System.IO.File.WriteAllText(fullpath, text);
        }

        /// <summary>
        /// Append the text content to a file specified by the path
        /// </summary>
        /// <param name="filePath">Path to write to</param>
        /// <param name="text">Text content</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        /// <search>append file,write file,text,file,filepath</search>
        public static void AppendText(string filePath, string text)
        {
            var fullpath = AbsolutePath(filePath);
            System.IO.File.AppendAllText(fullpath, text);
        }

        /// <summary>
        ///     Combines multiple strings into a single file path.
        /// </summary>
        /// <param name="strings">Strings to combine into a path</param>
        /// <returns name="string">Combined file path</returns>
        public static string CombinePath(params string[] strings)
        {
            return Path.Combine(strings);
        }

        /// <summary>
        /// Returns the extension from a file path.
        /// </summary>
        /// <param name="path">Path to get extension of</param>
        /// <returns name="string">Extension of file</returns>
        public static string FileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        ///     Changes the extension of a file path.
        /// </summary>
        /// <param name="path">Path to change extension of</param>
        /// <param name="newExtension">String representation of new extension</param>
        /// <returns name="string">File path with changed extension</returns>
        public static string ChangePathExtension(string path, string newExtension)
        {
            return Path.ChangeExtension(path, newExtension);
        }

        /// <summary>
        /// Returns the directory name of a file path.
        /// </summary>
        /// <param name="path">Path to get directory information of</param>
        /// <returns name="string">Directory name of file path</returns>
        /// <search>directorypath</search>
        public static string DirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Returns the file name of a file path.
        /// </summary>
        /// <param name="path">Path to get the file name of</param>
        /// <param name="withExtension">Toggle to include extension in result</param>
        /// <returns name="string">File name from file path</returns>
        public static string FileName(string path, bool withExtension = true)
        {
            return withExtension ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        ///     Determines whether or not a file path contains an extension.
        /// </summary>
        /// <param name="path">Path to check for an extension</param>
        /// <returns name="bool">True if file path contains extension, false if it doesn't</returns>
        public static bool FileHasExtension(string path)
        {
            return Path.HasExtension(path);
        }

        /// <summary>
        /// Will return a list of files and directories that are contained within a given directory. An optional searchString can be used to filter the results.
        /// </summary>
        /// <param name="directory">Directory to get contents of</param>
        /// <param name="searchString">Search string used to filter results</param>
        /// <param name="includeSubdirectories">Set to true to include files and folders in subdirectories (recursive) or set to false to include results from top-level of given directory only.</param>
        /// <returns name="files">Resulting files from query</returns>
        /// <returns name="directories">Resulting directories from query</returns>
        [MultiReturn("files", "directories")]
        public static Dictionary<string, IList> GetDirectoryContents(DirectoryInfo directory, string searchString = "*.*", bool includeSubdirectories = false)
        {
            var searchOptions = SearchOption.TopDirectoryOnly;
            if (includeSubdirectories == true) searchOptions = SearchOption.AllDirectories;

            return new Dictionary<string, IList>
            {
                { "files", directory.EnumerateFiles(searchString, searchOptions).Select(x => x.FullName).ToList() },
                { "directories", directory.EnumerateDirectories(searchString, searchOptions).Select(x => x.FullName).ToList() }
            };
        }
        #endregion

        #region directory methods
        /// <summary>
        ///     Copies a directory to a destination location.
        /// </summary>
        /// <param name="directory">Directory to copy</param>
        /// <param name="destinationPath">Destination of the copy operation on disk</param>
        /// <param name="overwriteFiles">Toggle to overwrite existing directory</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        public static void CopyDirectory(DirectoryInfo directory, string destinationPath, bool overwriteFiles = false)
        {
            if (!FileExists(destinationPath))
                System.IO.Directory.CreateDirectory(destinationPath);

            foreach (var file in directory.EnumerateFiles())
            {
                var newFilePath = Path.Combine(destinationPath, file.Name);
                CopyFile(file, newFilePath, overwriteFiles);
            }

            foreach (var dir in directory.EnumerateDirectories())
            {
                var newDirPath = Path.Combine(destinationPath, dir.Name);
                CopyDirectory(dir, newDirPath, overwriteFiles);
            }
        }

        /// <summary>
        ///     Deletes a directory.
        /// </summary>
        /// <param name="path">Path to a directory on disk</param>
        /// <param name="recursive">Whether or not to delete all contents of the directory, defaults to false.</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        public static void DeleteDirectory(string path, bool recursive = false)
        {
            Directory.Delete(path, recursive);
        }

        /// <summary>
        ///     Determines if a directory exists at the given path.
        /// </summary>
        /// <param name="path">Path to a directory on disk</param>
        /// <returns name="bool">True if directory exists, false if it doesn’t</returns>
        /// <search>directorypath</search>
        public static bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        [IsVisibleInDynamoLibrary(false)]
        public static DirectoryInfo DirectoryFromPath(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
            return new DirectoryInfo(path);
        }

        /// <summary>
        ///     Moves a directory to a new location.
        /// </summary>
        /// <param name="path">String representation of existing path</param>
        /// <param name="newPath">String representation of new path</param>
        /// <param name="overwriteFiles">Toggle to overwrite existing files</param>
        /// <returns name="void">Node performs a task, doesn’t produce an output </returns>
        public static void MoveDirectory(string path, string newPath, bool overwriteFiles = false)
        {
            if (!DirectoryExists(newPath))
            {
                Directory.Move(path, newPath);
                return;
            }

            var info = new DirectoryInfo(path);
            foreach (var file in info.EnumerateFiles())
            {
                var newFilePath = Path.Combine(newPath, file.Name);
                MoveFile(file.FullName, newFilePath, overwriteFiles);
            }

            foreach (var dir in info.EnumerateDirectories())
            {
                var newDirPath = Path.Combine(newPath, dir.Name);
                MoveDirectory(dir.FullName, newDirPath, overwriteFiles);
            }
        }
        #endregion



    }


}
