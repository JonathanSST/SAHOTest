<%@ Import Namespace="SahoAcs.DBClass" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IDLogin.aspx.cs" Inherits="SahoAcs.IDLogin" EnableViewStateMac="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Manage System(SMS)</title>
    <link href="Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="Css/Style.css" rel="stylesheet" type="text/css" />
    <link href="Css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%=Pub.JqueyNowVer %>"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>

    <style type="text/css">
        .BgAlert
        {
	        width: 750px;
	        height: 684px;
            background: url(../Img/bg_login.png) left 50px no-repeat;
	        margin: 0 auto 0 auto;
	        font-family: 微軟正黑體,Microsoft JhengHei,新細明體,PMingLiU;
        }
    </style>

    <script type="text/javascript">
        $(function () {
            $("#radio").buttonset();
            //$("#lblTitle").text('<%=System.Configuration.ConfigurationManager.AppSettings["TitleName"].ToString()%>');
            
        });

        $("#bt_Login").button({
            text: false,
            icons: {
                primary: "ui-icon-triangle-1-s"
            }
        });

        $(document).ready(function () {
            const now = new Date();
            now.getHours(); // 取得本地的小時（0~23）            
            console.log(now.getTimezoneOffset());
            $('[name*="HiddenTimeOffset"]').val(now.getTimezoneOffset());
            if (navigator.userAgent.indexOf('MSIE') != -1)
                var detectIEregexp = /MSIE (\d+\.\d+);/ //test for MSIE x.x
            else // if no "MSIE" string in userAgent
                var detectIEregexp = /Trident.*rv[ :]*(\d+\.\d+)/ //test for rv:x.x or rv x.x where Trident string exists

            if (detectIEregexp.test(navigator.userAgent)) { //if some form of IE
                var ieversion = new Number(RegExp.$1) // capture x.x portion and store as a number
                if (ieversion < 10) {                    
                    $(".BgLogin").hide();
                    $(".BgAlert").show();
                } else {
                    $(".BgLogin").show();
                    $(".BgAlert").hide();
                }
            }
            else {

            }
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="text-align: left; font-size: 20px;">
            <asp:HiddenField ID="HiddenTimeOffset" runat="server" />
            <asp:LinkButton ID="hlkTW" runat="server" OnClick="hlkTW_Click" ForeColor="White">繁體中文</asp:LinkButton>
            |
            <asp:LinkButton ID="hlkUS" runat="server" OnClick="hlkUS_Click" ForeColor="White">English</asp:LinkButton>
        </div>
        <div hidden="hidden" class="BgAlert" style="margin-top:400px">
            <div style="color: cornsilk; font-size: 20px; text-align: center;">
                <%=Resources.Resource.lblBrowserAlarm %>
            </div>
        </div>
        <div class="BgLogin">
            <h1>
                <asp:Label ID="lblTitle" runat="server" Text="<%$ Resources:Resource, lblTitle %>"></asp:Label>
                <%if (Version != "")
                    { %>
                <br /><span style="font-size:18pt"><%=this.Version %></span>
                <%} %>
            </h1>            
            <asp:Panel ID="Panel1" runat="server">
                <ul>
                    <li>
                        <asp:Label ID="lblUid" runat="server" Text="<%$ Resources:Resource, lblUid %>"></asp:Label>：
                        <asp:TextBox ID="txtUid" runat="server" Width="200px"></asp:TextBox>
                    </li>
                    <li>
                        <asp:Label ID="lblPwd" runat="server" Text="<%$ Resources:Resource, lblPwd %>"></asp:Label>：
                        <asp:TextBox AutoCompleteType="Disabled" ID="txtPwd" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                    </li>
                </ul>
                <ul>
                    <li>
                    </li>
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Size="16" BackColor="White"></asp:Label>
                </ul>
                <h2>
                    <asp:Button ID="btnLogin" runat="server" Text="<%$ Resources:Resource, btnLogin %>"  CssClass="BtnLogin" />
                    <input type="hidden" id="DoAction" name="DoAction" value="Login" />
                </h2>
                <div style="color: cornsilk; font-size: 20px; text-align: center;">                    
                    <%=Resources.Resource.lblBrowserAlarm %>
                </div>                
            </asp:Panel>
        </div>        
        <%if (DongleVaries.GetDateAlive())
            { %>
        <script type="text/javascript">
            $.get("AliveDate.aspx").success(function (data) {                
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
                $("#BtnAlive").focus();
                $("#BtnAlive").click(function () {
                    $("#popOverlay").remove();
                    $("#ParaExtDiv").remove();
                    $('input[name*="txtUid"]').focus();
                });
            });
            
        </script>
        <%} %>
    </form>
</body>
</html>

