function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            break;
        case 'Edit':
            /*
            popInput_Class.disabled = true;
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            popInput_Value.disabled = false;
            popInput_Type.disabled = true;
            popInput_Desc.disabled = true;            
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            */
            break;
        case 'Delete':            
            break;
        case '':            
            break;
    }
}

// 呼叫編輯視窗
function CallEdit(Msg) {        
    if (IsEmpty($('#hSelectValue').val())){
        NotSelectForEdit(Msg);
    }else {
        //SetMode('Edit');        
        //PopupTrigger1.click();
        //PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
        $.ajax({
            type: "POST",
            url: window.location,
            data: { 'DoAction': 'Edit', 'ParaID': $("#hSelectValue").val() },
            dataType: "json",
            success: function (data) {
                SetUI(data);
                //SetMode('Edit');
                console.log(data);
                ShowOver('1');
            }
        });
    }
}

// 將資料帶回畫面UI
function SetUI(data) {
    $("#ContentPlaceHolder1_SelectValue").val(data.resp.ParaNo);
    $("#popInput_Class").val(data.resp.ParaClass);
    $("#popInput_No").val(data.resp.ParaNo);
    $("#popInput_Name").val(data.resp.ParaName);
    $("#popInput_Value").val(data.resp.ParaValue);
    $("#popInput_Type").val(data.resp.ParaType);
    $("#popInput_Desc").val(data.resp.ParaDesc);
}


    function Sort(name, obj) {
        $("#SortName").val(name);
        var hiddenField = $('#SortType'),
           val = hiddenField.val();
        var SpanHtml = "<span id='SortSpan'></span>";
        $("#SortSpan").remove();
        hiddenField.val(val === "ASC" ? "DESC" : "ASC");
        $(obj).append(SpanHtml);
        if ($("#SortType").val() == "ASC") {
            $("#SortSpan").text("▲");
        } else {
            $("#SortSpan").text("▼");
        }
        SetQuery();
    }


function SetQuery() {    
    $.ajax({
        type: "POST",
        url: location.href,
        //dataType: 'json',
        data: {'SortType':$("#SortType").val(),'SortName':$("#SortName").val(),'ParaClass':$("#ParaClass").val(),"DoAction":"Query"},
        success: function (data) {
            //DoCancel();
            $("#tablePanel").html($(data).find("#tablePanel").html());
            //BindEvent();
            //$.unblockUI();
        }
    });
}


// 執行編輯動作
function EditExcute() {
    //PageMethods.Update(popInput_Class.value, popInput_No.value, popInput_Value.value, Excutecomplete, onPageMethodsError);
    if ($('#popInput_Value').val() == "") {
        alert($("#popInput_Value").attr('FIELD_NAME') + "必須輸入");
    } else {
        $.ajax({
            type: "POST",
            url: location.href,
            dataType: 'json',
            data: {'ParaID':$('#hSelectValue').val(),'ParaValue':$('#popInput_Value').val(),'DoAction':'Update'},
            success: function (data) {
                //console.log(data);
                if (data.result) {
                    SetQuery();
                    GridSelect();
                    DoCancel('1');
                } else {
                    alert(data.message);
                }
            }
        });
    }
}

// GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    //console.log("SelectValue:"+SelectValue.value);
    GridGoToRow(1, 'tablePanel', 'ContentPlaceHolder1_MainGridView', 1, $("#ContentPlaceHolder1_SelectValue").val());
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
