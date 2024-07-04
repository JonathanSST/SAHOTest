//************************************************** 網頁畫面設計(一)相關的JavaScript方法 **************************************************

//主要作業畫面：執行「查詢」動作
function SelectState() {
    hComplexQueryWheresql.value = '';
    __doPostBack(QueryButton.id, '');
}

//主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}

//主要作業畫面：呼叫「複合查詢」視窗
function CallComplexQueryWindow(Title) {
    //ChangeText(L_popName0, Title);

    if (IsEmpty(hSaveComplexQueryData.value))
        SetComplexQueryWindowMode('');
    else
        SetComplexQueryWindowMode('ReadComplexQueryData');

    PopupTrigger0.click();
}

//主要作業畫面：呼叫「檢視」視窗
function CallViewWindow(Title, Msg) {
    //ChangeText(L_popName1, Title);

    if (IsEmpty(hSelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetViewWindowMode('View');

        PopupTrigger1.click();
        PageMethods.LoadViewWindowData(hSelectValue.value, SetViewWindowData, onPageMethodsError);
    }
}

//主要作業畫面：完成動作設定網頁相關的參數內容
function ExcuteComplete(MsgObj) {
    switch (MsgObj.result) {
        case true:
            switch (MsgObj.act) {
                case 'ComplexQuery':
                    hComplexQueryWheresql.value = MsgObj.message;
                    __doPostBack(ComplexQueryButton.id, '');
                    break;
            }

            CancelTrigger0.click();
            break;
        case false:
            alert(MsgObj.message);
            break;
        default:
            break;
    }
}

//主要作業畫面：更改「GridView」控制項所有資料列的勾選狀態
function ChangeAllRowCheckState(HeaderCheckState) {
    hSaveGVHCheckState.value = HeaderCheckState.checked;

    for (var i = 0; i < document.forms[0].length; i++) {
        if (document.forms[0][i].name.split('$')[4] == "RowCheckState") { document.forms[0].elements[i].checked = HeaderCheckState.checked; }
    }

    RefreshAllGVRCheckState(HeaderCheckState.checked);
}

//主要作業畫面：更新「GridView」控制項所有資料列的勾選記錄
function RefreshAllGVRCheckState(CheckState) {
    var GVRCheckStateArray = hSaveGVRCheckStateList.value.split('/');

    if (GVRCheckStateArray.length >= 2) {
        var Key0 = '', Key1 = '';
        var GVRCheckStateList = '/' + hSaveGVRCheckStateList.value + '/';

        if (CheckState) { Key0 = '/0/'; Key1 = '/1/'; } else { Key0 = '/1/'; Key1 = '/0/'; }
        for (var i = 0; i < GVRCheckStateArray.length; i += 2) { GVRCheckStateList = GVRCheckStateList.replace(Key0, Key1); }

        hSaveGVRCheckStateList.value = GVRCheckStateList.substr(1, (GVRCheckStateList.length - 2));
    }
}

//主要作業畫面：更改「GridView」控制項指定資料列的勾選記錄
function ChangeGVRCheckState(EquID) {
    if (hSaveGVRCheckStateList.value.split('/').length >= 2) {
        var Key0 = '/' + EquID + '/0/', Key1 = '/' + EquID + '/1/';
        var GVRCheckStateTemp = '/' + hSaveGVRCheckStateList.value + '/';

        if (GVRCheckStateTemp.search(Key0) <= -1) { Key0 = '/' + EquID + '/1/'; Key1 = '/' + EquID + '/0/'; }
        GVRCheckStateTemp = GVRCheckStateTemp.replace(Key0, Key1);

        hSaveGVRCheckStateList.value = GVRCheckStateTemp.substr(1, (GVRCheckStateTemp.length - 2));
    }

    RefreshGVHCheckState();
}

//主要作業畫面：更改與更新「GridView」控制項標題列碼的勾選記錄及狀態
function RefreshGVHCheckState() {
    hSaveGVHCheckState.value = 'true';

    if (hSaveGVRCheckStateList.value.split('/').length < 2) { hSaveGVHCheckState.value = 'false'; } else if (('/' + hSaveGVRCheckStateList.value + '/').search('/0/') > -1) { hSaveGVHCheckState.value = 'false'; }

    for (var i = 0; i < document.forms[0].length; i++) {
        if (document.forms[0][i].name.split('$')[4] == "HeaderCheckState") {
            if (hSaveGVHCheckState.value == 'true') { document.forms[0].elements[i].checked = true; } else { document.forms[0].elements[i].checked = false; }
            break;
        }
    }
}

//************************************************** 網頁畫面設計(二)相關的JavaScript方法 **************************************************

//複合查詢畫面：儲存「複合查詢」視窗相關的欄位狀態與內容
function SaveComplexQueryData() {
    hSaveComplexQueryData.value  = popInput0_EquNo.value.trim() + ',';
    hSaveComplexQueryData.value += popInput0_EquName.value.trim() + ',';
    hSaveComplexQueryData.value += popInput0_EquEName.value.trim() + ',';
    hSaveComplexQueryData.value += popInput0_IsAndTrt.selectedIndex + ',';
    hSaveComplexQueryData.value += document.getElementById('ContentPlaceHolder1_popInput0_EquModel').selectedIndex + ',';
    hSaveComplexQueryData.value += popInput0_EquClass.value + ',';
    hSaveComplexQueryData.value += popInput0_Floor.value.trim() + ',';
    hSaveComplexQueryData.value += popInput0_Building.value.trim();
}

//複合查詢畫面：設定「複合查詢」視窗不同模式相關的欄位狀態與內容
function SetComplexQueryWindowMode(SetMode) {
    switch (SetMode) {
        case 'ReadComplexQueryData':
            var ComplexQueryData = hSaveComplexQueryData.value.split(',');

            if (ComplexQueryData.length > 0) {
                popInput0_EquNo.value            = ComplexQueryData[0];
                popInput0_EquName.value          = ComplexQueryData[1];
                popInput0_EquEName.value         = ComplexQueryData[2];
                popInput0_IsAndTrt.selectedIndex = ComplexQueryData[3];
                document.getElementById('ContentPlaceHolder1_popInput0_EquModel').selectedIndex = ComplexQueryData[4];
                popInput0_EquClass.value         = ComplexQueryData[5];
                popInput0_Floor.value            = ComplexQueryData[6];
                popInput0_Building.value         = ComplexQueryData[7];
            }
            break;
        case '':
            hSaveComplexQueryData.value = '';

            //Value
            popInput0_EquNo.value            = '';
            popInput0_EquName.value          = '';
            popInput0_EquEName.value         = '';
            popInput0_IsAndTrt.selectedIndex = 0;
            document.getElementById('ContentPlaceHolder1_popInput0_EquModel').selectedIndex = 0;
            popInput0_EquClass.value         = '';
            popInput0_Floor.value            = '';
            popInput0_Building.value         = '';
            //Disabled
            popInput0_EquNo.disabled    = false;
            popInput0_EquName.disabled  = false;
            popInput0_EquEName.disabled = false;
            popInput0_IsAndTrt.disabled = false;
            document.getElementById('ContentPlaceHolder1_popInput0_EquModel').disabled = false;
            popInput0_EquClass.disabled = false;
            popInput0_Floor.disabled    = false;
            popInput0_Building.disabled = false;
            //Display
            popBtn0_Query.style.display           = 'inline';
            //popBtn0_Cancel.style.display          = 'inline';
            //popBtn0_ClearQueryParam.style.display = 'inline';
            break;
    }
}

//複合查詢畫面：執行「複合查詢」動作
function SetComplexQueryWheresql() {
    var qryParam = new Array(8);

    //設定複合查詢相關的參數資料
    qryParam[0] = popInput0_EquNo.value.trim();
    qryParam[1] = popInput0_EquName.value.trim();
    qryParam[2] = popInput0_EquEName.value.trim();
    qryParam[3] = popInput0_IsAndTrt.value.trim();
    qryParam[4] = document.getElementById('ContentPlaceHolder1_popInput0_EquModel').value.trim();
    qryParam[5] = popInput0_EquClass.value;
    qryParam[6] = popInput0_Floor.value.trim();
    qryParam[7] = popInput0_Building.value.trim();

    PageMethods.SetComplexQueryWheresql(qryParam, ExcuteComplete, onPageMethodsError);
}

//************************************************** 網頁畫面設計(三)相關的JavaScript方法 **************************************************

//彈出作業畫面：設定「檢視」視窗不同模式相關的欄位狀態與內容
function SetViewWindowMode(SetMode) {
    switch (SetMode) {
        case 'View':
            //Disabled
            popInput1_EquID.disabled    = true;
            popInput1_EquNo.disabled    = true;
            popInput1_EquName.disabled  = true;
            popInput1_EquEName.disabled = true;
            popInput1_IsAndTrt.disabled = true;
            popInput1_EquModel.disabled = true;
            popInput1_EquClass.disabled = true;
            popInput1_Floor.disabled    = true;
            popInput1_Building.disabled = true;
            //Display
            popBtn1_Exit.style.display = 'inline';
            break;
        case '':
            //Value
            popInput1_EquID.value    = '';
            popInput1_EquNo.value    = '';
            popInput1_EquName.value  = '';
            popInput1_EquEName.value = '';
            popInput1_IsAndTrt.value = '';
            popInput1_EquModel.value = '';
            popInput1_EquClass.value = '';
            popInput1_Floor.value    = '';
            popInput1_Building.value = '';
            //Disabled
            popInput1_EquID.disabled    = false;
            popInput1_EquNo.disabled    = false;
            popInput1_EquName.disabled  = false;
            popInput1_EquEName.disabled = false;
            popInput1_IsAndTrt.disabled = false;
            popInput1_EquModel.disabled = false;
            popInput1_EquClass.disabled = false;
            popInput1_Floor.disabled    = false;
            popInput1_Building.disabled = false;
            //Display
            popBtn1_Exit.style.display = 'none';
            break;
    }
}

//彈出作業畫面：設定「檢視」視窗相關的欄位狀態與內容
function SetViewWindowData(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput1_EquID.value    = DataArray[0];
        popInput1_EquNo.value    = DataArray[1];
        popInput1_EquName.value  = DataArray[2];
        popInput1_EquEName.value = DataArray[3];
        popInput1_IsAndTrt.value = DataArray[9];   //將資料代碼轉換成資料名稱
        popInput1_EquModel.value = DataArray[10];  //將資料代碼轉換成資料名稱
        popInput1_EquClass.value = DataArray[6];
        popInput1_Floor.value    = DataArray[7];
        popInput1_Building.value = DataArray[8];
    }
    else {
        var MsgObj = new Object;

        MsgObj.result  = false;
        MsgObj.message = DataArray[1];

        ExcuteComplete(MsgObj);
    }
}


function OpenPara(equ_id) {
    $.post("FloorPara.aspx",
           { DoAction: "Query", EquID: equ_id },
           function (data) {
               $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
               + ' -webkit-transform: translate3d(0,0,0);"></div>');
               $("#popOverlay").css("background", "#000");
               $("#popOverlay").css("opacity", "0.5");
               $("#popOverlay").width("100%");
               $("#popOverlay").height($(document).height());
               $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                   + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
               $("#ParaExtDiv").html(data);
               $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
               $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);

               $('#ParaGridView').each(function (index) {
                   $(this).find('select[name*="ParaValue"]:eq(0)').val($(this).find('input[name="O_ParaValue"]:eq(0)').val());
               });
           });
}


function SetBindPara() {
    $("#ContentPlaceHolder1_MainGridView").find("tr").each(function (index) {
        var equid = $(this).find('#EquID').val();
        $(this).find('#BtnFloorSetting').click(function () {
            alert(equid);
        });
    });
}

function SetSave() {
    //get form serial data
    var serialize = $("#Elevator_Panel input,select").serialize() + "&DoAction=Save";
    $.ajax({
        type: "POST",
        url: "FloorPara.aspx",
        data: serialize,
        dataType: "json",
        success: function (data) {
            if (data.isSuccess == "OK") {
                alert("更新完成!!");
            } else {

            }
        },
        fail: function () {
            console.log("Error....");
        }
    }).done(function () {
        
    });
    SetCancel();
}

function SetCancel() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
}


var target_para_obj = "";

function PopURL(obj) {
    var oTD = JsFunFindParent(obj, "TR");
    target_para_obj = $(oTD).find('input[name="ParaValue"]');
    $.post($(oTD).find('#EditFormURL').val(),
        {
            'EquID': $('input[name="EquID"]').val(),
            'EquParaID': $(oTD).find('input[name="EquParaID"]').val(),
            'ParaValue': $(oTD).find('input[name="ParaValue"]').val(),
            'ParaName': $(oTD).find('input[name="ParaName"]').val(),
            'FloorName': $(oTD).find('input[name="FloorName"]').val(),
            'ParaDesc': $(oTD).find('input[name="ParaDesc"]').val()
        },
        function (data) {
            $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:39999; overflow:hidden;'
                   + ' -webkit-transform: translate3d(0,0,0);"></div>');
            $("#popOverlay2").css("background", "#000");
            $("#popOverlay2").css("opacity", "0.5");
            $("#popOverlay2").width("100%");
            $("#popOverlay2").height($(document).height());
            $(document.body).append('<div id="ElevPop" style="position:absolute;z-index:40000;background-color:#1275BC;'
                + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
            $("#ElevPop").html(data);
            $("#ElevPop").css("left", ($(document).width() - $("#ElevPop").width()) / 2);
            $("#ElevPop").css("top", $(document).scrollTop() + 50);
            $("#popOverlay2").click(function () {
                $("#ElevPop").remove();
                $("#popOverlay2").remove();
            });
            var data = $('#FloorData').val().split("").reverse().join("");
            $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
                var booCheck = false;
                if (data[index] == "1") {
                    booCheck = true;                        
                }
                //console.log($('#ParaName').val());
                if ($(oTD).find('#ParaName').val() == "ElevCtrlOnOpen") {                    
                    booCheck = !booCheck;                    
                }
                $(this).find('input[name*="CheckIO"]').prop("checked", booCheck);
            });
        }
    );
}


function SaveFloor() {
    var floor_bin = "";
    var para_name = "";
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        para_name = $(this).find("#ParaName").val();
        if ($(this).find('input[name*="CheckIO"]').prop("checked") == true) {
            floor_bin = "1" + floor_bin;
        } else {
            floor_bin = "0" + floor_bin;
        }
    });
    //for (var s = floor_bin.length; s < 48; s++) {        
    //    floor_bin = "0" + floor_bin;
    //}
    console.log(floor_bin);
    if (para_name == "ElevCtrlOnOpen") {        
        //$(target_para_obj).val(floor_bin.replace("0","T").replace("1","0").replace("T", "1"));
        floor_bin = floor_bin.replaceAll("0", "F").replaceAll("1", "T");
        floor_bin = floor_bin.replaceAll("F", "1").replaceAll("T", "0");
    }
    console.log(floor_bin);
    var temp = parseInt(floor_bin, 2); // 先將字串以2進位方式解析為數值
    var result = temp.toString(16); // 將數值轉為16進位
    for (var s = result.length; s < 12; s++) {
        result = "0" + result;
    }
    $(target_para_obj).val(result.toUpperCase());
    CancelFloor();
}

function CancelFloor() {
    $("#ElevPop").remove();
    $("#popOverlay2").remove();
}


String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};