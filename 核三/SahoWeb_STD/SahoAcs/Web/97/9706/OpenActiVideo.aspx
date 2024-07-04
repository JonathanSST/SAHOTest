<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpenActiVideo.aspx.cs" Inherits="SahoAcs.Web._97._9706.OpenActiVideo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../../../Scripts/jquery-3.6.0.js"></script>
  
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <img id="img" src="https://example.com/media/photo.jpg" style="width: 800px; height: 640px" alt="一張圖片" />
            <input type="hidden" id="MyUrl" name="MyUrl" value="<%=this.MyUrl %>" />
            <input type="hidden" id="MyServer" name="MyServer" value="<%=this.ServerLocate %>" />
            <input type="hidden" id="ErrMsg" name="ErrMsg" value="<%=ErrMsg %>" />
        </div>
    </form>
      <script type="text/javascript">
         var ws;
        var pyip = $('#MyServer').val();
          function startWS() {
              if ($("#ErrMsg").val() !== "") {
                  alert($('#ErrMsg').val());
                  window.close();
                  return false;
              }
            console.log('start once again');
            ws = new WebSocket(pyip);
            ws.onopen = function (msg) {
                console.log('webSocket opened');
                 ws.send("OpenVideo," + $('#MyUrl').val());
            };           
            ws.onmessage = function (message) {
                console.log(message);
                $("#img").attr("src", message.data);
            };
            ws.onerror = function (error) {
                console.log('error :' + error.name + error.number);
            };
            ws.onclose = function () {
                console.log('webSocket closed');
                alert('伺服接收工具已經關閉，請重新開啟');
                window.close();
            };
           
            // ws.send("websocket from js");
        }
        startWS();
    </script>
</body>
</html>
