using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;

namespace SahoAcs.uc
{
    public static class Extension
    {
        static public bool IsInArray(this string[] sa, string schtext)
        {
            foreach (string s in sa)
            {
                if (s.Equals(schtext))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public partial class MultiSelectDropDown : System.Web.UI.UserControl
    {
        private string _SelectedText;
        protected System.Web.UI.HtmlControls.HtmlTableCell colDDImage;

        #region  Public properties

        /// <summary>
        /// 取得或指定下拉選單的寬度
        /// </summary>
        public double ListWidth
        {
            get { return Panel2.Width.Value; }
            set { Panel2.Width = (Unit)value; }
        }
        /// <summary>
        /// <para>取得或指定下拉選單的高度</para>
        /// <para>0表示清除設定(auto高度設定)</para>
        /// <para>自動高度 13個項目內採用自動高度，超過13個就固定高度</para>
        /// </summary>
        public double ListHeight
        {
            get { return DDList.Height.Value; }
            set {
                if (value == 0)
                {//自動高度 13個項目內採用自動高度，超過13個就固定高度
                    DDList.Height = (DDList.Items.Count > 13) ? (Unit)250 : Unit.Empty;
                }
                else
                {
                    DDList.Height = (Unit)value;
                }
            }
        }

        /// <summary>
        /// 取得挑選的值陣列(string) 
        /// </summary>
        public ArrayList SelectedValues
        {
            get
            {
                ArrayList selectedValues = new ArrayList();
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    { selectedValues.Add(li.Value); }
                }
                return selectedValues;
            }
        }
        /// <summary>
        /// <para>取得以逗號隔開的值字串 或 利用逗號隔開的值字串來指定挑選項目。</para>
        /// <para>必須DDList的Item項目大於0時才有效。</para>
        /// </summary>
        public string SelectedValuesCSV
        {
            get
            {
                StringBuilder vals = new StringBuilder();
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    {
                        vals.Append(li.Value);
                        vals.Append(",");
                    }
                }
                if (vals.Length > 1)
                {
                    vals = vals.Remove(vals.Length - 1, 1);
                }
                return vals.ToString();
            }
            set
            {
                if (DDList.Items.Count > 0)
                {//只有列表項目時才有效
                    string[] vals = value.Split(',');
                    StringBuilder lbtxt = new StringBuilder();
                    foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                    {
                        if (vals.IsInArray(li.Value))
                        {
                            li.Selected = true;
                            lbtxt.Append(li.Text); lbtxt.Append(",");
                        }
                    }
                    lbtxt.Remove(lbtxt.Length - 1, 1);
                    DDLabel.Text = lbtxt.ToString();
                    DDLabel.ToolTip = DDLabel.Text;
                }
            }
        }

        /// <summary>
        /// 取得所挑選項目的文字，並以逗號隔開 或 依指定文字設定相對應項目。
        /// </summary>
        public string SelectedTextCSV
        {
            get
            {
                string selText = string.Empty;
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    { selText += li.Text + ","; }
                }
                if (selText.Length > 0)
                    selText = selText.Length > 0 ? selText.Substring(0, selText.Length - 1) : selText;
                return selText;
            }
            set
            {
                if (DDList.Items.Count > 0)
                {//只有列表項目時才有效
                    _SelectedText = value;
                    DDLabel.Text = _SelectedText;
                    DDLabel.ToolTip = _SelectedText;

                    string[] vals = value.Split(',');
                    StringBuilder lbtxt = new StringBuilder();
                    foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                    {
                        if (vals.IsInArray(li.Text))
                        { li.Selected = true; }
                    }
                }
            }
        }

        /// <summary>
        /// 取得所挑選項目的文字陣列
        /// </summary>
        public ArrayList SelectedTexts
        {
            get
            {
                ArrayList selectedTexts = new ArrayList();
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    { selectedTexts.Add(li.Text); }
                }
                return selectedTexts;
            }
        }

        /// <summary>
        /// 取得所挑選項目的文字，並以逗號隔開。
        /// </summary>
        public string SelectedText
        {
            get
            {
                string selText = string.Empty;
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    { selText += li.Text + ","; }
                }
                if (selText.Length > 0)
                    selText = selText.Length > 0 ? selText.Substring(0, selText.Length - 1) : selText;
                return selText;
            }
            set
            {
                _SelectedText = value;
                DDLabel.Text = _SelectedText;
                DDLabel.ToolTip = _SelectedText;
            }
        }

        /// <summary>
        /// Gets the selected items of the list
        /// </summary>
        public ArrayList SelectedItems
        {
            get
            {
                ArrayList selectedItems = new ArrayList();
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                {
                    if (li.Selected)
                    { selectedItems.Add(li); }
                }
                return selectedItems;
            }
            set
            {
                ArrayList selectedItems = value;
                string selText = string.Empty;

                // Deselect all the selected items
                foreach (System.Web.UI.WebControls.ListItem li in DDList.Items)
                { li.Selected = false; }

                // Select the items from the list
                foreach (System.Web.UI.WebControls.ListItem selItem in selectedItems)
                {
                    System.Web.UI.WebControls.ListItem li = DDList.Items.FindByText(selItem.Text);
                    if (li != null)
                    { li.Selected = true; selText += li.Text + ","; }
                }
                if (selText.Length > 0)
                    selText = selText.Length > 0 ? selText.Substring(0, selText.Length - 1) : selText;

                SelectedText = selText;
            }
        }
        /// <summary>
        /// Gets the list
        /// </summary>
        public System.Web.UI.WebControls.CheckBoxList List
        {
            get { return DDList; }
            set { DDList = List; }
        }

        /// <summary>
        /// 取得列表中的所有項目
        /// </summary>
        public ListItemCollection Items
        {
            get { return DDList.Items; }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// 清除所有列表項目
        /// </summary>
        public void Clear()
        {
            DDList.Items.Clear();
        }
        #endregion

        #region Private methods

        /// <summary>
        /// 初始化控制項的相關屬性
        /// </summary>
        public void PageInit()
        {
            this.ClientIDMode = System.Web.UI.ClientIDMode.AutoID;
            string ctlID = this.ClientID + "_";
            DDList.Attributes.Add("onchange", "SelectedIndexChanged('" + ctlID + "');");
            ddlBOX.Attributes.Add("style", "visibility: hidden; z-index: 9999; position: absolute;");
            DDLabel.Attributes.Add("onclick", "OpenListBox('" + ctlID + "');");
            DDLabel.Attributes.Add("style", "overflow: hidden;");
            colDDImage.Attributes.Add("onclick", "OpenListBox('" + ctlID + "');");
            btnCloseList.Attributes.Add("style", "width:100%");
            btnCloseList.OnClientClick = "CloseListBox('" + ctlID + "'); return false;";
        }

        /// <summary>
        /// 控制項Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PageInit();
            }
            else
            {	// set the selected text and tooltip
                DDLabel.Text = SelectedText;
                DDLabel.ToolTip = SelectedText;
            }
        }

        #endregion
    }
}