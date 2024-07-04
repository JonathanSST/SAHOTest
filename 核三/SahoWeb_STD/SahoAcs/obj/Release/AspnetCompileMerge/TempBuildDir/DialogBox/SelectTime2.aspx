<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectTime2.aspx.cs" Theme="UI" Inherits="SahoAcs.DialogBox.SelectTime2" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>時間選取</title>
    <link href="../Css/Style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tableClose
        {
            border: 0px solid white;
            border-spacing: 0px;
        }

        .tablespacing2
        {
            border: 1px solid white;
            border-spacing: 2px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
            <table border="1" class="tableClose" style="width: 270px">
                <tr>
                    <td style="text-align: center; background-color: #005EBF;">
                        <asp:Label ID="Label_Hour" runat="server" Text="小　時" ForeColor="WhiteSmoke"></asp:Label></td>
                </tr>
                <%--小時列表--%>
                <tr>
                    <td style="border: 1px solid; background: white">
                        <table class="tablespacing2" style="width: 100%; height: 125px; text-align: center; border: 1px solid white;">
                            <tr>
                                <td id="Hour0">00</td>
                                <td id="Hour1">01</td>
                                <td id="Hour2">02</td>
                                <td id="Hour3">03</td>
                                <td id="Hour4">04</td>
                                <td id="Hour5">05</td>
                            </tr>
                            <tr>
                                <td id="Hour6">06</td>
                                <td id="Hour7">07</td>
                                <td id="Hour8">08</td>
                                <td id="Hour9">09</td>
                                <td id="Hour10">10</td>
                                <td id="Hour11">11</td>

                            </tr>
                            <tr>
                                <td id="Hour12">12</td>
                                <td id="Hour13">13</td>
                                <td id="Hour14">14</td>
                                <td id="Hour15">15</td>
                                <td id="Hour16">16</td>
                                <td id="Hour17">17</td>
                            </tr>
                            <tr>
                                <td id="Hour18">18</td>
                                <td id="Hour19">19</td>
                                <td id="Hour20">20</td>
                                <td id="Hour21">21</td>
                                <td id="Hour22">22</td>
                                <td id="Hour23">23</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center; background-color: #005EBF;">
                        <asp:Label ID="Label_Min" runat="server" Text="分　鐘" ForeColor="WhiteSmoke"></asp:Label></td>
                </tr>
                <%--分鐘列表--%>
                <tr>
                    <td style="border: 1px solid; background: white">
                        <table style="width: 100%;">
                            <tr>
                                <td id="BMin0" style="text-align: center">0</td>
                                <td id="BMin1" style="text-align: center">1</td>
                                <td id="BMin2" style="text-align: center">2</td>
                                <td id="BMin3" style="text-align: center">3</td>
                                <td id="BMin4" style="text-align: center">4</td>
                                <td id="BMin5" style="text-align: center">5</td>
                            </tr>
                        </table>
                        <table style="width: 100%;">
                            <tr>
                                <td id="SMin0" style="text-align: center">0</td>
                                <td id="SMin1" style="text-align: center">1</td>
                                <td id="SMin2" style="text-align: center">2</td>
                                <td id="SMin3" style="text-align: center">3</td>
                                <td id="SMin4" style="text-align: center">4</td>
                                <td id="SMin5" style="text-align: center">5</td>
                                <td id="SMin6" style="text-align: center">6</td>
                                <td id="SMin7" style="text-align: center">7</td>
                                <td id="SMin8" style="text-align: center">8</td>
                                <td id="SMin9" style="text-align: center">9</td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="width: 100%; background: white">
                            <%--快速列--%>
                            <tr>
                                <td style="text-align: center">
                                    <asp:Label ID="LaFirst" runat="server" Font-Size="12px" Font-Underline="true" Text="最初"></asp:Label>
                                    <asp:Label ID="Label1" runat="server" Font-Size="10px" Text="┊"></asp:Label>
                                    <asp:Label ID="La00Min" runat="server" Font-Size="12px" Font-Underline="true" Text="00分"></asp:Label>
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
                    <td style="border: 1px solid; background: white">
                        <table style="width: 100%;">
                            <tr>
                                <td style="white-space: nowrap">
                                    <asp:Label ID="LaShowS" runat="server" Font-Size="16px" Text="時間["></asp:Label>
                                    <asp:Label ID="LaHour" runat="server" Font-Size="18px" Text="00"></asp:Label>
                                    <asp:Label ID="LabelMark" runat="server" Font-Size="18px" Text=":"></asp:Label>
                                    <asp:Label ID="LaBMin" runat="server" Font-Size="18px" Text="0"></asp:Label>
                                    <asp:Label ID="LaSMin" runat="server" Font-Size="18px" Text="0"></asp:Label>
                                    <asp:Label ID="LaShowE" runat="server" Font-Size="16px" Text="]"></asp:Label>
                                </td>
                                <td style="text-align: right">
                                    <asp:Button ID="BSubmit" runat="server" Font-Size="12px" Text="確定" CssClass="IconOk" />
                                    <asp:Button ID="BCancel" runat="server" Font-Size="12px" Text="取消" CssClass="IconCancel" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
