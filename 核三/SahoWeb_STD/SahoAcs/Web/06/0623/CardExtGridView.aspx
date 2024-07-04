<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardExtGridView.aspx.cs" Inherits="SahoAcs.Web._06._0623.CardExtGridView" Theme="UI" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />

    <table class="TableS1">
        <asp:Literal ID="li_header" runat="server" />
        <tr>
            <td id="td_showGridView" runat="server">
                <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" Width="100%" Height="100px">
                    <asp:GridView ID="GridView1" runat="server" EnableViewState="true" SkinID="GridViewSkin" 
                        Width="100%" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound" OnDataBound="GridView1_DataBound" AllowPaging="True" PageSize="5" DataKeyNames="CardNo">
                        <Columns>
                            <asp:BoundField DataField="CardNo" HeaderText="卡片號碼" />
                            <asp:BoundField DataField="PsnName" HeaderText="人員姓名" />
                            <asp:BoundField DataField="LastTime" HeaderText="最後讀卡時間" />
                            <asp:BoundField DataField="LastDoorNo" HeaderText="最後讀卡位置" />
                            <asp:BoundField DataField="CtrlAreaNo" HeaderText="管制區編號" />
                        </Columns>
                        <PagerTemplate>
                            <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false" Text="FIRST"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false" Text="Prev"></asp:LinkButton>
                            <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                            <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false" Text="Next"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false" Text="<Last"></asp:LinkButton>
                        </PagerTemplate>                
                    </asp:GridView>
                </asp:Panel>
            </td>
        </tr>
        <asp:Literal ID="li_Pager" runat="server" />
    </table>

    
    </form>
</body>
</html>


