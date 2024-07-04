<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EvoList.aspx.cs" Inherits="SahoAcs.Web._98._9820.EvoList" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblEvoHost" runat="server" Text="EVO Host"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblEvoPre" runat="server" Text="登入帳號"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblEvoNext" runat="server" Text="登入密碼"></asp:Label>
            </th>
        </tr>
        <tr>
            <td>
                <input type="text" name="EvoHost" value="<%=this.EvoHost %>" style="width: 250px" />
            </td>
            <td>
                <input type="text" name="EvoUid" value="<%=this.EvoUid %>" style="width: 250px"/>
            </td>
            <td>
                <input type="text" name="EvoPwd" value="<%=this.EvoPwd %>" style="width: 250px"/>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td style="height: 250px; width: 820px; vertical-align: top" id="EquArea">
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 170px">設備編號</th>
                            <th scope="col" style="width: 255px">設備名稱</th>
                            <th scope="col">對應攝影機資訊</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 820px; overflow-y: scroll;">
                                    <div>
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var s in this.EquEvoList)
                                                    { %>
                                                <tr>
                                                    <td style="width: 174px; text-align: left"><%=s.EquNo %>
                                                        <input type="hidden" value="<%=s.EquNo %>" name="EquNo" />
                                                    </td>
                                                    <td style="width: 259px; text-align: left"><%=s.EquName %></td>
                                                    <td style="text-align: left">
                                                        <input type="button" value="攝影機清單設定" id="BtnAdd" class="IconNew" onclick="AddEvoDevice(this)" />
                                                        <input type="hidden" value="<%=s.EvoName %>" id="EvoName" name="EvoName" />
                                                        <input type="hidden" value="<%=s.EquNo %>" id="EvoNo" name="EvoNo" />
                                                        <span id="EvoInfo"><%=s.EvoName %></span>
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
                <input type="button" value="儲存" onclick="SaveEquEvo()" class="IconSave" />
            </td>
            </tr>
            </table>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
