using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Script.Serialization;
using DapperDataObjectLib;
using SahoAcs.DBModel;
using SahoAcs.DBClass;


namespace SahoAcs.Unittest
{
    public partial class QueryTool : System.Web.UI.Page
    {
        OrmDataObject odo = new OrmDataObject("MsSql", Pub.GetDapperConnString());
        public string MainCmdStr = "";
        public List<EquData> ParaModels = new List<EquData>();
        public List<SahoAcs.DBModel.ParameterData> ParaList = new List<ParameterData>();
        public DataTable DataResult = new DataTable();
        public string[] ParaNames = { };
        public string[] ParaValues = { };
        public string[] ParaTypes = { };
        public string ErrorMsg = "";
        public List<dynamic> MasterResult = new List<dynamic>();
        public int SkipCount = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            ParaModels.AddObj("utf-8", "nvarchar字串")
                .AddObj("ansi", "varchar字串")
                .AddObj("datetime", "日期")
                .AddObj("number", "數字")
                .AddObj("double", "浮點數");
            if(Request["PageEvent"] != null && Request["PageEvent"] == "Query" || Request["PageEvent"]=="SkipQuery")
            {
                this.SetQueryData();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Export")
            {
                this.SetExport();
            }
            if (Request["PageEvent"] != null && Request["PageEvent"] == "Upload")
            {
                this.SetUpload();
            }
        }

        private void SetUpload()
        {
            //string[] name1 = Request.Files[0].FileName.Split('.');
            if (Request.Files.Count > 0)
            {
                //Stream inputStream = Request.Files[0].InputStream;
                using (var reader = new System.IO.StreamReader(Request.Files[0].InputStream))
                {
                    string json = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    //var result = serializer.DeserializeObject(json);             
                    dynamic resp = serializer.DeserializeObject(json);
                    try
                    {
                        if (resp != null && resp["MainCmdStr"] != null)
                        {
                            this.MainCmdStr = Convert.ToString(resp["MainCmdStr"]);
                            foreach (var s in resp["ParaList"])
                            {
                                this.ParaList.Add(new ParameterData()
                                {
                                    ParaName = Convert.ToString(s["ParaName"]),
                                    ParaType = Convert.ToString(s["ParaType"]),
                                    ParaValue = Convert.ToString(s["ParaValue"])
                                });
                                this.ParaNames = this.ParaList.Select(i => i.ParaName).ToArray();
                                this.ParaValues = this.ParaList.Select(i => i.ParaValue).ToArray();
                                this.ParaTypes = this.ParaList.Select(i => i.ParaType).ToArray();
                            }
                            //string result_json = Convert.ToString(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ErrorMsg = ex.Message;
                    }                   
                }//完成檔案匯入
            }//完成檔案串流判斷
            
        }//end upload method


        private void SetExport()
        {
            var serialize = new JavaScriptSerializer();
            string CmdStr = Request["CommandTxt"].ToString();
            this.MainCmdStr = CmdStr;
            this.ParaTypes = Request.Form.GetValues("ParaType");
            this.ParaValues = Request.Form.GetValues("ParaValue");
            this.ParaNames = Request.Form.GetValues("ParaName");
            if (Request.Form.GetValues("ParaName") != null)
            {
                int index = 0;
                foreach (string p in Request.Form.GetValues("ParaName"))
                {
                    //this.ParaList.AddNewObj<ParameterData>(new ParameterData());
                    this.ParaList.Add(new DBModel.ParameterData()
                    {
                        ParaName=this.ParaNames[index],
                        ParaType=this.ParaTypes[index],
                        ParaValue=this.ParaValues[index]
                    }); 
                    index++;
                }
            }
            var StrJson = serialize.Serialize(new { MainCmdStr = Request["CommandTxt"].ToString(), ParaList=this.ParaList});
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=QueryFile.json");
            Response.ContentType = "text/plain";
            //Response.BinaryWrite(pck.GetAsByteArray());
            Response.Write(StrJson);
            Response.End();
            int DoLength = StrJson.Length;
        }

        private void SetQueryData()
        {
            if (Request["PageEvent"] == "SkipQuery")
            {
                SkipCount = int.Parse(Request["SkipCount"]);
                SkipCount += 500;
            }
            string CmdStr = Request["CommandTxt"].ToString();
            this.MainCmdStr = CmdStr;
            int index = 0;
            try
            {
                this.ParaTypes = Request.Form.GetValues("ParaType");
                this.ParaValues = Request.Form.GetValues("ParaValue");
                this.ParaNames = Request.Form.GetValues("ParaName");
                if (Request.Form.GetValues("ParaName") != null)
                {
                    foreach (string p in Request.Form.GetValues("ParaName"))
                    {
                        this.odo.SetParameterClear();
                        string typeName = Request.Form.GetValues("ParaType")[index];
                        string value = Request.Form.GetValues("ParaValue")[index];
                        switch (typeName)
                        {
                            case "utf-8":
                                this.odo.AddStringDapper(p, value);
                                break;
                            case "ansi":
                                this.odo.AddAnsiDapper(p, value);
                                break;
                            case "datetime":
                                this.odo.AddDateTimeDapper(p, Convert.ToDateTime(value));
                                break;
                            case "number":
                                this.odo.AddIntegerDapper(p, Convert.ToInt64(value));
                                break;
                            case "double":
                                this.odo.AddDoubleDapper(p, Convert.ToDouble(value));
                                break;
                        }
                        index++;
                    }
                }
                this.MasterResult = this.odo.GetQueryResult(CmdStr).ToList();
                this.DataResult = this.ToDataTable<dynamic>(this.MasterResult.Skip(this.SkipCount).Take(500).ToList());
                var count = this.DataResult.Rows.Count;
                this.ErrorMsg = this.odo.DbExceptionMessage;
            }
            catch (Exception ex)
            {
                this.ErrorMsg = ex.Message;
            }            
        }//end method

        


        public DataTable ToDataTable<T>(List<dynamic> items)
        {

            DataTable dtDataTable = new DataTable();
            if (items.Count == 0)
                return dtDataTable;
            
            ((IEnumerable)items[0]).Cast<dynamic>().Select(p => p.Key).ToList().ForEach(col => { dtDataTable.Columns.Add(col); });

            ((IEnumerable)items).Cast<dynamic>().ToList().
                ForEach(data =>
                {
                    DataRow dr = dtDataTable.NewRow();
                    ((IEnumerable)data).Cast<dynamic>().ToList().ForEach(Col => { dr[Col.Key] = Col.Value; });
                    dtDataTable.Rows.Add(dr);
                });
            return dtDataTable;
        }


    }//end form class
}//end namespace