<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardRuleSettingPop.aspx.cs" Inherits="SahoAcs.CardRuleSettingPop" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>CardRuleSetting</title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="LoadData()">
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
        </div>
        <div>
            <table border="0">
                <tr>
                    <td>
                        <fieldset id="CardRule_List" runat="server" style="width: 410px">
                            <legend id="CardRule_Legend" runat="server">規則清單</legend>
                            <table class="TableS3" style="width:410px">
                                <tr>
                                    <th style="width: 39px;" scope="col">編號</th>
                                    <th style="width: 230px;" scope="col">時區規則</th>
                                    <th scope="col">刪除</th>
                                </tr>
                                <tr>
                                    <td style="padding: 0px;" colspan="3">
                                        <div id="Elevator_Panel" style="height: 200px; overflow-y: scroll;">
                                            <div>
                                                <table id="CardRuleGridView" style="border-width: 0px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                    <tbody>
                                                        <%foreach (System.Data.DataRow r in this.ProcessTable.Rows)
                                                          { %>
                                                        <tr style="background-color: #069; color: #fff">
                                                            <td align="center" style="width: 41px;"><%=r["CardRuleIndex"].ToString() %></td>
                                                            <td style="width: 232px;">
                                                                <%=r["CardRuleName"].ToString() %>
                                                                <input type="hidden" value="<%=r["CardRule"].ToString() %>" name="CardRule" />
                                                            </td>
                                                            <td align="center">
                                                                <input type="button" value="刪除" class="IconDelete" onclick="DoRemove(this)" />
                                                                <input type="hidden" value="<%=r["CardRuleSort"].ToString() %>" id="RemoveID"/>
                                                            </td>
                                                        </tr>
                                                        <%} %>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table border="0" class="Item">
                            <tr>
                                <th>
                                    <asp:Label ID="popL_CardRuleIndex" runat="server" Text="時區規則："></asp:Label>
                                    <asp:TextBox ID="popInput_CardRuleIndex" runat="server" Width="30px" MaxLength="2"></asp:TextBox>
                                    <asp:DropDownList ID="popInput_CardRule" runat="server" Font-Size="13px"></asp:DropDownList>                                    
                                    <input type="button" value="<%=Resources.Resource.btnJoin %>" class="IconRight" onclick="DoAdd()" />
                                </th>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="popL_FunctionRemind" runat="server" Font-Size="13px" ForeColor="#FFFFFF"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">
                        <%--<asp:Button ID="popB_Save" runat="server" CssClass="IconSave" EnableViewState="False" OnClick="popB_Save_Click"  Text="<%$ Resources:Resource, btnSave%>"  />--%>
                        <input type="button" value="<%=Resources.Resource.btnSave %>" class="IconSave" onclick="DoSave()" />
                        <input type="button" value="<%=Resources.Resource.btnCancel %>" class="IconCancel" onclick="DoClose()" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

