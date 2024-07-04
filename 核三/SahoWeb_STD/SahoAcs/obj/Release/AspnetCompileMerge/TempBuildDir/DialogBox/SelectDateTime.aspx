﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectDateTime.aspx.cs" Theme="UI" Inherits="SahoAcs.DialogBox.SelectDateTime" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>日期時間選取</title>
    <link href="../Css/Style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tableClose {
            border: 0px solid white;
            border-spacing: 0px;
        }

        .tablespacing2 {
            border: 1px solid white;
            border-spacing: 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>--%>
            <%--<asp:Panel ID="Panel1" runat="server" BackColor="#F7FFFF" Height="193px" Width="400px">--%>            
            <table border="1" class="tableClose">
                <%--行事曆主要列--%>
                <tr>
                    <td>
                        <%--<asp:Panel ID="Panel2" runat="server">--%>
                        <%--行事曆 Header--%>
                        <table class="tableClose" style="width: 100%; height: 26px; background-color: #005EBF;">
                            <tr>
                                <td style="text-align: left">
                                    <asp:ImageButton ID="ImgPrevious" runat="server" ImageUrl="~/Img/previous_Blue.png" Height="20px" /></td>
                                <td style="text-align: center">
                                    <asp:DropDownList ID="DDListY" runat="server">
                                        <asp:ListItem>2008</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="LaYear" runat="server" Font-Bold="true" Font-Size="17px" ForeColor="WhiteSmoke">年</asp:Label>
                                    <asp:DropDownList ID="DDListM" runat="server">
                                        <asp:ListItem>01</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="LaMonth" runat="server" Font-Bold="true" Font-Size="17px" ForeColor="WhiteSmoke">月</asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:ImageButton ID="ImgNext" runat="server" ImageUrl="~/Img/next_Blue.png" Height="20px" /></td>
                            </tr>
                        </table>
                        <%--</asp:Panel>--%>
                    </td>
                </tr>
                <%--行事曆 Content--%>
                <tr>
                    <td style="border: 1px solid">
                        <table class="tablespacing2" style="width: 100%; height: 135px; text-align: center; border: 1px solid white;">
                            <tr>
                                <td style="font-weight: bold; background-color: #D7D7D7">日</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">一</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">二</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">三</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">四</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">五</td>
                                <td style="font-weight: bold; background-color: #D7D7D7">六</td>
                            </tr>
                            <tr>
                                <td id="Box1">&nbsp;</td>
                                <td id="Box2">&nbsp;</td>
                                <td id="Box3">&nbsp;</td>
                                <td id="Box4">&nbsp;</td>
                                <td id="Box5">&nbsp;</td>
                                <td id="Box6">&nbsp;</td>
                                <td id="Box7">&nbsp;</td>
                            </tr>
                            <tr>
                                <td id="Box8">&nbsp;</td>
                                <td id="Box9">&nbsp;</td>
                                <td id="Box10">&nbsp;</td>
                                <td id="Box11">&nbsp;</td>
                                <td id="Box12">&nbsp;</td>
                                <td id="Box13">&nbsp;</td>
                                <td id="Box14">&nbsp;</td>
                            </tr>
                            <tr>
                                <td id="Box15">&nbsp;</td>
                                <td id="Box16">&nbsp;</td>
                                <td id="Box17">&nbsp;</td>
                                <td id="Box18">&nbsp;</td>
                                <td id="Box19">&nbsp;</td>
                                <td id="Box20">&nbsp;</td>
                                <td id="Box21">&nbsp;</td>
                            </tr>
                            <tr>
                                <td id="Box22">&nbsp;</td>
                                <td id="Box23">&nbsp;</td>
                                <td id="Box24">&nbsp;</td>
                                <td id="Box25">&nbsp;</td>
                                <td id="Box26">&nbsp;</td>
                                <td id="Box27">&nbsp;</td>
                                <td id="Box28">&nbsp;</td>
                            </tr>
                            <tr>
                                <td id="Box29">&nbsp;</td>
                                <td id="Box30">&nbsp;</td>
                                <td id="Box31">&nbsp;</td>
                                <td id="Box32">&nbsp;</td>
                                <td id="Box33">&nbsp;</td>
                                <td id="Box34">&nbsp;</td>
                                <td id="Box35">&nbsp;</td>
                            </tr>
                            <tr>
                                <td id="Box36">&nbsp;</td>
                                <td id="Box37">&nbsp;</td>
                                <td id="Box38">&nbsp;</td>
                                <td id="Box39">&nbsp;</td>
                                <td id="Box40">&nbsp;</td>
                                <td id="Box41">&nbsp;</td>
                                <td id="Box42">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <%--時間列--%>
                <tr>
                    <td style="border: 1px solid">
                        <table style="width: 100%;">
                            <%--基本列--%>
                            <tr>
                                <td>
                                    <asp:Label ID="LaTIme" runat="server" Font-Size="12px" Text="時 間 ："></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="DDList1" runat="server" Font-Size="12px">
                                        <asp:ListItem>00</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td>
                                    <asp:Label ID="LaHour" runat="server" Font-Size="12px" Text="時"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="DDList2" runat="server" Font-Size="12px">
                                        <asp:ListItem>00</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td>
                                    <asp:Label ID="LaMin" runat="server" Font-Size="12px" Text="分"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="DDList3" runat="server" Font-Size="12px">
                                        <asp:ListItem>00</asp:ListItem>
                                    </asp:DropDownList></td>
                                <td>
                                    <asp:Label ID="LaSec" runat="server" Font-Size="12px" Text="秒"></asp:Label>

                                </td>

                            </tr>
                            <%--快速列--%>
                            <tr>
                                <td style="text-align: center" colspan="7">
                                    <asp:Label ID="LaToDay" runat="server" Font-Size="12px" Font-Underline="true" Text="今日"></asp:Label>
                                    <asp:Label ID="Label1" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="LaFirst" runat="server" Font-Size="12px" Font-Underline="true" Text="最初"></asp:Label>
                                    <asp:Label ID="Label2" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="La15Min" runat="server" Font-Size="12px" Font-Underline="true" Text="15分"></asp:Label>
                                    <asp:Label ID="Label3" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="La30Min" runat="server" Font-Size="12px" Font-Underline="true" Text="30分"></asp:Label>
                                    <asp:Label ID="Label4" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="La45MIn" runat="server" Font-Size="12px" Font-Underline="true" Text="45分"></asp:Label>
                                    <asp:Label ID="Label5" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="LaLast" runat="server" Font-Size="12px" Font-Underline="true" Text="最終"></asp:Label>
                                    <asp:Label ID="Label6" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="LaClean" runat="server" Font-Size="12px" Font-Underline="true" Text="清除"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <%--底層的Button列--%>
                <tr>
                    <td style="border: 1px solid">
                        <table>
                            <tr>
                                <td style="white-space: nowrap">
                                    <asp:Label ID="LaData" runat="server" Font-Size="12px"></asp:Label></td>
                                <td style="text-align:right">
                                     <asp:Button ID="BSubmit" runat="server" Font-Size="12px" Text="確定" CssClass="IconOk" />
                                    <asp:Button ID="BCancel" runat="server" Font-Size="12px" Text="取消" CssClass="IconCancel" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <%--</asp:Panel>--%>
            <%-- </ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>
    </form>
</body>
</html>
