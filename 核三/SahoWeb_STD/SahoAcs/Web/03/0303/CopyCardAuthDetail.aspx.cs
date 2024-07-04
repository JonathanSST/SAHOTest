using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web._03._0303
{
    public partial class CopyCardAuthDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("CopyCardAuthDetail", "CopyCardAuthDetail.js");

            if (!IsPostBack)
            {
                if (Request["CardNo"] != null)
                {
                    CreateEuqList();
                }
            }
        }

        #region CreateEuqList

        private void CreateEuqList()
        {
            DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            string sSQL = "";
            List<string> liSqlPara = new List<string>();
            DataTable dt = new DataTable();
            bool isSuccess = true;
            bool isBtn01 = true;
            bool isBtn02 = true;

            string strUserId = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");

            #region 取得 B01_CardEquAdj 的資料，並秀在EquList上
            sSQL = @" 
                SELECT 
                    DISTINCT ED.EquNo, ED.EquName, CEA.OpMode 
                FROM B01_CardEquAdj AS CEA 
                INNER JOIN B01_Card AS CA ON CA.CardID = CEA.CardID	
                INNER JOIN B01_EquData AS ED ON ED.EquID = CEA.EquID
                INNER JOIN B01_EquGroupData AS EGD ON EGD.EquID = ED.EquID
                INNER JOIN B01_EquGroup AS EGP ON EGP.EquGrpID = EGD.EquGrpID
                INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EGP.EquGrpID
                INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID
                WHERE CEA.OpMode <> 'Del' AND CEA.OpMode <> '*' 
                AND CA.CardNo = ? 
                AND SUMS.UserID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + Request["CardNo"]);
            liSqlPara.Add("S:" + strUserId);

            isSuccess = oAcsDB.GetDataTable("dtEquList", sSQL, liSqlPara, out dt);

            if (isSuccess)
            {
                if (dt.Rows.Count > 0)
                {
                    TableRow tr = new TableRow();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            tr = new TableRow();
                        }

                        TableCell td = new TableCell();
                        td.Width = 280;
                        td.Text = "(" + dt.Rows[i]["EquNo"] + ") " + dt.Rows[i]["EquName"] + " [" + dt.Rows[i]["OpMode"] + "]";
                        td.Style.Add("white-space", "nowrap");
                        td.Style.Add("Padding", "2px");
                        tr.Controls.Add(td);

                        if (i == dt.Rows.Count - 1 && dt.Rows.Count % 2 != 0)
                        {
                            for (int k = 0; k < 2 - (dt.Rows.Count % 2); k++)
                            {
                                td = new TableCell();
                                td.Style.Add("white-space", "nowrap");
                                td.Width = 280;
                                td.Style.Add("Padding", "2px");
                                tr.Controls.Add(td);
                            }
                        }

                        EquList.Controls.Add(tr);
                    }

                    isBtn01 = true;
                }
                else
                {
                    TableRow tr = new TableRow();
                    TableCell td = new TableCell();
                    td.Text = GetLocalResourceObject("MsgEquAuth").ToString();
                    td.Width = 600;
                    tr.Controls.Add(td);
                    this.EquList.Controls.Add(tr);

                    isBtn01 = false;
                }
            }

            EquList.Style.Add("word-break", "break-all");
            EquList.Attributes.Add("border", "1");
            EquList.Style.Add("border-color", "#999999");
            EquList.Style.Add("border-style", "solid");
            EquList.Style.Add("border-collapse", "collapse");
            lblEquList.Text =
                string.Format(GetLocalResourceObject("lblCardAuthCopyList").ToString(), dt.Rows.Count);

            #endregion

            #region 取得 B01_CardEquGroup 的資料，並秀在EquGroupList上
            sSQL = @"
                SELECT
                    DISTINCT EG.EquGrpNo, EG.EquGrpName
                FROM B01_CardEquGroup AS CEG
                INNER JOIN B01_Card AS CA ON CA.CardID = CEG.CardID
                INNER JOIN B01_EquGroup AS EG ON EG.EquGrpID = CEG.EquGrpID
                INNER JOIN B01_MgnEquGroup AS MEG ON MEG.EquGrpID = EG.EquGrpID
                INNER JOIN B00_SysUserMgns AS SUMS ON SUMS.MgaID = MEG.MgaID
                WHERE CA.CardNo = ? AND SUMS.UserID = ? ";

            liSqlPara.Clear();
            liSqlPara.Add("S:" + Request["CardNo"]);
            liSqlPara.Add("S:" + strUserId);

            isSuccess = oAcsDB.GetDataTable("dtEquGroupList", sSQL, liSqlPara, out dt);

            if (isSuccess)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        TableRow tr = new TableRow();
                        TableCell td = new TableCell();
                        td.Width = 300;
                        td.Text = "(" + dt.Rows[i]["EquGrpNo"] + ") " + dt.Rows[i]["EquGrpName"];
                        td.Style.Add("white-space", "nowrap");
                        td.Style.Add("Padding", "2px");
                        tr.Controls.Add(td);

                        EquGroupList.Controls.Add(tr);
                    }

                    isBtn02 = true;
                }
                else
                {
                    TableRow tr = new TableRow();
                    TableCell td = new TableCell();
                    td.Text = GetLocalResourceObject("MsgEquGroupAuth").ToString();
                    td.Width = 280;
                    tr.Controls.Add(td);
                    EquGroupList.Controls.Add(tr);

                    isBtn02 = false;
                }
            }

            EquGroupList.Style.Add("word-break", "break-all");
            EquGroupList.Attributes.Add("border", "1");
            EquGroupList.Style.Add("border-color", "#999999");
            EquGroupList.Style.Add("border-style", "solid");
            EquGroupList.Style.Add("border-collapse", "collapse");
            this.lblEquGroupList.Text =
                string.Format(GetLocalResourceObject("lblCardAuthGroupCopyList").ToString(), dt.Rows.Count);

            #endregion

            #region 控制權限複製或權限附加按鈕的啟用狀況
            if (isBtn01 == true || isBtn02 == true)
            {
                Sa.Web.Fun.RunJavaScript(this, "ButtonDisabled('false');");
            }
            else
            {
                Sa.Web.Fun.RunJavaScript(this, "ButtonDisabled('true');");
            }
            #endregion
        }
        #endregion
    }


}