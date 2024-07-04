using Sa.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections;

namespace SahoAcs.WebService
{
    /// <summary>
    ///DriverWS 的摘要描述
    /// </summary>
    [WebService(Namespace = "SahoWS")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class DriverWS : System.Web.Services.WebService
    {
        [WebMethod(Description = "取得所有設消碼異動資料")]
        public XmlDocument GetCardAuthData(String sDciNo, String sIP, String sPassword, String sEquNo, String isMulti)
        {
            //webservice/driverws.asmx
            //sDciNo = "CH_Dci02";
            //sIP = "192.168.1.222";
            //sPassword = "1234";
            //if (sEquNo == "") sEquNo = "CH001";
            //isMulti = "1";
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            List<String> LCardData = new List<String>();
            Hashtable hCardData = new Hashtable();
            Hashtable hCardSetData = new Hashtable();
            Hashtable hCardOpMode = new Hashtable();
            Hashtable hCardOpStatus = new Hashtable();
            Hashtable hCardProcKey = new Hashtable();
            int istat = 0;
            String SendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            List<string> liSqlPara = new List<string>();
            string ColumnData = "CtrlID,CtrlNo,CtrlAddr,CtrlModel,ReaderNo,EquNo,CardNo,CardVer,CardPW,CardRule,CardExtData,BeginTime,EndTime,OpMode,OpStatus,ProcKey,MstConnParam";
            List<string> RowData = new List<string>();
            string sql = "", sql2 = "";
            #region 先取得該設備的設消資料
            sql += " SELECT TOP (100)" + ColumnData + " FROM V_CardAuthProc ";
            sql += " WHERE EquNo = '" + sEquNo + "' AND DciNo = '" + sDciNo + "' AND IpAddress = '" + sIP + "' AND DciPassWD = '" + sPassword + "' ";
            oAcsDB.GetDataReader(sql, out dr);
            sql = "";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    CardData oCard = new CardData();
                    CardCtrlData oCardCtrl = new CardCtrlData();
                    if (!hCardData.ContainsKey(dr.DataReader["CardNo"].ToString()))
                    {
                        oCardCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                        oCardCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                        oCardCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                        oCardCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                        oCardCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                        oCard.EquNo = dr.DataReader["EquNo"].ToString();
                        oCard.CardNo = dr.DataReader["CardNo"].ToString();
                        oCard.CardVer = dr.DataReader["CardVer"].ToString();
                        oCard.CardPW = dr.DataReader["CardPW"].ToString();
                        oCardCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                        oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString();
                        oCardCtrl.CardExtData = dr.DataReader["CardExtData"].ToString();
                        oCardCtrl.BeginTime = DateTime.Parse(dr.DataReader["BeginTime"].ToString());
                        oCardCtrl.EndTime = DateTime.Parse(dr.DataReader["EndTime"].ToString());
                        oCardCtrl.OpMode = dr.DataReader["OpMode"].ToString();
                        oCardCtrl.ProcKey = dr.DataReader["ProcKey"].ToString();
                        if (oCardCtrl.OpMode == "Del")
                        {
                            oCardCtrl.ReaderNo = 0;
                            sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Deleting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";
                        }
                        else if (oCardCtrl.OpMode == "Reset")
                            sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Resetting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";
                        else
                            sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Setting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";

                        oCardCtrl.OpStatus = dr.DataReader["OpStatus"].ToString();
                        oCardCtrl.SetData = oCardCtrl.ProcKey + ";" + oCard.CardVer + ";" + oCard.CardPW + ";" + oCardCtrl.CardRule + ";" + oCardCtrl.CardExtData;
                        oCard.hCardCtrlData.Add(oCardCtrl.CtrlNo, oCardCtrl);
                        LCardData.Add(oCard.CardNo);
                        hCardData.Add(oCard.CardNo, oCard);
                        hCardOpMode.Add(oCard.CardNo, oCardCtrl.OpMode);
                        hCardOpStatus.Add(oCard.CardNo, oCardCtrl.OpStatus);
                        hCardProcKey.Add(oCard.CardNo, oCardCtrl.ProcKey);
                        hCardSetData.Add(oCard.CardNo, oCardCtrl.ProcKey + ";" + oCard.CardVer + ";" + oCard.CardPW + ";" + oCard.CardRule + ";" + oCard.CardExtData);
                        sql += " SELECT * FROM V_CardAuth WHERE CardNo = '" + oCard.CardNo + "' AND CtrlNo = '" + oCardCtrl.CtrlNo + "' AND EquNo <> '" + sEquNo + "' UNION ";

                    }
                    else
                    {
                        oCard = (CardData)hCardData[dr.DataReader["CardNo"].ToString()];
                        if (oCard.hCardCtrlData.ContainsKey(dr.DataReader["CtrlNo"].ToString()))
                        {
                            oCardCtrl = (CardCtrlData)oCard.hCardCtrlData[dr.DataReader["CtrlNo"].ToString()];
                            if (dr.DataReader["OpMode"].ToString() == oCardCtrl.OpMode)
                            {
                                if (dr.DataReader["OpMode"].ToString() != "Del")
                                {
                                    oCardCtrl.ReaderNo += int.Parse(dr.DataReader["ReaderNo"].ToString());

                                    if (dr.DataReader["OpMode"].ToString() == "Reset")
                                        sql2 += "  UPDATE B01_CardAuth SET OpStatus = 'Resetting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                    else
                                        sql2 += "  UPDATE B01_CardAuth SET OpStatus = 'Setting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                }
                                else
                                {
                                    //oCard.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                                    oCardCtrl.ReaderNo = 0;
                                    sql2 += "  UPDATE B01_CardAuth SET OpStatus = 'Deleting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                }

                                if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                                    oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString() + "," + oCardCtrl.CardRule;
                                else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                                    oCardCtrl.CardRule += "," + dr.DataReader["CardRule"].ToString();
                            }
                            else
                            {
                                if (dr.DataReader["OpMode"].ToString() != "Del")
                                    oCardCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());

                                if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                                    oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString() + "," + oCardCtrl.CardRule;
                                else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                                    oCardCtrl.CardRule += "," + dr.DataReader["CardRule"].ToString();
                            }
                        }
                        else
                        {
                            oCardCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                            oCardCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                            oCardCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                            oCardCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                            oCardCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                            oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString();
                            oCardCtrl.CardExtData = dr.DataReader["CardExtData"].ToString();
                            oCardCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                            oCardCtrl.BeginTime = DateTime.Parse(dr.DataReader["BeginTime"].ToString());
                            oCardCtrl.EndTime = DateTime.Parse(dr.DataReader["EndTime"].ToString());
                            oCardCtrl.OpMode = dr.DataReader["OpMode"].ToString();
                            oCardCtrl.ProcKey = dr.DataReader["ProcKey"].ToString();
                            if (oCardCtrl.OpMode == "Del")
                            {
                                oCardCtrl.ReaderNo = 0;
                                sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Deleting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";
                            }
                            else if (oCardCtrl.OpMode == "Reset")
                                sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Resetting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";
                            else
                                sql2 += " UPDATE B01_CardAuth SET SendTime = '" + SendTime + "',OpStatus = 'Setting' WHERE ProcKey = '" + oCardCtrl.ProcKey + "'; ";

                            oCardCtrl.OpStatus = dr.DataReader["OpStatus"].ToString();
                            oCardCtrl.SetData = oCardCtrl.ProcKey + ";" + oCard.CardVer + ";" + oCard.CardPW + ";" + oCardCtrl.CardRule + ";" + oCardCtrl.CardExtData;
                            oCard.hCardCtrlData.Add(oCardCtrl.CtrlNo, oCardCtrl);
                        }
                    }
                    oCard = null;
                    oCardCtrl = null;
                }
            }

            #region 專門給兩台讀卡機以上的設備計算
            if (sql2 != "")
            {
                oAcsDB.BeginTransaction();
                istat = oAcsDB.SqlCommandExecute(sql2, liSqlPara);
                oAcsDB.Commit();

                if (isMulti != "")
                {
                    if (sql != "")
                    {
                        sql = sql.Substring(0, sql.Length - 6);
                        oAcsDB.GetDataReader(sql, out dr);
                        sql = "";
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                CardData oCard = new CardData();
                                CardCtrlData oCardCtrl = new CardCtrlData();
                                if (!hCardData.ContainsKey(dr.DataReader["CardNo"].ToString()))
                                {
                                    //oCard.CtrlID = dr.DataReader["CtrlID"].ToString();
                                    //oCard.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                                    //oCard.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                                    //oCard.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                                    //oCard.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                                    //oCard.EquNo = sEquNo;
                                    //oCard.CardNo = dr.DataReader["CardNo"].ToString();
                                    //oCard.CardVer = dr.DataReader["CardVer"].ToString();
                                    //oCard.CardPW = dr.DataReader["CardPW"].ToString();
                                    //oCard.CardRule = dr.DataReader["CardRule"].ToString();
                                    //oCard.CardExtData = dr.DataReader["CardExtData"].ToString();
                                    //oCard.BeginTime = DateTime.Parse(dr.DataReader["BeginTime"].ToString());
                                    //oCard.EndTime = DateTime.Parse(dr.DataReader["EndTime"].ToString());
                                    //if (dr.DataReader["EquNo"].ToString() == sEquNo)
                                    //{
                                    //    oCard.OpMode = dr.DataReader["OpMode"].ToString();
                                    //    oCard.OpStatus = dr.DataReader["OpStatus"].ToString();
                                    //    oCard.ProcKey = dr.DataReader["ProcKey"].ToString();
                                    //}
                                    //else
                                    //{
                                    //    oCard.OpMode = (String)hCardOpMode[oCard.CardNo];
                                    //    oCard.OpStatus = (String)hCardOpStatus[oCard.CardNo];
                                    //}
                                    //oCard.SetData = (String)hCardSetData[oCard.CardNo];
                                    //LCardData.Add(oCard.CardNo);
                                    //hCardData.Add(oCard.CardNo, oCard);
                                }
                                else
                                {
                                    oCard = (CardData)hCardData[dr.DataReader["CardNo"].ToString()];
                                    oCardCtrl = (CardCtrlData)oCard.hCardCtrlData[dr.DataReader["CtrlNo"].ToString()];
                                    if (dr.DataReader["OpMode"].ToString() == oCardCtrl.OpMode)
                                    {
                                        if (dr.DataReader["OpMode"].ToString() != "Del")
                                        {
                                            oCardCtrl.ReaderNo += int.Parse(dr.DataReader["ReaderNo"].ToString());

                                            if (dr.DataReader["OpMode"].ToString() == "Reset")
                                                sql += "  UPDATE B01_CardAuth SET OpStatus = 'Resetting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                            else
                                                sql += "  UPDATE B01_CardAuth SET OpStatus = 'Setting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                        }
                                        else
                                        {
                                            //oCard.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                                            oCardCtrl.ReaderNo = 0;
                                            sql += "  UPDATE B01_CardAuth SET OpStatus = 'Deleting',MultiKey = '" + (String)hCardProcKey[oCard.CardNo] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                        }

                                        if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                                            oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString() + "," + oCardCtrl.CardRule;
                                        else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                                            oCardCtrl.CardRule += "," + dr.DataReader["CardRule"].ToString();
                                    }
                                    else
                                    {
                                        if (dr.DataReader["OpMode"].ToString() != "Del")
                                            oCardCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());

                                        if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                                            oCardCtrl.CardRule = dr.DataReader["CardRule"].ToString() + "," + oCardCtrl.CardRule;
                                        else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                                            oCardCtrl.CardRule += "," + dr.DataReader["CardRule"].ToString();
                                    }
                                }
                                oCard = null;
                                oCardCtrl = null;
                            }
                        }
                        if (sql != "")
                        {
                            oAcsDB.BeginTransaction();
                            istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                            oAcsDB.Commit();
                        }
                    }
                }
            }
            #endregion

            #endregion

            if (LCardData.Count > 0)
            {
                string sdata = "";
                ColumnData = "EquNo,CardNo,CardVer,ReaderNo,CardPW,CardRule,CardExtData,ProcKey,OpMode,OpStatus,BeginTime,EndTime,SetData,MstConnParam,CtrlAddr";
                for (int i = 0; i < LCardData.Count; i++)
                {
                    CardData oCard = ((CardData)hCardData[LCardData[i]]);
                    CardCtrlData oCardCtrl = null;
                    foreach (DictionaryEntry obj in oCard.hCardCtrlData)
                    {
                        oCardCtrl = (CardCtrlData)obj.Value;
                        sdata = oCard.EquNo + "|";
                        sdata += oCard.CardNo + "|";
                        sdata += oCard.CardVer + "|";
                        sdata += oCardCtrl.ReaderNo.ToString() + "|";
                        sdata += oCard.CardPW + "|";
                        sdata += oCardCtrl.CardRule + "|";
                        sdata += oCardCtrl.CardExtData + "|";
                        sdata += oCardCtrl.ProcKey + "|";
                        sdata += oCardCtrl.OpMode + "|";
                        sdata += oCardCtrl.OpStatus + "|";
                        sdata += oCardCtrl.BeginTime + "|";
                        sdata += oCardCtrl.EndTime + "|";
                        sdata += oCardCtrl.SetData + "|";
                        sdata += oCardCtrl.MstConnParam + "|";
                        sdata += oCardCtrl.CtrlAddr + "|";
                        RowData.Add(sdata);
                        oCardCtrl = null;
                    }
                    oCard = null;
                }
            }
            LCardData = null;
            hCardData = null;
            hCardSetData = null;
            hCardOpStatus = null;
            hCardProcKey = null;
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得設備連線資訊資料")]
        public XmlDocument GetDeviceInfo(String sDciNo, String sIP, String sPassword)
        {
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string ColumnData = "DciID,DciNo,DciName,IsAssignIP,IpAddress,TcpPort,DciPassWD,MstID,MstNo,MstDesc,MstType,MstConnParam,MCtrlModel,LinkMode,AutoReturn,MstModel,MstFwVer,MstStatus,CtrlID,CtrlNo,CtrlName,CtrlDesc,CtrlAddr,CCtrlModel,CtrlFwVer,CtrlStatus,ReaderID,ReaderNo,ReaderName,ReaderDesc,EquID,EquNo,Dir,CardNoLen,EquClass,IsAndTrt,Building,Floor,InToCtrlAreaID,OutToCtrlAreaID";
            List<string> RowData = new List<string>();
            string sql = " SELECT " + ColumnData + " FROM V_MCRInfo WHERE DciNo = '" + sDciNo + "' AND IpAddress = '" + sIP + "' AND DciPassWD = '" + sPassword + "' ";
            
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sdata = "";
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        sdata += dr.DataReader[i].ToString() + "|";
                    RowData.Add(sdata);
                }
            }
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得批次設碼卡片資料")]
        public XmlDocument GetCardBlockInfo(String sDciNo, String sIP, String sPassword, String MstConnParam, String CtrlAddr)
        {
            Hashtable hCardData = new Hashtable();
            List<String> LCardData = new List<String>();
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            String CtrlNo = "";
            CardData oCardData = null;
            //string ColumnData = "CardNo,CardVer,ReadBit,CardPW,CardRule,CardExtData";
            string ColumnData = "CardNo,CardVer,ReaderNo,CardPW,CardRule,CardExtData";
            List<string> RowData = new List<string>();
            string sql = " SELECT CtrlNo FROM V_MCRInfo WHERE DciNo = '" + sDciNo + "' AND IpAddress = '" + sIP + "' AND DciPassWD = '" + sPassword + "' AND MstConnParam = '" + MstConnParam + "' AND CtrlAddr = '" + CtrlAddr + "' ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                    CtrlNo = dr.DataReader["CtrlNo"].ToString();
            }

            //sql = " SELECT CardNo,CardVer,SUM(ReaderNo) AS ReadBit,CardPW,CardRule,CardExtData FROM V_CardBlockData ";
            //sql += " WHERE CtrlNo = '" + CtrlNo + "' GROUP BY CardNo,CardVer,CardPW,CardRule,CardExtData ORDER BY CardNo ";
            sql = " SELECT " + ColumnData + " FROM V_CardBlockData ";
            sql += " WHERE CtrlNo = '" + CtrlNo + "' ORDER BY CardNo ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (!hCardData.ContainsKey(dr.DataReader["CardNo"].ToString()))
                    {
                        oCardData = new CardData();
                        oCardData.CardNo = dr.DataReader["CardNo"].ToString();
                        oCardData.CardVer = dr.DataReader["CardVer"].ToString();
                        oCardData.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                        oCardData.CardPW = dr.DataReader["CardPW"].ToString();
                        oCardData.CardRule = dr.DataReader["CardRule"].ToString();
                        oCardData.CardExtData = dr.DataReader["CardExtData"].ToString();
                        hCardData.Add(oCardData.CardNo, oCardData);
                        LCardData.Add(oCardData.CardNo);
                    }
                    else
                    {
                        oCardData = (CardData)hCardData[dr.DataReader["CardNo"].ToString()];
                        oCardData.ReaderNo += int.Parse(dr.DataReader["ReaderNo"].ToString());
                        oCardData.CardRule += "," + dr.DataReader["CardRule"].ToString();
                    }
                    oCardData = null;
                }
            }
            if (LCardData.Count > 0)
            {
                string sdata = "";
                for (int i = 0; i < LCardData.Count; i++)
                {
                    sdata = ((CardData)hCardData[LCardData[i]]).CardNo + "|";
                    sdata += ((CardData)hCardData[LCardData[i]]).CardVer + "|";
                    sdata += ((CardData)hCardData[LCardData[i]]).ReaderNo.ToString() + "|";
                    sdata += ((CardData)hCardData[LCardData[i]]).CardPW + "|";
                    sdata += ((CardData)hCardData[LCardData[i]]).CardRule + "|";
                    sdata += ((CardData)hCardData[LCardData[i]]).CardExtData + "|";
                    RowData.Add(sdata);
                }
            }
            hCardData = null;
            LCardData = null;
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得模組物件資訊")]
        public XmlDocument GetModelInfo(String sClass)
        {
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string ColumnData = "ItemClass,ItemOrder,ItemNo,ItemName,ItemInfo1,ItemInfo2,ItemInfo3";
            List<string> RowData = new List<string>();
            string sql = " SELECT " + ColumnData + " FROM B00_ItemList WHERE ItemClass = '" + sClass + "' ";

            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sdata = "";
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        sdata += dr.DataReader[i].ToString() + "|";
                    RowData.Add(sdata);
                }
            }
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得設備參數異動資料")]
        public XmlDocument GetEquParaData(String sDciNo, String sIP, String sPassword, String sEquNo, String isMulti)
        {
            //webservice/driverws.asmx
            //sDciNo = "CH_Dci02";
            //sIP = "192.168.1.222";
            //sPassword = "1234";
            //if (sEquNo == "") sEquNo = "CH001";
            //isMulti = "1";
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            List<String> LParaData = new List<String>();
            Hashtable hParaData = new Hashtable();
            Hashtable hParaSetData = new Hashtable();
            Hashtable hParaOpStatus = new Hashtable();
            Hashtable hParaProcKey = new Hashtable();
            int istat = 0;
            String SendTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            List<string> liSqlPara = new List<string>();
            string ColumnData = "EquNo,EquClass,EquModel,IsAndTrt,ParaName,ParaDesc,InputType,ValueOptions,ParaValue,M_ParaValue,ParaValueOther,IsReSend,OpStatus,ProcKey,CtrlID,CtrlNo,CtrlAddr,CtrlModel,ReaderNo,MstConnParam";
            List<string> RowData = new List<string>();
            string sql = "", sql2 = "";

            #region 先將無效指令先復歸
            sql = " UPDATE B01_EquParaData SET IsReSend = 0,OpStatus = '',SendTime = '',CompleteTime= '',MultiKey = '' ";
            sql += " WHERE ParaValue = '' AND M_ParaValue = '' AND IsReSend = 1 ;";
            oAcsDB.BeginTransaction();
            istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
            oAcsDB.Commit();
            #endregion

            #region 先取得該設備的設消資料
            sql += " SELECT " + ColumnData + " FROM V_EquParaProc ";
            sql += " WHERE EquNo = '" + sEquNo + "' AND DciNo = '" + sDciNo + "' AND IpAddress = '" + sIP + "' AND DciPassWD = '" + sPassword + "' AND (M_ParaValue <> ParaValue OR IsReSend = '1') ";
            oAcsDB.GetDataReader(sql, out dr);
            sql = "";
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EquParaData oEquPara = new EquParaData();
                    ParaCtrlData oParaCtrl = new ParaCtrlData();
                    if (!hParaData.ContainsKey(dr.DataReader["ParaName"].ToString() + "Set"))
                    {
                        oParaCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                        oParaCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                        oParaCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                        oParaCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                        oParaCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                        oEquPara.EquNo = dr.DataReader["EquNo"].ToString();
                        oEquPara.EquClass = dr.DataReader["EquClass"].ToString();
                        oEquPara.EquModel = dr.DataReader["EquModel"].ToString();
                        oEquPara.IsAndTrt = dr.DataReader["IsAndTrt"].ToString();
                        oEquPara.ParaName = dr.DataReader["ParaName"].ToString() + "Set";
                        oEquPara.ParaDesc = dr.DataReader["ParaDesc"].ToString();
                        oEquPara.InputType = dr.DataReader["InputType"].ToString();
                        oEquPara.ValueOptions = dr.DataReader["ValueOptions"].ToString();
                        oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString();
                        oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString();
                        oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString();
                        oEquPara.IsReSend = dr.DataReader["IsReSend"].ToString();
                        oEquPara.OpStatus = dr.DataReader["OpStatus"].ToString();
                        oEquPara.ProcKey = dr.DataReader["ProcKey"].ToString();
                        if (oEquPara.IsReSend == "0")
                            sql2 += " UPDATE B01_EquParaData SET SendTime = '" + SendTime + "',OpStatus = 'Setting' WHERE ProcKey = '" + oEquPara.ProcKey + "'; ";
                        else
                            sql2 += " UPDATE B01_EquParaData SET SendTime = '" + SendTime + "',OpStatus = 'Resetting' WHERE ProcKey = '" + oEquPara.ProcKey + "'; ";

                        oEquPara.SetData = oEquPara.ProcKey + ";" + oParaCtrl.ParaValue + ";" + oParaCtrl.ParaValueOther;
                        oParaCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                        oEquPara.hParaCtrlData.Add(oParaCtrl.CtrlNo, oParaCtrl);
                        LParaData.Add(oEquPara.ParaName);
                        hParaData.Add(oEquPara.ParaName, oEquPara);
                        hParaOpStatus.Add(oEquPara.ParaName, oEquPara.OpStatus);
                        hParaProcKey.Add(oEquPara.ParaName, oEquPara.ProcKey);
                        hParaSetData.Add(oEquPara.ParaName, oEquPara.SetData);
                        sql += " SELECT * FROM V_EquParaProc WHERE CtrlNo = '" + oParaCtrl.CtrlNo + "' AND EquNo <> '" + sEquNo + "' AND ParaName = '" + dr.DataReader["ParaName"].ToString() + "' UNION ";
                    }
                    else
                    {
                        oEquPara = (EquParaData)hParaData[dr.DataReader["ParaName"].ToString() + "Set"];
                        if (oEquPara.hParaCtrlData.ContainsKey(dr.DataReader["CtrlNo"].ToString()))
                        {
                            oParaCtrl = (ParaCtrlData)oEquPara.hParaCtrlData[dr.DataReader["CtrlNo"].ToString()];
                            oParaCtrl.ReaderNo += int.Parse(dr.DataReader["ReaderNo"].ToString());
                            if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                            {
                                oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString() + "@" + oParaCtrl.ParaValue;
                                oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString() + "@" + oParaCtrl.M_ParaValue;
                                oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString() + "@" + oParaCtrl.ParaValueOther;
                            }
                            else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                            {
                                oParaCtrl.ParaValue += "@" + dr.DataReader["ParaValue"].ToString();
                                oParaCtrl.M_ParaValue += "@" + dr.DataReader["M_ParaValue"].ToString();
                                oParaCtrl.ParaValueOther += "@" + dr.DataReader["ParaValueOther"].ToString();
                            }
                        }
                        else
                        {
                            oParaCtrl = new ParaCtrlData();
                            oParaCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                            oParaCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                            oParaCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                            oParaCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                            oParaCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                            oParaCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                            oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString();
                            oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString();
                            oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString();
                            oEquPara.hParaCtrlData.Add(oParaCtrl.CtrlNo, oParaCtrl);
                        }
                        hParaData[oEquPara.ParaName] = oEquPara;
                    }
                    oEquPara = null;
                    oParaCtrl = null;
                }
            }

            #region 專門給兩台讀卡機以上的設備計算
            if (sql2 != "")
            {
                oAcsDB.BeginTransaction();
                istat = oAcsDB.SqlCommandExecute(sql2, liSqlPara);
                oAcsDB.Commit();

                if (isMulti != "")
                {
                    if (sql != "")
                    {
                        sql = sql.Substring(0, sql.Length - 6);
                        oAcsDB.GetDataReader(sql, out dr);
                        sql = "";
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                EquParaData oEquPara = new EquParaData();
                                ParaCtrlData oParaCtrl = new ParaCtrlData();
                                if (!hParaData.ContainsKey(dr.DataReader["ParaName"].ToString() + "Set"))
                                {
                                    oParaCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                                    oParaCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                                    oParaCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                                    oParaCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                                    oParaCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                                    oEquPara.EquNo = sEquNo;
                                    oEquPara.EquClass = dr.DataReader["EquClass"].ToString();
                                    oEquPara.EquModel = dr.DataReader["EquModel"].ToString();
                                    oEquPara.IsAndTrt = dr.DataReader["IsAndTrt"].ToString();
                                    oEquPara.ParaName = dr.DataReader["ParaName"].ToString() + "Set";
                                    oEquPara.ParaDesc = dr.DataReader["ParaDesc"].ToString();
                                    oEquPara.InputType = dr.DataReader["InputType"].ToString();
                                    oEquPara.ValueOptions = dr.DataReader["ValueOptions"].ToString();
                                    oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString();
                                    oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString();
                                    oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString();
                                    oEquPara.IsReSend = dr.DataReader["IsReSend"].ToString();
                                    oEquPara.OpStatus = dr.DataReader["OpStatus"].ToString();
                                    oEquPara.ProcKey = dr.DataReader["ProcKey"].ToString();
                                    if (oEquPara.IsReSend == "0")
                                        sql += " UPDATE B01_EquParaData SET SendTime = '" + SendTime + "',OpStatus = 'Setting' WHERE ProcKey = '" + oEquPara.ProcKey + "'; ";
                                    else
                                        sql += " UPDATE B01_EquParaData SET SendTime = '" + SendTime + "',OpStatus = 'Resetting' WHERE ProcKey = '" + oEquPara.ProcKey + "'; ";

                                    oEquPara.SetData = oEquPara.ProcKey + ";" + oParaCtrl.ParaValue;

                                    if (dr.DataReader["EquNo"].ToString() == sEquNo)
                                    {
                                        oEquPara.OpStatus = dr.DataReader["OpStatus"].ToString();
                                        oEquPara.ProcKey = dr.DataReader["ProcKey"].ToString();
                                    }
                                    else
                                    {
                                        oEquPara.OpStatus = (String)hParaOpStatus[oEquPara.ParaName];
                                    }
                                    oEquPara.SetData = (String)hParaSetData[oEquPara.ParaName];
                                    oParaCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                                    oEquPara.hParaCtrlData.Add(oParaCtrl.CtrlNo, oParaCtrl);
                                    LParaData.Add(oEquPara.ParaName);
                                    hParaData.Add(oEquPara.ParaName, oEquPara);
                                }
                                else
                                {
                                    oEquPara = (EquParaData)hParaData[dr.DataReader["ParaName"].ToString() + "Set"];
                                    if (oEquPara.hParaCtrlData.ContainsKey(dr.DataReader["CtrlNo"].ToString()))
                                    {
                                        oParaCtrl = (ParaCtrlData)oEquPara.hParaCtrlData[dr.DataReader["CtrlNo"].ToString()];
                                        oParaCtrl.ReaderNo += int.Parse(dr.DataReader["ReaderNo"].ToString());
                                        if (dr.DataReader["IsReSend"].ToString() == "0")
                                            sql += "  UPDATE B01_EquParaData SET OpStatus = 'Setting',MultiKey = '" + (String)hParaProcKey[oEquPara.ParaName] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                        else
                                            sql += "  UPDATE B01_EquParaData SET OpStatus = 'Resetting',MultiKey = '" + (String)hParaProcKey[oEquPara.ParaName] + "' WHERE ProcKey = '" + dr.DataReader["ProcKey"].ToString() + "' ; ";
                                        if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 1)
                                        {
                                            oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString() + "@" + oParaCtrl.ParaValue;
                                            oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString() + "@" + oParaCtrl.M_ParaValue;
                                            oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString() + "@" + oParaCtrl.ParaValueOther;
                                        }
                                        else if (int.Parse(dr.DataReader["ReaderNo"].ToString()) == 2)
                                        {
                                            oParaCtrl.ParaValue += "@" + dr.DataReader["ParaValue"].ToString();
                                            oParaCtrl.M_ParaValue += "@" + dr.DataReader["M_ParaValue"].ToString();
                                            oParaCtrl.ParaValueOther += "@" + dr.DataReader["ParaValueOther"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        oParaCtrl = new ParaCtrlData();
                                        oParaCtrl.CtrlID = dr.DataReader["CtrlID"].ToString();
                                        oParaCtrl.CtrlNo = dr.DataReader["CtrlNo"].ToString();
                                        oParaCtrl.CtrlAddr = dr.DataReader["CtrlAddr"].ToString();
                                        oParaCtrl.CtrlModel = dr.DataReader["CtrlModel"].ToString();
                                        oParaCtrl.ReaderNo = int.Parse(dr.DataReader["ReaderNo"].ToString());
                                        oParaCtrl.MstConnParam = dr.DataReader["MstConnParam"].ToString();
                                        oParaCtrl.ParaValue = dr.DataReader["ParaValue"].ToString();
                                        oParaCtrl.M_ParaValue = dr.DataReader["M_ParaValue"].ToString();
                                        oParaCtrl.ParaValueOther = dr.DataReader["ParaValueOther"].ToString();
                                        oEquPara.hParaCtrlData.Add(oParaCtrl.CtrlNo, oParaCtrl);
                                    }
                                    hParaData[oEquPara.ParaName] = oEquPara;
                                }
                                oEquPara = null;
                                oParaCtrl = null;
                            }
                        }
                        if (sql != "")
                        {
                            oAcsDB.BeginTransaction();
                            istat = oAcsDB.SqlCommandExecute(sql, liSqlPara);
                            oAcsDB.Commit();
                        }
                    }
                }
            }
            #endregion

            #endregion

            if (LParaData.Count > 0)
            {
                string sdata = "";
                ColumnData = "EquNo,EquClass,EquModel,ReaderNo,ParaName,ParaValue,M_ParaValue,ParaValueOther,ProcKey,IsReSend,OpStatus,SetData,CtrlAddr,MstConnParam";
                for (int i = 0; i < LParaData.Count; i++)
                {
                    EquParaData oEquPara = ((EquParaData)hParaData[LParaData[i]]);
                    ParaCtrlData oParaCtrl = null;
                    foreach (DictionaryEntry obj in oEquPara.hParaCtrlData)
                    {
                        oParaCtrl = (ParaCtrlData)obj.Value;
                        sdata = oEquPara.EquNo + "|";
                        sdata += oEquPara.EquClass + "|";
                        sdata += oEquPara.EquModel + "|";
                        sdata += oParaCtrl.ReaderNo.ToString() + "|";
                        sdata += oEquPara.ParaName + "|";
                        sdata += oParaCtrl.ParaValue + "|";
                        sdata += oParaCtrl.M_ParaValue + "|";
                        sdata += oParaCtrl.ParaValueOther + "|";
                        sdata += oEquPara.ProcKey + "|";
                        sdata += oEquPara.IsReSend + "|";
                        sdata += oEquPara.OpStatus + "|";
                        sdata += oEquPara.SetData + "|";
                        sdata += oParaCtrl.CtrlAddr + "|";
                        sdata += oParaCtrl.MstConnParam + "|";
                        RowData.Add(sdata);
                        oParaCtrl = null;
                    }
                    oEquPara = null;
                }
            }
            LParaData = null;
            hParaData = null;
            hParaSetData = null;
            hParaOpStatus = null;
            hParaProcKey = null;
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得管制及刷卡時區資料")]
        public XmlDocument GetTimeTableInfo(String sCtrlModel, String sTimeID)
        {
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string ColumnData = "TimeInfo";
            List<string> RowData = new List<string>();
            string sql = " SELECT " + ColumnData + " FROM B01_TimeTableDef WHERE EquModel = '" + sCtrlModel + "' AND TimeID = " + sTimeID + " ; ";

            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sdata = "";
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        sdata += dr.DataReader[i].ToString() + "|";
                    RowData.Add(sdata);
                }
            }
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得系統參數資料")]
        public XmlDocument GetSysParameterInfo(String sParaClass, String sParaNo)
        {
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string ColumnData = "ParaClass,ParaNo,ParaName,ParaValue";
            List<string> RowData = new List<string>();
            string sql = " SELECT " + ColumnData + " FROM B00_SysParameter WHERE ParaClass = '" + sParaClass + "' ";
            if (sParaNo != "")
                sql += " AND ParaNo = '" + sParaNo + "' ; ";

            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sdata = "";
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        sdata += dr.DataReader[i].ToString() + "|";
                    RowData.Add(sdata);
                }
            }
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        [WebMethod(Description = "取得讀卡規則資料")]
        public XmlDocument GetCardRuleInfo(String sRuleID)
        {
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            string ColumnData = "RuleID,EquModel,RuleNo,RuleName,RuleInfo";
            List<string> RowData = new List<string>();
            string sql = " SELECT " + ColumnData + " FROM B01_CardRuleDef WHERE RuleID = '" + sRuleID + "' ; ";

            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string sdata = "";
                    for (int i = 0; i < dr.DataReader.FieldCount; i++)
                        sdata += dr.DataReader[i].ToString() + "|";
                    RowData.Add(sdata);
                }
            }
            oAcsDB.CloseConnect();
            return XmlData(ColumnData, RowData);
        }

        private bool CheckKey(String sDciNo, String sIP, String sPassword)
        {
            bool ret = false;
            //sIP = IPAddZero(sIP);
            DB_Acs oAcsDB = new DB_Acs(Pub.GetConnectionString(Pub.sConnName));
            DBReader dr = null;
            List<string> RowData = new List<string>();
            string sql = " SELECT * FROM V_MCRInfo WHERE DciNo = '" + sDciNo + "' AND IpAddress = '" + sIP + "' AND DciPassWD = '" + sPassword + "' ";
            oAcsDB.GetDataReader(sql, out dr);
            if (dr.HasRows)
                ret = true;
            oAcsDB.CloseConnect();
            return ret;
        }

        #region 物件
        private class CardData
        {
            public String CardNo { get; set; }
            public String CardVer { get; set; }
            public String CardPW { get; set; }
            public String CardRule { get; set; }        //單機設碼才會使用
            public String CardExtData { get; set; }     //單機設碼才會使用
            public String EquNo { get; set; }
            public int ReaderNo { get; set; }           //單機設碼才會使用

            public Hashtable hCardCtrlData = new Hashtable();   //自動設消碼使用，以CtrlNo為Key，CardCtrlData物件為Value
            //public String CtrlID { get; set; }
            //public String CtrlNo { get; set; }
            //public String CtrlAddr { get; set; }
            //public String CtrlModel { get; set; }
            //public int ReaderNo { get; set; }
            //public String EquNo { get; set; }
            //public DateTime BeginTime { get; set; }
            //public DateTime EndTime { get; set; }
            //public String OpMode { get; set; }
            //public String OpStatus { get; set; }
            //public String ProcKey { get; set; }
            //public String SetData { get; set; }
        }

        public class CardCtrlData       //自動設消碼使用
        {
            public String CtrlID { get; set; }
            public String CtrlNo { get; set; }
            public String CtrlAddr { get; set; }
            public String CtrlModel { get; set; }
            public int ReaderNo { get; set; }
            public String CardRule { get; set; }
            public String CardExtData { get; set; }
            public DateTime BeginTime { get; set; }
            public DateTime EndTime { get; set; }
            public String OpMode { get; set; }
            public String OpStatus { get; set; }
            public String ProcKey { get; set; }
            public String SetData { get; set; }
            public String MstConnParam { get; set; }
        }

        public class EquParaData
        {
            public String EquNo { get; set; }
            public String EquClass { get; set; }
            public String EquModel { get; set; }
            public String IsAndTrt { get; set; }
            public String ParaName { get; set; }
            public String ParaDesc { get; set; }
            public String InputType { get; set; }
            public String ValueOptions { get; set; }
            public String IsReSend { get; set; }
            public String OpStatus { get; set; }
            public String ProcKey { get; set; }
            public String MultiKey { get; set; }
            public String SetData { get; set; }
            public Hashtable hParaCtrlData = new Hashtable();   //以CtrlNo為Key，ParaCtrlData物件為Value
        }

        public class ParaCtrlData
        {
            public String CtrlID { get; set; }
            public String CtrlNo { get; set; }
            public String CtrlAddr { get; set; }
            public String CtrlModel { get; set; }
            public int ReaderNo { get; set; }
            public String MstConnParam { get; set; }
            public String ParaValue { get; set; }
            public String M_ParaValue { get; set; }
            public String ParaValueOther { get; set; }
        }

        #endregion

        #region 轉換XML

        public string ChangeXML(string sColumn, List<string> sData)
        {
            string[] ColumnList = sColumn.Split(',');
            string xml = "<SahoData>";
            for (int i = 0; i < sData.Count; i++)
            {
                xml += "<Row>";
                string[] DataList = sData[i].Split('|');
                for (int j = 0; j < DataList.Length - 1; j++)
                    xml += "<" + ColumnList[j] + ">" + DataList[j] + "</" + ColumnList[j] + ">";
                xml += "</Row>";
            }
            xml += "</SahoData>";
            return xml;
        }

        public XmlDocument XmlData(string ColumnData, List<String> RowData)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (RowData.Count > 0)
                xmlDoc.LoadXml(ChangeXML(ColumnData, RowData));
            else
                xmlDoc.LoadXml("<SahoData>not find</SahoData>");
            return xmlDoc;
        }

        #endregion

        #region IP轉換

        private String IPAddZero(String sIP)
        {
            if (sIP != "")
            {
                string[] siplist = sIP.Split('.');
                sIP = "";
                for (int i = 0; i < siplist.Length; i++)
                    sIP += siplist[i].PadLeft(3, '0') + ".";
                sIP = sIP.Substring(0, sIP.Length - 1);
            }
            return sIP;
        }

        private String IPRemoveZero(String sIP)
        {
            if (sIP != "")
            {
                string[] siplist = sIP.Split('.');
                sIP = "";
                for (int i = 0; i < siplist.Length; i++)
                    sIP += siplist[i].TrimStart('0') + ".";
                sIP = sIP.Substring(0, sIP.Length - 1);
            }
            return sIP;
        }

        #endregion
    }
}
