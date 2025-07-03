using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComerceProject.Libraries
{
  /// <summary>
  /// File Interface
  /// </summary>
  public interface IFile
  {
    /// <summary>
    /// Verifies file exists
    /// </summary>
    /// <param name="path">file path</param>
    /// <returns>true if file exists</returns>
    bool Exists(string path);

    /// <summary>
    /// Reads file data
    /// </summary>
    /// <param name="path">file path</param>
    /// <returns>file data</returns>
    string ReadFile(string path);

    /// <summary>
    /// Writes a file
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="isAppend">is append</param>
    /// <param name="content">content</param>
    void WriteFile(string fileName, bool isAppend, string content);

    /// <summary>
    /// Copies file
    /// </summary>
    /// <param name="source">source location</param>
    /// <param name="destination">destination</param>
    /// <param name="overWrite">Overwrite</param>
    void Copy(string source, string destination, bool overWrite);

    /// <summary>
    /// Moves file from one location to another
    /// </summary>
    /// <param name="fileName">original location</param>
    /// <param name="newFileName">new location</param>
    void Move(string fileName, string newFileName);

    /// <summary>
    /// Reads all text within file
    /// </summary>
    /// <param name="fileName">filename</param>
    /// <returns></returns>
    string ReadAllText(string fileName);

    /// <summary>
    /// Gets line from file that contains content
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="content">content</param>
    /// <returns>line if found else empty string</returns>
    string GetLine(string fileName, string content);

    /// <summary>
    /// Deletes file
    /// </summary>
    /// <param name="fileName">file name</param>
    void Delete(string fileName);
  }

  /// <summary>
  /// Native implementation of File
  /// </summary>
  public class FileWrapper : IFile
  {
    /// <summary>
    /// Verifies file exists
    /// </summary>
    /// <param name="path">file path</param>
    /// <returns>true if file exists</returns>
    public bool Exists(string path)
    {
      return File.Exists(path);
    }

    /// <summary>
    /// Reads file data
    /// </summary>
    /// <param name="path">file path</param>
    /// <returns>file data</returns>
    public string ReadFile(string path)
    {
      using (StreamReader sr = new StreamReader(path))
      {
        return sr.ReadToEnd();
      }
    }

    /// <summary>
    /// Writes a file
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="isAppend">is append</param>
    /// <param name="content">content</param>
    public void WriteFile(string fileName, bool isAppend, string content)
    {
      //StreamWriter creates new file with specified mode i.e Append or Overwrite
      using (StreamWriter sw = new StreamWriter(fileName, isAppend))
      {
        sw.Write(content);
        sw.Flush();
      }
    }

    /// <summary>
    /// Copies file
    /// </summary>
    /// <param name="source">source location</param>
    /// <param name="destination">destination</param>
    /// <param name="overWrite">Overwrite</param>
    public void Copy(string source, string destination, bool overWrite)
    {
      File.Copy(source, destination, overWrite);
    }

    /// <summary>
    /// Moves file from one location to another
    /// </summary>
    /// <param name="fileName">original location</param>
    /// <param name="newFileName">new location</param>
    public void Move(string fileName, string newFileName)
    {
      File.Move(fileName, newFileName);
    }

    /// <summary>
    /// Reads all text within file
    /// </summary>
    /// <param name="fileName">filename</param>
    /// <returns></returns>
    public string ReadAllText(string fileName)
    {
      return File.ReadAllText(fileName, Encoding.Default);
    }

    /// <summary>
    /// Gets line from file that contains content
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="content">content</param>
    /// <returns>line if found else empty string</returns>
    public string GetLine(string fileName, string content)
    {
      using (StreamReader reader = new StreamReader(fileName))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          if (line.ToLower().Replace(" ", "").Contains(content.ToLower()))
          {
            return line;
          }
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Deletes file
    /// </summary>
    /// <param name="fileName">file name</param>
    public void Delete(string fileName)
    {
      File.Delete(fileName);
    }
  }

  /// <summary>
  /// Directory Interface
  /// </summary>
  public interface IDirectory
  {
    /// <summary>
    /// Gets files from a directory
    /// </summary>
    /// <param name="DirectoryPath">Directory path</param>
    /// <param name="SearchPattern">Search pattern</param>
    /// <returns>Array of files names</returns>
    string[] GetFiles(string DirectoryPath, string SearchPattern);

    /// <summary>
    /// Gets files from a directory
    /// </summary>
    /// <param name="DirectoryPath">Directory path</param>
    /// <param name="SearchPattern">Search pattern</param>
    /// <param name="searchOption">Search Option</param>
    /// <returns>Array of files names</returns>
    string[] GetFiles(string DirectoryPath, string SearchPattern, SearchOption searchOption);

    /// <summary>
    /// Verifies whether directory exits
    /// </summary>
    /// <param name="directoryName">directory name</param>
    /// <returns>true if exists</returns>
    bool Exists(string directoryName);

    /// <summary>
    /// Creates a directory
    /// </summary>
    /// <param name="foldername">full name of folder</param>
    void CreateDirectory(string foldername);

    /// <summary>
    /// Deletes folder
    /// </summary>
    /// <param name="folderName">folder Name</param>
    void Delete(string folderName);
  }

  /// <summary>
  /// Native implementation of Directory
  /// </summary>
  public class DirectoryWrapper : IDirectory
  {
    /// <summary>
    /// Gets files from a directory
    /// </summary>
    /// <param name="DirectoryPath">Directory path</param>
    /// <param name="SearchPattern">Search pattern</param>
    /// <returns>Array of files names</returns>
    public string[] GetFiles(string DirectoryPath, string SearchPattern)
    {
      return Directory.GetFiles(DirectoryPath, SearchPattern);
    }

    /// <summary>
    /// Gets files from a directory
    /// </summary>
    /// <param name="DirectoryPath">Directory path</param>
    /// <param name="SearchPattern">Search pattern</param>
    /// <param name="searchOption">Search Option</param>
    /// <returns>Array of files names</returns>
    public string[] GetFiles(string DirectoryPath, string SearchPattern, SearchOption searchOption)
    {
      return Directory.GetFiles(DirectoryPath, SearchPattern, searchOption);
    }

    /// <summary>
    /// Verifies whether directory exits
    /// </summary>
    /// <param name="directoryName">directory name</param>
    /// <returns>true if exists</returns>
    public bool Exists(string directoryName)
    {
      return Directory.Exists(directoryName);
    }

    /// <summary>
    /// Creates a directory
    /// </summary>
    /// <param name="foldername">full name of folder</param>
    public void CreateDirectory(string foldername)
    {
      Directory.CreateDirectory(foldername);
    }

    /// <summary>
    /// Deletes folder
    /// </summary>
    /// <param name="folderName">folder Name</param>
    public void Delete(string folderName)
    {
      Directory.Delete(folderName, true);
    }
  }

  /// <summary>
  /// Interface for Path
  /// </summary>
  public interface IPath
  {
    /// <summary>
    /// Gets directory name
    /// </summary>
    /// <param name="fileName">full name</param>
    /// <returns>directory name</returns>
    string GetDirectoryName(string fileName);
  }

  /// <summary>
  /// Native wrapper class for Path
  /// </summary>
  public class PathWrapper : IPath
  {
    /// <summary>
    /// Gets directory name
    /// </summary>
    /// <param name="fileName">full name</param>
    /// <returns>directory name</returns>
    public string GetDirectoryName(string fileName)
    {
      return Path.GetDirectoryName(fileName);
    }
  }
}
