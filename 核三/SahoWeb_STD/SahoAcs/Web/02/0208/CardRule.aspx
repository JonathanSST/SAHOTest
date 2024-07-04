<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CardRule.aspx.cs" Inherits="SahoAcs.CardRule" EnableEventValidation="false" Theme="UI" %>

<%@ Register Src="/uc/PickTimePop.ascx" TagPrefix="uc1" TagName="PickTime" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideEquModel" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="HiddenMsg" value="<%=GetGlobalResourceObject("Resource", "NotSelectForEdit").ToString().Replace("\\n","|") %>" />
        <input type="hidden" id="AlertDelete" value="<%=Resources.ResourceAlert.AlertDelete %>" />
    </div>
    <table class="TableWidth">
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th><span class="Arrow01"></span>
                            <asp:Label ID="Label_EquModel" runat="server" Text="<%$Resources:ResourceEquData,EquModel %>"></asp:Label>
                        </th>
                        <th><span class="Arrow01"></span>
                            <asp:Label ID="Label_No" runat="server" Text="<%$Resources:RuleNo %>"></asp:Label></th>
                        <th><span class="Arrow01"></span>
                            <asp:Label ID="Label_Name" runat="server" Text="<%$Resources:RuleName %>"></asp:Label></th>
                        <td>&nbsp;</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="Input_EquModel" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:TextBox ID="Input_No" runat="server"></asp:TextBox></td>
                        <td>
                            <asp:TextBox ID="Input_Name" runat="server"></asp:TextBox></td>

                        <td>
                            <asp:Button ID="QueryButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" CssClass="IconSearch" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellspacing="0" class="TableS1">
                            <%--GridView Header的Html Code--%>
                            <asp:Literal ID="li_header" runat="server" />
                            <tr>
                                <td id="td_showGridView" runat="server" style="padding: 0">
                                    <asp:Panel ID="tablePanel" runat="server" ScrollBars="Vertical" CssClass="MinHeight">
                                        <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                            DataKeyNames="RuleNo" AllowPaging="True" AutoGenerateColumns="False"
                                            OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                            OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                            <Columns>
                                                <asp:BoundField DataField="RuleNo" HeaderText="<%$Resources:RuleNo %>" SortExpression="RuleNo" />
                                                <asp:BoundField DataField="RuleName" HeaderText="<%$Resources:RuleName %>" SortExpression="RuleName" />
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
                            <asp:Button ID="DeleteButton" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" CssClass="IconDelete" />
                        </td>
                        <td>
                            <input type="button" id="BtnDef" value="<%=GetLocalResourceObject("btnDef").ToString() %>" class="IconChange" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>


    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="930px" EnableViewState="False" CssClass="PopBg">
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
        <table>
            <tr>
                <td>
                    <table class="popItem">
                        <tr>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:RuleNo %>" Font-Bold="true"></asp:Label></th>
                            <th><span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:RuleName %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_No" runat="server" Width="200px" BorderWidth="1" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_Name" runat="server" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table border="0" class="Table01">
                        <tr>
                            <th></th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Mo" runat="server" Text="<%$Resources:ttMon %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Tu" runat="server" Text="<%$Resources:ttTue %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_We" runat="server" Text="<%$Resources:ttWed %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Th" runat="server" Text="<%$Resources:ttThu %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Fr" runat="server" Text="<%$Resources:ttFri %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Sa" runat="server" Text="<%$Resources:ttSat %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Su" runat="server" Text="<%$Resources:ttSun %>"></asp:Label>
                            </th>
                            <th colspan="2">
                                <asp:Label ID="popLabel_Ho" runat="server" Text="<%$Resources:ttHol %>"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>1.</td>
                            <td>
                                <asp:CheckBox ID="Mo_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Tu_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="We_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Th_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Fr_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Sa_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Su_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime1" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Ho_CheckBox1" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime1" />
                            </td>
                        </tr>
                        <tr>
                            <td>2.</td>
                            <td>
                                <asp:CheckBox ID="Mo_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Tu_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="We_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Th_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Fr_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Sa_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Su_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime2" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Ho_CheckBox2" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime2" />
                            </td>
                        </tr>
                        <tr>
                            <td>3.</td>
                            <td>
                                <asp:CheckBox ID="Mo_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Tu_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="We_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Th_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Fr_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Sa_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Su_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime3" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Ho_CheckBox3" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime3" />
                            </td>
                        </tr>
                        <tr>
                            <td>4.</td>
                            <td>
                                <asp:CheckBox ID="Mo_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Tu_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="We_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Th_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Fr_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Sa_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Su_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime4" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Ho_CheckBox4" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime4" />
                            </td>
                        </tr>
                        <tr>
                            <td>5.</td>
                            <td>
                                <asp:CheckBox ID="Mo_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Mo_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Tu_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Tu_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="We_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="We_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Th_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Th_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Fr_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Fr_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Sa_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Sa_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Su_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Su_PickTime5" />
                            </td>
                            <td>
                                <asp:CheckBox ID="Ho_CheckBox5" runat="server" Checked="true" />
                            </td>
                            <td>
                                <uc1:PickTime runat="server" ID="Ho_PickTime5" />
                            </td>
                        </tr>
                       <tr>
                            <td></td>
                            <%for (int i = 0; i < 8; i++)
                                { %>
                            <td colspan="2"><input type="button" value="<%=GetLocalResourceObject("btnCopyRule").ToString() %>" class="IconExport" onclick="DoCopy(this)" /></td>
                            <%} %>                            
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align:center">
                    <asp:Label ID="DeleteLableText" runat="server" ForeColor="#FFFFFF" Font-Bold="true" Font-Size="16px"></asp:Label>
                    <div style="color:#fff;font-size:12pt">「V」勾選之後，此時段起允許刷卡，時間空白則不進行比對</div>
                </td>
            </tr>
            <tr>
                <td>
                    <table style="margin: 10px 10px 10px 10px;">
                        <tr>
                            <td>
                                <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                                    <asp:Button ID="popB_Add" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                    <asp:Button ID="popB_Edit" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                    <asp:Button ID="popB_Delete" runat="server"  Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />                                    
                                    <asp:Button ID="popB_Cancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>

</asp:Content>
