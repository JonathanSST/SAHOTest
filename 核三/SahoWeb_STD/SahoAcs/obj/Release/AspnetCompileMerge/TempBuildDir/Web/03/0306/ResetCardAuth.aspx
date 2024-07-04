<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" EnableEventValidation="false" 
    Theme="UI" Debug="true" CodeBehind="ResetCardAuth.aspx.cs" 
    Inherits="SahoAcs.Web._03._0306.ResetCardAuth" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />        
    </div>    
            <table class="TableS1" style="width: 850px">
                <tbody>
                    <tr class="GVStyle">
                        <th scope="col" style="width: 150px;">姓名</th>
                        <th scope="col" style="width: 150px;">人員編號</th>
                        <th scope="col" style="width: 150px;">卡號</th>
                        <th scope="col" style="width:100px">重置</th>                        
                        <th scope="col">卡別</th>
                    </tr>
                    <tr>
                        <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                            <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                            <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1050px; overflow-y: scroll;">
                                <div>
                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                        <tbody>
                                            <%foreach(var log in this.logs){ %>                                            
                                            <tr class="GV_Row2" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)">
                                                <td style="width: 154px;"><%=log.PsnName %></td>
                                                <td style="width: 154px;"><%=log.PsnNo %></td>
                                                <td style="width: 154px;"><%=log.CardNo %></td>
                                                <td style="width: 104px; text-align:center"><input type="button" class="IconRefresh" value="重置"/></td>
                                                <td><%=log.ItemName %></td>
                                            </tr>
                                            <%                                                  
                                            } %>                                            
                                        </tbody>
                                    </table>                                    
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>        
    <div>               
    </div>
</asp:Content>
