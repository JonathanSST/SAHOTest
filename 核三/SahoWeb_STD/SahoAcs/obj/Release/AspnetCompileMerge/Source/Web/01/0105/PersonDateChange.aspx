<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonDateChange.aspx.cs" Inherits="SahoAcs.Web.PersonDateChange" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Src="/uc/Calendar.ascx" TagPrefix="uc1" TagName="Calendar" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td>
                <table class="Item">
                    <tr>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="lblDataList" runat="server" Text="<%$ Resources:lblDataList %>"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:ListBox ID="DataList" runat="server" Height="410px" Width="320px" SkinID="ListBoxSkin"></asp:ListBox>
                        </td>
                        <td style="vertical-align: top;">
                            <fieldset id="Pending_List" runat="server" style="height: 400px; margin-left: 10px">
                                <legend id="Pending_Legend" runat="server">
                                    <asp:Label ID="lblDeadlineAdj" runat="server" Text="<%$ Resources:lblDeadlineAdj %>"></asp:Label>
                                </legend>
                                <table>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblAdjClass" runat="server" Text="<%$ Resources:lblAdjClass %>"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlType" Width="170px" runat="server">
                                                <asp:ListItem Value="PsnSTime">啟用時間</asp:ListItem>
                                                <asp:ListItem Value="PsnETime">停用時間</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 10px">
                                            <asp:Label ID="lblSetTime" runat="server" Text="<%$ Resources:lblSetTime %>"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:Calendar runat="server" ID="Input_Time" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 10px">
                                            <%--<asp:Button ID="btExec" runat="server" Text="<%$ Resources:btnExec %>" CssClass="IconChange" />--%>
                                            <input type="button" id="btExec" name="btExec" value="<%=GetLocalResourceObject("btnExec") %>" class="IconChange" onclick="ExecProcData()" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 20px">
                                            <asp:Label ID="lblMsg" runat="server" Text="<%$ Resources:lblMsg %>"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:ListBox ID="List_Msg" runat="server" Width="840px" Height="160px" SkinID="ListBoxSkin"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input type="button" onclick="ShowOver('1')" value="<%=this.GetLocalResourceObject("btnSelectData") %>" class="IconChoose" id="btnSelectData" name="btnSelectData" />
                        </td>
                        <td></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />
    <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <div id="ContentPlaceHolder1_PanelDrag1" style="background-color: #2D89EF; height: 28px;">
            <table style="width: 100%">
                <tbody>
                    <tr>
                        <td>
                            <span id="ContentPlaceHolder1_L_popName1" style="color: White; font-weight: bold; vertical-align: middle"><%=this.GetLocalResourceObject("SelectData_Title") %></span>
                        </td>
                        <td style="text-align: right;">
                            <%--<input type="image" name="ctl00$ContentPlaceHolder1$ImgCloseButton1" id="ContentPlaceHolder1_ImgCloseButton1" src="/Img/close_button.png" onclick="CancelTrigger1.click(); return false;" style="height: 25px;" />--%>
                        </td>
                    </tr>
                </tbody>
            </table>

        </div>
        <table class="popItem">
            <tbody>
                <tr>
                    <td>
                        <table class="Item">
                            <tbody>
                                <tr>
                                    <th colspan="2">                                        
                                        <asp:Label ID="Label3" runat="server" Text="時間區間(依調整類型)"></asp:Label>
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <uc1:Calendar runat="server" ID="popCalendar1" />
                                    </td>
                                    <td>
                                        <uc1:Calendar runat="server" ID="popCalendar2" />
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="4">
                                        <span class="Arrow01"></span>
                                        <span id="ContentPlaceHolder1_lblKeyword"><%=GetLocalResourceObject("lblKeyword") %></span>
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="4" style="text-align: left">
                                        <input name="Input_TxtQuery" type="text" id="Input_TxtQuery" style="width: 250px;" />
                                        <input type="button" name="popB_Query" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="QueryPsnData();" id="popB_Query" class="IconSearch" /><br />
                                           <span id="ContentPlaceHolder1_lblTip" style="color: Red;"><%=GetLocalResourceObject("lblTip") %></span>
                                    </td>
                                </tr>                                
                                <tr>                                                                        
                                    <th colspan="4">
                                        <span class="Arrow01"></span>
                                        <span id="popLabel_OrgList" style="font-weight: bold;"><%=GetLocalResourceObject("popLabel_OrgList") %></span>
                                    </th>
                                </tr>
                                <tr>
                                    <td style="text-align: center">
                                        <select size="4" name="$popB_PsnList1" multiple="multiple" id="popB_PsnList1" class="ListBoxStyle" style="height: 150px; width: 220px;">
                                        </select>
                                    </td>
                                    <td colspan="2" style="text-align: center">
                                        <input type="button" name="popB_Enter1" value="<%=GetGlobalResourceObject("Resource","btnJoin") %>" onclick="DataEnterRemove('Add'); return false;" id="popB_Enter1" class="IconRight" />
                                        <br />
                                        <br />
                                        <input type="button" name="popB_Remove1" value="<%=GetGlobalResourceObject("Resource","btnRemove") %>" onclick="DataEnterRemove('Del'); return false;" id="popB_Remove1" class="IconLeft" />
                                    </td>
                                    <td style="text-align: center">
                                        <select size="4" name="popB_PsnList2" multiple="multiple" id="popB_PsnList2" class="ListBoxStyle" style="height: 150px; width: 220px;">
                                        </select>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4" style="text-align: center">
                                        <span id="ContentPlaceHolder1_DeleteLableText1" style="font-weight: bold;"></span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="ContentPlaceHolder1_PanelEdit1">
                            <input type="button" name="popB_OK1" value="<%=GetGlobalResourceObject("Resource","btnOK") %>" onclick="LoadPsnDataList()" id="popB_OK1" class="IconOk" />
                            <input type="button" name="popB_Cancel1" value="<%=GetGlobalResourceObject("Resource","btnCancel") %>" onclick="DoCancel('1')" id="popB_Cancel1" class="IconCancel" />
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>

    </div>
        <style>
        #ContentPlaceHolder1_Label1 ,#ContentPlaceHolder1_Label2 {
            color:white
        }
    </style>
</asp:Content>
