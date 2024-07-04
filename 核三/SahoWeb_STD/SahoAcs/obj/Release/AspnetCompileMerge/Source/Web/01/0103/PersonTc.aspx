<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonTc.aspx.cs" Inherits="SahoAcs.PersonTc" Debug="true" EnableEventValidation="false" Theme="UI" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>
<%--<%@ Register Src="/uc/PickDate.ascx" TagPrefix="uc2" TagName="PickDate" %>--%>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <link href="../../../Css/colorbox.css" rel="stylesheet" />
    <script src="../../../Scripts/jquery.colorbox-min.js"></script>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblCompany" runat="server" Text="<%$ Resources:lblCompany %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblDepartment" runat="server" Text="<%$ Resources:lblDepartment %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblPsnType" runat="server" Text="<%$ Resources:lblPsnClass %>"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblKeyWord" runat="server" Text="<%$ Resources:lblKeyWord %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
             <td>
                <asp:DropDownList ID="dropCompany" runat="server" Width="150px"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="dropDepartment" runat="server" Width="200px"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="dropPsnType" runat="server" Width="100px"></asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="InputWord" runat="server"></asp:TextBox>
            </td>            
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />
                <asp:Button ID="btnPsnAdd" runat="server" Text="<%$ Resources:btnPsnAdd %>" CssClass="IconNew" />
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse; border: solid 0px;">
        <tr>
            <td style="vertical-align:top">
                <fieldset id="Pending_Data" runat="server"  style="height:610px">
                    <legend id="Legend1" runat="server"><%=GetLocalResourceObject("ttPerson") %></legend>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:HiddenField ID="CurrentPerson" runat="server" EnableViewState="False" />
                            <table class="TableS1" style="width: 450px;">
                                <%--GridView Header的Html Code--%>
                                <asp:Literal ID="li_header" runat="server" />
                                <tr>
                                    <td id="td_showGridView" runat="server">
                                        <asp:Panel ID="tablePanel" runat="server" ScrollBars="Auto" Height="470px" Width="450px">
                                            <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                            <asp:GridView runat="server" ID="MainGridView" SkinID="GridViewSkin" PageSize="5"
                                                DataKeyNames="PsnID" AllowPaging="True" AutoGenerateColumns="False"
                                                OnRowDataBound="GridView_Data_RowDataBound" OnDataBound="GridView_Data_DataBound"
                                                OnPageIndexChanging="GridView_PageIndexChanging" AllowSorting="True" OnSorting="GridView_Sorting">
                                                <Columns>
                                                    <asp:BoundField DataField="PsnID" HeaderText="<%$ Resources:PsnID %>" SortExpression="PsnID" Visible="false" />
                                                    <asp:BoundField DataField="PsnNo" HeaderText="<%$ Resources:PsnNo %>" SortExpression="PsnNo" />
                                                    <asp:BoundField DataField="PsnName" HeaderText="<%$ Resources:PsnName %>" SortExpression="PsnName" />
                                                </Columns>
                                                <PagerTemplate>
                                                    <asp:LinkButton ID="lbtnFirst" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnFirst %>"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbtnPrev" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnPrev %>"></asp:LinkButton>
                                                    <asp:PlaceHolder ID="phdPageNumber" runat="server"></asp:PlaceHolder>
                                                    <asp:LinkButton ID="lbtnNext" runat="server" Font-Overline="false" Text="<%$ Resources:Resource, lbtnNext %>"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbtnLast" runat="server" CommandName="Page" Font-Overline="false" Text="<%$ Resources:Resource, lbtnLast %>"></asp:LinkButton>
                                                    <asp:PlaceHolder ID="phdPageInfo" runat="server"></asp:PlaceHolder>
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
                </fieldset>
            </td>
            <td style="vertical-align:top">
                <fieldset id="Pending_List" runat="server"  style="height:610px">
                    <legend id="Pending_Legend" runat="server"><%=GetLocalResourceObject("ttPersonEdit") %></legend>
                    <table class="Item">
                        <tr>
                            <td style="vertical-align: top; width: 250px;">
                                <div>
                                    <asp:Label ID="lb_PsnNo" runat="server" Text="業戶代碼"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnNo" runat="server" Width="180px" MUST_KEYIN_YN="Y" FIELD_NAME="業戶代碼"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lb_PsnName" runat="server" Text="業戶名稱"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnName" runat="server" Width="180px" MUST_KEYIN_YN="Y" FIELD_NAME="業戶名稱"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lb_PsnEName" runat="server" Text="業戶識別碼"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnEName" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnClass" runat="server" Text="<%$ Resources:lblPsnClass %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:DropDownList ID="Input_PsnType" runat="server" Width="180px">
                                    </asp:DropDownList>
                                </div>
                                <div>
                                    <asp:Label ID="lblIDNum" runat="server" Text="<%$ Resources:lblIDNum %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnIDNum" runat="server" Width="180px" MaxLength="10"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblBirthday" runat="server" Text="<%$ Resources:lblBirthday %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:DropDownList ID="Input_PsnBirthdayYear" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="Input_PsnBirthdayMonth" runat="server"></asp:DropDownList>
                                    <asp:DropDownList ID="Input_PsnBirthdayDate" runat="server"></asp:DropDownList>
                                </div>
                                <div>
                                    <asp:Table ID="tblOrgData" runat="server"></asp:Table>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnAccount" runat="server" Text="<%$ Resources:lblPsnAccount %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnAccount" runat="server" Width="180px" AutoCompleteType="Disabled" 
									onfocus="this.removeAttribute('readonly');" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnPW" runat="server" Text="<%$ Resources:lblPsnPW %>"></asp:Label>
                                </div>
                                <div>
                                   <asp:TextBox ID="Input_PsnPW" runat="server" Width="180px" TextMode="Password"  AutoCompleteType="Disabled" 
									onfocus="this.removeAttribute('readonly');" ReadOnly="true"></asp:TextBox>
                                </div>
                            </td>
                            <td style="vertical-align: top; width: 250px;">
                                <div>
                                    <asp:Label ID="lblCardNo" runat="server" Text="<%$ Resources:lblCardNo %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_CardNo" runat="server" Width="180px" MUST_KEYIN_YN="Y" FIELD_NAME="卡號"></asp:TextBox>
                                </div>                                
                                <div>
                                    <asp:Label ID="lblPsnAuthAllow" runat="server" Text="<%$ Resources:lblPsnAuthAllow %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:DropDownList ID="Input_PsnAuthAllow" runat="server" Width="80px">
                                        <asp:ListItem Value="0" Text="<%$Resources:SelectDisabled %>"></asp:ListItem>
                                        <asp:ListItem Value="1" Selected="True" Text="<%$Resources:SelectEnabled %>"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnSTime" runat="server" Text="<%$ Resources:lblPsnSTime %>"></asp:Label>
                                </div>
                                <div>
                                    <uc1:Calendar ID="Input_PsnSTime" runat="server" />
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnETime" runat="server" Text="<%$ Resources:lblPsnETime %>"></asp:Label>
                                </div>
                                <div>
                                    <uc1:Calendar ID="Input_PsnETime" runat="server" />
                                </div>
                               <%foreach (DataRow r in this.DataInputText.Rows)
                                   { %>
                                <div>
                                   <span id="<%="lbl"+r["ParaNo"] %>"><%=r["ParaName"].ToString() %>：</span>
                                </div>
                                <div>
                                    <input type="text" name="<%=r["ParaNo"].ToString() %>" id="<%=r["ParaNo"].ToString() %>" />
                                </div>
                                <%} %>
                                <%-- <div>
                                    <asp:Label ID="lblText1" runat="server" Text="<%$ Resources:lblText1 %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Text1" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblText2" runat="server" Text="<%$ Resources:lblText2 %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Text2" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblText3" runat="server" Text="<%$ Resources:lblText3 %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Text3" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblText4" runat="server" Text="<%$ Resources:lblText4 %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Text4" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblText5" runat="server" Text="<%$ Resources:lblText5 %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Text5" runat="server" Width="180px"></asp:TextBox>
                                </div>--%>
                                <div>
                                    <asp:Label ID="lblRemark" runat="server" Text="<%$ Resources:lblRemark %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Remark" runat="server" Height="52px" Width="190px" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </td>
                            <td style="vertical-align: top; width: 250px;">
                                <div>
                                    <asp:Label ID="lbl_CardVer" runat="server" Text="<%$ Resources:popLabel_CardVer %>" Font-Bold="True"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_CardVer" runat="server" Width="90px" MaxLength="1"></asp:TextBox>
                                </div>
                                <div runat="server" id="DivVerifiTitle">
                                    <asp:Label ID="lblVerifiMode" runat="server" Text="<%$ Resources:AuthMode %>"></asp:Label>
                                </div>
                                <div runat="server" id="DivVerifiContent">
                                    <asp:DropDownList ID="ddlVerifiMode" runat="server" Style="width: 180px"></asp:DropDownList>
                                     <p style="height: 3px">&nbsp;</p>
                                    <asp:Button ID="BtnAuthMode" runat="server" Text="<%$ Resources:BtnAuthMode %>" CssClass="IconTransit" />
                                </div>
                                <p style="height:16px">&nbsp;</p>                                
                                <div>
                                    <asp:Label ID="lblCardInfo" runat="server" Text="<%$ Resources:lblCardInfo %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:ListBox ID="List_CardInfo" runat="server" Height="50px" Width="180px" SkinID="ListBoxSkin"></asp:ListBox>
                                </div>                               
                                <div>
                                    <p style="height: 3px">&nbsp;</p>
                                    <asp:Button ID="btCardInfo" runat="server" Text="<%$ Resources:btCardInfo %>" Enabled="False" CssClass="IconLook" />
                                    <p style="height: 3px">&nbsp;</p>
                                    <asp:Button ID="btCardAdd" runat="server" Text="<%$ Resources:Resource, btnAdd %>" Enabled="False" CssClass="IconNew" />                                    
                                     <p style="height: 3px">&nbsp;</p>
                                    <input type="submit" class="IconSet" value="<%=GetLocalResourceObject("btnCardAuth") %>" onclick="SetCardGroupEdit(); return false;" id="BtCardGroup" />
                                    <p style="height: 3px">&nbsp;</p>
                                    <input type="submit" class="IconPassword" value="<%=GetLocalResourceObject("btnSetCode") %>" onclick="SetCardCode(); return false;" id="BtSetting" />
                                    <p style="height: 3px">&nbsp;</p>
                                    <input type="button" class="IconChange" value="<%=GetLocalResourceObject("btnCardChange") %>" id="btnChange" name="btnChange" onclick="ChangeCard2(1)" />
                                    <p style="height: 3px">&nbsp;</p>
                                    <input type="button" class="IconSet" value="<%=GetLocalResourceObject("btnPic") %>" id="btnPic" name="btnPic" onclick="UploadPicSource()" />
                                </div>                            
                            </td>
                            <td style="vertical-align: top; width: 250px;display:none">
                                <div>
                                    <asp:Image ID="PsnPic" runat="server" Height="189px" Width="148px" BorderStyle="Groove" ImageUrl="~/Img/default.png" />
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnPicSource" runat="server" Text="<%$ Resources:lblPsnPicSource %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnPicSource" runat="server" Width="170px"></asp:TextBox>
                                </div>                                
                            </td>
                        </tr>
                    </table>
                    <table class="Item">
                        <tr>
                            <td colspan="4" style="text-align: center; width: 1250px;">
                                <hr />
                                <asp:Button ID="btSave" runat="server" Text="<%$ Resources:Resource, btnSave %>" Enabled="False" CssClass="IconSave" />
                                <asp:Button ID="btDelete" runat="server" Text="<%$ Resources:Resource, btnDelete %>" Enabled="False" CssClass="IconDelete" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="ShowCardAuthAdj" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectNowNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hPsnId" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectCardValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hItemInfo2" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hRemoveData" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hFinalFloorData" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectCompanyNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectDepartmentNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectPsnTypeNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="DefaultOrg" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hCardPwdAlertMsg" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="MaxPerson" runat="server" EnableViewState="False" />
    <input type="hidden" id="AlertMsg" value="<%=GetLocalResourceObject("AlertMsg") %>" />
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="1050px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%; padding: 0px;">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White"
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
                            <td>
                                <table style="padding: 0px; border-collapse: collapse;">
                                    <tr>
                                        <th colspan="2">
                                            <asp:Label ID="popLabel_CardNo" runat="server" Text="<%$ Resources:popLabel_CardNo %>" Font-Bold="True"></asp:Label>
                                        </th>
                                        <th colspan="2">
                                            <asp:Label ID="popLabel_CardVer" runat="server" Text="<%$ Resources:popLabel_CardVer %>" Font-Bold="True"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:TextBox ID="popInput_CardNo" runat="server" Width="180px"></asp:TextBox>
                                        </td>
                                        <td colspan="2">
                                            <asp:TextBox ID="popInput_CardVer" runat="server" Width="70px" MaxLength="1"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardPW" runat="server" Text="<%$ Resources:popLabel_CardPW %>" Font-Bold="True"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:TextBox ID="popInput_CardPW" runat="server" Width="180px" MaxLength="4"></asp:TextBox>
                                        </td>
                                        <td colspan="2"></td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardSerialNo" runat="server" Text="<%$ Resources:popLabel_CardSerialNo %>" Font-Bold="true"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:TextBox ID="popInput_CardSerialNo" runat="server" Width="180px"></asp:TextBox>
                                        </td>
                                        <td colspan="2"></td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardNum" runat="server" Text="<%$ Resources:popLabel_CardNum %>" Font-Bold="true"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:TextBox ID="popInput_CardNum" runat="server" Width="180px"></asp:TextBox>
                                        </td>
                                        <td colspan="2"></td>
                                    </tr>
                                    <tr>
                                        <th colspan="2">
                                            <asp:Label ID="popLabel_CardType" runat="server" Text="<%$ Resources:popLabel_CardType %>" Font-Bold="true"></asp:Label>
                                        </th>
                                        <th colspan="2">
                                            <asp:Label ID="popLabel_CardAuthAllow0" runat="server" Font-Bold="true" Text="<%$ Resources:popLabel_CardAuthAllow0 %>"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:DropDownList ID="popInput_CardType" runat="server" Width="190px">
                                            </asp:DropDownList>
                                        </td>
                                        <td colspan="2">

                                            <asp:DropDownList ID="popInput_CardAuthAllow" runat="server" Width="85px">
                                                <asp:ListItem Value="0" Text="<%$Resources:SelectDisabled %>"></asp:ListItem>
                                                <asp:ListItem Value="1" Text="<%$Resources:SelectEnabled %>" Selected="True"></asp:ListItem>
                                            </asp:DropDownList>

                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardSTime" runat="server" Text="<%$ Resources:popLabel_CardSTime %>" Font-Bold="true"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <uc1:Calendar ID="popInput_CardSTime" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardETime" runat="server" Text="<%$ Resources:popLabel_CardETime %>" Font-Bold="true"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <uc1:Calendar ID="popInput_CardETime" runat="server" />
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <th colspan="4">
                                            <asp:Label ID="popLabel_CardDesc" runat="server" Text="<%$ Resources:popLabel_CardDesc %>" Font-Bold="true"></asp:Label>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            <asp:TextBox ID="popInput_CardDesc" runat="server" Height="45px" TextMode="MultiLine" Width="290px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" style="align-content: center">
                                            <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                                    <asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                    <asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                    <asp:Button ID="popB_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                    <asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td id="td_showEquList" runat="server" style="padding: 0">
                    <table>
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <table class="TableS1">
                                            <%--GridView Header的Html Code--%>
                                            <asp:Literal ID="li_header1" runat="server" />
                                            <tr>
                                                <td id="td_showGridView1" runat="server" style="padding: 0">
                                                    <asp:Panel ID="tablePanel1" runat="server" ScrollBars="Vertical" Width="680px" Height="460px">
                                                        <%--<asp:HiddenField ID="SelectValue" runat="server" Value="" />--%>
                                                        <%-- 設備-已加入 --%>
                                                        <asp:GridView runat="server" ID="GridView1" SkinID="GridViewSkin" PageSize="5"
                                                            DataKeyNames="EquID" AutoGenerateColumns="False"
                                                            OnRowDataBound="GridView1_Data_RowDataBound" OnDataBound="GridView1_Data_DataBound">
                                                            <Columns>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox ID="CheckBox1" runat="server" onClick="spanChk1=this;SelectAllCheckboxes(this);" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="RowCheckState1" runat="server" onClick="spanChk1=ContentPlaceHolder1_GridView1_CheckBox1;GV1State();"></asp:CheckBox>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="EquNo" HeaderText="<%$Resources:ttEquNo %>" SortExpression="EquNo" />
                                                                <asp:BoundField DataField="EquName" HeaderText="<%$Resources:ttEquName %>" SortExpression="EquName" />
                                                                <%--<asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="CardRule" runat="server" Text="<%$Resources:ttCardRule %>"></asp:Label>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="ddlCardRule" runat="server"></asp:DropDownList>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>--%>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="Floor" runat="server" Text="<%$Resources:ttElevator %>"></asp:Label>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="btFloor" runat="server" Text="<%$Resources:ttFloor %>" CssClass="IconGroupSet" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>                                                                
                                                                <asp:BoundField DataField="EquID" HeaderText="ID" SortExpression="EquID" />
                                                                <%--<asp:BoundField DataField="EquModel" HeaderText="設備代碼" SortExpression="EquModel" />--%>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderTemplate>
                                                                        <asp:Label ID="CardVirife" runat="server" Text="<%$Resources:ttVerifiMode %>"></asp:Label>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="ddlCardVirife" runat="server"></asp:DropDownList>
                                                                        <input type="hidden" value="<%# DataBinder.Eval(Container.DataItem,"EquID")%>" id="EquID" name="EquID" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <%--GridView Pager的Html Code--%>
                                            <asp:Literal ID="Literal1" runat="server" />
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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

