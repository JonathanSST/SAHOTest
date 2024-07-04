<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquGroupSettingPop.aspx.cs" Inherits="SahoAcs.Web._02._0207.EquGroupSettingPop" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid; height: 510px">
            <table>
                <tr>
                    <td style="width: 420px; height: 400px; vertical-align: top">
                        <table border="0" class="Item">
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="PendingLabelEquClass" runat="server" Text="<%$Resources:ResourceGrp,EquType %>" Font-Size="14px"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="PendingLabelEquNo" runat="server" Text="<%$Resources:ResourceGrp,EquNo %>" Font-Size="14px"></asp:Label></th>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="PendingInput_EquClass" runat="server" CssClass="DropDownListStyle" Font-Size="13px" OnInit="PendingInput_EquClass_Init"></asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="PendingInput_EquNo" runat="server" Font-Size="14px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="PendingButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" Font-Size="14px" CssClass="IconSearch" />
                                </td>
                            </tr>
                        </table>
                        <fieldset id="Pending_List" runat="server" style="width: 400px; height: 380px">
                            <legend id="Pending_Legend" runat="server">
                                <%=Resources.ResourceGrp.ttUnJoin %>
                            </legend>
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">
                                        <th scope="col" style="width: 20px;">
                                            <input type="checkbox" name="PendCheckAll" />
                                        </th>
                                        <th scope="col" style="width: 80px;">設備編號</th>
                                        <th scope="col">設備名稱</th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="6">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 320px; width: 400px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                                        <tbody>
                                                            <%foreach (var o in this.PendingList)
                                                                {%>
                                                            <tr id="GV_Row2" class="ChR">
                                                                <td align="center" style="width: 24px;">
                                                                    <span onchange="">
                                                                        <input name="PendCheck" type="checkbox" value="<%=o.EquID %>" /></span>
                                                                </td>
                                                                <td title="<%=o.EquNo %>" style="width: 84px;"><%=o.EquNo %></td>
                                                                <td title="<%=o.EquName %>"><%=o.EquName%></td>
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
                        </fieldset>
                    </td>
                    <td style="width: 50px">
                        <input type="button" value="<%=Resources.Resource.btnJoin %>" onclick="GroupSetting('add'); return false;" class="IconRight" /><br />
                        <br />
                        <input type="button" value="<%=Resources.Resource.btnRemove %>" onclick="GroupSetting('del'); return false;" class="IconLeft" />
                    </td>
                    <td style="width: 620px; height: 400px; vertical-align: top">
                        <table border="0" class="Item">
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="QueueLabelEquClass" runat="server" Text="<%$Resources:ResourceGrp,EquType %>" Font-Size="14px"></asp:Label>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="QueueLabelEquNo" runat="server" Text="<%$Resources:ResourceGrp,EquNo %>" Font-Size="14px"></asp:Label></th>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="QueueInput_EquClass" runat="server" CssClass="DropDownListStyle" OnInit="QueueInput_EquClass_Init" Font-Size="13px"></asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="QueueInput_EquNo" runat="server" Font-Size="14px"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Button ID="QueueButton" runat="server" Text="<%$Resources:Resource,btnQuery %>" Font-Size="14px" CssClass="IconSearch" />
                                </td>
                            </tr>
                        </table>
                        <fieldset id="Fieldset1" runat="server" style="width: 600px; height: 380px">
                            <legend id="Legend1" runat="server">
                                <%=Resources.ResourceGrp.ttJoin %>
                            </legend>
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">
                                        <th scope="col" style="width: 20px;">
                                            <input type="checkbox" name="QueueCheckAll" />
                                        </th>
                                        <th scope="col" style="width: 60px;">設備編號</th>
                                        <th scope="col" style="width:120px;">設備名稱</th>
                                        <th scope="col" style="width: 170px;">
                                            <span id="ContentPlaceHolder1_GridView1_CardRule">規則</span>
                                        </th>
                                        <th scope="col">
                                            <span id="ContentPlaceHolder1_GridView1_Floor">電梯</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView1" style="padding: 0" colspan="6">
                                            <div id="ContentPlaceHolder1_tablePanel1" style="height: 320px; width: 600px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_GridView1" style="border-collapse: collapse;">
                                                        <tbody>
                                                            <%foreach(var o in this.QueueList) { %>
                                                            <tr id="GV_Row1">
                                                                <td align="center" style="width: 24px;">
                                                                    <span onchange="">
                                                                        <input type="checkbox" name="QueueCheck" value="<%=o.EquID %>"/></span>
                                                                </td>
                                                                <td title="<%=o.EquNo %>" style="width: 64px;"><%=o.EquNo %></td>
                                                                <td title="<%=o.EquName %>" style="width: 124px;"><%=o.EquName %></td>
                                                                <td align="center" style="width: 174px;">
                                                                </td>
                                                                <td align="center">
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
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <%--<asp:Button ID="popB_Save" runat="server"  Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" OnClick="popB_Save_Click" CssClass="IconSave" />
                        <asp:Button ID="popB_Cancel" runat="server"  Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />--%>
                        <input type="button" value="<%=Resources.Resource.btnSave %>" class="IconSave" onclick="CloseSetting()"/>
                        <input type="button" value="<%=Resources.Resource.btnCancel %>" class="IconCancel"  onclick="CloseSetting()"/>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
