﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrgGroupEdit.aspx.cs" Inherits="SahoAcs.Web._01._0102.OrgGroupEdit" %>
<%@ Import Namespace="SahoAcs.Web._01._0103" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form_adj" runat="server">
        <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;height:610px">            
            <div>
                <table class="popItem">
                <tr>
                    <th colspan="3">
                        <span class="Arrow01"></span>
                        <asp:Label ID="popLabel_OrgList" runat="server" Text="<%$Resources:ttEquGroup %>" Font-Bold="True"></asp:Label>
                    </th>
                </tr>
                    </table>
                <table>
                <tr>
                    <td style="width: 200px; height: 100px; vertical-align: top">
                        <fieldset id="Pending_List" runat="server" style="width: 300px; height: 100px">
                            <legend id="Pending_Legend" runat="server">
                                <asp:Label ID="lblNotAdd" runat="server" Text="<%$Resources:ttUnJoin %>"></asp:Label>
                            </legend>
                            <select class="ListBoxStyle" name="OutCardEquGroup" id="OutCardEquGroup" multiple="multiple" style="width: 95%; height: 90%">
                                <%foreach(var a in this.out_group){ %>
                                    <option value="<%=a.EquGrpID %>"><%=string.Concat("[",a.EquGrpNo,"]",a.EquGrpName) %></option>
                                <%} %>
                            </select>
                        </fieldset>
                    </td>
                    <td style="width: 50px">
                        <input type="button" value="<%=this.GetLocalResourceObject("btnJoin") %>" onclick="GroupSetting('add'); return false;" class="IconRight" /><br />
                        <br />
                        <input type="button" value="<%=this.GetLocalResourceObject("btnRemove") %>" onclick="GroupSetting('del'); return false;" class="IconLeft" />
                    </td>
                    <td style="width: 200px; vertical-align: top">
                        <fieldset id="Fieldset1" runat="server" style="width: 300px; height: 100px">
                            <legend id="Legend1" runat="server">
                                <asp:Label ID="Label1" runat="server" Text="<%$Resources:ttJoin %>"></asp:Label>
                            </legend>
                            <select class="ListBoxStyle" name="InCardEquGroup" id="InCardEquGroup" multiple="multiple" style="width: 95%; height: 90%">
                                <%foreach(var a in this.in_group){ %>
                                    <option value="<%=a.EquGrpID %>"><%=string.Concat("[",a.EquGrpNo,"]",a.EquGrpName) %></option>
                                <%} %>
                            </select>
                        </fieldset>
                    </td>
                </tr>
                </table>
                <table class="popItem">
                <tr>
                    <th colspan="3">
                        <span class="Arrow01"></span>
                        <asp:Label ID="Label2" runat="server" Text="<%$Resources:ttEquName %>" Font-Bold="True"></asp:Label>
                    </th>
                </tr>
                </table>
                <table>
                <tr>
                    <td style="height: 250px; width:960px; vertical-align: top" id="EquArea">
                        <fieldset id="Fieldset3" runat="server" style="width: 940px; height: 350px">
                            <legend id="Legend3" runat="server">
                                <asp:Label ID="Label4" runat="server" Text="<%$Resources:ttEquName %>"></asp:Label>
                            </legend>
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">                                        
                                        <th scope="col" style="width: 60px;"><%=this.GetLocalResourceObject("ttEquNo") %></th>
                                        <th scope="col" style="width:170px"><%=this.GetLocalResourceObject("ttEquName") %></th>
                                        <th scope="col" style="width:55px"><%=this.GetLocalResourceObject("ttAuth") %></th>
                                        <th scope="col" style="width:185px"><%=this.GetLocalResourceObject("ttCardRule") %></th>
                                        <th scope="col" style="width:95px"><%=this.GetLocalResourceObject("ttElev") %></th>
                                        <th scope="col"><%=this.GetLocalResourceObject("ttRule") %></th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="6">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 940px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" 
                                                        id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                                        <tbody>                                                            
                                                            <%foreach(var a in this.in_adj.OrderByDescending(i=>i.OrgStrucID)){ %>
                                                                <tr>                                                                    
                                                                    <td style="width:64px">                                                                        
                                                                        <%=a.EquNo %>
                                                                        <input type="hidden" name="Mode_hid" value="<%=a.OpMode %>" />                                                                        
                                                                    </td>
                                                                    <td style="width:174px">
                                                                        <%=a.EquName %>
                                                                    </td>
                                                                    <td style="width:59px;text-align:center">
                                                                        <%if(a.OrgStrucID!=0&&a.OpMode!="-"){ %>
                                                                            V
                                                                            <input type="hidden" value="V" name="HasAuth" />
                                                                        <%}else{ %>
                                                                            <input type="hidden" value="" name="HasAuth" />    
                                                                        <%} %>
                                                                    </td>
                                                                    <td style="width:189px;text-align:center">
                                                                        <select id="CardRule_<%=a.EquID %>" name="CardRule_<%=a.EquID %>" style="width:95%">
                                                                            <%if (this.RuleResult.Where(i => i.EquID == a.EquID).Count() > 0)
                                                                                    { %>
                                                                                <%foreach (var rule in this.RuleResult.Where(i => i.EquID == a.EquID))
                                                                                        { %>
                                                                                    <option value="<%=rule.RuleID %>"><%=string.Format("{0}-{1}({2})",rule.ParaValue,rule.RuleNo,rule.RuleName) %></option>
                                                                                <%} %>
                                                                            <%}else { %>
                                                                                <option value="">未定義規則</option>
                                                                            <%} %>
                                                                        </select>
                                                                        <input type="hidden" name="CardRuleID" value="<%=a.CardRule %>" />
                                                                    </td>
                                                                    <td style="width:99px;text-align:center">
                                                                        <input type="button" value="<%=this.GetLocalResourceObject("btnElev") %>" class="IconGroupSet"  />
                                                                        <input type="hidden" value="<%=a.EquClass %>" name="EquClass" />
                                                                        <input type="hidden" value="<%=a.CardExtData %>" name="CardExtData_<%=a.EquID %>" />
                                                                    </td>
                                                                    <td>                                                                           
                                                                        <input type="radio"  value="1" name='OpMode_<%=a.EquID %>' checked="checked"/><%=this.GetLocalResourceObject("rbtAdd") %>
                                                                        <input type="radio"  value="0" name='OpMode_<%=a.EquID %>' /><%=this.GetLocalResourceObject("rbtRemove") %>
                                                                        <input type="radio"  value="2" name='OpMode_<%=a.EquID %>' /><%=this.GetLocalResourceObject("rbtEquGroup") %>                                                                        
                                                                        <input type="hidden" name="EquID" value="<%=a.EquID %>" />
                                                                        <input type="hidden" name="EquName" value="<%=a.EquName %>" />
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
                    <td style="text-align:center;width:940px">
                        <input type="button" value="<%= this.GetGlobalResourceObject("Resource","btnOK")%>" class="IconSave" onclick="SaveSetting()"/>
                        <input type="button" value="<%=this.GetGlobalResourceObject("Resource","btnExit") %>" class="IconCancel" onclick="DoCancel()"/>                        
                        <input type="hidden" id="EquIDAll" name="EquIDAll" value="" />
                    </td>
                </tr>
            </table>
            <input type="hidden" name="OrgStrucID" id="OrgStrucID" value="<%=Request["OrgStrucID"] %>" />    
            <input type="hidden" name="DoAction" value="Save" />
                <input type="hidden" id="ResultMsg" name="ResultMsg" value="<%=this.GetLocalResourceObject("ttResult") %>" />
            </div>              
        </div>        
    </form>
</body>
</html>
