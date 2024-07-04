<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="MultiSelectDropDown.ascx.cs" Inherits=" SahoAcs.uc.MultiSelectDropDown" %>

<style type="text/css">
    .TextBox {
        border: 1px solid #FFF;
        -webkit-border-radius: 3px;
        -moz-border-radius: 2px;
        border-radius: 3px;
    }

    .ListBox {
        overflow: auto;
        font-family: "微軟正黑體","Arial";
        font-size: 14px;
        color: black;
    }

    .down_arrow {
        background-image: url("data:image/jpeg;base64,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");
        background-size: 22px;
        background-repeat: no-repeat;
        height: 10px;
    }

    .ddltd {
        padding: 0px 2px;
        margin: 0px;
        color: black;
    }
</style>
<script type="text/javascript" lang="javascript">

    function SelectedIndexChanged(ctlId) {
        var control = document.getElementById(ctlId + 'DDList');
        var chks = control.getElementsByTagName('input');
        var lbs = control.getElementsByTagName('label');
        var strSelText = '';

        for (var i = 0; i < chks.length; i++) {
            var chk = chks[i];
            if (chk.checked) {
                strSelText += lbs[i].innerHTML + ',';
                strSelText = strSelText.replace('<span style="color:#FFFFFF\">--</span>', '');
            }
        }
        if (strSelText.length > 0)
            strSelText = strSelText.substring(0, strSelText.length - 1);
        console.log(strSelText);
        var ddLabel = document.getElementById(ctlId + "DDLabel");
        ddLabel.innerHTML = strSelText;
        ddLabel.innerText = strSelText;
        ddLabel.title = strSelText;
    }

    function OpenListBox(ctlId) {
        var pnl = document.getElementById(ctlId + "Panel2");
        var lstBox = document.getElementById(ctlId + "DDList");
        var ddlbox = document.getElementById(ctlId + "ddlBOX");

        if (ddlbox.style.visibility == "visible")
        { CloseListBox(ctlId); }
        else {
            ddlbox.style.visibility = "visible";
            if (lstBox != null) {
                //lstBox.style.height = "300px";
                lstBox.style.width = pnl.clientWidth - 3 + 'px';
            }
        }
    }

    function CloseListBox(ctlId) {
        var ddlBox = document.getElementById(ctlId + "ddlBOX");
        var tabl = document.getElementById(ctlId + "Table2");
        var panel = document.getElementById(ctlId + "Panel2");
        ddlBox.style.visibility = "hidden";
        ddlBox.style.heght = "0px;";
        panel.style.height = tabl.style.height;
    }
</script>
<asp:Panel ID="Panel2" runat="server" BackColor="White" Height="22px"
    Width="200px" CssClass="TextBox">
    <table id="Table2" runat="server" cellpadding="0" cellspacing="0"
        style="table-layout: fixed;" width="100%">
        <tr id="rowdd" runat="server" style="height: 22px">
            <td class="ddltd">
                <asp:Label ID="DDLabel" runat="server" BorderColor="Transparent"
                    BorderStyle="None" Height="18px" Font-Size="14px" Font-Names="微軟正黑體"
                    Style="cursor: default;" Width="100%"></asp:Label>
            </td>
            <td id="colDDImage" runat="server" class="down_arrow ddltd"
                style="padding-right: 0px; padding-left: 0px; padding-bottom: 0px; padding-top: 0px; width: 22px;"></td>
        </tr>
    </table>
    <asp:Panel ID="ddlBOX" runat="server">
        <asp:CheckBoxList ID="DDList" runat="server"
            CellPadding="2" CellSpacing="2" Width="100%"
            BackColor="White" RepeatLayout="Flow"
            BorderStyle="Solid" BorderWidth="1px" CssClass="ListBox">
        </asp:CheckBoxList>

        <div id="btnClose" style="width: 100%">
            <asp:Button ID="btnCloseList" runat="server" Text="關閉選單"
                CausesValidation="False" UseSubmitBehavior="False" />
        </div>

    </asp:Panel>
</asp:Panel>

