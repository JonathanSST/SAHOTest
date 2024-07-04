<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="HolidayExMng.aspx.cs" Inherits="SahoAcs._0701.HolidayExMng" Debug="true" EnableEventValidation="false" Theme="UI" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <style type="text/css">
        .style1{
            color:black;
            background-color:white;
            position:absolute;
            top:10px;
            left:10px;
        }
    </style>
    <table class="Item">
        <tr>
            <th style="text-align:left">
                <asp:Label ID="lblYear" runat="server" Text="<%$ Resources:lblCondition %>"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td style="width: 120px">
                <asp:DropDownList ID="Input_Year" runat="server" Width="90%"></asp:DropDownList>
            </td>
            <td>
                <%--<asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch" />--%>
                <input type="button" id="QueryButton" name="QueryButton" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" class="IconSearch" />
                <%--<asp:Button ID="BulidButton" runat="server" Text="<%$ Resources:btnBulid %>" CssClass="IconNew" />--%>
                <input type="button" id="BuildButton" name="BuildButton" value="<%=GetLocalResourceObject("btnBulid") %>" class="IconNew" />
            </td>
        </tr>
    </table>
<div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:700px">
            <tbody>
                <tr class="GVStyle">             
                    <th scope="col" class="TitleRow" style="text-align:center;width:100px">
                        <%=GetLocalResourceObject("HEDate") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;width:300px">
                        <%=GetLocalResourceObject("HEDesc") %>
                    </th>
                    <th scope="col" class="TitleRow" style="text-align:center;">
                        <%=GetLocalResourceObject("CreateUserID") %>
                    </th>                    
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="3">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 700px; overflow-y: scroll;">
                            <div id="DivSaveArea">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="3">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);" 
                                            ondblclick="CallEdit('<%=this.GetLocalResourceObject("CallEdit_Title") %>','<%=this.GetGlobalResourceObject("Resource","NotSelectForEdit") %>')" onclick="SingleRowSelect(0, this, SelectValue, <%=o.HEID %>, '', '')">          
                                            <td style="width:104px;text-align:left">
                                               <%=o.HEDate %>
                                            </td>
                                            <td style="width:304px">
                                                 <%=o.HEDesc %>
                                            </td>
                                            <td style="text-align:left">
                                                <%=o.CreateUserID %>
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
                    <td colspan="3">
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
    <asp:HiddenField ID="SelectYear" runat="server" EnableViewState="False" />
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
                       <%-- <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
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
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_Year" runat="server" Text="<%$ Resources:lblCondition_Add %>" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="popLabel_YYYY" runat="server" Text="yyyy" Font-Size="X-Large" ForeColor="Black" Font-Bold="True"></asp:Label>
                                <asp:Label ID="Label1" runat="server" Text="年" Font-Bold="True"></asp:Label>
                                <asp:DropDownList ID="popInput_MM" runat="server" Width="70px"></asp:DropDownList>
                                <asp:Label ID="Label2" runat="server" Text="月" Font-Bold="True"></asp:Label>
                                <asp:DropDownList ID="popInput_DD" runat="server" Width="70px"></asp:DropDownList>
                                <asp:Label ID="Label3" runat="server" Text="日" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_HEDesc" runat="server" Text="<%$ Resources:HEDesc %>" Font-Bold="True"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:TextBox ID="popInput_HEDesc" runat="server" Width="390px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2" style="text-align: center">
                                <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                            </th>
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