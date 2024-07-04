<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI" CodeBehind="QueryCardBadge.aspx.cs" Inherits="SahoAcs.Web._06._0620.CardBadge" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../../../Css/colorbox.css" rel="stylesheet" />
    <script src="../../../Scripts/jquery.colorbox-min.js"></script>
    <script src="QueryCardBadge.js"></script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="人員編號、姓名或車號"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td runat="server" id="ShowPsnInfo2">
                            <asp:TextBox ID="TextBox_CardNo_PsnName" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <%--<asp:Button ID="QueryButton" runat="server" Text="查　詢" CssClass="IconSearch" OnClick="QueryButton_Click" />--%>
                            <input type="button" id="QueryButton" value="查詢" class="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table border="0" class="Item">
                <tr>
                    <th colspan="2">
                        <span class="Arrow01"></span>識別證與車證資訊：
                    </th>
                </tr>
                <tr>
                    <td>
                        <fieldset id="Pending_List" runat="server">
                            <legend id="Pending_Legend" runat="server">識別證資訊</legend>
                            <table class="Item">
                                <tr>
                                    <td style="vertical-align: top; width: 250px;">
                                        <div>
                                            <asp:Label ID="lb_AccountNo" runat="server" Text="帳號"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_AccountNo" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_BadgeNo" runat="server" Text="證號"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_BadgeNo" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_Name" runat="server" Text="姓名"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_Name" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_EName" runat="server" Text="英文姓名"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_EName" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_Company" runat="server" Text="">分類</asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_Company" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td style="vertical-align: top; width: 250px;">
                                        <div>
                                            <asp:Label ID="lb_CardNo" runat="server" Text="卡號"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_CardNo" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_PsnSTime" runat="server" Text="報到日期"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_PsnSTime" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_PsnETime" runat="server" Text="有效期限"></asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_PsnETime" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_Title" runat="server" Text="">職稱</asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_Title" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                        <div>
                                            <asp:Label ID="lb_Unit" runat="server" Text="">單位</asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_Unit" runat="server" Text="" Enabled="false"></asp:TextBox>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div>
                                            <asp:Label ID="lb_Desc" runat="server" Text="">備註</asp:Label>
                                        </div>
                                        <div>
                                            <asp:TextBox ID="txt_Desc" runat="server" Text="" Enabled="false" Width="420px"></asp:TextBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                    <td>
                        <fieldset id="Fieldset1" runat="server">
                            <legend id="Legend1" runat="server">車證資訊</legend>
                            <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                id="ContentPlaceHolder1_GridViewFloor" style="border-collapse: collapse;">
                                <tbody>
                                    <tr>
                                        <th style="width: 110px">汽車
                                        </th>
                                        <th style="width: 110px">車號
                                        </th>
                                        <th style="width: 110px">車輛證號
                                        </th>
                                        <th style="width: 110px">廠區位置
                                        </th>
                                        <th style="width: 110px">備註                                                                                                                                                 
                                        </th>
                                    </tr>  
                                    <%foreach(var o in this.make_cards.Where(i=>i.CarType=="Car")){ %>                                  
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                        <td><%=o.CarA1 %></td>
                                        <td><%=o.CarA3 %></td>
                                        <td><%=o.zonetype %></td>
                                        <td></td>
                                    </tr>
                                    <%} %>
                                    <%for(int i=0;i<3-(this.make_cards.Where(car=>car.CarType=="Car").Count());i++){ %>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <%} %>
                                    <tr>
                                        <th>機車
                                        </th>
                                        <th>車號
                                        </th>
                                        <th>車輛證號
                                        </th>
                                        <th>廠區位置
                                        </th>
                                        <th>備註                                                                                                                                                 
                                        </th>
                                    </tr>
                                     <%foreach(var o in this.make_cards.Where(i=>i.CarType=="Motor")){ %>                                  
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                        <td><%=o.CarA1 %></td>
                                        <td><%=o.CarA3 %></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <%} %>
                                    <%for(int i=0;i<3-(this.make_cards.Where(car=>car.CarType=="Motor").Count());i++){ %>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <%} %>
                                </tbody>
                            </table>
                            <p style="height:56px">&nbsp;</p>
                        </fieldset>                        
                    </td>
                </tr>
                <tr>
                    <th colspan="2">
                        <span class="Arrow01"></span>門禁權限資訊：
                    </th>
                 </tr>
                <tr>
                    <td colspan="2">
                            <table class="TableS1" style="width:880px">
                                <tbody>
                                    <tr class="GVStyle">                                        
                                        <th scope="col" style="width: 140px">讀卡機編號</th>
                                        <th scope="col" style="width:160px">讀卡機名稱</th>
                                        <th scope="col" style="width:70px">時區</th>
                                        <th scope="col" style="width:120px">設消碼狀態</th>
                                        <th scope="col">說明</th> 
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 225px; width: 880px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" 
                                                        id="ContentPlaceHolder1_GridViewAuth" style="border-collapse: collapse;">
                                                        <tbody>                                                            
                                                            <%foreach(var s in this.auth_lists)
                                                              { %>
                                                                <tr>                                                                    
                                                                    <td style="width:144px">
                                                                        <%=s.EquNo %>
                                                                    </td>
                                                                    <td style="width:164px">
                                                                        <%=s.EquName %>
                                                                    </td>
                                                                    <td style="width:74px">
                                                                        <%=s.TimeName %>
                                                                    </td>
                                                                    <td style="width:124px">
                                                                        <%=s.OpStatus=="Setted"?"設碼OK":"" %>
                                                                    </td>
                                                                    <td>
                                                                    </td>
                                                                </tr>
                                                            <%} %>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
