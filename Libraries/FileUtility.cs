using EComerceProject.Libraries.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace EComerceProject.Libraries
{
  public class FileUtility : UtilityBase
  {
    private static IFile file = new FileWrapper();
    private static IDirectory directory = new DirectoryWrapper();
    private static IPath path = new PathWrapper();

    private static IFile FileWrapper { set { file = value; } }

    private static IDirectory DirectoryWrapper { set { directory = value; } }

    private static IPath PathWrapper { set { path = value; } }

    /// <summary>
    /// Check if specified file is existing
    /// </summary>
    /// <param name="filename">File full path and name</param>
    /// <returns>true or false</returns>
    public static bool FileExists(string filename)
    {
      try
      {
        if (file.Exists(filename))
          return true;
        else
          return false;
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred - " + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// This Method will verify that the File with given search pattern Exists or not
    /// </summary>
    /// <param name="DirectoryPath">DirectoryPath</param>
    /// <param name="SearchPattern">SearchPattern- This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but doesn't support regular expressions.</param>
    /// <returns>true if exists else false</returns>
    public static bool FileExists(string DirectoryPath, string SearchPattern)
    {
      try
      {
        string[] files = directory.GetFiles(DirectoryPath, SearchPattern);

        if (files.Length != 0)
        {
          Log.WriteLine(string.Format("Successfully Verified that the File with the Search Pattern {0} Exists at the path: {1}", SearchPattern, DirectoryPath), Log.LogType.INFO);
          return true;
        }
        else
        {
          Log.WriteLine(string.Format("Failed to verify that the File with the Search Pattern {0} Exists at the path: {1}", SearchPattern, DirectoryPath), Log.LogType.ERROR);
          return false;
        }
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred - " + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Check if specified folder is existing
    /// </summary>
    /// <param name="foldername">folder full path and name</param>
    /// <returns>true or false</returns>
    public static bool FolderExists(string foldername)
    {
      try
      {
        if (directory.Exists(foldername))
          return true;
        else
          return false;
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred - " + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Method to get path of user directory Downloads folder or local Downloads folder path.
    /// </summary>
    /// <param name="needUserDirectoryDownloadpath"> when its 'true' user will get path of user directory Downloads folder; else user will get local Downloads folder path</param> 
    /// <returns>path of Downloads folder in string format</returns>
    public static string GetDownloadsFolderPath(bool needUserDirectoryDownloadpath = true)
    {
      string downloadsPath = "";
      try
      {
        if (needUserDirectoryDownloadpath)
        {
          string path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
          string downloadPath = "";
          if (path.Contains("OneDrive"))
          {
            string[] strPathSplits = path.Split('\\');
            for (int i = 0; i < strPathSplits.Length; i++)
            {
              if (strPathSplits[i].Contains("OneDrive"))
                continue;
              else if (i == strPathSplits.Length - 1)
                downloadPath = downloadPath + strPathSplits[i];
              else
                downloadPath = downloadPath + strPathSplits[i] + "\\";
            }
            path = downloadPath;
            downloadsPath = Path.Combine(path, "Downloads");
          }
          else
            downloadsPath = Path.Combine(path, "Downloads");
        }
        else
        {
          downloadsPath = @"C:\Downloads";
        }
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception occurred while getting downloads folder path : " + ex.ToString(), Log.LogType.ERROR);
      }
      return downloadsPath;
    }

    /// <summary>
    /// Method to delete a file.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>true: if file is deleted successfully, else returns false.</returns>
    public static bool DeleteFile(string fileName)
    {
      bool flag = true;
      try
      {
        File.Delete(fileName);
        Thread.Sleep(500);

        if (!File.Exists(fileName))
          Log.WriteLine("File has been deleted successfully", Log.LogType.PASS);
        else
        {
          Log.WriteLine("Unable to delete given file.", Log.LogType.FAIL);
          flag = false;
        }
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred while deleteing a file : " + ex.ToString(), Log.LogType.ERROR);
        flag = false;
      }
      return flag;
    }

    /// <summary>
    /// Creates the folder.
    /// </summary>
    /// <param name="foldername">Full path along with Folder Name which need to be created</param>
    public static bool CreateFolder(string foldername)
    {
      try
      {
        if (FolderExists(foldername))
        {
          //Log.WriteLine("Directory already exists", Log.LogType.ERROR);
          return false;
        }

        directory.CreateDirectory(foldername);

        return FolderExists(foldername);
      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception Occurred - " + ex.ToString(), Log.LogType.ERROR);
        throw;
      }
    }

    /// <summary>
    /// Compares/Validate excel sheet data
    /// </summary>
    /// <param name="startRowNumber"></param>
    /// <param name="sourceExcelFile"></param>
    /// <param name="destExcelFile"></param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    public static bool ValidateExcelData(int startRowNumber, string sourceExcelFile, string destExcelFile, string sheetName = "Sheet1")
    {
      bool compareResult = true;
      try
      {
        string excelQuery = "Select * From [" + sheetName + "$A" + startRowNumber + ":CZ]";

        #region Source Excel Data Fetching
        OpenSaveExcel(sourceExcelFile, sheetName);
        DataTable dtSource = GetDataFromExcel(filePath: sourceExcelFile, sqlQuery: excelQuery);
        #endregion

        #region Destination Excel Data Fetching
        OpenSaveExcel(destExcelFile, sheetName);
        DataTable dtDestination = GetDataFromExcel(filePath: destExcelFile, sqlQuery: excelQuery);
        #endregion

        #region DataCompare
        compareResult = CompareDataTable(dtSource, dtDestination);
        #endregion

        return compareResult;
      }
      catch (Exception ex)
      {
        Log.WriteLine(ex.StackTrace);
        return false;
      }

    }

    /// <summary>
    /// Open and save excel in order to remove data access
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sheetName"></param>
    public static void OpenSaveExcel(string filePath, string sheetName)
    {
      #region Open & Save Excel
      try
      {
        HelperUtility.KillProcessByName("EXCEL");

        Excel.Application ExcelApp = null;
        Excel.Workbook ExcelWorkBook = null;
        Excel.Sheets ExcelSheets = null;

        Excel.Worksheet MySheet = null;

        ExcelApp = new Excel.Application
        {
          Visible = false
        };
        ExcelWorkBook = ExcelApp.Workbooks.Open(filePath, 0, false, 1, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);

        ExcelSheets = ExcelWorkBook.Worksheets;
        MySheet = (Excel.Worksheet)ExcelSheets.get_Item(sheetName);
        ExcelWorkBook.Save();
        ExcelWorkBook.Close();
        ExcelApp.Workbooks.Close();
      }
      catch (Exception ex)
      {
        Log.WriteLine(ex.StackTrace);
      }
      #endregion
    }

    /// <summary>
    /// Fetch data from Excel through SQL query
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sqlQuery"></param>
    /// <returns></returns>
    public static DataTable GetDataFromExcel(string filePath, string sqlQuery)
    {
      OleDbConnection con = null;
      try
      {
        Log.WriteLine("Fetch excel data from provided filepath : " + filePath);
        HelperUtility.KillProcessByName("EXCEL");
        DataSet ds = new DataSet();
        bool found = File.Exists(filePath);
        string constring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;IMEX=1;HDR=No;\"";
        con = new OleDbConnection(constring + "");
        OleDbDataAdapter da = new OleDbDataAdapter(sqlQuery, con);
        da.Fill(ds);
        DataTable dt = ds.Tables[0];
        con.Close();
        dt.Columns.Add();

        Log.WriteLine("Fetched excel data into data table from filepath : " + filePath);
        for (int col = dt.Columns.Count - 1; col >= 0; col--)
        {
          bool removeColumn = true;
          foreach (DataRow row in dt.Rows)
          {
            if (!row.IsNull(col))
            {
              removeColumn = false;
              break;
            }
          }
          if (removeColumn)
            dt.Columns.RemoveAt(col);
        }

        return dt;
      }
      catch (Exception ex)
      {

        Log.WriteLine("Failed to get data from path : " + filePath + "\n" + ex.StackTrace);
        return null;
      }
      finally
      {
        //   change and added the dispose for the variable

        con.Close();
        if (con != null)
        {
          con.Dispose();
          con = null;
        }
      }
    }

    /// <summary>
    /// Compare 2 data tables
    /// </summary>
    /// <param name="actualData"></param>
    /// <param name="expectedData"></param>
    /// <returns></returns>
    public static bool CompareDataTable(DataTable actualData, DataTable expectedData)
    {
      bool compareResult = true;
      try
      {
        #region DataCompare

        int refineDataSourceColumnCount = actualData.Columns.Count;
        int refineDataDestColumnCount = expectedData.Columns.Count;

        int refineDataSourceRowCount = actualData.Rows.Count;
        int refineDataDestRowCount = expectedData.Rows.Count;

        //Validating columns count
        if (refineDataSourceColumnCount != refineDataDestColumnCount)
        {
          Log.WriteLine("Columns Count Missmatch : 'Column Count - " + refineDataSourceColumnCount + "' Column Count - " + refineDataDestColumnCount);
          compareResult = false;
        }

        //Validating rows count
        if (refineDataSourceRowCount != refineDataDestRowCount)
        {
          Log.WriteLine("Rows Count Missmatch : 'Row Count - " + refineDataSourceRowCount + "' Row Count - " + refineDataSourceRowCount);
          compareResult = false;
        }

        #region Comparing cell values
        //Comparing cell values
        if (actualData != null && expectedData != null)
        {
          int sourceRow = 0, destRow = 0;
          int sourceColumn = 0, destColumn = 0;

          for (; sourceRow < refineDataSourceRowCount && destRow < refineDataDestRowCount;)
          {
            if (sourceColumn == refineDataSourceColumnCount && destColumn == refineDataDestColumnCount)
            {
              sourceColumn = 0;
              destColumn = 0;
              sourceRow++;
              destRow++;
            }
            else
            {
              if (String.IsNullOrEmpty(actualData.Rows[sourceRow][sourceColumn].ToString()) && String.IsNullOrEmpty(expectedData.Rows[destRow][destColumn].ToString()))
              {
                // ExtentLogger.LogInfo("Empty cell in both the file : Skiped comparison for the cell");
              }

              #region --------------------Kavita------- Code to replace '()' with '-' and handling comma vales in both expected and actual files

              if (actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Contains(",") || actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Contains(".0000")) // --------check comma in actual file 
              {
                try
                {
                  if (actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Contains("(")) // If value contains both () and Comma
                  {
                    // First split brackets
                    string[] numbers = actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Split('(');
                    numbers = numbers[1].Split(')');
                    if (char.IsDigit(numbers[0], 1))
                    {
                      // Convert comma to number
                      double number1 = Convert.ToDouble(numbers[0]);
                      int numberint1 = Convert.ToInt32(number1);
                      actualData.Rows[sourceRow][sourceColumn] = numberint1.ToString();
                    }
                  }
                  else
                  {
                    if (char.IsDigit(actualData.Rows[sourceRow][sourceColumn].ToString().Trim(), 1)) // If value contains comma but no numbers eg. labels
                    {
                      double number = Convert.ToDouble(actualData.Rows[sourceRow][sourceColumn].ToString().Trim());
                      int numberint = Convert.ToInt32(number);
                      actualData.Rows[sourceRow][sourceColumn] = numberint.ToString();
                    }
                  }
                }
                catch (Exception ex)
                {
                  Log.WriteLine("Exception occured while replacing value containing , (comma) : " + ex.Message);
                }
              }
              if (expectedData.Rows[destRow][destColumn].ToString().Trim().Contains(",") || expectedData.Rows[destRow][destColumn].ToString().Trim().Contains(".0000")) // -------- check comma in expected file
              {
                try
                {

                  if (expectedData.Rows[destRow][destColumn].ToString().Trim().Contains("(")) // If value contains both () and Comma
                  {
                    // First split brackets
                    string[] numbers = expectedData.Rows[destRow][destColumn].ToString().Trim().Split('(');
                    numbers = numbers[1].Split(')');
                    if (char.IsDigit(numbers[0], 1))
                    {
                      // Convert comma to number
                      double number1 = Convert.ToDouble(numbers[0]);
                      int numberint1 = Convert.ToInt32(number1);
                      expectedData.Rows[destRow][destColumn] = numberint1.ToString();
                    }
                  }
                  else
                  {
                    if (char.IsDigit(expectedData.Rows[destRow][destColumn].ToString().Trim(), 1)) // If value contains comma but no numbers eg. labels
                    {
                      double number = Convert.ToDouble(expectedData.Rows[destRow][destColumn].ToString().Trim());
                      int numberint = Convert.ToInt32(number);
                      expectedData.Rows[destRow][destColumn] = numberint.ToString();
                    }
                  }
                }
                catch (Exception ex)
                {
                  Log.WriteLine("Exception occured while replacing value containing , (comma) : " + ex.Message);
                }
              }
              if (actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Contains("(")) // -------------- check (brackets) in actual file
              {

                string[] numbers = actualData.Rows[sourceRow][sourceColumn].ToString().Trim().Split('(');
                numbers = numbers[1].Split(')');
                try
                {
                  if (char.IsDigit(numbers[0], 1))
                  {
                    double number = Convert.ToDouble(numbers[0]);
                    int numberint = Convert.ToInt32(number);
                    actualData.Rows[sourceRow][sourceColumn] = "-" + numberint.ToString();
                  }
                }
                catch (Exception ex)
                {
                  Log.WriteLine("Value observed with bracket is : " + actualData.Rows[sourceRow][sourceColumn].ToString());
                  Log.WriteLine("Exception occured while replacing value containing () to - sign : " + ex.Message);
                }
              }
              if (expectedData.Rows[destRow][destColumn].ToString().Trim().Contains("(")) // ----- Check (brackets) in expected files
              {
                string[] numbers = expectedData.Rows[destRow][destColumn].ToString().Trim().Split('(');
                numbers = numbers[1].Split(')');
                try
                {
                  if (char.IsDigit(numbers[0], 1))
                  {
                    double number = Convert.ToDouble(numbers[0]);

                    int numberint = Convert.ToInt32(number);
                    expectedData.Rows[destRow][destColumn] = "-" + numberint.ToString();
                  }
                }
                catch (Exception ex)
                {
                  Log.WriteLine("Value observed with bracket is : " + expectedData.Rows[destRow][destColumn].ToString().Trim());
                  Log.WriteLine("Exception occured while replacing value containing () to - sign : " + ex.Message);
                }
              }
              #endregion------------- End of Code replacing ',' and '()' '-' ------ Kavita

              if (actualData.Rows[sourceRow][sourceColumn].ToString().Trim() != expectedData.Rows[destRow][destColumn].ToString().Trim())
              {
                // Code to skip verification related to Modified by, last accessed by, created by, modified date etc. as those are dynamic values
                if (actualData.Rows[0][sourceColumn].ToString().Trim() == expectedData.Rows[0][destColumn].ToString().Trim())
                {
                  if (actualData.Rows[0][sourceColumn].ToString().Trim().Contains("Modified") || actualData.Rows[0][sourceColumn].ToString().Trim().Contains("Last Accessed By") || actualData.Rows[0][sourceColumn].ToString().Trim().Contains("Created By"))
                  {
                    Log.WriteLine("Skipping dynamic column data verification for Columns  : " + actualData.Rows[0][sourceColumn].ToString().Trim());
                  }
                  else
                  {
                    Log.WriteLine("Data Mismatched for Actual Data '" + (actualData.Rows[sourceRow][sourceColumn]).ToString() + "' against Expected Data '" + expectedData.Rows[destRow][destColumn].ToString() + "'");
                    compareResult = false;
                  }
                }
                else
                {
                  Log.WriteLine("Data Mismatched for Actual Data '" + (actualData.Rows[sourceRow][sourceColumn]).ToString() + "' against Expected Data '" + expectedData.Rows[destRow][destColumn].ToString() + "'");
                  compareResult = false;
                }
              }
              else
              {
                //ExtentLogger.LogInfo("Data Verification successful for 'Source Cell' : '" + actualData.Rows[sourceRow][sourceColumn].ToString() + "' -'Destination Cell' : '" + expectedData.Rows[destRow][destColumn].ToString() + "'");
              }
              sourceColumn++;
              destColumn++;
            }
          }
        }
        else
        {
          compareResult = false;
          if (actualData == null)
            Log.WriteLine("Actual Datatable is null");
          else
            Log.WriteLine("Expected Datatable is null");
        }

        #endregion

        return compareResult;

        #endregion
      }
      catch (Exception ex)
      {
        Log.WriteLine("Fail to compare data : " + ex.StackTrace);
        return false;
      }
    }

    /// <summary>
    /// Converts excel data into data table
    /// </summary>
    /// <param name="excelFilePath"></param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    public static DataTable GetDataTable(string excelFilePath, string sheetName = "Sheet1")
    {
      try
      {
        string excelQuery = "Select * From [" + sheetName + "$A0:AZ]";
        OpenSaveExcel(excelFilePath, sheetName);
        return GetDataFromExcel(excelFilePath, excelQuery);

      }
      catch (Exception ex)
      {
        Log.WriteLine("Not able to fetch data from excel : " + excelFilePath + " /n StackTrace :" + ex.StackTrace);
        return null;
      }
    }
    public static bool CompareDataOfSchedulesWithExpectedFile(DataTable dtSource, DataTable dtDestination, string selectedScheduleLeftNavigation)
    {
      bool isSameData = true;
      bool apportionedCheck = true;

      try
      {
        // Clean up objects.
        DataRow[] rows1 = dtSource.Select();
        DataRow[] rows2 = dtDestination.Select();

        DataRow datarow1, datarow2;
        int i, j, rowCount = 0;
        rowCount = rows1.Length;
        for (i = 0, j = 0; i < rowCount; i++)
        {
          datarow1 = rows1[i];
          string column1 = datarow1[0].ToString().Trim();

          datarow2 = rows2[j];
          string column2 = datarow2[0].ToString().Trim();
          int n;
          for (n = 0; n < datarow1.ItemArray.Length; n++)
          {
            string columnName1 = dtSource.Columns[n].ToString().Trim();
            string columnName2 = dtDestination.Columns[n].ToString().Trim();

            string value1 = datarow1.ItemArray[n].ToString().Trim();
            string value2 = datarow2.ItemArray[n].ToString().Trim();

            //temporary adjmnt.
            if (columnName2 == "Apportioned Loss" && value1.CompareTo(value2) != 0)
            {
              apportionedCheck = false;
            }

            if (value1.Contains("  "))
            {
              value1 = value1.Replace("  ", " ");
            }
            if (value2.Contains("  "))
            {
              value2 = value2.Replace("  ", " ");
            }
            if (selectedScheduleLeftNavigation == "Assets" && value1.Contains("TDA"))
            {
              string[] splitter = value1.Split(' ');
              value1 = splitter[0].Trim();
            }
            if (value1.ToLower().CompareTo(value2.ToLower()) != 0)
            {
              if (column1.Equals(""))
              {
                column1 = datarow1[1].ToString().Trim();
              }
              Log.WriteLine("MISMATCH FOUND FOR ACCOUNT- " + column1 + " for Column Name : " + columnName1, Log.LogType.FAIL);
              Log.WriteLine("Expected Value: " + value2 + " , Actual Value: " + value1);

              isSameData = false;
            }
          }
          j++;
        }
        if (!apportionedCheck)
          Log.WriteLine("Defect : Bug #656 raised - Value for “Apportioned Loss” is displayed on the aggregated Losses schedule but it is not displayed correctly on the excel report & drill down", Log.LogType.FAIL);

      }
      catch (Exception ex)
      {
        Log.WriteLine("Exception occur " + ex.Message, Log.LogType.ERROR);
        return false;
      }

      return isSameData;
    }

    /// <summary>
    /// Desc : Method to copy folder 
    /// </summary>
    /// <param name="sourcePath">string : source path including foldername</param>
    /// <param name="targetPath">string : target path including foldername</param>
    /// <returns>true : if folder copy operation completes successfully, else returns false</returns>
    public static bool CopyFolder(string sourcePath, string targetPath)
    {
      try
      {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
          Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
          File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
