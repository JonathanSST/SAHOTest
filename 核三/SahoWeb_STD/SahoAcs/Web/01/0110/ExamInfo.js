// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case '':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            //popInput_Desc.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_OrgNo.value = '';
            popInput_ExamBeginTime.value = '';
            popInput_ExamEndTime.value = '';
            popInput_ExamBeginTime.disabled = false;
            popInput_ExamEndTime.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            //AuthpopInput_No.disabled = false;
            //AuthpopInput_No.value = '';
            while (AuthpopInput_MA.length > 0)
                AuthpopInput_MA.remove(0);
            break;
        case 'Add':
            popInput_No.disabled = false;
            popInput_Name.disabled = false;
            popInput_No.value = '';
            popInput_Name.value = '';
            popInput_OrgNo.value = '';
            popInput_ExamBeginTime.value = '';
            popInput_ExamEndTime.value = '';
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            popB_Confirm.style.display = "none";
            DeleteLableText.innerText = "";
            break;
        case 'Edit':
            popInput_No.disabled = true;
            popInput_Name.disabled = false;
            //alert(popInput_ExamBeginTime.disabled);
            popInput_ExamBeginTime.disabled = false;
            popInput_ExamEndTime.disabled = false;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "none";
            popB_Confirm.style.display = "none";
            DeleteLableText.innerText = "";
            //document.getElementById('ContentPlaceHolder1_popInput_ExamBeginTime_CalendarTextBox').disanbled = false;
            document.getElementById('ContentPlaceHolder1_popInput_ExamBeginTime_Clock').disabled = false;
            document.getElementById('ContentPlaceHolder1_popInput_ExamEndTime_Clock').disabled = false;
            break;
        case 'Delete':
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            //popInput_Desc.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "inline";
            popB_Confirm.style.display = "none";
            break;
        case 'Confirm':
            popInput_No.disabled = true;
            popInput_Name.disabled = true;
            //popInput_Desc.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            popB_Confirm.style.display = "inline";
            break;        
    }
}

//設定使用者按鈕的權限
function SetUserLevel(str) {
    alert(str);
    AddButton.disabled = true;
    EditButton.disabled = true;
    DeleteButton.disabled = true;
    btSelectData.disabled = true;
    btAuthConfirm.disabled = true;

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
                btSelectData.disabled = false;
                btAuthConfirm.disabled = false;
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
    PageMethods.Insert(hideUserID.value, popInput_No.value, popInput_Name.value, popInput_OrgNo.value, popInput_ExamBeginTime.value, popInput_ExamEndTime.value, Excutecomplete, onPageMethodsError);
}

// 呼叫編輯視窗
function CallEdit(title, Msg) {
    L_popName1.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else
    {
        SetMode('Edit');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
        //document.getElementById('ContentPlaceHolder1_popInput_ExamEndTime_Clock').click();
        PageMethods.EndTimeChk(hideUserID.value, SelectValue.value, '編輯', AlarmEndDate, onPageMethodsError);       
    }
}

// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        popInput_No.value = DataArray[0];
        popInput_Name.value = DataArray[1];
        popInput_OrgNo.value = DataArray[2];
        popInput_ExamBeginTime.value = DataArray[3];
        popInput_ExamEndTime.value = DataArray[4];
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
    PageMethods.Update(hideUserID.value, SelectValue.value, popInput_No.value, popInput_Name.value, popInput_OrgNo.value, popInput_ExamBeginTime.value, popInput_ExamEndTime.value, Excutecomplete, onPageMethodsError);
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
        PageMethods.EndTimeChk(hideUserID.value, SelectValue.value, '刪除', AlarmEndDate, onPageMethodsError);
    }
}

// 呼叫權限設定確認視窗
function CallConfirm(title, Msg) {
    L_popName1.innerText = title;
    DeleteLableText.innerText = "您確定要重整閱卷通行權限嗎？";
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        SetMode('Confirm');
        PopupTrigger1.click();
        PageMethods.LoadData(SelectValue.value, SetUI, onPageMethodsError);
        PageMethods.EndTimeChk(hideUserID.value, SelectValue.value, '重整通行權限', AlarmEndDate, onPageMethodsError);
    }
}

// 呼叫人員選擇
function PersonChoice(title, Msg) {
    L_popName2.innerText = title;
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        PopupTrigger2.click();
        PageMethods.QueryExamPsnData(SelectValue.value, 'Saho', InitExamPsn, onPageMethodsError);
        PageMethods.EndTimeChk(hideUserID.value, SelectValue.value, '', ProcAddRemove, onPageMethodsError);
    }
}

function QueryPsnData() {
    while (popB_PsnList1.length != 0)
        popB_PsnList1.remove(0);
    PageMethods.QueryPsnData(Input_TxtQuery.value, hUserId.value, SelectValue.value, LoadPsnData, onPageMethodsError);
}
// 執行刪除動作
function DeleteExcute() {
    PageMethods.Delete(SelectValue.value, hideUserID.value,popInput_OrgNo.value, Excutecomplete, onPageMethodsError);
}

// 執行權限設定動作
function ConfirmExcute() {
    PageMethods.AuthConfirm(SelectValue.value,hideUserID.value,popInput_OrgNo.value, Excutecomplete, onPageMethodsError);
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
                case "Confirm":
                    alert("通行權限重整完成");
                    SelectValue.value = '';
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
            }
            //SetMode('');
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

// 呼叫權限視窗
function CallAuth(title, Msg) {
    L_popName2.innerText = title;
    DeleteLableText2.innerText = $("#propReadOnly").val();
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        //SetMode('');
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
//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = '';
    if (str == 'Add') {
        for (var i = 0; i < popB_PsnList1.options.length; i++) {
            if (popB_PsnList1.options[i].selected) {
                popB_PsnList2.add(new Option(popB_PsnList1.options[i].text, popB_PsnList1.options[i].value));
            }
        }
        for (var i = popB_PsnList1.options.length - 1; i >= 0; i--) {
            if (popB_PsnList1.options[i].selected) {
                popB_PsnList1.remove(i);
            }
        }
    }
    else if (str == 'Del') {
        for (var i = 0; i < popB_PsnList2.options.length; i++) {
            if (popB_PsnList2.options[i].selected) {
                popB_PsnList1.add(new Option(popB_PsnList2.options[i].text, popB_PsnList2.options[i].value));
            }
        }
        for (var i = popB_PsnList2.options.length - 1; i >= 0; i--) {
            if (popB_PsnList2.options[i].selected) {
                popB_PsnList2.remove(i);
            }
        }
    }
}

function LoadPsnDataList() {
    CancelTrigger1.click();
    while (DataList.length != 0)
        DataList.remove(0);
    if (popB_PsnList2.length > 0) {
        for (var i = 0; i < popB_PsnList2.options.length; i++) {
            DataList.add(new Option(popB_PsnList2.options[i].text, popB_PsnList2.options[i].value));
        }
    }
}
//更新閱卷人員
function UpdateExamPsn() {
    var psnList = [];
    for (var i = 0; i < popB_PsnList2.length;i++) {
        psnList.push(popB_PsnList2.options[i].value);
    }
    PageMethods.InsertExamPsn(hideUserID.value, SelectValue.value, psnList,Finish,onPageMethodsError);

}

function QueryPsnData() {
    while (popB_PsnList1.length != 0)
        popB_PsnList1.remove(0);
    PageMethods.QueryPsnData(Input_TxtQuery.value, hideUserID.value, LoadPsnData, onPageMethodsError);

}function LoadPsnData(data) {
    if (data[0] != 'Saho_SysErrorMassage') {
        var psnno = data[0].split("|");
        var psnhead = data[1].split("|");
        var psnname = data[2].split("|");
        var psntype = data[3].split("|");
        var cardno = data[4].split("|");
        var option = null;
        for (var i = 0; i < psnno.length; i++) {
            var state = 0;
            option = document.createElement("option");
            option.text = psnhead[i] + ' ' + psnname[i];
            option.value = psnno[i];
            for (var j = 0; j < popB_PsnList2.length; j++) {
                if (psnno[i] == popB_PsnList2.options[j].value) {
                    state += 1;
                }
            }
            if (state == 0)
                popB_PsnList1.options.add(option);
        }
    }
    else
        alert(data[1]);
}
//把已加入此考試的人員放到右邊listbos
function InitExamPsn(data) {
    if (data[0] != 'Saho_SysErrorMassage') {
        var psnno = data[0].split("|");
        var psnhead = data[1].split("|");
        var psnname = data[2].split("|");
        var psntype = data[3].split("|");
        var examno = data[4].split("|");
        var cardno = data[5].split("|");
        //先全部清空
        for (var i = popB_PsnList1.options.length - 1; i >= 0; i--) {
            popB_PsnList1.remove(i);
        }
        for (var i = popB_PsnList2.options.length - 1; i >= 0; i--) {
             popB_PsnList2.remove(i);
        }

        var option = null;
        for (var i = 0; i < psnno.length; i++) {
                option = document.createElement("option");
                option.text = psnhead[i] + ' ' + psnname[i];
                option.value = psnno[i];
                if (examno[i] == SelectValue.value)
                {
                    popB_PsnList2.options.add(option);
                }
                else
                {
                    popB_PsnList1.options.add(option);
                }                
        }
    }
    else
        alert(data[1]);
}

function ExecProcData() {
    var psndata = '';
    var statetype = ddlType.options[ddlType.selectedIndex].value;
    for (var i = 0; i < DataList.length; i++) {
        psndata += DataList.options[i].value + '|' + DataList.options[i].text + '|';
    }
    PageMethods.ExecProcData(psndata, statetype, hUserId.value, ExecProcMsg, onPageMethodsError);
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

function AlarmEndDate(optype) {
    if (optype.result)
    {
        return true;
    }
    else {
        alert("閱卷期間已結束，考試資料不可" + optype.act);
        CancelTrigger1.click();
        return false;
    }
}

//加入移除按鈕狀態處理
function ProcAddRemove(optype) {
    if (optype.result) {
        popB_Enter1.disabled = false;
        popB_Remove1.disabled = false;
        return true;
    }
    else {
        alert("閱卷期間已結束，閱卷相關人員只能查詢");
        popB_Enter1.disabled = true;
        popB_Remove1.disabled = true;
        return false;
    }
}

function Finish(optype) {
    if (optype.result) {
        alert("作業已順利完成"); 
        return true;
    }
    else {
        alert("作業失敗");
        return false;
    }
}

$(document).ready(function () {
    $("#btSelectOrgData").click(function () {
        $.post("OrgStrucSelect.aspx",
           { DoAction: "Query" },
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
           }
       );
    });
});