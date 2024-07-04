<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MsgPage.aspx.cs" Inherits="SahoAcs.MsgPage" EnableEventValidation="false" Theme="UI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"></asp:ScriptManager>
        <table style="width: 100%">
            <tr>
                <td style="text-align: right">
                    <asp:UpdatePanel ID="NewMsg_UpdatePanel" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:Timer ID="NewMsgTimer" runat="server" Interval="1000" OnTick="NewMsgTimer_Tick"></asp:Timer>
                            <asp:LinkButton ID="LabNewMsg" runat="server" Text="你有新的訊息" Visible="false" Font-Underline="false" ForeColor="#ff3300" OnClientClick="MsgWin();"></asp:LinkButton>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </form>
    <script type="text/javascript">
        function MsgWin() {
            var sURL = "Web/MessagePage/MessageList.aspx";
            var vArguments = "";
            var sFeatures = "dialogHeight:440px;dialogWidth:1035px;center:yes";
            window.showModalDialog(sURL, vArguments, sFeatures);
        }
    </script>
</body>
</html>
