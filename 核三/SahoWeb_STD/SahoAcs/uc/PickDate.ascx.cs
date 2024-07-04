using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.uc
{
    public partial class PickDate : System.Web.UI.UserControl
    {
        #region Main Description
        bool readflag = false;
        #endregion

        #region Events

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && !readflag)
            {
                Clock.Attributes.Add("OnClick", "Call_PickDate('" + this.PickDateTextBox.ClientID.ToString() + "'); return false;");
                PickDateTextBox.Attributes.Add("OnClick", "Call_PickDate('" + this.PickDateTextBox.ClientID.ToString() + "'); return false;");
                PickDateTextBox.Attributes.Add("OnBlur", "CheckDate('" + this.PickDateTextBox.ClientID.ToString() + "')");
            }
            else if (readflag)
            {
                Clock.Attributes.Add("OnClick", "return false;");
                PickDateTextBox.Attributes.Add("OnClick", "return false;");
                PickDateTextBox.ReadOnly = true;
                Clock.ImageUrl = "/img/Clock_Gray.jpg";
            }
        }
        #endregion

        #region DateValue
        public string DateValue
        {
            get
            {
                return PickDateTextBox.Text.ToString();
            }
            set
            {
                PickDateTextBox.Text = value;
            }
        }
        #endregion

        #region SetWidth
        /// <summary>
        /// 欄位寬度設定
        /// </summary>
        /// <param name="Width">欄位寬度(需大於22)</param>
        public void SetWidth(int Width)
        {
            PickDateTextBox.Width = Width - 22;
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