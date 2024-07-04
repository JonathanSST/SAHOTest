﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CardVerChange.aspx.cs" Inherits="SahoAcs.Web._01._0103.CardVerChange" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <div>
        <table>
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 100px">卡別</th>
                    <th scope="col" style="width: 200px">舊版次</th>
                    <th scope="col" style="width:200px">新版次</th>
                </tr>
                <tr class="GVStyle">
                    <th scope="col" style="width: 100px">
                        <%=this.cardentity.CardTypeName %>
                    </th>
                    <th scope="col" style="width: 200px">
                        <%=this.cardentity.CardVer %>
                          <%if (this.cardentity.CardNo == null || this.cardentity.CardNo=="")
                            { %>
                        <span>主卡已借用臨時卡，無法變更</span>
                        <%} %>
                    </th>
                    <th scope="col" style="width:200px">
                        <input name="NewCardVer" autocomplete="off" type="text" id="NewCardVer" must_keyin_yn="Y" field_name="新版次" style="width:180px;" maxlength="1" />
                    </th>
                </tr>
                <tr>
                    <td style="text-align: center" colspan="3">                              
                         <%if(this.cardentity.CardNo!=null && this.cardentity.CardNo!="") { %>
                            <input type="button" id="btnUpdate" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave" />
                        <%} %>
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel"/>
                        <input type="hidden" id="CardID" value="<%=this.cardentity.CardID %>" name="CardID"/>
                        <input type="hidden" id="CardNo" value="<%=this.cardentity.CardNo %>" name="CardNo"/>
                        <input type="hidden" id="CardVer" value="<%=this.cardentity.CardVer %>" name="CardVer"/>
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
</body>
</html>
