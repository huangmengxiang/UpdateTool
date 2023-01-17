using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XFaceUpdateTool.Util;

namespace XFaceUpdateTool
{
    class DevicePool
    {
        public static readonly string EXCEL_FILE_NAME = "device.xlsx";
        private Regex reg = new Regex("[a-fA-F0-9]{12}");
        private ExcelEdit myExcel = null;
        private List<string> devLst = new List<string>();
        private int _mode = 0;  //0-无设备列表excel文件  1-有设备列表文件
        public DevicePool()
        {
            LogWriter.Instance.Save(string.Format("try init dev pool\r\n"), DateTime.Now);
            ReadExcel();

          //  Init();
        }

        public int Mode 
        {
            get { return _mode; }
        }

        public int ReadExcel()
        {
            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\" + EXCEL_FILE_NAME;
            try
            {
                using (ExcelHelper excelHelper = new ExcelHelper(fileName))
                {
                    System.Data.DataTable dt = excelHelper.ExcelToDataTable("MySheet", true);
                    _mode = 1;
                    SaveData(dt);

                  //  PrintData(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return 0;
        }

        void SaveData(System.Data.DataTable data)
        {
            if (data == null) return;

            for (int i = 0; i < data.Rows.Count; ++i)
            {
                if (data.Columns.Count < 2)
                {
                    continue;
                }
           //     for (int j = 0; j < data.Columns.Count; ++j)
                int j = 1;
                string devno = (string)data.Rows[i][j];
                Match match = reg.Match(devno);
                if (match.Success == true && devno.Length == 12)
                {
                    devLst.Add(devno.ToUpper());
                }
                else
                {
                    LogWriter.Instance.Save(string.Format("invalid devno={0}.\r\n", devno), DateTime.Now);
                }
                
              //      Console.Write("{0} ", data.Rows[i][j]);
               // Console.Write("\n");
            }
        }

        public int Init()
        {
            try
            {
                myExcel = new ExcelEdit();
                myExcel.Create();
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Save(ex.Message, DateTime.Now);
                return 1;
            }

            string fileName = System.IO.Directory.GetCurrentDirectory() + "\\" + EXCEL_FILE_NAME;  
            if (0 != myExcel.Open(fileName))
            {
                LogWriter.Instance.Save(string.Format("open file={0}.failed!\r\n", fileName), DateTime.Now);
                myExcel.Close();
                return 1;
            }
            _mode = 1;
            try
            {
                Microsoft.Office.Interop.Excel.Worksheet ws = myExcel.GetSheet("dev");
                int iRowCount = ws.UsedRange.Rows.Count;
                int iColCount = ws.UsedRange.Columns.Count;
                string devno = "";

                //生成行数据
                Microsoft.Office.Interop.Excel.Range range;

                for (int iRow = 1; iRow <= iRowCount; iRow++)
                {
                    //    DataRow dr = dt.NewRow();
                    for (int iCol = 1; iCol <= iColCount; iCol++)
                    {
                        range = (Microsoft.Office.Interop.Excel.Range)ws.Cells[iRow, iCol];
                        devno = (range.Value2 == null) ? "" : range.Text.ToString();
                        Match match = reg.Match(devno);
                        if (match.Success == true && devno.Length == 12)
                        {
                            devLst.Add(devno.ToUpper());
                        }
                        else
                        {
                            LogWriter.Instance.Save(string.Format("invalid devno={0}.\r\n", devno), DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                myExcel.Close();
            }
            
            return 0;
        }

        public bool isValid(string devno)
        {
            bool ret = false;
            if(0 == _mode)
            {
                return true;
            }

            string no = devno.ToUpper();
            if(devLst.Exists(item=>item==no))
            {
                ret = true;
            }

            return ret;
        }

        public bool addDevno(string devno)
        {
            if(true == isValid(devno))
            {
                return true;
            }
            Match match = reg.Match(devno);
            if (match.Success == true && devno.Length == 12)
            {
                devLst.Add(devno.ToUpper());
                return true;
            }
            
            return false;
        }
        static void PrintData(System.Data.DataTable data)
        {
            if (data == null) return;
            for (int i = 0; i < data.Rows.Count; ++i)
            {
                for (int j = 0; j < data.Columns.Count; ++j)
                    Console.Write("{0} ", data.Rows[i][j]);
                Console.Write("\n");
            }
        }
        static void TestExcelRead(string file)
        {
            try
            {
                using (ExcelHelper excelHelper = new ExcelHelper(file))
                {
                    System.Data.DataTable dt = excelHelper.ExcelToDataTable("MySheet", true);
                    PrintData(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }


}
