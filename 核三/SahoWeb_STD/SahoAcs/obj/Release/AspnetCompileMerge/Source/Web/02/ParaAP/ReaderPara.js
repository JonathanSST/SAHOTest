var objForm = document.forms["form1"];
var objLen = objForm.length;
var LeftLabel = new Array();
var RightLabel = new Array();
var Leftcount = 0, Rightcount = 0;

// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');

    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    
    if (hideParaValue.value != "") {
        var Facestr = hideParaValue.value.substr(0, 2);
        var LoadModestr = hideParaValue.value.substr(2, 2);
        var BlockLengthstr = hideParaValue.value.substr(4, 2);
        var Buzzerstr = hideParaValue.value.substr(6, 2);
        BlockLengthstr = parseInt(BlockLengthstr, 16);

        if (Buzzerstr == "AA")
            Buzzerstr = "0";
        else
            Buzzerstr = "1";
        
        switch (Facestr)
        {
            case "0A":
                this.popInput_Face0.checked = true;
                break;
            case "0B":
                this.popInput_Face1.checked = true;
                break;
            case "0C":
                this.popInput_Face2.checked = true;
                break;
            case "1C":
                this.popInput_Face3.checked = true;
                break;
            case "2C":
                this.popInput_Face4.checked = true;
                break;
        }

        switch (LoadModestr)
        {
            case "0A":
                this.popInput_LoadMode0.checked = true;
                break;
            case "0B":
                this.popInput_LoadMode1.checked = true;
                break;
            case "0C":
                this.popInput_LoadMode2.checked = true;
                break;
        }

        popInput_BlockLength.value = BlockLengthstr;
        popInput_Buzzer.value = Buzzerstr;

        GetBlockLengthStr(BlockLengthstr);
        GetBuzzerStr(Buzzerstr);
    }
    Def(); SetMode('Add');
}

function Def() {
    var el = objForm.getElementsByTagName("span");
    for (var i = 0; i < el.length; i++) {
        if (el[i].id.slice(0, 6) == "popL_L") {
            LeftLabel[Leftcount] = el[i];
            Leftcount++;
        }
        if (el[i].id.slice(0, 6) == "popL_R") {
            RightLabel[Rightcount] = el[i];
            Rightcount++;
        }
    }
}

// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            var el = objForm.getElementsByTagName("span");
            for (var i = 0; i < el.length; i++) {
                if (el[i].id.slice(-5) != "Title" && el[i].id.slice(0, 5) == "popL_")
                    el[i].style.color = "#A9A9A9";
            }
            break;
        case '':
            var el = objForm.getElementsByTagName("span");
            for (var i = 0; i < el.length; i++) {
                el[i].style.color = "#A9A9A9";
            }
            break;
    }
}

//儲存畫面資料
function SaveReaderPara() {

    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var FaceValue = "";
    var LoadModeValue = "";
    var BlockLengthValue = "";
    var BuzzerValue = "";

    for (var i = 0; i < objForm.Face.length; i++) {
        if (objForm.Face[i].checked) {
            FaceValue = objForm.Face[i].value;
            break;
        }
    }

    for (var i = 0; i < objForm.LoadMode.length; i++) {
        if (objForm.LoadMode[i].checked) {
            LoadModeValue = objForm.LoadMode[i].value;
            break;
        }
    }

    if (popInput_BlockLength.value != "")
        BlockLengthValue = popInput_BlockLength.value;

    if (popInput_Buzzer.value != "")
        BuzzerValue = popInput_Buzzer.value;

    PageMethods.SaveData(hideUserID.value, hideEquID.value, hideEquParaID.value, hideTarget.value, FaceValue, LoadModeValue, padLeft(BlockLengthValue, 2), padLeft(BuzzerValue, 2), Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "SaveData":
                    window.returnValue = objRet.message;
                    window.close();
                    break;
            }
            break;
        case false:
            alert(objRet.message);
            break;
        default:

            break;
    }
}

// Radio選取
function RadioSelected(Obj) {
    document.getElementById(Obj).checked = true;
}

// 轉換Label顏色
function ChangeLabelColor(Target) {
    if (Target == "popL_LTitle") {
        for (i = 0; i < LeftLabel.length; i++) {
            RightLabel[i].style.color = "#A9A9A9";
            if (LeftLabel[i].innerHTML != "無效參數")
                LeftLabel[i].style.color = "black";

        }
    }
    else if (Target == "popL_RTitle") {
        for (i = 0; i < LeftLabel.length; i++) {
            LeftLabel[i].style.color = "#A9A9A9";
            if (RightLabel[i].innerHTML != "無效參數")
                RightLabel[i].style.color = "black";
        }
    }
    else {
        var el = objForm.getElementsByTagName("span");
        for (var i = 0; i < el.length; i++) {
            el[i].style.color = "#A9A9A9";
        }
    }
    hideTarget.value = Target;
}

// 取得區塊參數
function GetBlockLengthStr() {
    PageMethods.GetBlockLengthStr(popInput_BlockLength.value, SetBlockLengthUI, onPageMethodsError);
}

// 處理區塊參數UI
function SetBlockLengthUI(StrArray) {
    popL_LBlockLengthText.innerHTML = StrArray[0];
    popL_RBlockLengthText.innerHTML = StrArray[1];
    if (hideTarget.value != "") {
        if (hideTarget.value == "popL_LTitle") {
            if (popL_LBlockLengthText.innerHTML != "無效參數")
                popL_LBlockLengthText.style.color = "black";
            else
                popL_LBlockLengthText.style.color = "#A9A9A9";
            popL_RBlockLengthText.style.color = "#A9A9A9";
        }
        if (hideTarget.value == "popL_RTitle") {
            if (popL_RBlockLengthText.innerHTML != "無效參數")
                popL_RBlockLengthText.style.color = "black";
            else
                popL_RBlockLengthText.style.color = "#A9A9A9";
            popL_LBlockLengthText.style.color = "#A9A9A9";
        }
    }
}

// 取得鳴蜂器參數
function GetBuzzerStr() {
    PageMethods.GetBuzzerStr(popInput_Buzzer.value, SetBuzzerUI, onPageMethodsError);
}

// 處理鳴蜂器參數UI
function SetBuzzerUI(StrArray) {
    popL_LBuzzerText.innerHTML = StrArray[0];
    popL_RBuzzerText.innerHTML = StrArray[1];
    if (hideTarget.value != "") {
        if (hideTarget.value == "popL_LTitle") {
            if (popL_LBuzzerText.innerHTML != "無效參數")
                popL_LBuzzerText.style.color = "black";
            else
                popL_LBuzzerText.style.color = "#A9A9A9";
            popL_RBuzzerText.style.color = "#A9A9A9";
        }
        if (hideTarget.value == "popL_RTitle") {
            if (popL_RBuzzerText.innerHTML != "無效參數")
                popL_RBuzzerText.style.color = "black";
            else
                popL_RBuzzerText.style.color = "#A9A9A9";
            popL_LBuzzerText.style.color = "#A9A9A9";
        }
    }
}

// 左邊補0
function padLeft(str, lenght) {
    if (str.length >= lenght || str.length == 0)
        return str;
    else
        return padLeft("0" + str, lenght);
}

