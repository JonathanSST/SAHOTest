<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaInfo.aspx.cs" Inherits="SahoAcs.AreaInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/bootstrap-dialog.css" rel="stylesheet" />
    <link href="../../../Css/bootstrap-theme.css" rel="stylesheet" />
    <link href="../../../Css/bootstrap.css" rel="stylesheet" />
   <%-- <script src="../../../Scripts/jquery-3.6.0.js"></script>--%>
    <style type="text/css">
        .dot {
            height: 135px;
            width: 135px;
            font-size: 40pt;
            border-color: green;
            border-style: solid;
            border-width: 10px;
            border-radius: 50%;
            display: inline-block;
        }

        .panel-green {
            border-color: #5cb85c;
        }

        .panel-heading {
            border-color: #5cb85c;
            color: #fff;
            background-color: #5cb85c;
        }
    </style>
    <script type="text/javascript">  
        $(document).ready(function () {
            //$("#StatuInfo").load("/MainInfoFrm/DemoPanel");
            //SetCountData();
        });


        function SetRefresh() {
            $.post('AreaInfo.aspx', {}, function (data) {
                $("#AreaListInfo").html($(data).find("#AreaListInfo").html());
                $("#AreaCountInfo").html($(data).find("#AreaCountInfo").html());              
                $("#RealPsnList").html($(data).find("#RealPsnList").html());              
                $("#RealCnt").click(function () {
                    CountRealPsnList = true;    
                    $("#RealPsnList").css("display", "block");
                });
                $('#CloseList').click(function () {
                    CountRealPsnList = false;
                     $("#RealPsnList").css("display", "none");
                })
                if (CountRealPsnList) {
                    $("#RealPsnList").css("display", "block");
                }
                //adjustAutoLocation(mainDiv, textObj, 25, 0);
                //SetCountData();
            });
        }


        function SetCountData() {
            var PsnCount = 0;
                $(".panel-body").each(function (index) {
                    console.log($(this).find("#OneCount").text());
                    PsnCount += parseInt($(this).find("#OneCount").text());
                    $("#RealCnt").text(PsnCount);
                });
        }

    </script>
</head>
<body style="background-color: white; width: 95%;height:95%">
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-lg-12 text-right">
                    <button type="button" class="btn-close btn-close-white close" aria-label="Close">X</button>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12 text-center">
                    <h1 id="NowTime">目前時間:
                    <script type="text/javascript">
                        setInterval(function () {
                            var nt = new Date();
                            var datenow = nt.toLocaleDateString() + " " + nt.toLocaleTimeString();
                            console.log(datenow);
                            $("#NowTime").html(datenow);
                            if (nt.getSeconds() % 5 == 0) {
                                SetRefresh();
                            }
                        }, 1000);
                    </script>
                    </h1>
                </div>
                <!-- /.col-lg-12 -->
            </div>
            <br />
            <div class="row">
                <div class="col-lg-4" id="AreaListInfo" style="height:550px; overflow-y:scroll">
                    <%foreach (var o in this.AreaListForTrt)
                        { %>
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="panel panel-green">
                                <div class="panel-heading">
                                    <%=o.CtrlAreaName %>
                                </div>
                                <div class="panel-body">
                                    <p>目前人數 <span id="OneCount"><%=this.PersonExt.Where(i=>Convert.ToString(i.PsnAreaNo).Equals(Convert.ToString(o.CtrlAreaNo))).Count()%>/<%=o.CtrlAreaPsnCount %></span></p>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%} %>
                    <h5>最後更新時間：<%=DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") %></h5>
                </div>
                <div class="col-lg-8">
                    <div class="row" id="AreaCountInfo">
                        <div class="col-lg-4 text-left">
                            <span id="AllCnt" class="dot" style="border-color: green; text-align: center"><%=this.AllPsnCount %></span><br />
                            <h5 style="margin-left: 33px">應到人數</h5>
                        </div>
                        <div class="col-lg-4 text-left">
                            <span id="RealCnt" class="dot" style="border-color: blue; text-align: center"><%=this.AreaCount %></span><br />
                            <h5 style="margin-left: 33px">實到人數</h5>
                        </div>
                        <div class="col-lg-4 text-left">
                            <span id="NonCnt" class="dot" style="border-color: red; text-align: center"><%=this.AllPsnCount-this.AreaCount  %></span><br />
                            <h5 style="margin-left: 33px">未到人數</h5>
                        </div>
                    </div>
                </div>
                <br />
                <div class="col-lg-8" style="display:none; height:400px; overflow-y:scroll" id="RealPsnList">
                    目前實到人員：
                    <%foreach (var area in this.AreaListForTrt)
                        { %>
                    <%if (this.PersonExt.Where(i => Convert.ToString(i.PsnAreaNo).Equals(area.CtrlAreaNo)).Count() > 0)
                        { %>
                    <div class="row">
                        <div class="col-lg-4 text-left">
                            <%=area.CtrlAreaName %>
                        </div>
                    </div>
                    <div class="row">
                        <%foreach (var p in this.PersonExt.Where(i => Convert.ToString(i.PsnAreaNo).Equals(area.CtrlAreaNo)).ToList())
                            { %>
                        <div class="col-lg-4">
                            <%=string.Format("{1}({0})", p.PsnNo, p.PsnName, p.LastTime) %><br />
                            <%=string.Format("{0:yyyy/MM/dd HH:mm:ss}", p.LastTime) %><br /><br />
                        </div>
                        <%} %>
                    </div>
                    <br/>
                    <%} %>
                    <%} %>
                    <br />
                    <div class="row">
                        <div class="col-lg-8 text-left">
                            <button type="button" id="CloseList"
                                class="btn btn-warning btn-xs">
                                關閉
                            </button>
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
        <script src="Scripts/bootstrap.min.js"></script>
        <script type="text/javascript">
            $(".btn-close").click(function () {
                $("#popOverlay2").remove();
                $("#ParaExtDiv2").remove();
            });
        </script>
    </form>
</body>
</html>
