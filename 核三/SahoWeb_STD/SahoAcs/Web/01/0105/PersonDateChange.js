function Default(dt) {
    Input_Time.value = dt;
    $('[name*="popCalendar1"]').val(dt);
    $('[name*="popCalendar2"]').val(dt);
}

//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = '';
    if (str == 'Add') {
        $("#popB_PsnList2").append($("#popB_PsnList1 option:selected"));
        $("#popB_PsnList1 option:selected").remove();
    }
    else if (str == 'Del') {        
        $("#popB_PsnList1").append($("#popB_PsnList2 option:selected"));
        $("#popB_PsnList2 option:selected").remove();
    }
}

function LoadPsnDataList() {
    DoCancel('1');
    if ($('[name*="DataList"] option').length > 0) {
        $('[name*="DataList"] option').remove();
    }
    if ($("#popB_PsnList2 option").length > 0) {
        $('[name*="DataList"]').append($("#popB_PsnList2 option").clone());
    }
}


function QueryPsnData() {
    let poptype = $('#ContentPlaceHolder1_DropDownList1 option:selected').val()
    let popsettime = $('[id*="ContentPlaceHolder1_popCalendar_CalendarTextBox"]').val()
    //console.log(popsettime);
    //console.log(poptype)
   // console.log(popsettime)
    if (poptype == "" || popsettime == "") {
        alert("請選擇時間")
    } else {
        $('#popB_PsnList2').empty()
        $('#ContentPlaceHolder1_DataList').empty()
        $.ajax({
            type: "POST",
            url: window.location,
            data: { 'DoAction': 'Query', 'KeyName': $("#Input_TxtQuery").val(), "UserID": $('[name*="UserId"]').val(), 'PsnTimeS': $('[name*="popCalendar1"]').val(), 'PsnTimeE': $('[name*="popCalendar2"]').val(), 'ddlType': $('[name*="ddlType"]').val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.message != "") {
                    alert(data.message);
                    return false;
                }
                if (data.result.length > 0) {
                    $('#popB_PsnList1 option').remove();
                    var arr = Array.prototype.slice.call(data.result);
                    arr.forEach(function (ele) {
                        var state = 0;
                        $("#popB_PsnList2 option").each(function () {
                            if ($(this).val() == ele.PsnID) {
                                state += 1;
                                return false;
                            }
                        });
                        if (state == 0) {
                            var o = new Option('[工號 : ' + ele.PsnNo + '] ' + ele.PsnName, ele.PsnID);
                            $('#popB_PsnList1').append(o);
                        }
                    });
                } else {
                    alert('系統查無所需資料');
                }
            }
        });      
    }

}

function ExecProcData()
{        
    //PageMethods.ExecProcData(psndata, timetype, datetime, hUserId.value, ExecProcMsg, onPageMethodsError);
    var values = $.map($('[name*="DataList"] option'), function (ele) {
        return ele.value;
    });
    console.log(values);
    //console.log($('[name*="Input_Time_CalendarTextBox"]').val());
    
    Block();
    $.ajax({
        type: "POST",
        url: window.location,
        data: { 'DoAction': 'Exec', 'PsnList': values.join(','), 'PsnDate': $('[name*="Input_Time"]').val(), 'ddlType': $('[name*="ddlType"]').val() },
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            if (data.message != "") {
                var datalist = data.message.split('|');
                for (var i = 0; i < datalist.length; i++) {
                    $('[name*="List_Msg"]').append(new Option(datalist[i], 'none'));                    
                }
                $('[name*="List_Msg"] option').eq($('[name*="List_Msg"] option').length - 1).prop('selected', true);
            }
        }
    });
    
}

function ExecProcMsg(data) {
    if (data.length > 0) {
        if (data[0] != 'Saho_SysErrorMassage') {
            var datalist = data[1].split('|');
            for (var i = 0; i < datalist.length; i++) {
                List_Msg.add(new Option(datalist[i], 'none'));
                List_Msg.selectedIndex = List_Msg.length - 1;
            }
            List_Msg.add(new Option(data[0], 'none'));
            List_Msg.selectedIndex = List_Msg.length - 1;
        }
        else
            alert(data[1]);
    }
}




function ShowOver(val_key) {
    $("#popOverlay" + val_key).css("display", "block");
    $("#ParaExtDiv" + val_key).css("display", "block");
    $("#popOverlay" + val_key).width($(document).width());
    //$("#popOverlay" + val_key).height($("#ParaExtDiv" + val_key).height() + 130);
    $("#ParaExtDiv" + val_key).css("left", ($(document).width() - $("#ParaExtDiv" + val_key).width()) / 2);
    $("#ParaExtDiv" + val_key).css("top", $(document).scrollTop() + 20);
}


function DoCancel(val_key) {
    //$("#ParaExtDiv"+val_key).remove();
    $("#popOverlay" + val_key).css("display", "none");
    $("#ParaExtDiv" + val_key).css("display", "none");
}