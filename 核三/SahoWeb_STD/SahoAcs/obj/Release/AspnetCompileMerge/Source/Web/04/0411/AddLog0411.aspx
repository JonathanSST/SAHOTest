<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddLog0411.aspx.cs" Inherits="SahoAcs.Web.AddLog0411" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register src="/uc/CalendarFrm.ascx" tagname="NewCalendar" tagprefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div>
        <table class="popItem">
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>                                
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PsnID" runat="server" Text="人員ID" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PsnNo" runat="server" Text="學號" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_PsnID" runat="server" Enabled="False" Width="190px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_PsnNo" runat="server" Enabled="False" Width="190px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_PsnName" runat="server" Text="人員姓名" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_CardNo" runat="server" Text="卡號" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="popInput_PsnName" runat="server" Enabled="False" Width="190px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="popInput_CardNo" runat="server" Enabled="False" Width="190px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table >
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_EquName" runat="server" Text="廠商" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_LogStatus" runat="server" Text="設備名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="DropMgaList" runat="server" Width="200px" CssClass="DropDownListStyle">
                                </asp:DropDownList>
                            </td>
                            <td id="EquNameArea">
                                <asp:DropDownList ID="popDrop_EquName" runat="server" Width="200px" CssClass="DropDownListStyle">
                                </asp:DropDownList>
                            </td>                            
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table >
                        <tr>
                            <th>
                                <span class="Arrow01"></span>
                                <asp:Label ID="Label1" runat="server" Text="刷卡結果" Font-Bold="true"></asp:Label>
                            </th>
                            <th>                               
                            </th>
                        </tr>
                        <tr>
                            <td>
                                 <asp:DropDownList ID="popDrop_LogStatus" runat="server" Width="200px" CssClass="DropDownListStyle">
                                </asp:DropDownList>
                            </td>
                            <td>                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <th colspan="1">
                                <span class="Arrow01"></span>
                                <asp:Label ID="popLabel_CardTime" runat="server" Text="補登日期" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td colspan="1" id="DateArea1">                                
                                <uc2:NewCalendar ID="NewCalendar1" runat="server" />                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>                
                    <input type="button" name="BtnSave" value="<%=Resources.Resource.btnSave %>"  id="BtnSave" class="IconSave"/>
                    <input type="button" name="BtnCancel" value="<%=Resources.Resource.btnCancel %>"  id="BtnCancel" class="IconCancel"/>
                    <input type="hidden" name="CardID" id="CardID" value="<%=this.entity.CardID%>" />                
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
