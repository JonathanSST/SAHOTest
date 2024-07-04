<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ClassDataMng.aspx.cs" Inherits="SahoAcs._0702.ClassDataMng" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <style type="text/css">
        .style1{
            color:black;
            background-color:white;
            position:absolute;
            top:10px;
            left:10px;
        }
    </style>
<div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1260px">
            <tbody>
                <tr class="GVStyle">             
                    <th scope="col" class="TitleRow" style="text-align:center;width:70px">
                        <%=GetLocalResourceObject("CNo") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:80px">
                        <%=GetLocalResourceObject("CName") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:50px">
                        <%=GetLocalResourceObject("WBTime") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:50px">
                        <%=GetLocalResourceObject("WETime") %>
                    </th>
                     <th scope="col" class="TitleRow" style="text-align:center;width:70px">
                       中間休息起
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:70px">
                        中間休息止
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:50px">
                        <%=GetLocalResourceObject("BERange") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:50px">
                        <%=GetLocalResourceObject("WTMin") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:80px">
                        <%=GetLocalResourceObject("CreateUserID") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:150px">
                        <%=GetLocalResourceObject("CreateTime") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:80px">
                        <%=GetLocalResourceObject("UpdateUserID") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;">
                        <%=GetLocalResourceObject("UpdateTime") %>
                    </th>                    
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="12">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1260px; overflow-y: scroll;">
                            <div id="DivSaveArea">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="12">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);" 
                                            ondblclick="CallEdit('<%=this.GetLocalResourceObject("CallEdit_Title") %>','<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit") %>')" onclick="SingleRowSelect(0, this, SelectValue, <%=o.CID %>, '', '')">          
                                            <td style="width:74px;text-align:left">
                                               <%=o.CNo %>
                                            </td>
                                            <td style="width:84px;text-align:left">
                                               <%=o.CName %>
                                            </td>
                                            <td style="width:54px;text-align:left">
                                               <%=o.WBTime %>
                                            </td>
                                            <td style="width:54px;text-align:left">
                                               <%=o.WETime %>
                                            </td>
                                            <td style="width:74px;text-align:left">
                                               <%=o.OBTime %>
                                            </td>
                                            <td style="width:74px;text-align:left">
                                               <%=o.OETime %>
                                            </td>
                                            <td style="width:54px;text-align:left">
                                               <%=o.BERange %>
                                            </td>
                                            <td style="width:54px;text-align:left">
                                               <%=o.WTMin %>
                                            </td>
                                            <td style="width:84px">
                                                <%=o.CreateUserID %>
                                            </td>
                                            <td style="width:154px">
                                                <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}", o.CreateTime) %>
                                            </td>
                                            <td style="width:84px">
                                                <%=!String.IsNullOrEmpty(o.UpdateUserID)?o.UpdateUserID:"" %>
                                            </td>
                                            <td style="text-align:left">
                                                <%=o.UpdateTime!=null?o.UpdateTime.ToString("yyyy/MM/dd HH:mm:ss"):"" %>
                                            </td>                            
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <%if (this.PagedList != null)
                        { %>
                <tr class="GVStylePgr">
                    <td colspan="12">
                        <a id="btnFirst" href="#" style="text-decoration: none;" onclick="ShowPage(1)">第一頁</a>
                        <a id="btnPrev" href="#" style="text-decoration: none;"  onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                        { %>
                        <%if (pageIndex == this.PageIndex)
                        { %>
                                    <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;color:white"><%=pageIndex %></a>
                        <%}
                        else
                        {%>
                                <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                                <%} %>
                        <%} %>
                            <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>       
                        <a id="btnNext" href="#" style="text-decoration: none; " onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div> 
    <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="SelectNowNo" runat="server" EnableViewState="False" />
    <asp:HiddenField ID="hUserId" runat="server" EnableViewState="False" />    
    <asp:HiddenField ID="hSelectState" runat="server" EnableViewState="False" />  
    <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
    <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />  
    <input type="hidden" id="ddlSelectDefault" value="<%=GetGlobalResourceObject("Resource","ddlSelectDefault") %>" />
    <table>
        <tr>
            <td>
               <%-- <asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <input type="button" id="AddButton" name="AddButton" value="<%=GetGlobalResourceObject("Resource","btnAdd") %>" class="IconNew" onclick="CallAdd('<%=this.GetLocalResourceObject("CallAdd_Title") %>')" />
            </td>
            <td>
                <%--<asp:Button ID="EditButton" runat="server" Text="<%$ Resources:Resource, btnEdit %>" CssClass="IconEdit" />--%>
                <input type="button" id="EditButton" name="EditButton" value="<%=GetGlobalResourceObject("Resource","btnEdit") %>" class="IconEdit" onclick="CallEdit('<%=this.GetLocalResourceObject("CallEdit_Title") %>','<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit") %>')" />
            </td>
            <td>
                <%--<asp:Button ID="DeleteButton" runat="server" Text="<%$ Resources:Resource, btnDelete %>" CssClass="IconDelete" />--%>
                <input type="button" id="DeleteButton" name="DeleteButton" value="<%=GetGlobalResourceObject("Resource","btnDelete") %>" class="IconDelete" 
                    onclick="CallDelete('<%=this.GetLocalResourceObject("CallDelete_Title") %>', '<%=this.GetLocalResourceObject("CallDelete_DelLabel")%>', '<%=this.GetGlobalResourceObject("Resource", "NotSelectForDelete") %>')" />
            </td>
        </tr>
    </table>
    
     <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
                <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <%--<asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />--%>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_CNo" runat="server" Text="<%$ Resources:CNo %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_CName" runat="server" Text="<%$ Resources:CName %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_CNo" runat="server" Width="185px" BorderWidth="1" CssClass="TextBoxRequired" MaxLength="15"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_CName" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblWBTime" runat="server" Text="<%$ Resources:WBTime %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblWETime" runat="server" Text="<%$ Resources:WETime %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_WBTime" runat="server" Width="185px"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_WETime" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblBERange" runat="server" Text="<%$ Resources:BERange %>" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="lblWTMin" runat="server" Text="<%$ Resources:WTMin %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_BERange" runat="server" Width="185px"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_WTMin" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>                        
                         <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="Label1" runat="server" Text="中間休息開始時間" Font-Bold="True"></asp:Label>
                            </th>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="Label2" runat="server" Text="中間休息結束時間" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_OBTime" runat="server" Width="185px"></asp:TextBox>
                            </td>
                            <td colspan="1">
                                <asp:TextBox ID="popInput_OETime" runat="server" Width="185px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="popItem">
                    <asp:Panel ID="PanelEdit1" runat="server" EnableViewState="False">
                        <%--<asp:Button ID="popB_Add" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />--%>
                        <input type="button" id="popB_Add" value="<%=this.GetGlobalResourceObject("resource","btnSave") %>" class="IconSave" />
                        <%--<asp:Button ID="popB_Edit" runat="server" Text="<%$ Resources:Resource, btnSave %>" EnableViewState="False" CssClass="IconSave" />--%>
                        <input type="button" id="popB_Edit" value="<%=this.GetGlobalResourceObject("resource","btnSave") %>" class="IconSave" />
                        <input type="button" id="popB_Delete" value="<%=this.GetGlobalResourceObject("resource","btnDelete") %>" class="IconDelete" />
                        <%--<asp:Button ID="popB_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />--%>
                        <input type="button" id="popB_Cancel" value="<%=this.GetGlobalResourceObject("resource","btnCancel") %>" class="IconCancel" onclick="DoCancel('1')"/>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>

</asp:Content>