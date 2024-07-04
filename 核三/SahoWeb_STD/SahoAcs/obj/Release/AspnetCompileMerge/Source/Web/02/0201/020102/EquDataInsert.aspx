<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquDataInsert.aspx.cs" Inherits="SahoAcs.Web._02._0201._020102.EquDataInsert" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form_insert" runat="server">
    <div id="MainDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;">
        <table class="popItem">
            <tr>
                <td>
                    <table>
                    <tr>
                            <th>
                                <asp:Label ID="Label1" runat="server" Text="設備類型" Font-Bold="true"></asp:Label>
                            </th>                            
                        </tr>
                        <tr>
                            <td>                                
                             <asp:DropDownList ID="EquClass" runat="server" Width="253px" BackColor="#FFE5E5" class="DropDownListStyle">
                            </asp:DropDownList>                            
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
                                <asp:Label ID="popLabel_Building" runat="server" Text="建築物名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_Floor" runat="server" Text="樓層" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="Building" runat="server" Width="180px" CssClass="TextBoxRequired"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="Floor" runat="server" Width="130px" CssClass="TextBoxRequired"></asp:TextBox>
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
                                <asp:Label ID="popLabel_EquModel" runat="server" Text="設備型號" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_EquNo" runat="server" Text="設備編號" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                                                
                                <input name="EquModel" type="text" id="EquModel" readonly="true" 
                                    class="TextBoxRequired" style="width:100px;" value="<%=Request["CtrlEquModel"] %>"/>
                            </td>
                            <td>
                                <asp:TextBox ID="EquNo" runat="server" Width="200px" CssClass="TextBoxRequired"></asp:TextBox>
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
                                <asp:Label ID="popLabel_EquName" runat="server" Text="設備名稱" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_EquEName" runat="server" Text="設備英文名稱" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>                                
                                <input name="EquName" type="text" id="EquName" class="TextBoxRequired" style="width:180px;" value="<%=Request["EquName"] %>"/>
                            </td>
                            <td>
                                <asp:TextBox ID="EquEName" runat="server" Width="155px"></asp:TextBox>
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
                                <asp:Label ID="popLabel_Dci" runat="server" Text="設備連線" Font-Bold="true"></asp:Label>
                            </th>
                            <th>
                                <asp:Label ID="popLabel_CardNoLen" runat="server" Text="卡號長度" Font-Bold="true"></asp:Label>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList ID="Dci" runat="server" Width="253px" BackColor="#FFE5E5" class="DropDownListStyle"></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="CardNoLen" runat="server" Width="60px" CssClass="TextBoxRequired" ReadOnly="true"></asp:TextBox>                                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div  class="ShowInTrt">
                        <table>
                        <tr>
                            <th>
                                顯示姓名
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <asp:CheckBox ID="popIsShowName" runat="server" />
                            </td>
                        </tr>
                    </table>
                    </div>                    
                    <div class="ShowInDoor">
                        <table>
                            <tr>
                                <th>進入管制區
                                </th>
                                <th>離開管制區
                                </th>
                                <th>
                                    卡鐘模式
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="InToCtrlAreaID" runat="server" Width="130px" BackColor="#FFE5E5" class="DropDownListStyle">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="OutToCtrlAreaID" runat="server" Width="130px" BackColor="#FFE5E5" class="DropDownListStyle">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="Input_Trt" runat="server" Width="90px" BackColor="#FFE5E5" class="DropDownListStyle">
                                    <asp:ListItem Value="1">是</asp:ListItem>
                                    <asp:ListItem Value="0" Enabled="True" Selected="True">否</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>                    
                </td>
            </tr>
            <tr>
                <th style="text-align: center">
                    <asp:Label ID="DeleteLableText" runat="server" Font-Bold="true"></asp:Label>
                </th>
            </tr>
            <tr>
                <td style="text-align: center">
                    <input type="button" value="儲存" class="IconSave" onclick="SaveEquData()"/>
                    <input type="button" value="取消" class="IconCancel" onclick="Cancel()"/>
                </td>
            </tr>
        </table>        
        <input type="hidden" id="DoAction" name="DoAction" value="insert" />
    </div>
    </form>
</body>
</html>
