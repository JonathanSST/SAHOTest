


$(document).ready(function () {
    $("#DataGridList").find("tr").each(function () {
        //console.log($(this).find("td:eq(3)").find("#CardType").val());
        var card_type = $(this).find("td:eq(3)").find("#CardType").val();
        $(this).find("td:eq(3)").find("#BtnAdd").click(function () {
            SetCardAuth(card_type);
        });
    });
});


function SetCardAuth(card_type) {
    //$('#ProcDiv').css('display', 'block');
    Block();
    $.ajax({
        type: "POST",
        url: "ResetCardAuth3.aspx",
        data: { 'CardType': card_type, "PageEvent": "Reset" },
        dataType:'json',
        success: function (data) {
            //console.log(data);
            var obj = $('[name*="CardType"][value="' + card_type + '"]');
            console.log($(obj).val());
            var that = JsFunFindParent($(obj).first(), "TR");
            //console.log($(that).find('td:eq(1)').html());
            $(that).find('td:eq(2)').html(data.ready_count);
            alert('已送出 ' + data.proc_count + ' 筆重整記錄');
            //$('#ProcDiv').css('display', 'none');
            $.unblockUI();
        },
        fail: function () {
            $.unblockUI();
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

