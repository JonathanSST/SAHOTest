//******* 各公用方法：執行臨時卡建檔作業相關的公用方法 *******


//*各公用方法：自動設置GridView控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    console.log(GridGoToRow(0, "tablePanel", 'MainGridView', 0, $('[name*="hSelectValue"]').val()));
}

//******* 主作業畫面：呼叫臨時卡建檔作業相關的視窗方法 *******

//*主作業畫面：呼叫新增視窗
function CallAdd() {
    ShowOver('1');
    SetMode('Add');
    $('[id*="L_popName1"]').text($('#msgAdd').val());
    //清除UI輸入介面
    $('[id*="DeleteLableText"]').text('');
}

//*主作業畫面：呼叫編輯視窗
function CallEdit() {
    if (IsEmpty($('#hSelectValue').val())) {
        NotSelectForEdit($('#msgNonSelect').val().replace(/\\n/g, '\n'));
    } else {        
        //SetMode('Edit');
        $('[id*="L_popName1"]').text($('#msgEdit').val());
        $.ajax({
            type: "POST",
            url: "TempCardCreate.aspx",
            data: { 'DoAction': 'LoadData', 'CardNo': $("#hSelectValue").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.success) {
                    ShowOver("1");
                    SetMode('Edit');                    
                    $('[id*="DeleteLableText"]').text('');
                    $('[name*="popInput_CardNo"]').val(data.resp[0]);
                    $('[name*="popInput_CardDesc"]').val(data.resp[1]);
                } else {
                    alert(data.resp[1]);
                }
            }
        });
    }
    
}

//*主作業畫面：呼叫刪除視窗
function CallDelete() {
    if (IsEmpty($('#hSelectValue').val())) {
        NotSelectForEdit($('#msgNonDelete').val().replace(/\\n/g, '\n'));
    } else { 
        $.ajax({
            type: "POST",
            url: "TempCardCreate.aspx",
            data: { 'DoAction': 'LoadData', 'CardNo': $("#hSelectValue").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.success) {
                    ShowOver("1");
                    SetMode('Delete');
                    $('[id*="L_popName1"]').text($('#msgDel').val());
                    $('[id*="DeleteLableText"]').text($('#msgChkDelete').val());
                    $('[name*="popInput_CardNo"]').val(data.resp[0]);
                    $('[name*="popInput_CardDesc"]').val(data.resp[1]);
                } else {
                    alert(data.resp[1]);
                }
            }
        });
    }
}

//主作業畫面：輸入模式設定視窗相關的欄位狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            $('#popB_Add').css('display', 'inline');
            $('#popB_Delete').css('display', 'none');
            $('#popB_Edit').css('display', 'none');
            $("#popInput_CardNo").prop("disabled", false);
            $("#popInput_CardNo").val("");
            $("#popInput_CardDesc").val("");
            break;
        case 'Edit':
            $('#popB_Edit').css('display', 'inline');
            $('#popB_Delete').css('display', 'none');
            $('#popB_Add').css('display', 'none');
            $("#popInput_CardNo").prop("disabled", true);
            break;
        case 'Delete':
            $('#popB_Delete').css('display', 'inline');
            $('#popB_Add').css('display', 'none');
            $('#popB_Edit').css('display', 'none');
            $("#popInput_CardNo").prop("disabled", true);
            break;
        case '':
            
            break;
    }
    CheckCurrentCard();
}


//主作業畫面：完成動作設定網頁相關的參數內容

function SetOrder(name, obj) {
    $("#OrderName").val(name);
    var hiddenField = $('#OrderType'),
       val = hiddenField.val();
    var SpanHtml = "<span id='OrderSpan'></span>";
    $("#OrderSpan").remove();
    hiddenField.val(val === "ASC" ? "DESC" : "ASC");
    $(obj).append(SpanHtml);
    if ($("#OrderType").val() == "ASC") {
        $("#OrderSpan").text("▲");
    } else {
        $("#OrderSpan").text("▼");
    }
    SetQuery();
}

function SetQuery() {
    $.ajax({
        type: "POST",
        url: "TempCardCreate.aspx",
        data: { 'DoAction': 'Query', 'query_CardNo': $("#QueryInput_CardNo").val(), 'OrderName': $("#OrderName").val(), 'OrderType': $("#OrderType").val() },
        success: function (data) {
            $("#tablePanel").html($(data).find("#tablePanel").html());
            //$("#SelectValue").val("");
            GridSelect();
            //SetCheckMax();
        }
    });
}


function CheckCurrentCard()
{
    console.log(parseInt($('[name*="hCurrentCard"]').val()));
    console.log(parseInt($('[name*="hMaxCard"]').val()));
    if (parseInt($('[name*="hCurrentCard"]').val()) >= parseInt($('[name*="hMaxCard"]').val())) {
        //AddButton.disabled = true;
        $('#AddButton').prop("disabled", true);
    } else {
        $('#AddButton').prop("disabled", false);        
    }
}


//******* 次作業畫面一：執行臨時卡建檔作業相關的動作方法 *******

//次作業畫面一：執行新增動作
function AddExcute() {
    if (!JsFunBASE_VERTIFY()) {
        return false;
    }        
    $.ajax({
        type: "POST",
        url: "TempCardCreate.aspx",
        data: { 'DoAction': 'Insert', 'CardNo': $("#popInput_CardNo").val(),'CardDesc':$("#popInput_CardDesc").val()},
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.result) {
                DoCancel('1');
                $("#hSelectValue").val(data.message);
                SetQuery();
            } else {
                alert(data.message);
            }
        }
    });    
}

//次作業畫面一：執行編輯動作
function EditExcute() {
    if (!JsFunBASE_VERTIFY()) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "TempCardCreate.aspx",
        data: { 'DoAction': 'Update', 'CardNo': $("#hSelectValue").val(), 'CardDesc': $("#popInput_CardDesc").val() },
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.result) {
                DoCancel('1');
                $("#hSelectValue").val(data.message);
                SetQuery();
            } else {
                alert(data.message);
            }
        }
    });
}

//*次作業畫面一：執行刪除動作
function DeleteExcute() {
    if (!JsFunBASE_VERTIFY()) {
        return false;
    }
    $.ajax({
        type: "POST",
        url: "TempCardCreate.aspx",
        data: { 'DoAction': 'Delete', 'CardNo': $("#hSelectValue").val()},
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.result) {
                DoCancel('1');
                $("#hSelectValue").val('0');
                SetQuery();
            } else {
                alert(data.message);
            }
        }
    });
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