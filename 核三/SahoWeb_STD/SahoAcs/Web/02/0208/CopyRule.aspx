<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopyRule.aspx.cs" Inherits="SahoAcs.Web.CopyRule" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
   <form id="form1" runat="server">
    <div>
        <table>
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 150px">設備型號</th>
                    <th scope="col" style="width: 150px">卡片規則編號</th>
                    <th scope="col" style="width:330px">卡片規則名稱</th>
                </tr>
                <tr class="GVStyle">
                    <th scope="col" style="width: 150px">
                        <select id="CopyEquModel" name="CopyEquModel" style="width:95%">
                           <%foreach (var o in this.EquModelList)
                               { %>
                            <option value="<%=o.EquModel %>"><%=o.EquName %></option>
                        <%} %>
                        </select>
                    </th>
                    <th scope="col" style="width: 150px">
                        <input name="RuleNo" autocomplete="off" type="text" id="RuleNo" style="width:140px;" />
                    </th>
                    <th scope="col" style="width:330px">
                        <input name="RuleName" autocomplete="off" type="text" id="RuleName" style="width:320px;" value="<%=this.RuleName %>" />
                    </th>
                </tr>
                <tr>
                    <td style="text-align: center" colspan="3">                              
                        <input type="button" id="btnCopy" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave" />
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel"/>           
                        <input type="hidden" id="RuleID" name="RuleID" value="<%=Request["rule_id"] %>" />           
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
