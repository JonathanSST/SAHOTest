//設定使用者按鈕的權限
function SetUserLevel(str) {
    ViewButton.disabled = true;

    if (str != '') {

        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'View') {
                ViewButton.disabled = false;
            }
        }
    }
}

//*主要作業畫面：執行「查詢」動作
function SelectState() {
    Block();
    SelectValue.value = '';
    //console.log(QueryButton.id);
    __doPostBack(QueryButton.id, '');
}

//*主要作業畫面：執行「進階查詢」動作
function AVDQuery() {
    Block();
    SelectValue.value = '';
    __doPostBack(ADVQueryButton.id, '');
}

//*主要作業畫面：執行「匯出」動作
function ExportQuery() {
    SelectValue.value = '';
    __doPostBack(ExportButton.id, '');
}

//*主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}

//*主要作業畫面：呼叫「進階查詢」視窗
function CallAdvancedQuery() {
        SetMode('AdvancedQuery');
        PopupTrigger2.click();
}

//*主要作業畫面：呼叫「檢視」視窗
function CallShowLogDetail(strCardTime, strEquNo) {
    if (IsEmpty(SelectValue.value)){
        //NotSelectForEdit(Msg);
        NotSelectForEdit('No Data!');
    }
    else {
        SetMode('View');
        //PopupTrigger1.click();
        //PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);

        var tbl = document.getElementById('ContentPlaceHolder1_MainGridView');

        for (i = 0; i < tbl.rows.length; i++) {
            if (tbl.rows[i].id == 'GV_Row' + SelectValue.value) {
                //alert(tbl.rows[i].id + '__'
                //+ tbl.rows[i].cells[0].innerText + '__'
                //+ tbl.rows[i].cells[1].innerText + '__'
                //+ tbl.rows[i].cells[2].innerText + '__'
                //+ tbl.rows[i].cells[3].innerText + '__'
                //+ tbl.rows[i].cells[4].innerText + '__'
                //+ tbl.rows[i].cells[5].innerText + '__'
                //+ tbl.rows[i].cells[6].innerText + '__'
                //+ tbl.rows[i].cells[7].innerText + '__'
                //+ tbl.rows[i].cells[8].innerText + '__'
                //+ tbl.rows[i].cells[9].innerText + '__'
                //+ tbl.rows[i].cells[10].innerText + '__');

                PopupTrigger1.click();
                ADV_TextBox_CardTime.value = tbl.rows[i].cells[0].innerText;
                ADV_TextBox_DepNo.value = tbl.rows[i].cells[1].innerText;
                ADV_TextBox_DepName.value = tbl.rows[i].cells[2].innerText;
                ADV_TextBox_PsnNo.value = tbl.rows[i].cells[3].innerText;
                ADV_TextBox_PsnName.value = tbl.rows[i].cells[4].innerText;
                ADV_TextBox_CardNo.value = tbl.rows[i].cells[5].innerText;
                ADV_TextBox_CardVer.value = tbl.rows[i].cells[6].innerText;
                ADV_TextBox_EquNo.value = tbl.rows[i].cells[7].innerText;
                ADV_TextBox_EquName.value = tbl.rows[i].cells[8].innerText;
                ADV_TextBox_LogStatus.value = tbl.rows[i].cells[9].innerText;
                ADV_TextBox_LogTime.value = tbl.rows[i].cells[10].innerText;
            }
        } 
    }
}

//彈出作業畫面：設定「檢視」視窗不同模式相關的欄位狀態與內容
function SetMode(SetMode) {
    switch (SetMode) {
        case 'View':
            break;
        case '':
            break;
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        ADV_TextBox_CardTime.value = DataArray[0];
        ADV_TextBox_LogTime.value = DataArray[1];
        ADV_TextBox_LogStatus.value = DataArray[2];
        ADV_TextBox_PsnNo.value = DataArray[3];
        ADV_TextBox_PsnName.value = DataArray[4];
        ADV_TextBox_CardNo.value = DataArray[5];
        ADV_TextBox_CardVer.value = DataArray[6];
        ADV_TextBox_EquNo.value = DataArray[7];
        ADV_TextBox_EquName.value = DataArray[8];
        ADV_TextBox_DepNo.value = DataArray[9];
        ADV_TextBox_DepName.value = DataArray[10];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

function OpenVideo(card_time,now_time,vmshost,device) {
    console.log(card_time);
    var video_url = "VmsVideo.aspx";   
    $.ajax({
        type: "POST",
        url: video_url,
        data: {
            "timestamp": card_time, "nowtime": now_time, "vmshost": vmshost, "device": device
        },
        async: true,
        success: function (data) {
            $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
            $("#popOverlay").css("background", "#000");
            $("#popOverlay").css("opacity", "0.5");
            $("#popOverlay").width("100%");
            $("#popOverlay").height($(document).height());
            $("#popOverlay").click(function () {
                $("#popOverlay").remove();
                $("#ParaExtDiv").remove();
            });
            $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
            $("#ParaExtDiv").html(data);
            $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
            $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
        }
    });
    return false;

}
