using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.uc
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        #region Main Description
        bool readflag = false;
        //protected System.Web.UI.HtmlControls.HtmlTable ucCalendar;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !readflag)
            {
                Clock.Attributes.Add("OnClick", "Call_Calendar('" + this.CalendarTextBox.ClientID.ToString() + "'); return false;");
                CalendarTextBox.Attributes.Add("OnClick", "Call_Calendar('" + this.CalendarTextBox.ClientID.ToString() + "'); return false;");
                CalendarTextBox.Attributes.Add("OnBlur", "CheckTime('" + this.CalendarTextBox.ClientID.ToString() + "')");
        }
            else if (readflag)
            {
                Clock.Attributes.Add("OnClick", "return false;");
                CalendarTextBox.Attributes.Add("OnClick", "return false;");
                CalendarTextBox.ReadOnly = true;
                Clock.ImageUrl = "/img/Clock_Gray.jpg";
            }
}
        #endregion

        #region DateValue
        public string DateValue
        {
            get { return CalendarTextBox.Text; }
            set { CalendarTextBox.Text = value; }
        }
        #endregion

        #region SetWidth
        /// <summary>
        /// 欄位寬度設定
        /// </summary>
        /// <param name="Width">欄位寬度(需大於22)</param>
        public void SetWidth(int Width)
        {
            CalendarTextBox.Width = Width - 22;
        }
        #endregion

        #region SetRequired
        /// <summary>
        /// 必填顏色更改
        /// </summary>
        public void SetRequired()
        {
            CalendarTextBox.BackColor = Color.FromName("#FFE5E5");
        }
        #endregion

        #region SetReadOnly
        public void SetReadOnly()
        {
            readflag = true;
        }
        #endregion

        #endregion

        #region Method

        #endregion
    }
}