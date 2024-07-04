<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="0605_Formosa.aspx.cs" Inherits="SahoAcs._0605_Formosa" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <style type="text/css">
        #mark1
        {
        	border-style: solid; 
        	border-color: inherit; 
        	border-width: 4px; 
            border-color: mediumblue;
        	background-color: silver; 
        	position: absolute;
            top: 40%;
            left: 40%; 
            padding-left: 0px; 
            padding-top: 0px; 
            z-index: 999; 
            width: 200px; 
            height: 30px; 
            text-align: center; 
            vertical-align: middle;
        }
    </style>
    
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <%--<asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />--%>
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" 
                DisplayAfter="100">
                <ProgressTemplate>                     
                    <div id="mark1">
                    <table align="center" style="width: 200px">
                        <tr>
                            <td align="center" style="font-size: x-large; color: #0066FF;">
                                Now Loading...</td>
                        </tr>
                    </table>
                </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
            
            <table class="Item">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                </th>
                                <th>
                                    <asp:Label ID="Label1" runat="server" Text="<%$Resources:ttBuild %>"></asp:Label>
                                </th>
                                <th></th>

                                <th>
                                    <span class="Arrow01"></span>
                                </th>
                                <th>
                                    <asp:Label ID="Label_Condition" runat="server" Text="<%$Resources:Label_Condition %>"></asp:Label>
                                </th>
                                <td></td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:DropDownList ID="Input_Building" runat="server" Width="120px">
                                    </asp:DropDownList>
                                </td>
                                <td></td>

                                <td></td>
                                <td style="width: 500px">
                                    <asp:TextBox ID="Input_Condition" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                                </td>
                                <td></td>
                                <th>
                                    <asp:CheckBox ID="ChkOutDate" runat="server" Width="20" Height="20" /><%=GetLocalResourceObject("ttWithExpired") %>
                                </th>
                                <td>
                                    <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <table cellspacing="0" class="TableS1">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1225px" EnableViewState="False">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                DataKeyNames="EquNo" AllowPaging="true" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting" EnableViewState="False">
                                <Columns>
                                    <asp:BoundField DataField="Building" HeaderText="<%$Resources:ttBuild %>" SortExpression="Building" />
                                    <asp:BoundField DataField="Floor" HeaderText="<%$Resources:ttFloor %>" SortExpression="Floor" />
                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo" />
                                    <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ttEquName %>" SortExpression="EquName" />
                                    <asp:BoundField DataField="EquClass" HeaderText="<%$Resources:ttEquType %>" SortExpression="EquClass" />
                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$Resources:ttPsnNo %>" SortExpression="PsnNo" />
                                    <asp:BoundField DataField="PsnName" HeaderText="<%$Resources:ttPsnName %>" SortExpression="PsnName" />
                                    <asp:BoundField DataField="OrgNo" HeaderText="<%$Resources:ttUnitNo %>" SortExpression="OrgNo" />
                                    <asp:BoundField DataField="OrgName" HeaderText="<%$Resources:ttUnitName %>" SortExpression="OrgName" />
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
      <div>        
        <asp:Button ID="ExportButton" runat="server" Text="<%$Resources:Resource,btnExport %>" OnClick="ExportButton_Click" CssClass="IconExport" />
    </div>
</asp:Content>
