<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RtspList.aspx.cs" Inherits="SahoAcs.Web._97._9705.RtspList" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblEvoHost" runat="server" Text="影片服務端位置"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <input type="text" name="RtspHost" value="<%=RtspHost %>" style="width: 250px" />
            </td>            
            <td>                                
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <input type="button" id="AddButton" name="AddButton" value="新增攝影機位置"  class="IconNew" />
            </td>
        </tr>
        <tr>
            <td style="height: 250px; width: 820px; vertical-align: top" id="EquArea">
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">                            
                            <th scope="col" style="width: 355px">位置資訊</th>
                            <th scope="col">備註</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 820px; overflow-y: scroll;">
                                    <div>
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var s in this.GlobalRtspList)
                                                    { %>
                                                    <tr>      
                                                        <td style="width:359px;">
                                                            <input type="text" name="RtspVideo"  id="RtspVideo" value="<%=s.RtspVideo %>" style="width:98%"/>
                                                            <input type="hidden" name="ResourceID" id="ResourceID" value="<%=s.ResourceID %>" />
                                                        </td>
                                                        <td>
                                                            <input type="text" name="RtspMemo" id="RtspMemo" value="<%=s.RtspMemo %>"   style="width:98%"/>
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
    <!--這個是空白資料行-->
    <div id="NewRow" style="display: none">
        <table id="TableEmpty">
            <tbody>
            <tr id="GV_Row20" class="ChR" style="color: rgb(0, 0, 0);">
                <td title="位置資訊" style="width: 359px">
                    <input type="text" name="RtspVideo"  id="RtspVideo" value=""  style="width:98%"/>
                    <input type="hidden" name="ResourceID" id="ResourceID" value="0" />
                </td>                                
                <td>
                     <input type="text" name="RtspMemo" id="RtspMemo" value=""  style="width:98%"/>
                </td>
            </tr>
                </tbody>
        </table>
    </div>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <input type="button" value="儲存" id="SaveButton" class="IconSave" />
            </td>
            </tr>
            </table>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
