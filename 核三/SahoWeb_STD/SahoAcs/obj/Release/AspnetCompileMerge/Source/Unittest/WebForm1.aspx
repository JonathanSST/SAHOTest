<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="SahoAcs.WebForm1" %>

<!DOCTYPE html>

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
<form action="WebForm1.aspx" runat="server">
    <table>
        <tr>
            <td>
                <input type="text" id="input1" disabled="disabled"/><br />
                <canvas id="canvas" width="800" height="600"></canvas>
            </td>
        </tr>
    </table>
    </form>

<script type="text/javascript">
    
    var i = 0;

    var ts = 1489481995000;
    var img = new Image();
    var start_date = new Date();
    img.src = 'http://192.168.0.151:8080/archive-video-frame.dyn?device={localhost:60554-Media%20Source%5c003}&time=' + ts
        + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts='+start_date.getTime();


    function draw()
    {
        ts += 45;        
        img.src = 'http://192.168.0.151:8080/archive-video-frame.dyn?device={localhost:60554-Media%20Source%5c003}&time='
            + ts + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF';
        img.onload = function () {
            document.getElementById("input1").value = ts;
            var ctx = document.getElementById('canvas').getContext('2d');
            ctx.drawImage(img, 0, 0, 800, 600);
            tmid=setTimeout(draw, 45);
        };
        //setTimeout(draw, 45);
    }
    var tmid=setTimeout(draw, 45);
</script>
    </body>