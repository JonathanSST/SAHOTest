<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MapList.aspx.cs" Inherits="SahoAcs.MapList" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="height: 250px; width: 980px; vertical-align: top" id="EquArea">
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 190px">圖幅名稱</th>
                            <th scope="col" style="width: 270px">圖幅路徑</th>
                            <th scope="col">地圖編輯</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="3">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 980px; overflow-y: scroll;">
                                    <div id="DataResult">
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var s in MapDataList)
                                                  { %>
                                                <tr>
                                                    <td style="width: 194px; text-align: left"><%=s.PicDesc %></td>
                                                    <td style="width: 274px; text-align: left"><%=s.PicName %></td>
                                                    <td style="text-align: left">
                                                       <input type="button" id="BtnPoint" value="設備座標" class="IconSet" />
                                                       <input type="button" id="BtnLine" value="路線編輯" class="IconSet"  />
                                                       <input type="button" id="BtnRoute" value="路線清單" class="IconLook"/>
                                                        <input type="button" id="BtnEdit" value="資料編輯" class="IconEdit"/>                                                        
                                                        <input type="hidden" id="EditID" value="<%=s.PicID %>" />
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
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <input type="button" value="新增" onclick="AddMap()" class="IconNew" />
            </td>
        </tr>
    </table>
</asp:Content>
