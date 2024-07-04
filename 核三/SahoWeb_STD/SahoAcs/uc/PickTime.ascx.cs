using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.uc
{
    public partial class PickTime : System.Web.UI.UserControl
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
                Clock.Attributes.Add("OnClick", "Call_PickTime('" + this.PickTimeTextBox.ClientID.ToString() + "'); return false;");
                PickTimeTextBox.Attributes.Add("OnClick", "Call_PickTime('" + this.PickTimeTextBox.ClientID.ToString() + "'); return false;");
                PickTimeTextBox.Attributes.Add("OnBlur", "CheckTime('" + this.PickTimeTextBox.ClientID.ToString() + "')");
            }
            else if (readflag)
            {
                Clock.Attributes.Add("OnClick", "return false;");
                PickTimeTextBox.Attributes.Add("OnClick", "return false;");
                PickTimeTextBox.ReadOnly = true;
                Clock.ImageUrl = "/img/Clock_Gray.jpg";
            }
        }
        #endregion

        #region DateValue
        public string DateValue
        {
            get
            {
                return PickTimeTextBox.Text.ToString();
            }
            set
            {
                PickTimeTextBox.Text = value;
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
            PickTimeTextBox.Width = Width - 22;
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