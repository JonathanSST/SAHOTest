<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="playvideo.aspx.cs" Inherits="SahoAcs.Web._06._0601.playvideo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../../../Scripts/jquery-3.6.0.js"></script>
    <script src="evoapi.js"></script>
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>    
</head>
<body onload="LoadVideo()">
    <form id="form1" runat="server">
        <div>
            <video id="video" controls="controls" autoplay="autoplay" width="900">
            </video>
            <input type="hidden" id="EvoPwd" value="<%=this.EvoPwd %>" />
            <input type="hidden" id="EvoUid" value="<%=this.EvoUid %>" />
            <input type="hidden" id="EvoHost" value="<%=this.EvoHost %>" />
        </div>
    </form>
</body>
</html>
