<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="VmsList.aspx.cs" Inherits="SahoAcs.Web._98._9820.VmsList" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblVmsHost" runat="server" Text="V.M.S Host"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblVmsPre" runat="server" Text="讀卡前秒數設定"></asp:Label>
            </th>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblVmsNext" runat="server" Text="讀卡後秒數設定"></asp:Label>
            </th>
        </tr>
        <tr>
            <td>
                <input type="text" name="VmsHost" value="<%=this.VmsHost %>" style="width: 250px" />
            </td>
            <td>
                <input type="text" name="VmsPre" value="<%=this.VmsPre %>" style="width: 250px"  onkeyup="ValidateNumber(this,value)"/>
            </td>
            <td>
                <input type="text" name="VmsNext" value="<%=this.VmsNext %>" style="width: 250px" onkeyup="ValidateNumber(this,value)"/>
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
                                                <%foreach (var s in this.EquVmsList)
                                                  { %>
                                                <tr>
                                                    <td style="width: 174px; text-align: left"><%=s.EquNo %>
                                                        <input type="hidden" value="<%=s.EquNo %>" name="EquNo" />
                                                    </td>
                                                    <td style="width: 259px; text-align: left"><%=s.EquName %></td>
                                                    <td style="text-align: left">
                                                        <input type="text" name="VmsName" value="<%=s.VmsName %>" 
                                                            style="width: 95%; border-style:solid; border-width:1px; border-color:Background" />
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
                <input type="button" value="儲存" onclick="SaveEquVms()" class="IconSave" />
            </td>
            </tr>
            </table>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
