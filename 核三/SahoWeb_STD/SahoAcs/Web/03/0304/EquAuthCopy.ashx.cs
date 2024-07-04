using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

using SahoAcs.DBModel;
using DapperDataObjectLib;


namespace SahoAcs.Web._03._0304
{
    /// <summary>
    /// EquAuthCopy 的摘要描述
    /// </summary>
    public class EquAuthCopy : IHttpHandler,System.Web.SessionState.IRequiresSessionState

    {

        string strCompareCmdData = @"SELECT DISTINCT Card.CardID, @TargetID AS EquID, '+' AS OpMode,
                                             CardAuth.CardRule, CardAuth.CardExtData,
                                             CardAuth.BeginTime, CardAuth.EndTime, @UserID AS CreateUserID, GETDATE() AS CreateTime,
                                             TargetCardAuth.CardRule AS TargetCardAuth_CardRule,
                                             TargetCardAuth.CardExtData AS TargetCardAuth_CardExtData,
                                             CASE TargetCardAuth.CardRule
                    	                         WHEN CardAuth.CardRule THEN 'Same'
                    	                         ELSE 'Diff'
                                             END AS RuleCompare,
                                             CASE TargetCardAuth.CardExtData
                    	                         WHEN CardAuth.CardExtData THEN 'Same'
                    	                         ELSE 'Diff'
                                             END AS ExtDataCompare
                                             FROM   B01_CardAuth AS CardAuth
                                             INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = CardAuth.CardNo
                                             LEFT JOIN B01_CardAuth AS TargetCardAuth ON TargetCardAuth.CardNo = Card.CardNo AND TargetCardAuth.EquID = @TargetID
                                             INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                                             INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                                             INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                                             INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquID = CardAuth.EquID
                                             INNER JOIN dbo.B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                                             INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                                             INNER JOIN dbo.B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID AND SysUserMgns.MgaID = MgnOrgStrucs.MgaID 
                                             WHERE  CardAuth.EquID = @SourceID AND CardAuth.OpMode != 'Del' AND SysUserMgns.UserID = @UserID";

        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
            string json = "";
            using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                json = reader.ReadToEnd();
            }
            dynamic resp = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(json);
            if (resp["page_event"].ToString() == "Reset")
            {
                this.SetCardAuth2(ref context,resp);
            }
            if (resp["page_event"].ToString() == "AuthCopy")
            {
                this.SetAuthCopy(ref context, resp);
            }
        }

        


        private void SetAuthCopy(ref HttpContext context,dynamic resp)
        {
            string sourceid = Convert.ToString(this.odo.GetQueryResult("SELECT * FROM B01_EquData WHERE EquNo=@EquNo", new { EquNo = resp["SourceID"] }).FirstOrDefault().EquID);
            int intError = 0;
            int intSuccess = 0;
            string ErrorMsg = "";
            foreach (var equid in resp["targets"])
            {
                if (resp["AuthActMode"].ToString() == "Cover")
                {
                    try
                    {
                        #region Covert Rule
                        var sql = @" DELETE FROM B01_CardAuth WHERE  EquID = @TargetID; 
                                         DELETE FROM B01_CardEquAdj WHERE  EquID = @TargetID; ";
                        sql += @" INSERT INTO B01_CardEquAdj(CardID, EquID, OpMode, CardRule, CardExtData, BeginTime, EndTime, CreateUserID, CreateTime)
                                          SELECT DISTINCT Card.CardID, @TargetID, '+',
                                          CardAuth.CardRule, CardAuth.CardExtData, 
                                          CardAuth.BeginTime, CardAuth.EndTime,
                                          @UserID, GETDATE()
                                          FROM B01_CardAuth AS CardAuth
                                          INNER JOIN dbo.B01_Card AS Card ON Card.CardNo = CardAuth.CardNo
                                          INNER JOIN dbo.B01_Person AS Person ON Person.PsnID = Card.PsnID
                                          INNER JOIN B01_OrgStruc AS OrgStruc ON OrgStruc.OrgStrucID = Person.OrgStrucID
                                          INNER JOIN B01_MgnOrgStrucs AS MgnOrgStrucs ON MgnOrgStrucs.OrgStrucID = OrgStruc.OrgStrucID
                                          INNER JOIN dbo.B01_EquData AS EquData ON EquData.EquID = CardAuth.EquID
                                          INNER JOIN dbo.B01_EquGroupData AS EquGroupData ON EquGroupData.EquID = EquData.EquID
                                          INNER JOIN B01_MgnEquGroup AS MgnEquGroup ON MgnEquGroup.EquGrpID = EquGroupData.EquGrpID
                                          INNER JOIN dbo.B00_SysUserMgns AS SysUserMgns ON SysUserMgns.MgaID = MgnEquGroup.MgaID AND SysUserMgns.MgaID = MgnOrgStrucs.MgaID 
                                          WHERE CardAuth.EquID = @SourceID AND CardAuth.OpMode != 'Del' AND SysUserMgns.UserID = @UserID ";
                        this.odo.Execute(sql, new { UserID = context.Session["UserID"].ToString(), TargetID = equid, SourceID = sourceid });
                        if (this.odo.isSuccess && this.odo.DbExceptionMessage == "")
                        {
                            intSuccess++;
                        }else
                        {
                            intError++;
                            ErrorMsg += "設備ID : " + equid + Environment.NewLine;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        intError++;
                        ErrorMsg += "處理錯誤設備ID : " + equid + Environment.NewLine;
                    }                   
                }
                else
                {
                    try
                    {
                        #region Add Mode Rule
                        var cardauthsql = this.strCompareCmdData;
                        var result = this.odo.GetQueryResult<EquCopyEntity>(cardauthsql, new { UserID = context.Session["UserID"].ToString(), TargetID = equid, SourceID = sourceid }).ToList();
                        string finalCardRule = "";
                        string finalCardExtData = "";
                        string SourceBinvalue = "";
                        string TargetBinvalu = "";
                        var auth_cmd = @"DELETE FROM B01_CardEquAdj WHERE CardID = @CardID AND EquID = @EquID; 
                                                    INSERT INTO B01_CardEquAdj(CardID, EquID, OpMode, CardRule, CardExtData, BeginTime, EndTime, CreateUserID, CreateTime)
                                                  VALUES (@CardID, @EquID, '+', @CardRule, @CardExtData, @BeginTime, @EndTime, @UserID, GETDATE())";
                        List<EquCopyEntity> insertEquData = new List<EquCopyEntity>();
                        foreach (var ddr in result)
                        {
                            finalCardRule = "";
                            finalCardExtData = "";
                            SourceBinvalue = "";
                            TargetBinvalu = "";

                            if (string.Compare(ddr.RuleCompare, "Same") == 0 && string.Compare(ddr.ExtDataCompare, "Same") == 0)
                                continue;
                            else
                            {
                                if (resp["ConflictActMode"].ToString() != "SourceValue")
                                {
                                    ddr.CardRule = ddr.TargetCardAuth_CardRule;
                                }
                                if (ddr.CardExtData != "" || ddr.TargetCardAuth_CardExtData != "")
                                {
                                    if (ddr.CardExtData == "")
                                    {
                                        ddr.CardExtData = "000000000000";
                                    }
                                    if (string.IsNullOrEmpty(ddr.TargetCardAuth_CardExtData))
                                    {
                                        ddr.TargetCardAuth_CardExtData = "000000000000";
                                    }
                                    SourceBinvalue = Sa.Change.HexToBin(ddr.CardExtData.ToString(), 48, false);
                                    TargetBinvalu = Sa.Change.HexToBin(ddr.TargetCardAuth_CardExtData.ToString(), 48, false);
                                    for (int k = 0; k < SourceBinvalue.Length; k++)
                                    {
                                        if (SourceBinvalue.Substring(k, 1).ToString() == "1" || TargetBinvalu.Substring(k, 1).ToString() == "1")
                                            finalCardExtData += "1";
                                        else
                                            finalCardExtData += "0";
                                    }
                                    finalCardExtData = Sa.Change.BinToHex(finalCardExtData, 12);
                                }
                                ddr.CardExtData = finalCardExtData;
                                ddr.UserID = context.Session["UserID"].ToString();
                                insertEquData.Add(ddr);
                            }
                        }//處理權限附加的資料處理
                        this.odo.Execute(auth_cmd, insertEquData);
                        #endregion
                        if (this.odo.isSuccess && this.odo.DbExceptionMessage == "")
                        {
                            intSuccess++;
                        }
                        else
                        {
                            intError++;
                            ErrorMsg += "設備ID : " + equid + Environment.NewLine;
                        }
                    }
                    catch (Exception ex)
                    {
                        intError++;
                        ErrorMsg += "處理錯誤設備ID : " + equid + Environment.NewLine;
                    }
                }//處理權限附加
            }//取得目地設備代碼
            string SuccessMsg = "處理完成！"+Environment.NewLine;
            SuccessMsg += "複製完成筆數："+intSuccess+Environment.NewLine;
            SuccessMsg += "複製失敗筆數：" + intError + Environment.NewLine;
            if (intError == 0)
            {
                ErrorMsg = "";
            }
            context.Response.Clear();
            context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { isSuccess = "OK", message=SuccessMsg+ErrorMsg}));
            context.Response.End();
        }


        private void SetCardAuth2(ref HttpContext context,dynamic resp)
        {          
            string sSuccess = "";
            string sError = "";
            foreach (var card_no in resp["cards"])
            {
                this.odo.Execute(@"EXEC CardAuth_Update @sCardNo = @CardNo,
                    @sUserID = @UserID,@sFromProc = @UserID,@sFromIP = '127.0.0.1',
                    @sOpDesc = '設備權限複製' ;", new { CardNo = card_no,UserID= context.Session["UserID"] });
                if (this.odo.isSuccess)
                {
                    sSuccess += card_no + ",";
                }
                else
                {
                    sError += card_no + ",";
                }
            }
            context.Response.Clear();
            context.Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(
                new { message = "OK", cards_ok = sSuccess.TrimEnd(','), cards_error = sError }));
            context.Response.End();
     }


    public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}