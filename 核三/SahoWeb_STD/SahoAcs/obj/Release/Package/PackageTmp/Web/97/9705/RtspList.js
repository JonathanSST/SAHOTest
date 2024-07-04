$(document).ready(function () {
    $("#AddButton").click(function () {
        var newrow = $('table[id="TableEmpty"]').find("tbody").last().html();
        $("#ContentPlaceHolder1_GridView2 tbody").append(newrow);
        $("#ContentPlaceHolder1_GridView2 tr:last").find("input[type='text']:eq(0)").focus();
    });
    $("#SaveButton").click(function () {
        SaveEquEvo();
    })
});


function SaveEquEvo() {
    console.log($("#form1").find('input,select').serialize());
    //return false;    
    $.ajax({
        type: "POST",
        url: window.location.url,
        data: $("#form1").find('input,select').serialize(),
        //contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.success === true) {
                alert("更新完成");
                window.location.href = window.location.href;
            }
        },
        fail: function () {
            alert("更新失敗");
        }
    });    
}

function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}


function AddEvoDevice(btnobj)
{
    
    that = JsFunFindParent(btnobj, "TD");
   
    $.ajax({
        type: "POST",
        url: "DeviceList.aspx",
        data:{'EvoNo':$(that).find('#EvoNo').val(),'EvoName':$(that).find('#EvoName').val()},
        success: function (data)
        {
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
            //$("#BtnAlive").focus();
            $("#BtnAlive").click(function () {
                $("#popOverlay").remove();
                $("#ParaExtDiv").remove();                
            });
        }
    });
}


function DoSave() {
    //這裡要將結果帶回去選單
    //console.log($('[name*="DeviceNo"]').val());
    var result = $('[name*="DeviceNo"]').map(function () {
        return $(this).val();
    }).get().join(',');
    console.log(result);
    console.log($(that).find('#EvoName').val());
    $(that).find('#EvoName').val(result);
    $(that).find("#EvoInfo").text(result);
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
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