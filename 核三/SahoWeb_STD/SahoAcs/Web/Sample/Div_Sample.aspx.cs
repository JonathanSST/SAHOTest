using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class Div_Sample : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 取得RecordID值之ModifyBackupInfo_Style1
        /// </summary>
        /// <param name="TableName">Table名稱</param>
        /// <param name="RecordID">備份的ID值</param>
        /// <returns></returns>
        #region ModifyBackupInfo_Style1
        public static string ModifyBackupInfo_Style1(string TableName, string RecordID)
        {
            Table MainTable;
            TableRow MainTr;
            TableCell MainTd;

            #region Get ModifyBackupTable & FieldTable

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable ModifyBackupTable;
            DataTable FieldTable;

            #region Process-GetModifyBackupTable
            sql = @" SELECT 
                     ModifyBackup.BackupTime, 
                     ModifyBackup.BackupUserID+' : '+SysUser.UserName AS BackupUserID,
                     ModifyBackup.TableName,
                     CASE (ModifyBackup.ModifyMode)
                       WHEN 'M' THEN '編輯'
                       WHEN 'D' THEN '刪除' END AS ModifyMode,
                     ModifyBackup.RecordID,
                     ModifyBackup.FieldInfo,
                     ModifyBackup.DataInfo
                     FROM B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = ModifyBackup.BackupUserID
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
            liSqlPara.Add("S:" + TableName.Trim());
            liSqlPara.Add("S:" + RecordID.Trim());
            oAcsDB.GetDataTable("ModifyBackupTable", sql, liSqlPara, out ModifyBackupTable);
            #endregion

            liSqlPara.Clear();

            #region Process-GetFieldTable
            sql = @" SELECT DISTINCT 
                     FieldNameList.FieldName , FieldNameList.ChtName 
                     FROM  B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_FieldNameList AS FieldNameList ON ModifyBackup.FieldInfo like '%' + FieldNameList.FieldName + '%'
	                       AND FieldNameList.TableName = ModifyBackup.TableName OR FieldNameList.tablename = 'Common'
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
            liSqlPara.Add("S:" + TableName.Trim());
            liSqlPara.Add("S:" + RecordID.Trim());
            oAcsDB.GetDataTable("FieldTable", sql, liSqlPara, out FieldTable);
            #endregion

            #endregion

            MainTable = new Table();

            #region Header
            MainTr = new TableRow();

            #region Fixed-Header
            MainTd = new TableCell();
            MainTd.Text = "異動時間";
            MainTd.Width = 150;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTd = new TableCell();
            MainTd.Text = "異動者帳號";
            MainTd.Width = 150;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);


            MainTd = new TableCell();
            MainTd.Text = "異動模式";
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);
            #endregion

            #region GetTotleField
            List<string> FieldList = new List<string>();
            foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
            {
                string[] FieldSplit;
                FieldSplit = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < FieldSplit.Length; i++)
                {
                    if (!FieldList.Contains(FieldSplit[i].ToString()))
                        FieldList.Add(FieldSplit[i].ToString());
                }
            }
            #endregion

            #region Flexible-Header

            for (int i = 0; i < FieldList.Count; i++)
            {
                MainTd = new TableCell();

                foreach (DataRow FieldRow in FieldTable.Rows)
                {
                    if (string.Compare(FieldRow["FieldName"].ToString(), FieldList[i].ToString(), false) == 0)
                    {
                        MainTd.Text = FieldRow["ChtName"].ToString();
                        break;
                    }
                }
                MainTd.Attributes.Add("nowrap", "norwap");
                MainTd.Style.Add("background-color", "#016B0A");
                MainTd.Style.Add("color", "#FBFBFB");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);
            }

            #endregion
            MainTable.Controls.Add(MainTr);
            #endregion

            if (ModifyBackupTable.Rows.Count > 0)
            {
                #region DataRow
                foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
                {
                    MainTr = new TableRow();
                    #region Fixed-Data
                    MainTd = new TableCell();
                    MainTd.Text = DateTime.Parse(ModifyBackupRow["BackupTime"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("background-color", "#F5FFF5");
                    MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("border-top", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);

                    MainTd = new TableCell();
                    MainTd.Text = ModifyBackupRow["BackupUserID"].ToString();
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("background-color", "#F5FFF5");
                    MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("border-top", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);


                    MainTd = new TableCell();
                    MainTd.Text = ModifyBackupRow["ModifyMode"].ToString();
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("background-color", "#F5FFF5");
                    MainTd.Style.Add("border-top", "1px solid black");
                    MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);
                    #endregion

                    #region Flexible-Data

                    string[] FieldInfoSplit;
                    string[] DataInfoSplit;

                    FieldInfoSplit = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.None);
                    DataInfoSplit = ModifyBackupRow["DataInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.None);

                    for (int i = 0; i < FieldList.Count; i++)
                    {
                        MainTd = new TableCell();

                        for (int x = 0; x < FieldInfoSplit.Length; x++)
                        {
                            if (string.Compare(FieldInfoSplit[x].ToString(), FieldList[i].ToString(), false) == 0)
                            {
                                MainTd.Text = DataInfoSplit[x].ToString();
                                break;
                            }
                        }
                        MainTd.Attributes.Add("nowrap", "norwap");
                        MainTd.Style.Add("border-top", "1px solid black");
                        if (i != FieldList.Count - 1)
                            MainTd.Style.Add("border-right", "1px solid black");
                        MainTd.Style.Add("padding", "3px");
                        MainTr.Controls.Add(MainTd);
                    }

                    #endregion
                    MainTable.Controls.Add(MainTr);
                }
                #endregion
            }
            else
            {
                MainTr = new TableRow();

                MainTd = new TableCell();
                MainTd.Text = "尚無備份資料";
                MainTd.ColumnSpan = 3;
                MainTd.Attributes.Add("nowrap", "norwap");
                MainTd.Style.Add("background-color", "#F5FFF5");
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);

                MainTable.Controls.Add(MainTr);
            }

            #region MainTable Style
            MainTable.Style.Add("word-break", "break-all");
            MainTable.Style.Add("border", "1px solid black");
            MainTable.Attributes.Add("cellpadding", "0");
            MainTable.Attributes.Add("cellspacing", "0");
            #endregion

            #region 讀取Main_RenderControl
            StringWriter StringWriter = new StringWriter();
            HtmlTextWriter HtmlWriter = new HtmlTextWriter(StringWriter);
            MainTable.RenderControl(HtmlWriter);
            #endregion

            return StringWriter.ToString();
        }
        #endregion

        /// <summary>
        /// 取得RecordID值之ModifyBackupInfo_Style2
        /// </summary>
        /// <param name="_width">設定ModifyBackup視窗大小</param>
        /// <param name="TableName">Table名稱</param>
        /// <param name="RecordID">備份的ID值</param>
        /// <returns></returns>
        #region ModifyBackupInfo_Style2
        public static string ModifyBackupInfo_Style2(int _width, string TableName, string RecordID)
        {
            Table MainTable, DigitalTable;
            TableRow MainTr, DigitalTr;
            TableCell MainTd, DigitalTd;
            MainTable = new Table();

            MainTr = new TableRow();
            #region Header
            MainTd = new TableCell();
            MainTd.Text = "異動時間";
            MainTd.Width = 150;
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTd.HorizontalAlign = HorizontalAlign.Center;
            MainTr.Controls.Add(MainTd);

            MainTd = new TableCell();
            MainTd.Text = "異動者帳號";
            MainTd.Width = 150;
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTd.HorizontalAlign = HorizontalAlign.Center;
            MainTr.Controls.Add(MainTd);


            MainTd = new TableCell();
            MainTd.Text = "異動模式";
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);
            #endregion
            MainTable.Controls.Add(MainTr);

            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "";
            List<string> liSqlPara = new List<string>();
            DataTable ModifyBackupTable;
            DataTable FieldTable;

            #region Process-GetModifyBackupTable
            sql = @" SELECT 
                     ModifyBackup.BackupTime, 
                     ModifyBackup.BackupUserID+' : '+SysUser.UserName AS BackupUserID,
                     ModifyBackup.TableName,
                     CASE (ModifyBackup.ModifyMode)
                         WHEN 'M' THEN '編輯'
                         WHEN 'D' THEN '刪除' END AS ModifyMode,
                     ModifyBackup.RecordID,
                     ModifyBackup.FieldInfo,
                     ModifyBackup.DataInfo
                     FROM B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_SysUser AS SysUser ON SysUser.UserID = ModifyBackup.BackupUserID
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
            liSqlPara.Add("S:" + TableName.Trim());
            liSqlPara.Add("S:" + RecordID.Trim());
            oAcsDB.GetDataTable("ModifyBackupTable", sql, liSqlPara, out ModifyBackupTable);
            #endregion

            liSqlPara.Clear();

            #region Process-GetFieldTable
            sql = @" SELECT DISTINCT 
                     FieldNameList.FieldName , FieldNameList.ChtName 
                     FROM  B00_ModifyBackup AS ModifyBackup
                     LEFT JOIN B00_FieldNameList AS FieldNameList ON ModifyBackup.FieldInfo like '%' + FieldNameList.FieldName + '%'
	                       AND FieldNameList.TableName = ModifyBackup.TableName OR FieldNameList.tablename = 'Common'
                     WHERE ModifyBackup.TableName = ? AND ModifyBackup.RecordID = ? ";
            liSqlPara.Add("S:" + TableName.Trim());
            liSqlPara.Add("S:" + RecordID.Trim());
            oAcsDB.GetDataTable("FieldTable", sql, liSqlPara, out FieldTable);
            #endregion

            #region GetTotleField
            List<string> FieldList = new List<string>();
            foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
            {
                string[] FieldSplit;
                FieldSplit = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < FieldSplit.Length; i++)
                {
                    if (!FieldList.Contains(FieldSplit[i].ToString()))
                        FieldList.Add(FieldSplit[i].ToString());
                }
            }
            #endregion

            foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
            {
                MainTr = new TableRow();
                #region Master
                MainTd = new TableCell();
                MainTd.Text = DateTime.Parse(ModifyBackupRow["BackupTime"].ToString()).ToString("yyyy/MM/dd hh:mm:ss");
                MainTd.Width = 150;
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);

                MainTd = new TableCell();
                MainTd.Text = ModifyBackupRow["BackupUserID"].ToString();
                MainTd.Width = 150;
                MainTd.Style.Add("border-right", "1px solid black");
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);


                MainTd = new TableCell();
                MainTd.Text = ModifyBackupRow["ModifyMode"].ToString();
                MainTd.Style.Add("border-top", "1px solid black");
                MainTd.Style.Add("padding", "3px");
                MainTr.Controls.Add(MainTd);
                #endregion
                MainTr.Attributes.Add("OnClick", "ReverseDisplay('Div_" + ModifyBackupRow["BackupTime"].ToString() + "')");
                MainTable.Controls.Add(MainTr);

                MainTr = new TableRow();
                #region Digital
                MainTd = new TableCell();
                string[] Splitstr;

                #region Process-FieldInfo
                Splitstr = ModifyBackupRow["FieldInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.RemoveEmptyEntries);
                DigitalTable = new Table();
                DigitalTr = new TableRow();

                for (int i = 0; i < Splitstr.Length; i++)
                {
                    DigitalTd = new TableCell();

                    for (int x = 0; x < FieldTable.Rows.Count; x++)
                    {
                        if (string.Compare(Splitstr[i].ToString(), FieldTable.Rows[x]["FieldName"].ToString(), false) == 0)
                        {
                            DigitalTd.Text = FieldTable.Rows[x]["ChtName"].ToString();
                            break;
                        }
                    }
                    DigitalTd.Style.Add("border-top", "1px solid black");
                    DigitalTd.Style.Add("border-bottom", "1px dashed black");
                    if (i != Splitstr.Length - 1)
                        DigitalTd.Style.Add("border-right", "1px dashed black");
                    DigitalTr.Controls.Add(DigitalTd);
                }
                DigitalTable.Controls.Add(DigitalTr);
                #endregion

                #region Process-DataInfo
                Splitstr = ModifyBackupRow["DataInfo"].ToString().Split(new string[] { "|;" }, StringSplitOptions.RemoveEmptyEntries);
                DigitalTr = new TableRow();
                for (int i = 0; i < Splitstr.Length; i++)
                {

                    DigitalTd = new TableCell();
                    DigitalTd.Text = Splitstr[i].ToString();

                    DigitalTd.Style.Add("border-bottom", "1px dashed black");
                    if (i != Splitstr.Length - 1)
                        DigitalTd.Style.Add("border-right", "1px dashed black");
                    DigitalTr.Controls.Add(DigitalTd);
                }
                DigitalTable.Controls.Add(DigitalTr);
                #endregion

                #region DigitalTable Style
                DigitalTable.Style.Add("font-size", "12px");
                DigitalTable.Style.Add("background", "#F2F2F2");
                DigitalTable.Style.Add("word-break", "keep-all");
                DigitalTable.Style.Add("white-space", "nowrap");
                DigitalTable.Attributes.Add("cellpadding", "5");
                DigitalTable.Attributes.Add("cellspacing", "0");
                #endregion

                Panel DivPanel = new Panel();
                DivPanel.ID = "Div_" + ModifyBackupRow["BackupTime"].ToString();
                DivPanel.Style.Add("display", "none");
                DivPanel.ScrollBars = ScrollBars.Horizontal;
                DivPanel.Width = _width;
                DivPanel.Controls.Add(DigitalTable);

                MainTd.ColumnSpan = 3;
                MainTd.Controls.Add(DivPanel);
                MainTr.Controls.Add(MainTd);
                #endregion
                MainTable.Controls.Add(MainTr);
            }

            #region MainTable Style
            MainTable.Style.Add("word-break", "break-all");
            MainTable.Style.Add("border", "1px solid black");

            MainTable.Attributes.Add("cellpadding", "0");
            MainTable.Attributes.Add("cellspacing", "0");
            MainTable.Width = _width;
            #endregion

            #region 讀取Main_RenderControl
            StringWriter StringWriter = new StringWriter();
            HtmlTextWriter HtmlWriter = new HtmlTextWriter(StringWriter);
            MainTable.RenderControl(HtmlWriter);
            #endregion

            return StringWriter.ToString();
        }
        #endregion
    }
}