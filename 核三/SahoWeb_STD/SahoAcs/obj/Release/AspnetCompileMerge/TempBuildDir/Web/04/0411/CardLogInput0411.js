//*主要作業畫面：執行「查詢」動作
function SelectState() {
    SelectValue.value = "";
    Block();
    __doPostBack(QueryButton.id, '');
}

// 呼叫編輯視窗
function CallEdit(card_id) {
    $.post("AddLog0411.aspx",
            { DoAction: "Query", card_id: card_id },
            function (data) {
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
                $("#DateArea1").html($("#DateHide").html());
                $("#BtnSave").click(function () {
                    SaveCardLog();
                });
                $("#BtnCancel").click(function () {
                    $("#ParaExtDiv").remove();
                    $("#popOverlay").remove();
                });
            }
        );
}

//將資料帶回畫面UI
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        popInput_PsnID.value = DataArray[0];
        popInput_PsnNo.value = DataArray[1];
        popInput_PsnName.value = DataArray[2];
        popInput_CardNo.value = DataArray[3];
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}




//新增補登記錄
function SaveCardLog() {
    $.post("AddLog0411.aspx",
            { PageEvent: "Save", CardID: $('#CardID').val(), EquID: $('select[name*="EquName"]').val(), LogStatus: $('select[name*="LogStatus"]').val(), CardTime: $('input[name*="CalendarTextBox"]').val() },
             function (data) {                 
                 alert(data);
                 if (data == "新增完成") {
                     $("#ParaExtDiv").remove();
                     $("#popOverlay").remove();
                 }                 
             });
}

//補登記錄是否成功
function FillCardLogExcute(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        CancelTrigger1.click();
        alert(DataArray[1])
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            CancelTrigger1.click();
            alert(sRet.message);
            break;
        default:
            CancelTrigger1.click();
            break;
    }
}

function ChangeMga()
{
    //alert($("#DropMgaList").val());
    $.post("AddLog0411.aspx",
          { PageEvent: "Change", MgaID:$("#DropMgaList").val()},
           function (data) {
               $("#EquNameArea").html($(data).find("#EquNameArea").html());
    });
}