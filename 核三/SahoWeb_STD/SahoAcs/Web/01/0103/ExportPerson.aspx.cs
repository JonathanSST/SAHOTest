using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using System.IO;

namespace SahoAcs.Web._01._0103
{
    public partial class ExportPerson : Sa.BasePage
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Session["UserID"] != null)
            {
                //取得使用者管理區資區的組織架構資料
                string sql = @"SELECT DISTINCT(OrgStrucID), OrgStrucNo FROM 
                (SELECT B00_SysUserMgns.UserID, B00_SysUserMgns.MgaID, B01_MgnOrgStrucs.OrgStrucID, B01_OrgStruc.OrgStrucNo,
                B01_OrgStruc.OrgIDList FROM B00_SysUserMgns 
                INNER JOIN B01_MgnOrgStrucs ON B00_SysUserMgns.MgaID = B01_MgnOrgStrucs.MgaID
                INNER JOIN B01_OrgStruc ON B01_MgnOrgStrucs.OrgStrucID = B01_OrgStruc.OrgStrucID) AS Mgns
                WHERE Mgns.UserID = @UserID ";

                var OrgStrucList = this.odo.GetQueryResult(sql,new {UserID=Session["UserID"].ToString() }).ToList();

                //查詢人員基本資料
                sql = @"SELECT DISTINCT(B01_Person.PsnID), B01_Person.PsnNo, B01_Person.PsnName, B01_Person.PsnEName,
                                ItemName AS PsnType, B01_Person.IDNum, B01_Person.Birthday, B01_Person.OrgStrucID, B01_Person.PsnAccount,
                                B01_Person.PsnPW, B01_Person.PsnAuthAllow, B01_Person.PsnSTime, B01_Person.PsnETime, B01_Person.PsnPicSource,
                                B01_Person.Remark, B01_Person.CreateUserID, B01_Person.CreateTime, B01_Person.UpdateUserID,
                                B01_Person.UpdateTime, B01_Person.Rev01, B01_Person.Rev02, OrgStrucAllData_1.OrgStrucNo,CardNo,
                                OrgStrucAllData_1.OrgNameList, OrgStrucAllData_1.OrgNoList FROM B01_Person 
								INNER JOIN B00_ItemList ON B01_Person.PsnType=ItemNo AND ItemClass='PsnType'
                                INNER JOIN OrgStrucAllData('') AS OrgStrucAllData_1 ON B01_Person.OrgStrucID = OrgStrucAllData_1.OrgStrucID
                                LEFT OUTER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID WHERE B01_Person.OrgStrucID IN @OrgIDList ";
                if (Request["PsnName"] != null && Request["PsnName"].ToString() != "")
                {
                    sql += " AND ( PsnNo LIKE @PsnName OR PsnName LIKE @PsnName OR (CardNo=@CardNo AND ISNULL(B01_Card.PsnID,0)!=0)) ";
                }
                if(Request["PsnType"]!=null && Request["PsnType"].ToString() != "")
                {
                    sql += " AND PsnType=@PsnType ";
                }
                string Comp = "", Dept = "";
                if (Request["Company"] != null)
                    Comp = Request["Company"].ToString();
                if (Request["Department"] != null)
                    Dept = Request["Department"].ToString();

                if (Comp!="" && Dept != "")
                {
                    //if (wheresql != "") { wheresql += " AND"; }
                    sql += string.Format(@" AND (OrgNoList LIKE '\{0}\%' AND OrgNoList LIKE '%\{1}\%')",Comp, Dept);
                }
                else if (Comp != "" || Dept != "")
                {
                    //if (wheresql != "") { wheresql += " AND"; }
                    sql += string.Format(@" AND (OrgNoList LIKE '%\{0}\%')", (Comp != "" ? Comp : Dept));
                }
                var PsnData = this.odo.GetQueryResult<DBModel.PersonEntity>(sql, 
                    new {PsnType=Request["PsnType"],PsnName=Request["PsnName"]+"%", CardNo=Request["PsnName"], OrgIDList=OrgStrucList.Select(entity=>Convert.ToInt32(entity.OrgStrucID) )});
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("PersonData");
                ws.Cells[1, 1].Value = "人員編號";
                ws.Cells[1, 2].Value = "人員姓名";
                ws.Cells[1, 3].Value = "英文姓名";
                ws.Cells[1, 4].Value = "卡號";
                ws.Cells[1, 5].Value = "單位部門";
                ws.Cells[1, 6].Value = "職稱";
                ws.Cells[1, 7].Value = "卡片版次";
                int i = 2;
                foreach(var psnobj in PsnData)
                {
                    ws.Cells[i, 1].Value = psnobj.PsnNo;
                    ws.Cells[i, 2].Value = psnobj.PsnName;
                    ws.Cells[i, 3].Value = psnobj.PsnEName;
                    ws.Cells[i, 4].Value = psnobj.CardNo;
                    ws.Cells[i, 5].Value = psnobj.OrgNameList;
                    ws.Cells[i, 6].Value = psnobj.PsnType;
                    ws.Cells[i, 7].Value = psnobj.CardVer;
                    i++;
                }
                ws.Cells.AutoFitColumns(); //自動欄寬
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=PersonData.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
            else
            {
                Response.Write("查無資料!!");
            }
            
            
               
        }






    }//end page report class
}//end namespace