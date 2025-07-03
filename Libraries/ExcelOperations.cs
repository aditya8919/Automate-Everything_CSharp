using EComerceProject.Libraries.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.IO;

namespace EComerceProject.Libraries
{
  public class ExcelOperations
  {
    private Microsoft.Office.Interop.Excel.Application excelApplication;
    private Microsoft.Office.Interop.Excel.Workbooks excelWorkbooks;
    public Microsoft.Office.Interop.Excel.Workbook excelWorkbook;
    private Microsoft.Office.Interop.Excel.Sheets excelSheets = null;
    public Microsoft.Office.Interop.Excel.Worksheet excelSheet = null;
    private int lastRow;
    private int lastColumn;
    private String filePath = string.Empty;
    public Microsoft.Office.Interop.Excel.Range excelUsedRange = null;

    public ExcelOperations()
    { }

    /// <summary>
    ///Constructor
    /// </summary>
    /// <param name="filePath">string : file path to fetch the file.</param>
    public ExcelOperations(String filePath)
    {
      this.filePath = filePath;
      this.excelApplication = new Microsoft.Office.Interop.Excel.Application();
      this.excelApplication.Visible = false;
      this.excelWorkbooks = this.excelApplication.Workbooks;
      this.excelWorkbook = this.excelWorkbooks.Open(filePath);
      this.excelSheets = this.excelWorkbook.Sheets;
      this.excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)this.excelSheets[1];
      this.excelUsedRange = this.excelSheet.UsedRange;
      this.lastRow = this.excelSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Row;
      this.lastColumn = this.excelSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Column;
    }

    private Microsoft.Office.Interop.Excel.Application Application
    {
      get
      {
        return this.excelApplication;
      }
    }

    private Microsoft.Office.Interop.Excel.Workbooks Workbooks
    {
      get
      {
        return this.excelWorkbooks;
      }
    }

    private Microsoft.Office.Interop.Excel.Workbook Workbook
    {
      get
      {
        return this.excelWorkbook;
      }
    }

    private Microsoft.Office.Interop.Excel.Sheets Sheets
    {
      get
      {
        return this.excelSheets;
      }
    }

    private Microsoft.Office.Interop.Excel.Worksheet Worksheet
    {
      get
      {
        return this.excelSheet;
      }
    }
    public int LastRow
    {
      get
      {
        return this.excelSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Row;
      }
    }

    public int LastColumn
    {
      get
      {
        return this.excelSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Column;
      }
    }
    #region ExcelUtility

    public void SetValue(int rowNumber, int columnNumber, string value)
    {
      this.excelSheet.Cells[rowNumber, columnNumber] = value;
    }

    public string GetValue(int rowNumber, int columnNumber)
    {
      //return this.excelSheet.Cells[rowNumber, columnNumber].Value;
      return this.excelSheet.Cells[rowNumber, columnNumber].GetType().Name;
    }

    public void Save()
    {
      this.Workbook.Save();
    }

    /// <summary>
    /// Close the workbook,excelworkbook,excelworkbooks and excelApplication.
    /// </summary>
    public void Close()
    {
      //this.Save();
      this.Workbook.Close();
      this.Workbooks.Close();
      this.excelWorkbook = null;
      this.excelWorkbooks = null;
      this.excelApplication = null;
    }

    /// <summary>
    /// Getting Row Data from excel
    /// </summary>
    /// <param name="rowNumber">integer : row number from which to end read the rows data</param>
    /// <returns>list number of rows read.</returns>
    public List<string> GetRowData(int rowNumber)
    {
      List<string> rowData = new List<string>();
      int columnsCount = this.LastColumn;

      for (int columnCounter = 1; columnCounter <= columnsCount; columnCounter++)
      {
        var value = this.CellData(rowNumber, columnCounter);
        if (!(value == null))
          rowData.Add(Convert.ToString(value).Trim());
        else
          rowData.Add(string.Empty);
      }

      return rowData;
    }

    /// <summary>
    /// Gets column data.
    /// </summary>
    /// <param name="columnNumber">integer : column number to be read</param>
    /// <returns>returns list of column data</returns>
    public List<string> GetColumnData(int columnNumber)
    {
      List<string> columnData = new List<string>();
      int rowsCount = this.LastRow;
      for (int rowCounter = 2; rowCounter <= rowsCount; rowCounter++)
      {
        var value = this.CellData(rowCounter, columnNumber);
        if (!(value == null))
          columnData.Add(Convert.ToString(value).Trim());
        else
          columnData.Add(string.Empty);
      }

      return columnData;
    }

    public List<string> ColumnDataByHeader(string columnHeaderName)
    {
      int columnIndex = ColumnIndex(columnHeaderName);
      List<string> columnData = GetColumnData(columnIndex + 1);

      return columnData;
    }

    public int ColumnIndex(string HeaderName)
    {
      List<string> headerRow = GetRowData(1);
      return headerRow.IndexOf(HeaderName);
    }

    public List<List<string>> ExcelRowDataAsList()
    {
      List<string> rowData = new List<string>();
      List<List<string>> columnData = new List<List<string>>();
      int rowsCount = this.excelUsedRange.Rows.Count;

      for (int rowCounter = 1; rowCounter <= rowsCount; rowCounter++)
        columnData.Add(GetRowData(rowCounter));

      return columnData;
    }

    public List<List<string>> ExcelColumnDataAsList()
    {
      List<string> columnData = new List<string>();
      List<List<string>> rowData = new List<List<string>>();
      int columnsCount = this.excelUsedRange.Columns.Count;

      for (int columnCounter = 1; columnCounter <= columnsCount; columnCounter++)
        rowData.Add(GetColumnData(columnCounter));

      return rowData;
    }

    public dynamic CellData(int rowIndex, int columnIndex)
    {
      return ((dynamic)((Microsoft.Office.Interop.Excel.Range)this.excelSheet.Cells[rowIndex, columnIndex]).Value2);
    }

    public string CellData(int rowIndex, string columnHeaderName)
    {
      int columnIndex = ColumnIndex(columnHeaderName);
      return CellData(rowIndex, columnIndex + 1);
    }

    public Dictionary<string, string> GetTwoColumnValuesAsDictionary(string Header1, string Header2)
    {
      List<string> accountsList = this.ColumnDataByHeader(Header1);
      List<string> valueList = this.ColumnDataByHeader(Header2);

      Dictionary<string, string> excelAccountValues = new Dictionary<string, string>();
      for (int accountCounter = 0, valueCounter = 0; accountCounter < accountsList.Count; accountCounter++, valueCounter++)
      {
        if (!(accountsList[accountCounter] == null || accountsList[accountCounter] == "" || accountsList[accountCounter] == string.Empty))
          excelAccountValues.Add(accountsList[accountCounter], valueList[valueCounter]);
      }


      return excelAccountValues;
    }


    public bool GetTwoColumnValuesAsDictionaryForDrillDown(string Header1, string Header2, string account, int guiValue)
    {
      List<string> accountsList = this.ColumnDataByHeader(Header1);
      List<string> valueList = this.ColumnDataByHeader(Header2);
      bool isMatch = false;
      Dictionary<string, string> excelAccountValues = new Dictionary<string, string>();
      for (int accountCounter = 0, valueCounter = 0; accountCounter < accountsList.Count; accountCounter++, valueCounter++)
      {
        if (!(accountsList[accountCounter] == null || accountsList[accountCounter] == "" || accountsList[accountCounter] == string.Empty) &&
            (accountsList[accountCounter] == account && valueList[valueCounter].Contains(guiValue.ToString())))
        {
          isMatch = true;
          break;
        }
      }
      return isMatch;
    }


    public double ReadExcelData(string filePath, string account)
    {
      double value = 0.0;

      // filePath = Config.SolutionPath + filePath;
      Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
      Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
      Excel.Worksheet xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);
      Excel.Range range = xlWorksheet.UsedRange;

      for (int rowCount = 1; rowCount <= range.Rows.Count; rowCount++)
      {
        var v = (range.Cells[rowCount, 1] as Microsoft.Office.Interop.Excel.Range).Text;
        if (!string.IsNullOrEmpty((string?)(range.Cells[rowCount, 1] as Excel.Range).Text))
        {
          if (((string)(range.Cells[rowCount, 1] as Microsoft.Office.Interop.Excel.Range).Value).Contains(account))
          {
            value = Convert.ToDouble((range.Cells[rowCount, 2] as Microsoft.Office.Interop.Excel.Range).Value);
            break;
          }
        }

      }

      return value;
    }

    /// <summary>
    /// Method to open excel file
    /// </summary>
    public void OpenExcelFile()
    {
      try
      {
        excelApplication.Visible = true;
        excelApplication.Workbooks.Open(filePath);
      }
      catch (Exception ex)
      {
        UtilityBase.Log.WriteLine("Exception while opening the excel file : " + ex.Message);
        throw ex;
      }
    }

    public static void WriteValuesToExcel(int rowNumber, int columnNumber, string value)
    {
      Excel.Application myExcelApp;
      Microsoft.Office.Interop.Excel.Workbooks myExcelWorkbooks;
      Microsoft.Office.Interop.Excel.Workbook myExcelWorkbook;
      Microsoft.Office.Interop.Excel.Worksheet MySheet = null;
      Microsoft.Office.Interop.Excel.Sheets sheets = null;
      int lastRow;
      object misValue = System.Reflection.Missing.Value;

      myExcelApp = new Microsoft.Office.Interop.Excel.Application();
      myExcelApp.Visible = true;
      myExcelWorkbooks = myExcelApp.Workbooks;
      String fileName = "C:\\book1.xls";
      myExcelWorkbook = myExcelWorkbooks.Open(fileName);
      sheets = myExcelWorkbook.Sheets;
      MySheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets[1];
      lastRow = MySheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell).Row;

      lastRow += 1;
      MySheet.Cells[rowNumber, columnNumber] = "";
      myExcelWorkbook.Save();
    }

    /// <summary>
    /// Desc : Method to get specific excel data, 'PIV Websheet' specific method
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="currencyValue"></param>
    /// <param name="integerValue"></param>
    /// <param name="textValue"></param>
    /// <param name="dateValue"></param>
    /// <param name="decimalValue"></param>
    /// <param name="percentageValue"></param>
    /// <param name="listValue"></param>
    public void GetDataFromExcelPIVWebsheet(string filePath, out double currencyValue, out string integerValue, out string textValue, out string dateValue, out string decimalValue, out string percentageValue, out string listValue)
    {
      currencyValue = 0.0;
      integerValue = string.Empty;
      textValue = string.Empty;
      dateValue = string.Empty;
      decimalValue = string.Empty;
      percentageValue = string.Empty;
      listValue = string.Empty;


      Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
      Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
      Microsoft.Office.Interop.Excel.Worksheet xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);
      Microsoft.Office.Interop.Excel.Range range = xlWorksheet.UsedRange;

      int rowCount = 1;
      System.Threading.Thread.Sleep(2000);
      for (int colCount = 1; colCount <= range.Columns.Count; colCount++)
      {
        if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("currency"))
          currencyValue = Convert.ToDouble((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("integer"))
          integerValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("decimal"))
          decimalValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("text"))
          textValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("percentage"))
          percentageValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("List"))
          listValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
        else if (((string)(range.Cells[rowCount, colCount] as Microsoft.Office.Interop.Excel.Range).Value).Equals("date"))
        {
          dateValue = Convert.ToString((range.Cells[rowCount + 1, colCount] as Microsoft.Office.Interop.Excel.Range).Value);
          dateValue = Convert.ToDateTime(dateValue).ToString("d/MM/yyyy");
        }
      }
    }

    // <summary>
    /// Desc : To get mapping data from csv file loaded
    /// </summary>
    /// <param name="filePath"</param>
    /// <returns>true if data from csv is copied into data table else returns false</returns>
    public static System.Data.DataTable GetDataFromCSV(string filePath)
    {
      try
      {
        HelperUtility.KillProcessByName("EXCEL");
        string[] rows = File.ReadAllLines(filePath);
        int columnLength = rows[0].Split(',').Length;
        List<string> columns = new List<string>();
        //int counter = 0;
        for (int i = 1; i <= columnLength; i++)
        {
          columns.Add("Column" + i.ToString());
        }
                System.Data.DataTable dt = new System.Data.DataTable();
        DataRow columnRow = dt.NewRow();
        foreach (var columnName in columns)
        {
          dt.Columns.Add(columnName);
        }
        string[] rowValues = null;
        DataRow dr = dt.NewRow();
        for (int row = 0; row < rows.Length; row++)
        {
          rowValues = rows[row].Split(',');
          dr = dt.NewRow();
          dr.ItemArray = rowValues;
          dt.Rows.Add(dr);
        }
        return dt;
      }
      catch (Exception ex)
      {
        UtilityBase.Log.WriteLine("Exception : " + ex.Message);
        UtilityBase.Log.WriteLine("Failed to get data from excel file" + ex.StackTrace, Log.LogType.FAIL, true, true);
        return null;
      }
    }

    /// <summary>
    /// Method to get all data from excel file into datatable
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="startRowNumber"></param>
    /// <returns>DataTable with all data in excel file which is present on provided path.</returns>
    public static System.Data.DataTable GetExcelData(string filePath, int startRowNumber, string sheetName = "GIData")
    {
      try
      {
        using (OleDbConnection conn = new OleDbConnection())
        {
          System.Data.DataTable dt = new System.Data.DataTable();
          conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath
          + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=0'";
          using (OleDbCommand comm = new OleDbCommand())
          {
            comm.CommandText = "Select * From [" + sheetName + "$A" + startRowNumber + ":CZ]";
            comm.Connection = conn;
            using (OleDbDataAdapter da = new OleDbDataAdapter())
            {
              da.SelectCommand = comm;
              da.Fill(dt);
              return dt;
            }
          }
        }
      }
      catch (Exception ex)
      {
        UtilityBase.Log.WriteLine("Exception : " + ex.Message);
        UtilityBase.Log.WriteLine("Failed to get data from excel file" + ex.StackTrace, Log.LogType.FAIL, true, true);
        return null;
      }
    }
    #endregion ExcelUtility
  }
}
