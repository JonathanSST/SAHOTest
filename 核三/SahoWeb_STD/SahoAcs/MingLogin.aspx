<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MingLogin.aspx.cs" Inherits="SahoAcs.MingLogin" Theme="UI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<script type="text/javascript" src="/scripts/jquery-3.6.0.js"></script>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <%--<link href="Css/Style.css" rel="stylesheet" type="text/css" />--%>
    <style type="text/css">
        .IconCancel
        {
	        background: url(../Img/icon_cancel.png) 6px center no-repeat;
        }

        .IconOk
        {
	        background: url(../Img/icon_ok.png) 6px center no-repeat;
        }

    </style>

<%--    <script src="<%=Pub.JqueyNowVer %>" type="text/javascript"></script>--%>
    <script type="text/javascript">

        $(document).ready(function () {
            SetLogin();
        });

        function SetLogin() {
            var data = $("#LoginContent").html();
            $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
              + ' -webkit-transform: translate3d(0,0,0);"></div>');
            $("#popOverlay").css("background", "#000");
            $("#popOverlay").css("opacity", "0.5");
            $("#popOverlay").width($(document).width());
            $("#popOverlay").height($(document).height());
            $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                  + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
            $("#ParaExtDiv").html(data);
            $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
            $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
            $('input[name*="txtUid"]').focus();
            funTabEnter();
        }

        function SetCancel() {
            $("#popOverlay").remove();
            $("#ParaExtDiv").remove();
        }

        function DoLogin() {
            $.ajax({
                type: "POST",
                url: "MingLogin.aspx",
                data: { "PageEvent": "Login", "txtUid": $("#ParaExtDiv").find('input[name*="txtUid"]').val(), "txtPwd": $("#ParaExtDiv").find('input[name*="txtPwd"]').val() },
                dataType: "json",
                success: function (data) {
                    console.log(data.result_message);
                    if (data.result_message != "") {
                        $("#ParaExtDiv").find("#lblMessage").text(data.result_message);
                    }
                    else {
                        window.location = data.redirect_page;
                    }
                },
                fail: function () {
                    alert("登入失敗");
                }
            });
        }

        function funTabEnter() {
            var focusables = $(':input[type=text],input[type=password],select').not($("#LoginContent").find(':input[type=text],input[type=password],select'));
            focusables.keydown(function (e) {
                if (e.keyCode == 13) {
                    var current = focusables.index(this),
                                                         next = focusables.eq(current + 1).length ? focusables.eq(current + 1) : focusables.eq(0);                    
                    next.focus();
                    closeAllCompont();
                }
            });
        }

    </script>

</head>
<body style="background-image: url('Img/ScAcs.jpg'); background-repeat: no-repeat; background-position: top center; margin:0px 0px 0px 0px" >
    <form id="form1" runat="server">
    <table style="width:100%;background-color:#9FBFF5">
            <tr>
                <td style="width:100%"> 
                  <%--<input type="button" value="系統登入" onclick="SetLogin()"/>--%>
                    <asp:Menu ID="SysMenu" runat="server" Orientation="Horizontal" BackColor="#9FBFF5">
                        <StaticSelectedStyle BackColor="#FFFBD6" />
                        <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <DynamicHoverStyle BackColor="#9FBFF5" ForeColor="White" />
                        <DynamicMenuStyle BackColor="#9FBFF5" />
                        <DynamicSelectedStyle BackColor="#FFCC66" />
                        <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
                        <StaticHoverStyle BackColor="#9FBFF5" ForeColor="White" />
                        <Items>
                            <asp:MenuItem Text="系統管理" NavigateUrl="#">
                                <asp:MenuItem Text="登入" NavigateUrl="Javascript:SetLogin()"/>
                            </asp:MenuItem>                            
                        </Items>
                    </asp:Menu>
                </td>                
            </tr>
        </table>
        <div id="LoginContent" style="display:none">
            <table class="form" style="width:100%;text-align:center">
                <tr>
                    <th class="form" style="width:30%;color:#fff">帳號：</th>
                    <td class="form" style="text-align:center">
                        <asp:TextBox ID="txtUid" runat="server" CssClass="not_null"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <th class="form" style="color:#fff">密碼：</th>
                    <td class="form" style="text-align:center">
                        <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" CssClass="not_null"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <%--<asp:Button ID="btnLogin" runat="server" Text="<%$ Resources:Resource, btnLogin %>" OnClick="btnLogin_Click" CssClass="IconOk" />--%>
                        <input type="button" onclick="DoLogin()" value="登入" class="IconOk" />
                        <input type="button" onclick="SetCancel()" value="取消" class="IconCancel"/>
                        <br />
                        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Size="16" BackColor="White"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
