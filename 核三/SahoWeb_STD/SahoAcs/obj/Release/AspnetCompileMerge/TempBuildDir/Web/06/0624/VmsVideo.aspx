<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VmsVideo.aspx.cs" Inherits="SahoAcs.VmsVideo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="DivContent" style="text-align: center">
            <input type="button" value="-10s" onclick="DelSec(10000)" id="BtnDelSec10" class="IconLeft"/>
            <input type="button" value="-3s" onclick="DelSec(3000)" id="BtnDelSec" class="IconLeft"/>
            <%--<input type="button" value="Stop" onclick="Stop()" id="BtnStop" class="IconPassword" />--%>
            <span id="lblNowTime"></span>
            <input type="button" value="+3s" onclick="AddSec(3000)" id="BtnAddSec" class="IconRight" />
            <input type="button" value="+10s" onclick="AddSec(10000)" id="BtnAddSec10" class="IconRight" />
            <input type="hidden" value="<%=Request["timestamp"] %>" id="timestamp" />
            <input type="hidden" value="<%=Request["nowtime"] %>" id="nowtime" />
            <input type="hidden" value="<%=Request["vmshost"] %>" id="vmshost" />
            <input type="hidden" value="<%=Request["device"] %>" id="device" />
            <input type="hidden" value="<%=this.VmsPre %>" id="VmsPre" />            
            <input type="hidden" value="<%=this.VmsNext %>" id="VmsNext" />            
            <table>
                <tr>
                    <td>
                        <canvas id="canvas" width="600" height="480"></canvas>
                    </td>
                </tr>
            </table>
        </div>
            <script type="text/javascript">

        var vmshost = $("#vmshost").val();
        var device = $("#device").val();

        function DelSec(val) {
            done = false;
            $("#BtnStop").val("Start");
            start_date = new Date();
            video_url = vmshost+'/archive-video-frame.dyn?device='+device+'&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
            tmid = setTimeout(draw, 45);
            ts = ts - val;
            img = new Image();
            img.src = vmshost+'/archive-video-frame.dyn?device='+device+'&time=' + ts.toString()
                + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
            tmid = setTimeout(draw, 1000);
            done = true;
            $("#BtnStop").val("Stop");
        }


        function AddSec(val) {
            done = false;
            $("#BtnStop").val("Start");
            start_date = new Date();
            video_url = vmshost+'/archive-video-frame.dyn?device='+device+'&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
            tmid = setTimeout(draw, 45);
            ts = ts + val;
            img = new Image();
            img.src = vmshost+'/archive-video-frame.dyn?device='+device+'&time=' + ts.toString()
                + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
            tmid = setTimeout(draw, 1000);
            done = true;
            $("#BtnStop").val("Stop");
        }


        function SetArchive(times) {
            start_date = new Date();
            ts = ts + times;
            if (img) {

            } else {
                img = new Image();
                img.src = vmshost+'/archive-video-frame.dyn?device='+device+'&time=' + ts.toString()
                    + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
            }
            done = true;
            tmid = setTimeout(draw, 1000);
            $("#BtnStop").val("Stop");
        }


        function Stop() {
            if ($("#BtnStop").val() == "Stop") {
                done = false;
                $("#BtnStop").val("Start");
                start_date = new Date();
                video_url = vmshost+'/archive-video-frame.dyn?device='+device+'&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
                tmid = setTimeout(draw, 100);
            } else {
                start_date = new Date();
                if (img) {

                } else {
                    img = new Image();
                    img.src = vmshost+'/archive-video-frame.dyn?device='+device+'&time=' + ts
                        + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
                }
                tmid = setTimeout(draw, 1000);
                done = true;
                $("#BtnStop").val("Stop");
            }
        }


        var i = 0;
        var ts = parseInt($("#timestamp").val()) - parseInt($("#VmsPre").val()) * 1000;
        var tsmove = ts;
        var tsend = parseInt($("#timestamp").val()) + parseInt($("#VmsNext").val()) * 1000;
        var img = new Image();
        var done = true;
        var start_date = new Date();
        var video_url = vmshost+'/archive-video-frame.dyn?device='+device+'&time=' + ts
            + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
        img.src = video_url;

        function draw() {
            if (tsmove > tsend) {
                $("#popOverlay").remove();
                $("#ParaExtDiv").remove();
            }
            tsmove += 15;
            getCookie("X-Br-Archive-Time");
            if (done) {
                start_date = new Date();                               
                img.src = vmshost + '/archive-video-frame.dyn?device=' + device + '&time='
                    + ts + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&ts=' + start_date.getTime();
                img.onload = function () {
                    $("#lblNowTime").text(new Date(ts));
                    LoadCanvas(this);
                };
                img.onerror = function (ev) {                                // Error callback
                    console.log("Error==>" + ts);
                    tsend += 100;
                    //done = false;
                    //video_url = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=' + ts
                    //+ '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
                    //img.src = video_url;
                    tmId = setTimeout(draw, 100);
                    //done = true;
                };
            } else {
                $(img).unbind();
                delete img;
                img = null;
            }
            //setTimeout(draw, 45);
        }
        var tmid = setTimeout(draw, 1000);

        function getCookie(cname) {
            console.log(document.cookie);   
        }

        function LoadCanvas(pimg) {
            var ctx = document.getElementById('canvas').getContext('2d');
            ctx.drawImage(pimg, 0, 0, 600, 480);
            tmid = setTimeout(draw, 15);            
        }

    </script>
    </form>
</body>
</html>
