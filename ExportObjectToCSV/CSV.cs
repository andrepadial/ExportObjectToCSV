using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;


namespace Rendimento.Layout
{
    public class CSV
    {

        public string Caminho { get; set; }

        public CSV()
        {

        }

        public CSV (string path)
        {
            this.Caminho = path;
        }

        public System.Data.DataTable converterCSVToDatatable(string path, char delimitador)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            int cont = 0;

            using (StreamReader sr = new StreamReader(path))
            {
                
                var substituir = "\"" + "," + "\"";
                var file = sr.ReadLine().TrimEnd().TrimStart().Replace(substituir, ConfigurationManager.AppSettings["delimitador"]);
                file = file.Replace("\"", "");

                
                string[] headers = file.Split(delimitador);

                //Tratamento para colunas duplicadas no CSV
                var valores = (from h in headers
                               group h by h into teste
                               select new
                               {
                                   Coluna = teste,
                                   Contador = teste.Count(),
                               }
                               
                                );

                
                var colunas = valores.Where(x => x.Contador > 1);
                

                foreach (string header in headers)
                {
                    var valida = colunas.Where(x => x.Coluna.FirstOrDefault() == header);

                    if (valida.Count() == 0)
                    {
                        dt.Columns.Add(header);
                    }
                    else
                    {
                        if (cont == 0)
                        {
                            dt.Columns.Add(header);                            
                        }
                        else
                        {
                            dt.Columns.Add(header + cont.ToString());
                        }

                        cont += 1;
                    }

                }

                while (!sr.EndOfStream)
                {                    
                    file = sr.ReadLine().TrimEnd().TrimStart().Replace(substituir, delimitador.ToString());
                    file = file.Replace("\"", "");

                    string[] rows = file.Split(delimitador);

                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < headers.Length; i++)
                    {                      
                        dr[i] = rows[i];                          
                    }


                    dt.Rows.Add(dr);
                }

                return dt;
            }
        }

        public static System.Data.DataTable convertertExcelToDataTable(string FileName)
        {
            System.Data.DataTable dtResult = null;
            int totalSheet = 0; 

            using (OleDbConnection objConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.15.0;Data Source=" + FileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';"))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                System.Data.DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string sheetName = string.Empty;
                if (dt != null)
                {
                    var tempDataTable = (from dataRow in dt.AsEnumerable()
                                         where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase")
                                         select dataRow).CopyToDataTable();
                    dt = tempDataTable;
                    totalSheet = dt.Rows.Count;
                    sheetName = dt.Rows[0]["TABLE_NAME"].ToString();
                }
                cmd.Connection = objConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds, "excelData");
                dtResult = ds.Tables["excelData"];
                objConn.Close();
                return dtResult; 
            }
        }

        public static System.Data.DataTable converterExcelDT(string path)
        {            

            Microsoft.Office.Interop.Excel.Application objXL = null;
            Microsoft.Office.Interop.Excel.Workbook objWB = null;
            DataSet ds = new DataSet();

            try

            {                

                objXL = new Microsoft.Office.Interop.Excel.Application();
                objWB = objXL.Workbooks.Open(path);


                foreach (Microsoft.Office.Interop.Excel.Worksheet objSHT in objWB.Worksheets)
                {

                    int rows = objSHT.UsedRange.Rows.Count;
                    int cols = objSHT.UsedRange.Columns.Count;
                    System.Data.DataTable dt = new System.Data.DataTable();

                    int noofrow = 1;                  

                    for (int c = 1; c <= cols; c++)
                    {
                        string colname = objSHT.Cells[1, c].Text;
                        dt.Columns.Add(colname);
                        noofrow = 2;
                    }

                    
                    for (int r = noofrow; r <= rows; r++)
                    {
                        DataRow dr = dt.NewRow();

                        for (int c = 1; c <= cols; c++)
                        {
                            dr[c - 1] = objSHT.Cells[r, c].Text;
                        }

                        dt.Rows.Add(dr);

                    }

                    ds.Tables.Add(dt);
                }
                

                objWB.Close();
                objXL.Quit();

            }

            catch (Exception ex)
            {
                objWB.Saved = true;
                objWB.Close();
                objXL.Quit();
            }

            return ds.Tables[0];
        }


        public static bool exportObjectToCSV<T>(List<T> list, string separador, string path)
        {
            bool ret = false;

            try
            {

                Type t = list[0].GetType();
                string newLine = Environment.NewLine;
                                

                using (var sw = new StreamWriter(String.Concat(path, "_", DateTime.Now.ToString("ddMMyyyy_HHmmss"), ".csv")))
                {

                    object o = Activator.CreateInstance(t);
                    PropertyInfo[] props = o.GetType().GetProperties();


                    sw.Write(string.Join(separador, props.Select(d => d.Name).ToArray()) + newLine);


                    foreach (T item in list)
                    {

                        var row = string.Join(separador, props.Select(d => item.GetType()
                                                                        .GetProperty(d.Name)
                                                                        .GetValue(item, null)
                                                                        .ToString())
                                                                .ToArray());
                        sw.Write(row + newLine);

                    }
                }

                ret = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Problemas ao gerar arquivo: " + ex.Message.ToString());
            }

            return ret;
        }

        public static string exportObjectToCSV<T>(List<T> list, string separador)
        {
            string lines = String.Empty;

            try
            {

                Type t = list[0].GetType();
                string newLine = Environment.NewLine;
                object o = Activator.CreateInstance(t);
                PropertyInfo[] props = o.GetType().GetProperties();
                                               
                lines = string.Join(separador, props.Select(d => d.Name).ToArray()) + newLine;


                foreach (T item in list)
                {

                    var row = string.Join(separador, props.Select(d => item.GetType()
                                                                    .GetProperty(d.Name)
                                                                    .GetValue(item, null)
                                                                    .ToString())
                                                            .ToArray());
                    lines = String.Concat(lines, row, newLine);

                }
                
                
            }
            catch (Exception ex)
            {
                throw new Exception("Problemas ao gerar arquivo: " + ex.Message.ToString());
            }

            return lines;
        }

        
    }
}
