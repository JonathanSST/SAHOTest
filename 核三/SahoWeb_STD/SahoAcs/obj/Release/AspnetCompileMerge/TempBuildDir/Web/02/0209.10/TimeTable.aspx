<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Debug="true" CodeBehind="TimeTable.aspx.cs" Inherits="SahoAcs.TimeTable" EnableEventValidation="false" Theme="UI" %>

<%@ Register Src="/uc/PickTimePop.ascx" TagPrefix="uc1" TagName="PickTime" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideTimeType" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="HiddenMsg" value="<%=GetGlobalResourceObject("Resource", "NotSelectForEdit").ToString().Replace("\\n","|") %>" />
        <input type="hidden" id="TimeType" value="<%=Request.QueryString["Mode"] %>" />
        <input type="hidden" id="AlertDelete" value="<%=Resources.ResourceAlert.AlertDelete %>" />        
    </div>
    <table class="TableWidth">
        <tr>
            <td>
                <table class="Item">                    
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_EquModel" runat="server" Text="<%$Resources:ResourceEquData,EquModel %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_No" runat="server" Text="<%$Resources:RuleNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_Name" runat="server" Text="<%$Resources:RuleName %>"></asp:Label>
                        </th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="Input_EquModel" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_Name" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table class="TableS1">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                            DataKeyNames="TimeNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="TimeNo" HeaderText="<%$Resources:RuleNo %>" SortExpression="TimeNo" />
                                                <asp:BoundField DataField="TimeName" HeaderText="<%$Resources:RuleName %>" SortExpression="TimeName" />
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
            </td>
        </tr>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />
                        </td>
                        <td>
                            <asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />
                        </td>
                        <td>
                            <input type="button" id="BtnExport" value="<%=GetLocalResourceObject("btnExport").ToString() %>" class="IconCopyPermissions" />
                        </td>
                        <td>
                            <asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="1200px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName" runat="server" Text="讀卡規則資料" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="Item">
            <tr>
                <td>
                    <table class="popItem">
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:RuleNo %>" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:RuleName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No" runat="server" Width="150px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_Name" runat="server" Width="150px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="1">
                    <table id="UITable" runat="server" border="0" class="Table01">
                        <tr>
                            <th></th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Mo" runat="server" Text="<%$Resources:ttMon %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Tu" runat="server" Text="<%$Resources:ttTue %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_We" runat="server" Text="<%$Resources:ttWed %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Th" runat="server" Text="<%$Resources:ttThu %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Fr" runat="server" Text="<%$Resources:ttFri %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Sa" runat="server" Text="<%$Resources:ttSat %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Su" runat="server" Text="<%$Resources:ttSun %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Ho" runat="server" Text="<%$Resources:ttHol %>" Font-Bold="true" ForeColor="#FBFBFB"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>1.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime1" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList1" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime1" />
                            </td>
                        </tr>
                        <tr>
                            <td>2.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime2" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList2" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime2" />
                            </td>
                        </tr>
                        <tr>
                            <td>3.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime3" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList3" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime3" />
                            </td>
                        </tr>
                        <tr>
                            <td>4.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime4" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList4" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime4" />
                            </td>
                        </tr>
                        <tr>
                            <td>5.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime5" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList5" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime5" />
                            </td>
                        </tr>
                        <tr>
                            <td>6.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime6" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList6" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime6" />
                            </td>
                        </tr>
                        <tr>
                            <td>7.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime7" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList7" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime7" />
                            </td>
                        </tr>
                        <tr>
                            <td>8.</td>
                            <td>
                                <asp:DropDownList ID="Mo_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Tu_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="We_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Th_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Fr_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Sa_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Su_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime8" />
                            </td>
                            <td>
                                <asp:DropDownList ID="Ho_DropDownList8" runat="server" Width="48px">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime8" />
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                             <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                            <td colspan="2"><input type="button" value="複製規則" class="IconExport" onclick="DoCopy(this)" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th style="text-align: center">
                    <asp:Label ID="RemindLableText" runat="server" Text="<%$Resources:lblAlert %>"></asp:Label>
                </th>
            </tr>
            <tr>
                <th style="text-align: center">
                    <asp:Label ID="DeleteLableText" runat="server"></asp:Label>
                </th>
            </tr>
        </table>
        <table class="TableWidth">
            <tr>
                <td style="text-align: center">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                        <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
