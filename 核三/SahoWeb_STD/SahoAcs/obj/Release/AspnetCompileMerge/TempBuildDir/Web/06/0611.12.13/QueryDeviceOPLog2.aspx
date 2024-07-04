<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="True"
    Inherits="SahoAcs.QueryDeviceOPLog2" Debug="true" Theme="UI" EnableEventValidation="false" CodeBehind="QueryDeviceOPLog2.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideDeviceOPType" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_LogTimeBegin" runat="server" Text="<%$Resources:Resource,lblStart %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_LogTimeEnd" runat="server" Text="<%$Resources:Resource,lblEnd %>"></asp:Label>
                        </th>
                        <%--<th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_DOPActive" runat="server" Text="作業行為"></asp:Label>
                        </th>--%>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_DOPState" runat="server" Text="<%$Resources:ttRecordType %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_EquNo" runat="server" Text="<%$Resources:ttEquNo %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label_UserID" runat="server" Text="<%$Resources:ttUserID %>"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <uc1:Calendar runat="server" ID="Input_LogTimeBegin" />
                        </td>
                        <td>
                            <uc1:Calendar runat="server" ID="Input_LogTimeEnd" />
                        </td>
                        <%--<td>
                            <asp:DropDownList ID="Input_DOPActive" runat="server"></asp:DropDownList>
                        </td>--%>
                        <td>
                            <asp:DropDownList ID="Input_DOPState" runat="server"></asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_EquNo" runat="server" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_UserID" runat="server" Width="100px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table class="TableS1">
                <%--GridView Header的Html Code--%>
                <asp:Literal ID="li_header" runat="server" />
                <tr>
                    <td id="td_showGridView" runat="server" style="padding: 0">
                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight" Width="1500px">
                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                DataKeyNames="LogTime" AllowPaging="True" AutoGenerateColumns="False"
                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                <Columns>
                                    <asp:BoundField DataField="RecordID" HeaderText="RecordID" SortExpression="RecordID" Visible="false" />
                                    <asp:BoundField DataField="LogTime" HeaderText="<%$Resources:ttRecordTime %>" SortExpression="LogTime" />
                                    <%--<asp:BoundField DataField="DOPActiveName" HeaderText="作業行為" SortExpression="DOPActiveName" />--%>
                                    <asp:BoundField DataField="DOPState" HeaderText="<%$Resources:ttRecordType %>" SortExpression="DOPState" />
                                    <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo" />
                                    <asp:BoundField DataField="UserID" HeaderText="<%$Resources:ttUserID %>" SortExpression="UserID" />
                                    <asp:BoundField DataField="UserIP" HeaderText="<%$Resources:ttIP %>" SortExpression="UserIP" />
                                    <asp:BoundField DataField="ResultMsg" HeaderText="<%$Resources:ttResult %>" SortExpression="ResultMsg" />
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
        <asp:Button ID="DetailButton" runat="server" Text="<%$Resources:Resource,btnView %>" OnClick="ExcelButton_Click" CssClass="IconLook" />
        <asp:Button ID="ExcelButton" runat="server" Text="<%$Resources:Resource,btnExport %>" OnClick="ExcelButton_Click" CssClass="IconExport" />
    </div>

    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="450px" EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table class="TableWidth">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Text="<%$Resources:lblViewSysLog %>" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_LogTime" runat="server" Text="<%$Resources:ttRecordTime %>" Font-Bold="true"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_DOPActiveName" runat="server" Text="<%$Resources:ttAction %>" Font-Bold="true"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_DOPState" runat="server" Text="<%$Resources:ttRecordType %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <asp:Label ID="popInput_LogTime" runat="server"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <asp:Label ID="popInput_DOPActiveName" runat="server"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <asp:Label ID="popInput_DOPState" runat="server"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquNo" runat="server" Font-Bold="true" Text="<%$Resources:ttEquNo %>"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_UserID" runat="server" Font-Bold="true" Text="<%$Resources:ttUserID %>"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_UserIP" runat="server" Font-Bold="true" Text="<%$Resources:ttIP %>"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <th>
                                <asp:Label ID="popInput_EquNo" runat="server"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <asp:Label ID="popInput_UserID" runat="server"></asp:Label>
                            </th>
                            <td style="width: 6px"></td>
                            <th>
                                <asp:Label ID="popInput_UserIP" runat="server"></asp:Label>
                            </th>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_ResultMsg" runat="server" Text="<%$Resources:ttResult %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_ResultMsg" runat="server" TextMode="MultiLine" Rows="5" Width="420px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <asp:Button ID="popB_Cancel" runat="server" Text="<%$Resources:Resource,btnExit %>" EnableViewState="False" CssClass="IconChoose" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
</asp:Content>
