function ShowDetail() {
    //var x = $("#MainSqlShow").css("display");
    if ($("#MainSqlShow").css("display") === "none") {
        $("#MainSqlShow").css("display", "block");
    } else {
        $("#MainSqlShow").css("display", "none");
    }
}

//設定使用者按鈕的權限
function SetUserLevel(str) {
    //ViewButton.disabled = true;

    if (str != '') {

        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'View') {
                //ViewButton.disabled = false;
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
function CallShowLogDetail(recordid) {
    SelectValue.value = recordid;
    var evoyn = EvoYN.value;
    if (IsEmpty(SelectValue.value))
        NotSelectForEdit(Msg);
    else {
        SetMode('View');

        PopupTrigger1.click();
        if (evoyn) {
            ynstr = "1";
        } else {
            ynstr = "0";
        }
        PageMethods.LoadData(SelectValue.value, ynstr, SetUI, onPageMethodsError);
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
        ADV_TextBox_LogStatus.value = DataArray[1];
        ADV_TextBox_DepNo.value = DataArray[2];
        ADV_TextBox_DepName.value = DataArray[3];
        ADV_TextBox_PsnNo.value = DataArray[4];
        ADV_TextBox_PsnName.value = DataArray[5];
        ADV_TextBox_CardNo.value = DataArray[6];
        ADV_TextBox_CardVer.value = DataArray[7];
        ADV_TextBox_EquNo.value = DataArray[8];
        ADV_TextBox_EquName.value = DataArray[9];
        ADV_TextBox_LogTime.value = DataArray[10];

        var d = new Date(ADV_TextBox_CardTime.value);
        var yr = d.getFullYear();
        var mon = d.getMonth();
        var dt = d.getDate();
        var hr = d.getHours();
        var mins = d.getMinutes();
        var sc = d.getSeconds();
        //var utcDate = new Date(Date.UTC(yr, mon, dt, hr, mins, sc));
        var utcDate = new Date(yr, mon, dt, hr, mins, sc);
        var cam = DataArray[11];
        var mms = Date.parse(utcDate); //"1586281576000";
        bindurl(cam, mms, DataArray[12]);
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}


function bindurl(cam, mms, auth) {    
    //var anc = document.getElementById("videoAnchor");
    //var archive_video = "playvideo.aspx?id=" + cam + "&fp=" + mms;

    //anc.setAttribute("href", archive_video);
    //anc.setAttribute("target", "_new");
    //anc.innerText = cam + "," + auth;
    console.log(cam);
    if (auth == "1" && cam.split(',').length > 0) {
        $("#CameraArea").html("");
        for (var i = 0; i < cam.split(',').length ; i++) {
            console.log(i);
            var archive_video = "playvideo.aspx?id=" + cam.split(',')[i] + "&fp=" + mms;
            var htmlhref = '<a id="videoAnchor" target="_new" style="color:white; font-size: 18px; font-family: "Courier New"; text-shadow: 2px 2px 2px #6b6b6b;"></a>'
            $("#CameraArea").append(htmlhref);
            $("#CameraArea").find('a').last().prop("href", archive_video);
            $("#CameraArea").find('a').last().text(cam.split(',')[i] + ";");
        }
    } else {
        anc.setAttribute("href", "");
        anc.style.fontFamily = "微軟正黑體";
        anc.style.fontSize = "small";
        anc.style.pointerEvents = "none";
        anc.style.color = "red";
        anc.innerText = "讀卡記錄回放權限不足!";
    }
}



