using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace SahoAcs.Web._02._0201._020102
{
    public partial class DoorAccessForm : System.Web.UI.Page
    {
        public DataTable DataResult = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptInclude("DoorAccessForm", "DoorAccessForm.js");//加入同一頁面所需的JavaScript檔案
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sql = "";
            DataTable dt;

            #region Process String
            sql = @" SELECT * FROM B01_DeviceConnInfo ";
            #endregion
            oAcsDB.GetDataTable("DciItem", sql, out this.DataResult);

            foreach (DataRow dr in this.DataResult.Rows)
            {
                var Item = new System.Web.UI.WebControls.ListItem();
                Item.Text = dr["DciNo"].ToString() + " (" + dr["DciName"].ToString() + ")";
                Item.Value = dr["DciID"].ToString();
                this.Dci.Items.Add(Item);
            }

            sql = @" SELECT ParaValue FROM B00_SysParameter WHERE ParaNo = 'CardLen' ";
            oAcsDB.GetDataTable("DciItem", sql, out this.DataResult);
            foreach (DataRow dr in this.DataResult.Rows)
            {
                this.CardNoLen.Text = dr["ParaValue"].ToString();
            }

            #region 管制區設定
            var Item2 = new System.Web.UI.WebControls.ListItem();
            Item2.Text = "- 請選擇 -";
            Item2.Value = "";
            this.InToCtrlAreaID.Items.Add(Item2);
            this.OutToCtrlAreaID.Items.Add(Item2);
            #endregion

            #region Process String
            sql = @" SELECT * FROM B01_CtrlArea ";
            #endregion
            oAcsDB.GetDataTable("CtrlAreaItem", sql, out dt);
            foreach (DataRow dr in dt.Rows)
            {
                Item2 = new System.Web.UI.WebControls.ListItem();
                Item2.Text = dr["CtrlAreaName"].ToString();
                Item2.Value = dr["CtrlAreaID"].ToString();
                this.InToCtrlAreaID.Items.Add(Item2);
                this.OutToCtrlAreaID.Items.Add(Item2);
            }




            if (Request["DoAction"] != null && Request["DoAction"] == "insert")
            {
                string message_return = "";
                if (string.IsNullOrEmpty(Request["Building"]))
                {
                    message_return += "建築物名稱 必須輸入";
                }
                else if (Request["Building"].Trim().Count() > 50)
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "建築物名稱 字數超過上限";
                }
                if (string.IsNullOrEmpty(Request["Floor"].Trim()))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "樓層 必須輸入";
                }
                else if (Request["Floor"].Trim().Count() > 10)
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "樓層 字數超過上限";
                }

                if (string.IsNullOrEmpty(Request["EquModel"].Trim()))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備型號 必須指定";
                }

                if (string.IsNullOrEmpty(Request["EquNo"]))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備編號 必須輸入";
                }
                else if (Request["EquNo"].Trim().Count() > 20)
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備編號 字數超過上限";
                }

                if (string.IsNullOrEmpty(Request["EquName"].Trim()))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備名稱 必須輸入";
                }
                else if (Request["EquName"].Trim().Count() > 50)
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備名稱 字數超過上限";
                }
                if (Request["EquEName"].Trim().Count() > 50)
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備英文名稱 字數超過上限";
                }

                if (string.IsNullOrEmpty(Request["Dci"].Trim()))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "設備連線 必須指定";
                }
                int tempint = 0;
                if (string.IsNullOrEmpty(Request["CardNoLen"].Trim()))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "卡號長度 必須輸入";
                }
                else if (!int.TryParse(Request["CardNoLen"].Trim(), out tempint))
                {
                    if (message_return.Length > 0)
                        message_return += "\n";
                    message_return += "卡號長度 必需為數字";
                }

                if (int.TryParse(Request["CardNoLen"].Trim(), out tempint))
                {
                    if (tempint < 4 || tempint > 16)
                    {
                        if (message_return.Length > 0)
                            message_return += "\n";
                        message_return += "卡號長度 需介於 04 ~ 16 之間。";
                    }
                }
                if (message_return.Length == 0)
                {
                    sql = @" SELECT * FROM B01_EquData WHERE EquNo = ? ";
                    List<string> para = new List<string>();
                    para.Add("S:" + Request["EquNo"]);
                    oAcsDB.GetDataTable("EquData", sql, para, out this.DataResult);
                    if (this.DataResult != null && this.DataResult.Rows.Count > 0)
                    {
                        message_return += "此代碼已存在於系統中";
                    }
                    else
                    {
                        sql = @"INSERT INTO B01_EquData (EquNo,EquName,EquEName,EquClass,
                                        EquModel,CardNoLen,IsAndTrt,DciID,
                                        Building,Floor,InToCtrlAreaID,OutToCtrlAreaID,IsShowName,
                                        CreateUserID,CreateTime,UpdateUserID,UpdateTime
                                        ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,'Saho',GETDATE(),'Saho',GETDATE())";
                        para.Clear();
                        para.Add("S:" + Request["EquNo"]);
                        para.Add("S:" + Request["EquName"]);
                        para.Add("S:" + Request["EquEName"]);
                        para.Add("S:" + Request["EquClass"]);

                        para.Add("S:" + Request["EquModel"]);
                        para.Add("S:" + Request["CardNoLen"]);
                        para.Add("S:" + Request["Input_Trt"].ToString());
                        para.Add("S:" + Request["Dci"].ToString());

                        para.Add("S:" + Request["Building"]);
                        para.Add("S:" + Request["Floor"]);
                        para.Add("I:" + Request["InToCtrlAreaID"].ToString());
                        para.Add("I:" + Request["OutToCtrlAreaID"].ToString());
                        para.Add("S:" + (Request["popIsShowName"] != null ? "1" : "0"));
                        int result = oAcsDB.SqlCommandExecute(sql, para);
                        int result1 = result;
                    }
                }



                Response.Clear();
                Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new { message = message_return }));
                Response.End();
            }
        }//end method

    }//end class
}//end namespace