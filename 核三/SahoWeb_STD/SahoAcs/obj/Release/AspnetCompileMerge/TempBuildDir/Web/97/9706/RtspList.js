var ws;
var pyip = $('#RtspHost').val();
var video_locate = '';
var w_page = null;
$(document).ready(function () {
    $('[name*="BtnOpen"]').click(function () {
        /***
        if (ws != null) {
            ws.close();
        }
        video_locate = $(this).prev('#hUrl').val();
        ws = new WebSocket(pyip);
        ws.onopen = function (msg) {
            console.log('webSocket opened');
        };
        ws.onmessage = function (message) {
            console.log(message);
            //$("#img").attr("src", message.data);
            if (message.data === "VideoClosed") {
                ws.send(video_locate);
            }
        };
        ws.onerror = function (error) {
            console.log('error :' + error.name + error.number);
            alert('連線錯誤，連線未開啟');
        };
        ws.onclose = function () {
            console.log('webSocket closed');
        };
        **/

        if (w_page != null) {
            w_page.close();
        }
        video_locate = $(this).prev('#hUrl').val();
        server_locate = $("#RtspHost").val();
        chk_code = $(this).parent("div").find('#ChkTarget').val();
        w_page = window.open('OpenActiVideo.aspx?RtspVideo=' + video_locate + "&ServerLocate=" + server_locate + "&TargetCode=" + chk_code);

    });
});


function ShowVideo(video_no) {
    console.log(video_no);
}


function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}




var that;





function DoRefresh()
{
    $.ajax({
        type: "POST",
        url: "DeviceList.aspx",
        data: { 'PageEvent': 'Refresh', "admin": $('[name*="EvoUid"]').val(), "password": $('[name*="EvoPwd"]').val(), "host": $('[name*="EvoHost"]').val() },
        dataType: "json",
        success: function (data) {
            $("#SelectEvo option").remove();
            var arr = Array.prototype.slice.call(data.resource);
            arr.forEach(function (ele) {
                console.log(ele);
                var o = new Option(ele, ele);
                $("#SelectEvo").append(o);
            });
            if (data.err_message == "") {
                alert('更新完成!!');
            } else {
                alert(data.err_message);
            }
        }
    });
}