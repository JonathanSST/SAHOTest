<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FloorPara.aspx.cs" Inherits="SahoAcs.Web._02._0212.FloorPara" %>

<%@ Import Namespace="SahoAcs.DBModel" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;">
            <%-- <input type="hidden" value="<%=Request["EquID"] %>" name="EquIDForChange" />--%>
            <table class="Item" style="background-color: #069">
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" runat="server" style="width: 500px">
                            <legend id="Elevator_Legend" runat="server" style="font-size: 15px">參數列表</legend>
                            <table class="TableS3" border="0">
                                <tbody>
                                    <tr>
                                        <th style="width: 260px;" scope="col">參數名稱</th>
                                        <th scope="col">參數數值</th>
                                    </tr>
                                    <tr>
                                        <td style="padding: 0px;" colspan="3">
                                            <div id="Elevator_Panel" style="height: 300px; overflow-y: scroll;">
                                                <div>
                                                    <table id="ParaGridView" style="border-width: 0px; border-collapse: collapse;width:100%" rules="all" cellspacing="0">
                                                        <tbody>
                                                            <%foreach (var ep in this.EquParaList)
                                                            { %>
                                                            <tr style="background-color: #069; color: #fff">
                                                                <td style="width: 262px;"><%=ep.ParaDesc %></td>
                                                                <td style="text-align: center">
                                                                    <%if (ep.InputType.Equals("0"))
                                                                    { %>
                                                                    <input type="text" style="width: 125px" value="<%=ep.ParaValue %>" name="ParaValue" />
                                                                    <%}
                                                                        else if (ep.InputType.Equals("1"))
                                                                        { %>
                                                                    <input type="text" style="width: 125px" value="<%=ep.ParaValue %>" name="ParaValue" />
                                                                    <%}
                                                                        else if (ep.InputType.Equals("2"))
                                                                        { %>
                                                                    <select name="ParaValue" style="width: 125px" class="DropDownListStyle">
                                                                        <%foreach (var s in ep.ValueOptions.ToString().Split('/'))
                                                                        { %>
                                                                        <option value="<%=s.Split(':')[1] %>"><%=s.Split(':')[0] %></option>
                                                                        <%} %>
                                                                    </select>
                                                                    <%}
                                                                    else
                                                                    { %>
                                                                    <input name="ParaBtn" class="IconSet" id="ParaValue" style="margin: 2px; padding: 2px 10px; width: 135px; font-size: 10pt;"
                                                                        type="button" value="設　　定" onclick="PopURL(this)" />
                                                                    <input type="hidden" name="ParaValue" value="<%=ep.ParaValue %>" />
                                                                    <%} %>
                                                                    <input type="hidden" value="<%=ep.EquID %>" name="EquID" id="EquID" />
                                                                    <input type="hidden" value="<%=ep.EquNo %>" name="EquNo" id="EquNo" />
                                                                    <input type="hidden" value="<%=ep.EquName %>" name="EquName" id="EquName" />
                                                                    <input type="hidden" value="<%=ep.EquParaID %>" name="EquParaID" id="EquParaID" />
                                                                    <input type="hidden" value="<%=ep.EditFormURL %>" name="EditFormURL" id="EditFormURL" />
                                                                    <input type="hidden" value="" name="ParaUI" id="ParaUI" />
                                                                    <input type="hidden" value="<%=ep.M_ParaValue %>" name="M_ParaValue" id="M_ParaValue" />
                                                                    <input type="hidden" value="<%=ep.ParaValue %>" name="O_ParaValue" id="O_ParaValue" />
                                                                    <input type="hidden" value="<%=ep.FloorName %>" name="FloorName" id="FloorName" />
                                                                    <input type="hidden" value="<%=ep.ParaName %>" name="ParaName" id="ParaName" />
                                                                    <input type="hidden" value="<%=ep.ParaDesc %>" name="ParaDesc" id="ParaDesc" />
                                                                    <input type="hidden" value="<%=ep.ParaType %>" name="ParaType" id="ParaType" />
                                                                    <input type="hidden" value="<%=ep.CtrlID %>" name="CtrlID" id="CtrlID" />
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
                    <td style="text-align: center">                        
                        <input type="button" value="確定" class="IconSave" onclick="SetSave()" />
                        <input type="button" value="取消" class="IconCancel" onclick="SetCancel()" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
