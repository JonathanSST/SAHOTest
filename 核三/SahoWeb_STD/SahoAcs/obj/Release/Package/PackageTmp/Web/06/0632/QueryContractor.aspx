<%@ Page Language="C#" Inherits="SahoAcs.QueryContractor" MasterPageFile="~/Site1.Master" Debug="true" 
    AutoEventWireup="true" EnableEventValidation="false" Theme="UI" CodeBehind="QueryContractor.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="~/uc/MultiSelectDropDown.ascx" TagPrefix="uc2" TagName="MultiSelectDropDown" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_Time" runat="server" Text="<%$Resources:ttTime %>"></asp:Label></th>
                        
                        <th></th>
                        
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblCompany" runat="server" Text="公司"></asp:Label></th>
                        <th></th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="labStatus" runat="server" Text="<%$Resources:ttStatus %>"></asp:Label></th>
                        <th></th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="labDetail" runat="server" Text="<%$Resources:ttDetail %>"></asp:Label></th>
                        <th></th>
                        <th runat="server" id="ShowPsnInfo1">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardNo_PsnName" runat="server" Text="<%$Resources:lblPsnNo %>"></asp:Label></th>
                        <th></th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_LogStatus" runat="server" Text="<%$Resources:ttResult %>"></asp:Label></th>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" /></td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" /></td>
                        
                        <td></td>
                        
                        <td>
                            <asp:DropDownList ID="dropCompany" runat="server" Width="120px"></asp:DropDownList></td>
                        
                        <td></td>
                        
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server">
                                <asp:ListItem Value="">- 請選擇 -</asp:ListItem>
                                <asp:ListItem>正常</asp:ListItem>
                                <asp:ListItem>異常</asp:ListItem>
                            </asp:DropDownList></td>
                        <td></td>
                        <td>
                            <asp:DropDownList ID="ddlDetail" runat="server">
                                <asp:ListItem Value="">- 請選擇 -</asp:ListItem>
                                <asp:ListItem>正常報到</asp:ListItem>
                                <asp:ListItem>延遲報到</asp:ListItem>
                                <asp:ListItem>入廠有刷卡，沒有報到刷卡</asp:ListItem>
                                <asp:ListItem>入廠沒刷卡，有報到刷卡</asp:ListItem>
                                <asp:ListItem>入廠沒刷卡，延遲報到刷卡</asp:ListItem>
                                <asp:ListItem>入廠有刷卡，其時間晚於比對或報到時間</asp:ListItem>
                            </asp:DropDownList></td>
                        <td></td>
                        <td  runat="server" id="ShowPsnInfo2">
                            <asp:TextBox ID="TextBox_CardNo_PsnName" runat="server" Width="150px"></asp:TextBox></td>
                        <td></td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1230px">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" Width="100%" PageSize="5"
                                EnableViewState="False" DataKeyNames="NewIDNum" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="CardTime" HeaderText="<%$Resources:ttCardTime %>" SortExpression="CardTime" />
                                    <asp:BoundField DataField="CardTime2" HeaderText="<%$Resources:ttCardTime2 %>" SortExpression="CardTime2" />
                                    <asp:BoundField DataField="OrgName" HeaderText="<%$Resources:ttOrgName %>" SortExpression="OrgName" />
                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                    <asp:BoundField DataField="Status" HeaderText="<%$Resources:ttStatus %>" SortExpression="Status" />
                                    <asp:BoundField DataField="Detail" HeaderText="<%$Resources:ttDetail %>" SortExpression="Detail" />
                                </Columns>
                                <PagerTemplate>
                                    <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnFirst %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnPrev %>"></asp:LinkButton>
                                    <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                                    <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnNext %>"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnLast %>"></asp:LinkButton>
                                </PagerTemplate>
                            </asp:GridView>
                        </asp:Panel>
                    </td>
                </tr>
                <%--GridView Pager的Html Code--%>
                <asp:Literal ID="li_Pager" runat="server" />
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <div>
    <table class="Item">
        <tr>
            <td>
                <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" CssClass="IconExport" /></td>
            <td></td>
            <td>
                <table>
                    <tr>
                        <th class="auto-style1">
                            <span class="Arrow01"></span></th>
                        <th class="auto-style1">
                            <asp:Label ID="labRegTime" runat="server" Text="報到時間"></asp:Label></th>
                        <td class="auto-style1">
                            <asp:DropDownList ID="ddlHour" runat="server">
                            </asp:DropDownList></td>
                        <th class="auto-style1">
                            <asp:Label ID="labRegHour" runat="server" Text="時"></asp:Label></th>
                        <td class="auto-style1">
                            <asp:DropDownList ID="ddlMin" runat="server">
                            </asp:DropDownList></td>
                        <th class="auto-style1">
                            <asp:Label ID="labRegMin" runat="server" Text="分"></asp:Label></th>
                        <td class="auto-style1"></td>
                        <th class="auto-style1">
                            <asp:Label ID="labRegEquGroup" runat="server" Text="報到設備群組"></asp:Label></th>
                        <td class="auto-style1">
                            <asp:DropDownList ID="ddlRegEquGroup" runat="server">
                            </asp:DropDownList></td>
                        <td class="auto-style1">
                            <asp:Button ID="btnSetup" runat="server" Text="設定報到時間和設備群組" CssClass="IconSave" /></td>
                    </tr>
                </table></td>
        </tr>
    </table>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style1 {
            height: 28px;
        }
    </style>
</asp:Content>

