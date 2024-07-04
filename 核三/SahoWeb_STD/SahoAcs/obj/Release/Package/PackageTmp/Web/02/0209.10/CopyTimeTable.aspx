<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CopyTimeTable.aspx.cs" Inherits="SahoAcs.Web.CopyTimeTable" %>

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
                        <th colspan="3">
                            <%=GetLocalResourceObject("ttTitle").ToString() %>
                        </th>
                </tr>
                <tr class="GVStyle">
                    <th scope="col" style="width: 150px"><%=GetGlobalResourceObject("ResourceEquData","EquModel").ToString() %></th>
                    <th scope="col" style="width: 150px"><%=GetLocalResourceObject("RuleNo").ToString() %></th>
                    <th scope="col" style="width:330px"><%=GetLocalResourceObject("RuleName").ToString() %></th>
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
                        <input name="TimeNo" autocomplete="off" type="text" id="TimeNo" style="width:140px;" />
                    </th>
                    <th scope="col" style="width:330px">
                        <input name="TimeName" autocomplete="off" type="text" id="TimeName" style="width:320px;" value="<%=this.TimeName %>" />
                    </th>
                </tr>
                <tr>
                    <td style="text-align: center" colspan="3">                              
                        <input type="button" id="btnCopy" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave" />
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel"/>           
                        <input type="hidden" id="TimeID" name="TimeID" value="<%=this.TimeID %>" />           
                        <input type="hidden" id="TimeTableMode" value="<%=Request["time_type"] %>" />
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
