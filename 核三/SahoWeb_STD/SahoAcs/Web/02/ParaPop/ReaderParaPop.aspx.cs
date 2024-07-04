using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;


namespace SahoAcs
{
    public partial class ReaderParaPop : Sa.BasePage
    {
        #region Main Description
        //AjaxControlToolkit.ToolkitScriptManager oScriptManager;
        Hashtable TableInfo;
        static string[] LBlockLength = new string[] { "無效參數", "26 BITs", "32 BITs", "34 BITs" };
        static string[] RBlockLength = new string[] { "讀序列號", "BLOCK", "無效參數" };
        static string[] LBuzzer = new string[] { "ON", "OFF", "無效參數" };
        static string[] RBuzzer = new string[] { "ON", "無效參數" };
        public string Facestr, LoadModestr, BlockLengthstr, Buzzerstr;
        #endregion

        #region Events

        #region Page_Load
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            #region LoadProcess
            //oScriptManager = (AjaxControlToolkit.ToolkitScriptManager)this.Page.FindControl("ToolkitScriptManager1");
            //oScriptManager.EnablePageMethods = true;
            //oScriptManager.RegisterAsyncPostBackControl(this.popB_Save);

            string js = Sa.Web.Fun.ControlToJavaScript(this);
            js += "LoadData();";
            js = "<script type='text/javascript'>" + js + "</script>";

            ClientScript.RegisterStartupScript(js.GetType(), "OnPageLoad", js);
            ClientScript.RegisterClientScriptInclude("ReaderParaPop", "/Web/02/ParaPop/ReaderParaPop.js");//加入同一頁面所需的JavaScript檔案
            ClientScript.RegisterClientScriptInclude("xFun", "/Scripts/xFun.js");
            ClientScript.RegisterClientScriptInclude("xObj", "/Scripts/xObj.js");
            ClientScript.RegisterClientScriptInclude("Common", "/Scripts/Common.js");
            ClientScript.RegisterClientScriptInclude("HighLight", "/Scripts/HighLight.js");

            #region 註冊Button動作
            //popB_Save.Attributes["onClick"] = "SaveReaderPara(); return false;";
            //popInput_BlockLength.Attributes["onKeyup"] = "GetBlockLengthStr();";
            //popInput_Buzzer.Attributes["onKeyup"] = "GetBuzzerStr();";
            #endregion

            #region 語系切換
            //DB_Acs oAcsDB = new DB_Acs(this.Page, Pub.GetConnectionString(Pub.sConnName));
            //oAcsDB.GetTableHash("B01_EquParaDef", "zhtw", out TableInfo);
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

            if (!IsPostBack)
            {
                if (Request["PageEvent"] == null)
                {
                    #region Give hideValue
                    this.hideUserID.Value = Sa.Web.Fun.GetSessionStr(this.Page, "UserID");
                    this.hideEquID.Value = Request["EquID"];
                    this.hideEquParaID.Value = Request["EquParaID"];
                    this.hideParaValue.Value = Request["ParaValue"];
                    #endregion
                }
                else
                {
                    if (Request["PageEvent"] == "CheckInput")
                    {
                        this.Check_Input();
                    }
                    else if(Request["PageEvent"]== "GetBuzzer")
                    {
                        this.SetBuzzerStr(Request["Buzzer"]);
                    }
                    else if(Request["PageEvent"]== "GetBlockLength")
                    {
                        this.SetBlockLengthStr(Request["BlockLength"]);
                    }
                }




                //pop_BlockLengthRemind.Text = "設定值為0 ~ 64。";
                //pop_BuzzerRemind.Text = "設定值為0 ~ 1。";
            }
            else
            {
                //Sa.Web.Fun.RunJavaScript(Page, "Def(); SetMode('Add');");
                //string sFormTarget = Request.Form["__EVENTTARGET"];
                //string sFormArg = Request.Form["__EVENTARGUMENT"];
                //string[] LableArray = sFormArg.Split('&');
                //LoadData();
            }
        }
        #endregion

        #endregion

        #region Method

        #region LoadData
        public void LoadData()
        {
            string Facestr, LoadModestr, BlockLengthstr, Buzzerstr;
            string[] StrItem = new string[2];

            if (!string.IsNullOrEmpty(hideParaValue.Value))
            {
                Facestr = hideParaValue.Value.Substring(0, 2);
                LoadModestr = hideParaValue.Value.Substring(2, 2);
                BlockLengthstr = hideParaValue.Value.Substring(4, 2);
                Buzzerstr = hideParaValue.Value.Substring(6, 2);

                BlockLengthstr = Convert.ToString(Convert.ToInt32(BlockLengthstr, 16));

                if (string.Compare(Buzzerstr, "AA") == 0)
                    Buzzerstr = "0";
                else
                    Buzzerstr = "1";
                #region SetRadio
                switch (Facestr)
                {
                    case "0A":
                        this.popInput_Face0.Checked = true;
                        break;
                    case "0B":
                        this.popInput_Face1.Checked = true;
                        break;
                    case "0C":
                        this.popInput_Face2.Checked = true;
                        break;
                    case "1C":
                        this.popInput_Face3.Checked = true;
                        break;
                    case "2C":
                        this.popInput_Face4.Checked = true;
                        break;
                }

                switch (LoadModestr)
                {
                    case "0A":
                        this.popInput_LoadMode0.Checked = true;
                        break;
                    case "0B":
                        this.popInput_LoadMode1.Checked = true;
                        break;
                    case "0C":
                        this.popInput_LoadMode2.Checked = true;
                        break;
                }
                #endregion
                //popInput_BlockLength.Text = BlockLengthstr;
                //popInput_Buzzer.Text = Buzzerstr;

                //StrItem = GetBlockLengthStr(BlockLengthstr);
                this.popL_LBlockLengthText.Text = StrItem[0];
                this.popL_RBlockLengthText.Text = StrItem[1];

                //StrItem = GetBuzzerStr(Buzzerstr);
                this.popL_LBuzzerText.Text = StrItem[0];
                this.popL_RBuzzerText.Text = StrItem[1];
            }
            //UpdatePanel1.Update();
        }
        #endregion
        

        
        
        #region GetBlockLengthStr

        public void SetBlockLengthStr(string BlockLength)
        {
            int BlockLengthInt;
            string[] StrItem = new string[2];
            if (int.TryParse(BlockLength.Trim(), out BlockLengthInt))
            {
                if (BlockLengthInt >= 0)
                {
                    #region Left BlockLength
                    if (BlockLengthInt >= 0 && BlockLengthInt < 4)
                        StrItem[0] = LBlockLength[BlockLengthInt];
                    else
                        StrItem[0] = LBlockLength[0];
                    #endregion
                    #region Right BlockLength
                    if (BlockLengthInt == 0)
                        StrItem[1] = RBlockLength[0];
                    else if (BlockLengthInt > 0 && BlockLengthInt <= 64)
                        StrItem[1] = RBlockLength[1] + " " + BlockLengthInt;
                    else
                        StrItem[1] = RBlockLength[2];
                    #endregion
                }
                else
                {
                    StrItem[0] = "無效參數";
                    StrItem[1] = "無效參數";
                }
            }
            else
            {
                StrItem[0] = "無效參數";
                StrItem[1] = "無效參數";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(StrItem));
            Response.End();
        }
        #endregion
        

        
        #region GetBuzzerStr        
        public void SetBuzzerStr(string Buzzer)
        {
            string[] StrItem = new string[2];

            if (Buzzer != "AA")
            {
                StrItem[0] = "OFF";
                StrItem[1] = "無效參數";
            }
            else
            {
                StrItem[0] = "ON";
                StrItem[1] = "ON";
            }
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(StrItem));
            Response.End();
        }
        #endregion


        protected void Check_Input()
        {
            var serializer = new JavaScriptSerializer();
            dynamic resp = serializer.DeserializeObject(Request["PostData"]);
            string Target = Convert.ToString(resp["Target"]);
            string Face = Convert.ToString(resp["Face"]);
            string LoadMode = Convert.ToString(resp["LoadMode"]);
            string BlockLength = Convert.ToString(resp["BlockLength"]);
            string Buzzer = Convert.ToString(resp["Buzzer"]);
            Pub.MessageObject objRet = new Pub.MessageObject();
            int tempint = 0;

            #region 空白檢查

            if (string.IsNullOrEmpty(Target))
            {
                if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                objRet.result = false;
                objRet.message += "讀卡機系列 必須指定";
            }
            else
            {
                if (string.IsNullOrEmpty(Face))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "輸出介面設定 必須指定";
                }
                if (string.IsNullOrEmpty(LoadMode))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "卡片格式 必須指定";
                }
                if (string.IsNullOrEmpty(BlockLength))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "WIEGAND 輸出的BITs 必須指定";
                }
                if (string.IsNullOrEmpty(Buzzer))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "讀卡機蜂鳴器 必須指定";
                }
            }
            #endregion

            #region LTitle
            if (string.Compare(Target, "popL_LTitle") == 0)
            {

                if (string.Compare(Face, "2C") == 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "輸出介面設定 為無效參數";
                }

                if (!int.TryParse(BlockLength.Trim(), out tempint) && !string.IsNullOrEmpty(BlockLength))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "WIEGAND 輸出的BITs 為無效參數";
                }
                else if (int.TryParse(BlockLength.Trim(), out tempint))
                {
                    if (tempint < 1 || tempint > 3)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "WIEGAND 輸出的BITs 為無效參數";
                    }
                }

                               
            }
            #endregion

            #region RTitle
            if (string.Compare(Target, "popL_RTitle") == 0)
            {

                if (string.Compare(LoadMode, "0C") == 0)
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "MIFARE KEY模式 為無效參數";
                }

                if (!int.TryParse(BlockLength.Trim(), out tempint) && !string.IsNullOrEmpty(BlockLength))
                {
                    if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                    objRet.result = false;
                    objRet.message += "讀取的BLOCK 為無效參數";
                }
                else if (int.TryParse(BlockLength.Trim(), out tempint))
                {
                    if (tempint < 0 || tempint > 64)
                    {
                        if (!string.IsNullOrEmpty(objRet.message)) objRet.message += "\n";
                        objRet.result = false;
                        objRet.message += "讀取的BLOCK 為無效參數";
                    }
                }

                if (Buzzer != "AA")
                {
                    if (!string.IsNullOrEmpty(objRet.message))
                    {
                        objRet.message += "\n";
                    }
                    objRet.result = false;
                    objRet.message += "讀卡機蜂鳴器 為無效參數";
                }
            }
            #endregion

            objRet.act = "CheckData";
            Response.Clear();
            Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(objRet));
            Response.End();
            //return objRet;
        }


        #endregion
    }
}
