<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquGroupEdit.aspx.cs" Inherits="SahoAcs.EquGroupEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">         
        <div id="GroupEdit">
        <input type="hidden" id="PageEvent" name="PageEvent" value="Save" />
        <input type="hidden" id="InEquGroup" name="InEquGroup" value="<%=Request["InEquGroup"]%>" />
            <input type="hidden" id="InputEquGrp" name="InputEquGrp" value="<%=this.grpid %>" />
            <fieldset id="Fieldset3" runat="server" style="width: 940px; height: 450px">
                <legend id="Legend3" runat="server">
                    <%=this.EquName %>-<asp:Label ID="Label4" runat="server" Text="<%$Resources:Resource,EquMap %>"></asp:Label>
                </legend>
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 80px;"></th>
                            <th scope="col" style="width: 100px;"><%=Resources.ResourceGrp.EquNo %></th>
                            <th scope="col" style="width: 200px"><%=Resources.ResourceGrp.EquName %></th>
                            <th scope="col" style="width: 100px"><%=Resources.ResourceGrp.lblFloorName %></th>
                            <th scope="col"><%=Resources.ResourceGrp.Rule %></th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 405px; width: 940px; overflow-y: scroll;">
                                    <div>
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var o in this.groupdata)
                                                    { %>                                                
                                                        <tr>
                                                        <td style="width: 84px">
                                                            <input type="checkbox" id="CHK_COL_1" name="CHK_COL_1" value="<%=o.EquID %>" style="width: 20px; height: 20px"
                                                                <%if (o.EquGrpID != 0)
                                                                { %>
                                                                checked="checked"
                                                                <%} %> />
                                                            <input type="hidden" id="CardExtData_<%=o.EquID %>" name="CardExtData_<%=o.EquID %>" value="<%=o.CardExtData %>" />
                                                            <input type="hidden" id="CardRuleID" name="CardRuleID" value="<%=o.CardRule %>" />
                                                            <input type="hidden" id="EquGrpID" name="EquGrpID" value="<%=o.EquGrpID %>" />
                                                        </td>
                                                        <td style="width: 104px">
                                                            <%=string.Format("{0}",o.EquNo) %>
                                                        </td>
                                                        <td style="width: 204px">
                                                            <%=o.EquName %>
                                                        </td>
                                                        <td style="width: 104px">
                                                            <%if (o.EquClass == "Elevator")
                                                                { %>
                                                            <input type="button" id="BtnFloor" value="<%=Resources.ResourceGrp.btnSetting2 %>" class="IconSet" />
                                                            <%} %>
                                                        </td>
                                                        <td>
                                                            <select id="CardRule_<%=o.EquID %>" name="CardRule_<%=o.EquID %>" style="width: 95%">
                                                                <%if (this.RuleResult.Where(i => i.EquID == o.EquID).Count() > 0)
                                                                    { %>
                                                                <%foreach (var rule in this.RuleResult.Where(i => i.EquID == o.EquID))
                                                                    { %>
                                                                <option value="<%=rule.RuleID %>"><%=string.Format("{0}-{1}({2})",rule.ParaValue,rule.RuleNo,rule.RuleName) %></option>
                                                                <%} %>
                                                                <%}
                                                                    else
                                                                    { %>
                                                                <option value="">未定義規則(Not Define Rule)</option>
                                                                <%} %>
                                                            </select>
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
                <input type="button" value="<%= this.GetGlobalResourceObject("Resource","btnOK")%>" class="IconSave" onclick="SetSave()" />
                                <input type="button" value="<%=this.GetGlobalResourceObject("Resource","btnExit") %>" class="IconCancel" onclick="DoCancel()" />
            </fieldset>
        </div>
    </form>
</body>
</html>
