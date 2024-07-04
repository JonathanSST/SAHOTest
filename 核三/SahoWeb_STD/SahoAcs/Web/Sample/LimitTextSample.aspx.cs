using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SahoAcs.Web.Sample
{
    public partial class LimitTextSample : System.Web.UI.Page
    {
        string engTest = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz";
        string chtTest = "臣亮言：先帝創業未半，而中道崩殂。今天下三分，益州疲弊，此誠危急存亡之秋也。然侍衛之臣，不懈於內；忠志之士，忘身於外者，蓋追先帝之殊";
        protected void Page_Load(object sender, EventArgs e)
        {
            Button1.Style.Add("margin", "0px 2px 0px 2px");
            Button1.Style.Add("padding", "2px 15px");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Table tb = new Table();
            TableRow tr;
            TableCell td;
            int tempint;
            if (!string.IsNullOrEmpty(TextBox1.Text.Trim()) && int.TryParse(TextBox1.Text.Trim(), out tempint))
            {
                tr = new TableRow();
                td = new TableCell();
                td.Width = tempint;

                td.Text = LimitText(engTest, (int)(tempint / 7.3), true);

                tr.Controls.Add(td);
                Table1.Controls.Add(tr);

                tr = new TableRow();
                td = new TableCell();
                td.Width = tempint;

                td.Text = LimitText(chtTest, (int)(tempint / 7.3), true);

                tr.Controls.Add(td);
                Table1.Controls.Add(tr);

                Table1.Style.Add("padding", "0");
                Table1.Style.Add("word-break", "break-all");
                Table1.Attributes.Add("border", "1");

                Label1.Text = "長度請設定為 : " + Math.Floor(tempint / 7.5);
            }
            else
            {
                TextBox1.Text = "請輸入長度!";
            }
        }

        public string LimitText(string str, int len, bool ellipsis)
        {
            Encoding big5 = Encoding.GetEncoding("big5");
            byte[] b = big5.GetBytes(str);
            if (b.Length <= len)
                return str;
            else
            {
                if (ellipsis) len -= 3;

                string res = big5.GetString(b, 0, len);
                if (!big5.GetString(b).StartsWith(res))
                    res = big5.GetString(b, 0, len - 1);
                return res + (ellipsis ? "..." : "");
            }
        }
    }
}