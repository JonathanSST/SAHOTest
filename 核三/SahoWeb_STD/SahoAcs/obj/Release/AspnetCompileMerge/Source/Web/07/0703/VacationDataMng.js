$(document).ready(function () {
    $("#QueryButton").click(function () {
        QueryData();
    });
    $("#BuildButton").click(function () {
        BuildData();
    });
    $("#popB_Add").click(function () {
        AddExcute();
    });
    $("#popB_Edit").click(function () {
        EditExcute();
    });
    $("#popB_Delete").click(function () {
        DeleteExcute();
    });
});


// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    var popB_Add = $("#popB_Add")[0];
    var popB_Edit = $("#popB_Edit")[0];
    var popB_Delete = $("#popB_Delete")[0];
    popB_Delete.style.display = "none"
    popB_Add.style.display = "none";
    popB_Edit.style.display = "none";
    DeleteLableText.text = "";
    switch (sMode) {
        case 'Add':
            $('#ParaExtDiv1').find('input[type=text]').val('');
            $('#ParaExtDiv1').find('input[type=text]').prop('disabled', false);
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            popB_Add.style.display = "none";            
            popB_Edit.style.display = "inline";
            ChangeText(DeleteLableText, '');
            $('#ParaExtDiv1').find('input[type=text]').prop('disabled', false);
            break;
        case 'Delete':
            $('#ParaExtDiv1').find('input[type=text]').prop('disabled', true);
            popB_Delete.style.display = "inline";
            break;
        case '':
            //popInput_HEDesc.disabled = false;
            //popLabel_YYYY.disabled = false;
            //popInput_MM.disabled = false;
            //popInput_DD.disabled = false;
            //popInput_HEDesc.value = '';
            
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            break;
    }
}

function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;

    if (str != '') {
        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                AddButton.disabled = false;
            }

            if (data[i] == 'Edit') {
                EditButton.disabled = false;
            }

            if (data[i] == 'Del') {
                DeleteButton.disabled = false;
            }
        }
    }
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, "ContentPlaceHolder1_tablePanel", 'MainGridView', 0, SelectNowNo.value);
}

function ShowPage(index) {
    $("#PageIndex").val(index);
    QueryData();
}



//查詢整年度的休假
function QueryData() {
    $('[name*="SelectYear"]').val($('[name*="Input_Year"]').val());
    //console.log($('[name*="SelectYear"]').val());
    $.ajax({
        url: location.href,
        type: 'POST',
        data: {"Index":$("#PageIndex").val(), "Year":$('[name*="Input_Year"]').val(), "PageEvent":"Query"},
        //async: false,
        success: function (data) {            
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find('#ContentPlaceHolder1_UpdatePanel1').html());
            if (popPageQry) {
                popPageQry = false;
                GridSelect();
            }
        },
        error: function (result) {
            alert('發生錯誤' + result);
        },
       cache: false,
        //contentType: false,
        //processData: false
    });
}


//將資料帶回畫面UI
function SetEditUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        SelectNowNo.value = DataArray[1];
        ChangeText(popLabel_YYYY, DataArray[1].substr(0, 4) + ' ');
        popInput_HEDesc.value = DataArray[2];
        for (var i = 0; i < popInput_MM.children.length; i++) {
            if (popInput_MM[i].value == DataArray[1].substr(5, 2)) {
                popInput_MM.selectedIndex = i;
                break;
            }
        }
        for (var i = 0; i < popInput_DD.children.length; i++) {
            if (popInput_DD[i].value == DataArray[1].substr(8, 2)) {
                popInput_DD.selectedIndex = i;
                break;
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//將資料帶回畫面UI
function SetDeleteUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger2.click();
        popInput_YMD.value = DataArray[1];
        popInput_Desc.value = DataArray[2];
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

function SelectState() {
    SelectYear.value = Input_Year.options[Input_Year.selectedIndex].value;
    SelectValue.value = '';
    __doPostBack(QueryButton.id, 'NewQuery');
}

function BuildData() {
    SelectYear.value = $('[name*="Input_Year"]').val();
    SelectValue.value = '';
    $.ajax({
        url: location.href,
        type: 'POST',
        data: { "Year": $('[name*="Input_Year"]').val(), "PageEvent": "Build" },
        //async: false,
        success: function (data) {
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find('#ContentPlaceHolder1_UpdatePanel1').html());
        },
        error: function (result) {
            alert('發生錯誤' + result);
        }
        // cache: false,
        //contentType: false,
        //processData: false
    });
    
    //__doPostBack(BulidButton.id, 'Buliding');
}



// 呼叫新增視窗
function CallAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('Add');
    ShowOver('1');
    //while (popInput_MM.length > 0) {
    //    popInput_MM.remove(0);
    //}
    //while (popInput_DD.length > 0) {
    //    popInput_DD.remove(0);
    //}
    //$.when(ajax1, ajax2).done(function (id1, id2) {
    //    SetMMData(id1[0]['d']); //設定月
    //    SetDDData(id2[0]['d']); //設定日            
    //    ShowOver("1");
    //    SetMode('Add');
    //    ChangeText(popLabel_YYYY, SelectYear.value + ' ');
    //}).fail(function () { alert('error'); });
}

//執行新增動作
function AddExcute(UserID) {        
    SelectNowNo.value = $('[name*="popInput_VNo"]').val();
    $.ajax({
        url: location.href,
        type: "POST",
        data: {
            "VNo": $('[name*="popInput_VNo"]').val(), "VName": $('[name*="popInput_VName"]').val(), "PageEvent": "Add"
        },
        dataType:'json',
        success: function (data) {
            console.log(data);
            if (data.result) {                
                DoCancel('1');
                popPageQry = true;
                ShowPage('1');
                
            } else {
                alert(data.message);
            }
        }
    });
}

// 呼叫編輯視窗
function CallEdit(Title, Msg) {
    ChangeText(L_popName1, Title);
    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else {
        SetMode('Edit');
        $.ajax({
            url: location.href,
            type: "POST",
            data: {"PageEvent":"Edit", "VID":SelectValue.value },
            success: function (data) {
                var vdata = JSON.parse(data).cdata;
                ShowOver('1');
                $('[name*="popInput_VNo"]').val(vdata.VNo);
                $('[name*="popInput_VName"]').val(vdata.VName);
            }//end success function
        });
    }
}


var popPageQry = false;
//執行更新動作
function EditExcute(UserID) {
    //var HolidayData = $('[name*="Input_Year"]').val() + "-" + $('[name*="popInput_MM"]').val() + "-" + $('[name*="popInput_DD"]').val();
    //SelectNowNo.value = HolidayData;
    $.ajax({
        url: location.href,
        type: "POST",
        data: {
            "VID": SelectValue.value, "VNo": $('[name*="popInput_VNo"]').val(), "VName": $('[name*="popInput_VName"]').val(), "PageEvent":"Update"
        },
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (data.result) {
                DoCancel('1');
                popPageQry = true;
                ShowPage($("#PageIndex").val());                
            } else {
                alert(data.message);
            }
        }
    });
}

// 呼叫刪除視窗
function CallDelete(Title, DelLabel, Msg) {
    if (IsEmpty(SelectValue.value)) {
        NotSelectForDelete(Msg);
    }
    else {      
        $.ajax({
            url: location.href,
            type: "POST",
            data: { "PageEvent": "Edit", "VID": SelectValue.value },
            success: function (data) {
                var vdata = JSON.parse(data).cdata;
                SetMode('Delete');
                ShowOver('1');
                $('[name*="popInput_VNo"]').val(vdata.VNo);
                $('[name*="popInput_VName"]').val(vdata.VName);
                ChangeText(DeleteLableText, DelLabel);
                ChangeText(L_popName1, Title);
            }//end success function
        });
        
    }
}

//執行刪除動作
function DeleteExcute(UserID) {
    $.ajax({
        url: location.href,
        type: "POST",
        data: { 'VID': SelectValue.value, "PageEvent": "Delete" },
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (data.result) {
                DoCancel('1');
                ShowPage($("#PageIndex").val());
            } else {
                alert(data.message);
            }
        }
    });
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            CancelTrigger1.click();
            CancelTrigger2.click();
            alert(sRet.message);
            break;
        default:
            switch (sRet.act) {
                case "Add":
                    var data = sRet.message.split("|");
                    SelectNowNo.value = data[0];
                    SelectValue.value = data[1];
                    break;
                case "Edit":
                    //SelectNowNo.value = popLabel_YYYY.value + '-' + popInput_MM.value + '-' + popInput_DD.value;
                    break;
                case "Delete":
                    SelectNowNo.value = '';
                    SelectValue.value = '';
                    break;
            }
            SetMode('');
            __doPostBack(UpdatePanel1.id, 'popPagePost');
            CancelTrigger1.click();
            CancelTrigger2.click();
            break;
    }

}

function ShowOver(val_key) {
    $("#popOverlay" + val_key).css("display", "block");
    $("#ParaExtDiv" + val_key).css("display", "block");
    $("#popOverlay" + val_key).width($(document).width());
    //$("#popOverlay" + val_key).height($("#ParaExtDiv" + val_key).height() + 130);
    $("#ParaExtDiv" + val_key).css("left", ($(document).width() - $("#ParaExtDiv" + val_key).width()) / 2);
    $("#ParaExtDiv" + val_key).css("top", $(document).scrollTop() + 20);
    //$("#ParaExtDiv" + val_key).css("top", 20);
}

function DoCancel(val_key) {
    $("#popOverlay" + val_key).css("display", "none");
    $("#ParaExtDiv" + val_key).css("display", "none");
}