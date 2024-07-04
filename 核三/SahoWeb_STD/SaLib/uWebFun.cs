using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sa.Web
{
    public class Fun
    {
        //------------------------------------------------------------------------------------
        public static void GridViewAddColumn(GridView oGridView, string HeaderText, string DataField, HorizontalAlign HAlign, Int32 nWidth)
        {
            BoundField oCol = new BoundField();
            oCol.HeaderStyle.HorizontalAlign = HAlign;
            oCol.HeaderText = HeaderText;
            oCol.DataField = DataField;
            oCol.ItemStyle.Width = new Unit(nWidth);
            oGridView.Columns.Add(oCol);
            Int32 iAdd = (oGridView.CellPadding + oGridView.CellSpacing + Convert.ToInt32(oCol.ItemStyle.BorderWidth.Value)) * 2;
            if (oGridView.Columns.Count == 1) oGridView.Width = new Unit(nWidth + iAdd);
            else oGridView.Width = new Unit(oGridView.Width.Value + oCol.ItemStyle.Width.Value + iAdd);
        }
        //------------------------------------------------------------------------------------
        public static void GridViewAddColumnTime(GridView oGridView, string HeaderText, string DataField, HorizontalAlign HAlign, Int32 nWidth)
        {
            BoundField oCol = new BoundField();
            oCol.DataFormatString = "{0: yyyy/MM/dd HH:mm:ss}";
            oCol.HeaderStyle.HorizontalAlign = HAlign;
            oCol.HeaderText = HeaderText;
            oCol.DataField = DataField;
            oCol.ItemStyle.Width = new Unit(nWidth);
            oGridView.Columns.Add(oCol);
            Int32 iAdd = (oGridView.CellPadding + oGridView.CellSpacing + Convert.ToInt32(oCol.ItemStyle.BorderWidth.Value)) * 2;
            if (oGridView.Columns.Count == 1) oGridView.Width = new Unit(nWidth + iAdd);
            else oGridView.Width = new Unit(oGridView.Width.Value + oCol.ItemStyle.Width.Value + iAdd);
        }

        //------------------------------------------------------------------------------------
        public static void GridRowCellLengthSet(TableCell oCell, Int32 iLength)
        {
            if (oCell.Text.Length > iLength)
            {
                oCell.ToolTip = oCell.Text;
                oCell.Text = oCell.Text.Substring(0, iLength - 2) + "...";
            }
        }

        //------------------------------------------------------------------------------------
        // 取得 Session 的資料，如果不存在時，回傳空白字串
        public static string GetSessionStr(Page oPage, string sName)
        {
            string sRet = "";
            if (sName != "")
            {
                if (oPage.Session[sName] != null) sRet = (string)oPage.Session[sName];
            }
            return sRet;
        }

        public static DateTime GetLocalTime(Page oPage)
        {
            if (oPage.Session["TimeOffset"] != null)
            {
                var offset = int.Parse(oPage.Session["TimeOffset"].ToString());
                return DateTime.UtcNow.AddMinutes(-offset);
            }
            return DateTime.Now;
        }


        //------------------------------------------------------------------------------------
        // 取得網址列上的參數資料
        public static string GetRequestStr(Page oPage, string sName)
        {
            string sRet = "";
            if (sName != "")
            {
                if (oPage.Request[sName] != null) sRet = (string)oPage.Request[sName];
            }
            return sRet;
        }

        //------------------------------------------------------------------------------------
        // 將頁面上指定類別的控制項，全部轉換成 JavaScript 上的物件
        public static string ControlToJavaScript(Page oPage)
        {
            string js = "";
            for (Int32 i = 0; i < oPage.Controls.Count; i++)
            {
                string sObjType = oPage.Controls[i].GetType().ToString();
                if (sObjType == "System.Web.UI.UpdatePanel") js += ControlToJavaScript(oPage.Controls[i]);
                if (sObjType.Length > 25)
                    if (sObjType.Substring(0, 26) == "System.Web.UI.WebControls.") js += ControlToJavaScript(oPage.Controls[i]);

                if (oPage.Controls[i].HasControls())
                    js = ControlToJavaScript(js, oPage.Controls[i]);
            }
            return js;
        }

        private static string ControlToJavaScript(string js, Control oObj)
        {
            for (Int32 i = 0; i < oObj.Controls.Count; i++)
            {
                string sObjType = oObj.Controls[i].GetType().ToString();
                if (sObjType == "System.Web.UI.UpdatePanel") js += ControlToJavaScript(oObj.Controls[i]);
                if (sObjType == "System.Web.UI.HtmlControls.HtmlSelect") js += ControlToJavaScript(oObj.Controls[i]);
                if (sObjType == "System.Web.UI.HtmlControls.HtmlInputCheckBox") js += ControlToJavaScript(oObj.Controls[i]);

                if (sObjType.Length > 25)
                    if (sObjType.Substring(0, 26) == "System.Web.UI.WebControls.") js += ControlToJavaScript(oObj.Controls[i]);

                if (oObj.Controls[i].HasControls())
                    js = ControlToJavaScript(js, oObj.Controls[i]);

            }
            return js;
        }

        //------------------------------------------------------------------------------------
        // 將頁面上個別的控制項，轉換成 JavaScript 上的物件
        public static string ControlToJavaScript(Control oObj)
        {
            Boolean IsOK = false;
            string js = "";

            if ((oObj.ID != "") && (oObj.ID != null))
            {
                string sObjType = oObj.GetType().ToString();
                Int32 nLen = oObj.ID.Length;
                try
                {
                    if (sObjType == "System.Web.UI.UpdatePanel") IsOK = true;
                    if ((sObjType == "System.Web.UI.HtmlControls.HtmlSelect") && (oObj.ID.Substring(0, 6) != "Select")) IsOK = true;
                    if ((sObjType == "System.Web.UI.HtmlControls.HtmlInputCheckBox") && (oObj.ID.Substring(0, 8) != "Checkbox")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.GridView") && (oObj.ID.Substring(0, 8) != "GridView")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.Panel") && (oObj.ID.Substring(0, 5) != "Panel")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.Label") && (oObj.ID.Substring(0, 5) != "Label")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.Image") && (oObj.ID.Substring(0, 5) != "Image")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.Button") && (oObj.ID.Substring(0, 6) != "Button")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.ListBox") && (oObj.ID.Substring(0, 7) != "ListBox")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.TextBox") && (oObj.ID.Substring(0, 7) != "TextBox")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.CheckBox") && (oObj.ID.Substring(0, 8) != "CheckBox")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.CheckBoxList") && (oObj.ID.Substring(0, 12) != "CheckBoxList")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.HyperLink") && (oObj.ID.Substring(0, 9) != "HyperLink")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.RadioButton") && (oObj.ID.Substring(0, 11) != "RadioButton")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.DropDownList") && (oObj.ID.Substring(0, 12) != "DropDownList")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.ImageButton") && (oObj.ID.Substring(0, 11) != "ImageButton")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.HiddenField") && (oObj.ID.Substring(0, 11) != "HiddenField")) IsOK = true;
                    if ((sObjType == "System.Web.UI.WebControls.TreeView") && (oObj.ID.Substring(0, 8) != "TreeView")) IsOK = true;
                }
                catch { IsOK = true; }

                if (IsOK)
                {
                    string[] calendarID;
                    string realcalendarID = "";
                    calendarID = oObj.ClientID.Split('_');
                    if (calendarID[calendarID.Length - 1] == "CalendarTextBox" ||
                        calendarID[calendarID.Length - 1] == "PickDateTextBox" ||
                        calendarID[calendarID.Length - 1] == "PickTimeTextBox" ||
                        (calendarID.Length >= 5 && calendarID[calendarID.Length - 2] == "DDList"))
                    {
                        for (int i = 1; i < calendarID.Length-1; i++)
                        {
                            if (realcalendarID != "") realcalendarID += "_";
                            realcalendarID += calendarID[i];
                        }
                        js += "\n var " + realcalendarID + ";" + realcalendarID + " = GetObj('" + oObj.ClientID + "');";
                    }
                    else
                        js += "\n var " + oObj.ID + ";" + oObj.ID + " = GetObj('" + oObj.ClientID + "');";
                }


            }
            return js;
        }

        //------------------------------------------------------------------------------------
        // 將頁面上個別的控制項，轉換成 JavaScript 上的物件
        public static string ControlToJavaScript(WebControl oObj)
        {
            return "\n var " + oObj.ID + ";" + oObj.ID + " = GetObj('" + oObj.ClientID + "');";
        }

        //------------------------------------------------------------------------------------
        // 直接執行頁面寫好的 JavaScript 
        public static void RunJavaScript(Page oPage, string sCmd)
        {
            string js = "";
            js = "<script type='text/javascript'>" + sCmd + "</script>";
            ScriptManager.RegisterClientScriptBlock(oPage, js.GetType(), Sa.Fun.GetRndString(), js, false);
        }
    }
}
