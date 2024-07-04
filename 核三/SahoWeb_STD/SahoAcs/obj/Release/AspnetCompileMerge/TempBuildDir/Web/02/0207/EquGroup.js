// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_Desc.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_Desc.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_Desc.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Delete':
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            popInput_Desc.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            break;
        case '':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_Desc.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_Desc.value = '';
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";

            AuthpopInput_No.disabled = false;
            AuthpopInput_No.value = '';
            while (AuthpopInput_MA.length > 0)
                AuthpopInput_MA.remove(0);
            break;
    }
}

//設定使用者按鈕的權限
function SetUserLevel(str) {
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;
    GroupSettingButton.disabled = true;

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
            if (data[i] == 'Auth') {
                GroupSettingButton.disabled = false;
            }
        }
    }
}

// 呼叫新增視窗
function CallAdd(title) {
    L_popName1.innerText = title;
    SetMode('Add');
    PopupTrigger1.click();
}

// 執行新增動作
function AddExcute() {
    PageMethods.Insert(hideUserID.value, popInput_No.value, popInput_Name.value, popInput_Desc.value, hideOwnerList.value, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(title, Msg) {
    L_popName1.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_No.value = DataArray[0];
        popInput_Name.value = DataArray[1];
        popInput_Desc.value = DataArray[2];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

// 執行編輯動作
function EditExcute() {
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_No.value, popInput_Name.value, popInput_Desc.value, Excutecomplete, onPageMethodsError);
}

// 呼叫刪除視窗
function CallDelete(title, Msg) {
    L_popName1.innerText = title;
    DeleteLableText.innerText = "您確定要將這筆資料刪除嗎？";
    if (IsEmpty(SelectValue.value)) NotSelectForDelete(Msg);
    else {
        SetMode('Delete');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
    }
}

// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "Add":
                    SelectValue.value = popInput_No.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Edit":
                    SelectValue.value = popInput_No.value.trim();
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    SelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
            }
            SetMode('');
            CancelTrigger1.click();
            break;
        case false:
            alert(objRet.message);
            break;
        default:

            break;
    }
}

// 呼叫群組設定視窗
function CallGroupSetting(Msg) {
    if (IsEmpty(SelectValue.value)) {
        NotSelectForEdit(Msg);
    }
    else
    {
        //var sURL = "EquGroupSetting.aspx?EquGrpNo=" + SelectValue.value;
        //var vArguments = "";
        //var sFeatures = "dialogHeight:430px;dialogWidth:1250px;center:yes;scroll:no";
        //window.showModalDialog(sURL, vArguments, sFeatures);
        $.post("EquGroupEdit.aspx",
           { "InEquGroup": SelectValue.value, "PageEvent": "Query" },
           function (data) {
               OverlayContent(data);
           });
    }
}

// 儲存群組設定資料
function SaveTable() {

}

// 呼叫權限視窗
function CallAuth(title, Msg) {
    L_popName2.innerText = title;
    DeleteLableText2.innerText = $("#propReadOnly").val();
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('');
        PopupTrigger2.click();
        PageMethods.LoadMgaData(SelectValue.value, SetAuthUI, onPageMethodsError);
    }
}

//將權限資料帶回畫面UI
function SetAuthUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        AuthpopInput_No.value = DataArray[0];
        while (AuthpopInput_MA.length > 0)
            AuthpopInput_MA.remove(0);
        if (DataArray[1] != '') {
            var data = DataArray[1].split("|");
            for (var i = 0; i < data.length; i += 3) {
                var option = null;
                option = document.createElement("option");
                option.text = '[' + data[i] + '] ' + data[i + 1] + ' (' + data[i + 2] + ')';
                option.value = '';
                AuthpopInput_MA.add(option);
            }
        }
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

// GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectValue.value);
}

function SelectState() {
    __doPostBack(QueryButton.id, '');
}

//去除IP底線,配合IP格式化使用
function ReplaceBaseline(popInput_Ip) {
    var i;
    var Ipstring = "";
    var IpLength = popInput_Ip.value.length;
    if (popInput_Ip.value != "___.___.___.___") {
        for (i = 0 ; i < IpLength ; i++) {
            popInput_Ip.value = popInput_Ip.value.replace("_", "");
        }
        var ipstr = popInput_Ip.value.split(".");
        for (i = 0 ; i < ipstr.length ; i++) {
            if (Ipstring != "") Ipstring += ".";
            Ipstring += formatIP(ipstr[i]);
        }
    }
    popInput_Ip.value = Ipstring;
}
//IP格式化
function formatIP(str) {
    if (str.length >= 3) {
        return str;
    } else {
        return formatIP('0' + str);
    }
}

//Chrome 呼叫群組設定視窗
function SetGroup() {
    if (IsEmpty(SelectValue.value))
        NotSelectForEdit($("#HDNotSelect").val());
    else {
        $.post("EquGroupSettingPop.aspx",
           { "PageEvent": "Query","EquGrpID":SelectValue.value },
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
           });
    }
}

function CloseSetting() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


function OverlayContent(data) {
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
}

function DoCancel() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
}