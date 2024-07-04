using Sa.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SahoAcs
{
    public partial class ModifybackQuery : System.Web.UI.Page
    {
        #region Main Description
        AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            #region LoadProcess
            oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Master.FindControl("ToolkitScriptManager1");
            oScriptManager.EnablePageMethods = true;
            oScriptManager.RegisterAsyncPostBackControl(this.QueryButton);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            //ClientScript.RegisterClientScriptInclude("ChangePWD", "ChangePWD.js");//加入同一頁面所需的JavaScript檔案

            #region 註冊主頁Button動作
            //QueryButton.Attributes["onClick"] = "SaveExcute();return false;";  ModifybackQuery
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B00_SysRole", "zhtw", out TableInfo);
            //Label_No.Text = TableInfo["RoleNo"].ToString();
            //Label_Name.Text = TableInfo["RoleName"].ToString();
            //Label_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_No.Text = TableInfo["RoleNo"].ToString();
            //popLabel_Name.Text = TableInfo["RoleName"].ToString();
            //popLabel_EName.Text = TableInfo["RoleEName"].ToString();
            //popLabel_States.Text = TableInfo["RoleState"].ToString();
            //popLabel_Desc.Text = TableInfo["RoleDesc"].ToString();
            //popLabel_Remark.Text = TableInfo["Remark"].ToString();
            #endregion

            #endregion

            if (!IsPostBack && !oScriptManager.IsInAsyncPostBack)
            {
                Input_BackupTimeBegin.SetRequired();
                Input_BackupTimeEnd.SetRequired();
                this.Input_BackupTimeBegin.DateValue = string.Format("{0:yyyy/MM/dd 00:00:00}", DateTime.Now);
                this.Input_BackupTimeEnd.DateValue = string.Format("{0:yyyy/MM/dd 23:59:59}", DateTime.Now);
                CreateTableFromItem();
            }
        }
        #endregion

        #endregion

        #region Method

        public void CreateTableFromItem()
        {
            /*
           Input_TableFrom.Clear();
           for (int i = 1; i <= 30; i++)
           {
               Input_TableFrom.Items.Add(new ListItem() { Text = "測試項目A" + i, Value = "A" + i.ToString() });
           }
           Input_TableFrom.ListWidth = 200;
           Input_TableFrom.ListHeight = 350;
           */
        }

        #endregion


        protected void QueryButton_Click(object sender, EventArgs e)
        {
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            Pub.MessageObject objRet = new Pub.MessageObject();
            string sql = "", wheresql = "";
            List<string> liSqlPara = new List<string>();
            DataTable ModifyBackupTable;
            //DataTable FieldTable;

            Table MainTable;
            TableRow MainTr;
            TableCell MainTd;

            #region Process-GetFieldTable
            sql = @" SELECT DISTINCT BackupTime,BackupUserID,TableName,ModifyMode FROM B00_ModifyBackup ";
            wheresql = @" ? <= BackupTime AND BackupTime <= ? ";
            liSqlPara.Add("D:" + Input_BackupTimeBegin.DateValue.ToString());
            liSqlPara.Add("D:" + Input_BackupTimeEnd.DateValue.ToString());

            if (!string.IsNullOrEmpty(Input_ModifyMode.SelectedValue.Trim()))
            {
                if (wheresql != "") wheresql += " AND ";
                wheresql += " (ModifyMode = ?) ";
                liSqlPara.Add("S:" + Input_ModifyMode.SelectedValue.Trim());
            }

            if (wheresql != "")
            {
                sql += " WHERE ";
                sql += wheresql;
            }
            //sql += wheresql + " ORDER BY RoleNo ";
            oAcsDB.GetDataTable("ModifyBackupTable", sql, liSqlPara, out ModifyBackupTable);



            #endregion

            MainTable = new Table();

            #region Header
            MainTr = new TableRow();

            MainTd = new TableCell();
            MainTd.Text = "異動時間";
            MainTd.Width = 150;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTd = new TableCell();
            MainTd.Text = "使用者帳號";
            MainTd.Width = 90;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTd = new TableCell();
            MainTd.Text = "查詢來源";
            MainTd.Width = 150;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTd = new TableCell();
            MainTd.Text = "動作類型";
            MainTd.Width = 80;
            MainTd.Attributes.Add("nowrap", "norwap");
            MainTd.Style.Add("background-color", "#016B0A");
            MainTd.Style.Add("color", "#FBFBFB");
            MainTd.Style.Add("padding", "3px");
            MainTr.Controls.Add(MainTd);

            MainTable.Controls.Add(MainTr);
            #endregion

            if (ModifyBackupTable.Rows.Count > 0)
            {
                #region DataRow
                foreach (DataRow ModifyBackupRow in ModifyBackupTable.Rows)
                {
                    MainTr = new TableRow();

                    #region Add Cell
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
                    MainTd.Text = ModifyBackupRow["TableName"].ToString();
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("background-color", "#F5FFF5");
                    MainTd.Style.Add("border-top", "1px solid black");
                    MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);


                    MainTd = new TableCell();
                    MainTd.Text = (ModifyBackupRow["ModifyMode"].ToString() == "M") ? "編輯" : "刪除";
                    MainTd.Attributes.Add("nowrap", "norwap");
                    MainTd.Style.Add("background-color", "#F5FFF5");
                    MainTd.Style.Add("border-top", "1px solid black");
                    MainTd.Style.Add("border-right", "1px solid black");
                    MainTd.Style.Add("padding", "3px");
                    MainTr.Controls.Add(MainTd);
                    #endregion

                    MainTable.Controls.Add(MainTr);
                }
                #endregion
            }

            #region MainTable Style
            MainTable.Style.Add("word-break", "break-all");
            MainTable.Style.Add("border", "1px solid black");
            MainTable.Attributes.Add("cellpadding", "0");
            MainTable.Attributes.Add("cellspacing", "0");
            #endregion

            StringWriter StringWriter = new StringWriter();
            HtmlTextWriter HtmlWriter = new HtmlTextWriter(StringWriter);
            MainTable.RenderControl(HtmlWriter);
            ModifyInfo.Text = StringWriter.ToString();
            //ModifyInfo.Text += "ttt";
            ModifyInfo_UpdatePanel.Update();
        }
    }
}