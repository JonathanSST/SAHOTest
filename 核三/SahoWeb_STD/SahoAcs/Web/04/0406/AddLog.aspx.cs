using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DapperDataObjectLib;
using SahoAcs.DBModel;

namespace SahoAcs.Web._04._0406
{
    public partial class AddLog : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public CardEntity entity = new CardEntity();
        string MgaID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["PageEvent"] != null)
            {
                if (Request["PageEvent"] == "Save")
                {
                    this.SaveData();
                }
                else if (Request["PageEvent"] == "Change")
                {
                    this.ChangeArea();
                }
                    
            }
            else
            {
                this.SetInit();
            }


            
        }



        public void SetInit()
        {
            try
            {
                //this.DropMgaList.Attributes.Add("onchange", "ChangeMga()");
                var result_log = this.odo.GetQueryResult("SELECT * FROM B00_CardLogState WHERE Code IN (49,51,53)");
                foreach (var log in result_log)
                {
                    this.popDrop_LogStatus.Items.Add(new ListItem()
                    {
                        Text = Convert.ToString(log.StateDesc),
                        Value = Convert.ToString(log.Code)
                    });
                }
                var mga_list = this.odo.GetQueryResult("SELECT * FROM B00_ManageArea WHERE MgaID>1");
                foreach (var o in mga_list)
                {
                    this.DropMgaList.Items.Add(new ListItem()
                    {
                        Text = Convert.ToString(o.MgaName),
                        Value = Convert.ToString(o.MgaID)
                    });
                }
                
                if (mga_list.Count() > 0)
                {
                    MgaID =Convert.ToString(mga_list.FirstOrDefault().MgaID);
                }
                
                var result_equdata = this.odo.GetQueryResult(@"SELECT 
	                                                                                                            E.* 
                                                                                                            FROM 
	                                                                                                            B01_EquData E
	                                                                                                            INNER JOIN B01_EquGroupData EGD ON E.EquID=EGD.EquID
	                                                                                                            INNER JOIN B01_MgnEquGroup M ON M.EquGrpID=EGD.EquGrpID
	                                                                                                            INNER JOIN B00_ManageArea Area ON M.MgaID=Area.MgaID
                                                                                                            WHERE
	                                                                                                            Area.MgaID=@MgaID AND EquName NOT LIKE '%滿意度%' ", new { MgaID = MgaID });

                foreach (var o in result_equdata)
                {
                    this.popDrop_EquName.Items.Add(new ListItem()
                    {
                        Text = Convert.ToString(o.EquName),
                        Value = Convert.ToString(o.EquID)
                    });
                }
                
                this.ChangeArea();
                if (Request["card_id"] != null)
                {
                    var sql = @" SELECT B01_Person.PsnID,
                                B01_Person.PsnNo, B01_Person.PsnName, B01_Card.CardNo, CardID FROM B01_Person 
                                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID WHERE CardID=@CardID ";
                    var psndatas = this.odo.GetQueryResult<CardEntity>(sql, new { CardID = Request["card_id"] });
                    if (psndatas.Count() > 0)
                    {
                        this.entity = psndatas.First();
                        this.popInput_CardNo.Text = this.entity.CardNo;
                        this.popInput_PsnName.Text = this.entity.PsnName;
                        this.popInput_PsnNo.Text = this.entity.PsnNo;
                        this.popInput_PsnID.Text = this.entity.PsnID.ToString();
                        //this.popInput_PsnName.Text=this.entity.psn
                    }
                }

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        public void SaveData()
        {
            string card_id = Request["CardID"];
            string equ_id = Request["EquID"];
            string status = Request["LogStatus"];            
            string cardtime = Request["CardTime"];
            var sql = @"  SELECT B01_Person.PsnID, B01_Person.PsnNo, B01_Person.PsnName, B01_OrgStruc.OrgIDList AS OrgStruc,CardType
                ,B01_Card.CardNo, B01_Card.CardVer FROM B01_Person 
                INNER JOIN B01_Card ON B01_Person.PsnID = B01_Card.PsnID 
                INNER JOIN B01_OrgStruc ON B01_Person.OrgStrucID = B01_OrgStruc.OrgStrucID 
                WHERE B01_Card.CardID = @CardID ";
            List<CardLogFillData> list = this.odo.GetQueryResult<DBModel.CardLogFillData>(sql, new { CardID = card_id }).ToList();
            if (list.Count() > 0)
            {
                CardLogFillData data = list.First();
                sql = @" SELECT EquClass, EquNo, EquName, Dir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt FROM V_MCRInfo WHERE EquID = @EquID ";
                var result = this.odo.GetQueryResult(sql, new { EquID = equ_id });
                if (result.Count() == 0)
                {
                    result = this.odo.GetQueryResult(@"SELECT EquClass, EquNo, EquName,'進' AS Dir,'' AS MstConnParam,'1' CtrlAddr,'1' AS ReaderNo, IsAndTrt 
                                                                                        FROM B01_EquData WHERE EquID=@EquID", new { EquID = equ_id });
                }
                foreach(var o in result)
                {
                    data.EquClass = Convert.ToString(o.EquClass);
                    data.EquNo = Convert.ToString(o.EquNo);
                    data.EquName = Convert.ToString(o.EquName);
                    data.EquDir = Convert.ToString(o.Dir);
                    data.MstConnParam = Convert.ToString(o.MstConnParam);
                    data.CtrlAddr = Convert.ToString(o.CtrlAddr);
                    data.ReaderNo = Convert.ToString(o.ReaderNo);
                    data.IsAndTrt = Convert.ToString(o.IsAndTrt);
                }
                data.CardInfo = "";
                data.VerifyMode = "0";
                data.LogStatus = status;
                data.CardType = Convert.ToString(list.First().CardType);
                data.CardTime = cardtime;
                sql = @" INSERT INTO B01_CardLogFill (LogTime, CardTime, CardNo, CardVer, PsnNo, PsnName, OrgStruc
                    ,EquClass, EquNo, EquName, EquDir, MstConnParam, CtrlAddr, ReaderNo, IsAndTrt, CardInfo, VerifyMode
                    ,LogStatus, CardType) VALUES (getdate(), @CardTime, @CardNo, @CardVer, @PsnNo, @PsnName, @OrgStruc
                     ,@EquClass, @EquNo, @EquName, @EquDir, @MstConnParam, @CtrlAddr, @ReaderNo, @IsAndTrt, @CardInfo, @VerifyMode
                     ,@LogStatus, @CardType) ";
                this.odo.Execute(sql, data);
                string messae = this.odo.DbExceptionMessage;
            }
            
        }

        public void ChangeArea()
        {
            if (Request["MgaID"] != null)
            {
                this.MgaID = Request["MgaID"];
            }
            var result_equdata = this.odo.GetQueryResult(@"SELECT 
	                                                                                                            E.* 
                                                                                                            FROM 
	                                                                                                            B01_EquData E
	                                                                                                            INNER JOIN B01_EquGroupData EGD ON E.EquID=EGD.EquID
	                                                                                                            INNER JOIN B01_MgnEquGroup M ON M.EquGrpID=EGD.EquGrpID
	                                                                                                            INNER JOIN B00_ManageArea Area ON M.MgaID=Area.MgaID
                                                                                                            WHERE
	                                                                                                            Area.MgaID=@MgaID AND MgaName NOT LIKE '%滿意度%' ", new { MgaID = Request["MgaID"] });            
            foreach (var o in result_equdata)
            {
                this.popDrop_EquName.Items.Add(new ListItem()
                {
                    Text = Convert.ToString(o.EquName),
                    Value = Convert.ToString(o.EquID)
                });
            }

        }

    }//end class
}//end namespace