<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="MainInfoFrm.aspx.cs" Inherits="SahoAcs.MainInfoFrm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <script type="text/javascript">
        $(document).ready(function () {
            $.post("AreaInfo.aspx", {
            },
                function (data) {
                    $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:30000; overflow:hidden;'
                        + ' -webkit-transform: translate3d(0,0,0);"></div>');
                    $("#popOverlay2").css("background", "#000");
                    $("#popOverlay2").css("opacity", "0.5");
                    $("#popOverlay2").width($(document).width());
                    $("#popOverlay2").height($(document).height());
                    $(document.body).append('<div id="ParaExtDiv2" style="position:absolute;z-index:30001;background-color:white;'
                        + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                    $("#ParaExtDiv2").html(data);
                    $("#ParaExtDiv2").css("left", 90);
                    $(document).scrollTop(0);
                    $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);
                });
        });
    </script>
</asp:Content>
