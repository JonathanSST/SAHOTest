<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoexLogin.aspx.cs" Inherits="SahoAcs.MoexLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="Css/bootstrap-dialog.css" rel="stylesheet" />
    <link href="Css/bootstrap-theme.css" rel="stylesheet" />
    <link href="Css/bootstrap.css" rel="stylesheet" />
    <link href="Css/Reset.css" rel="stylesheet" type="text/css" />
    <%--<link href="Css/Style.css" rel="stylesheet" type="text/css" />--%>
    <link href="Css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%=Pub.JqueyNowVer %>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="nav-scroller bg-white shadow-sm">
            <nav class="nav nav-underline">
                <%foreach(System.Data.DataRow r in this.DataParaList.Rows) { %>
                <a target="_blank" class="nav-link" href="<%=r["ParaValue"].ToString() %>"><%=r["ParaName"].ToString() %></a>
                <%} %>
            </nav>
        </div>
        <div class="container">
            <div class="row">
                <div class="col-md-4 col-md-offset-4">
                    <div class="panel panel-default" style="margin-top: 25%">
                        <div class="panel-heading">
                            <h3 class="panel-title">系統登入</h3>
                        </div>
                        <div class="panel-body">
                            <fieldset>
                                <div class="form-group">
                                    <input class="form-control" placeholder="帳號" name="account" id="account"
                                        type="text" autofocus />
                                </div>
                                <div class="form-group">
                                    <input class="form-control" placeholder="密碼" id="password"
                                        name="password" type="password" value="" />
                                </div>
                                <!-- Change this to a button or input when using this as a form -->
                                <button type="submit" name="Login" class="btn btn-lg btn-primary btn-block" oncontextmenu="javascript: alert('登入'); return false;">登入</button>
                                <div class="form-group">
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
