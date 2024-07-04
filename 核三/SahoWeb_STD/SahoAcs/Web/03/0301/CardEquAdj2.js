var spanChk1;
var spanChk2;
var spanChk3;
var spanChk4;

//全選
function SelectAllCheckboxes(spanChk) {
    elm = document.forms[0];

    switch (spanChk) {
        case spanChk1:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk1.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState1") {
                        if (elm.elements[i].checked != spanChk1.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk2:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk2.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState2") {
                        if (elm.elements[i].checked != spanChk2.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk3:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk3.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState3") {
                        if (elm.elements[i].checked != spanChk3.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk4:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk4.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState4") {
                        if (elm.elements[i].checked != spanChk4.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        default:
            break;
    }
}

//統計GV1所勾選的資料列
function GV1State() {
    elm = document.forms[0];
    hRemoveData.value = '';
    var num = 0;
    var data = '';

    for (var i = 0; i < elm.length; i++) {
        if (elm[i].type == "checkbox" && elm[i].id != spanChk1.id) {
            var str = elm[i].name.split('$');
            if (str[4] == "RowCheckState1") {
                if (elm.elements[i].checked == true) {
                    data += num + '|';
                }
                num++;
            }
        }
    }

    hRemoveData.value += num + '|' + data;
}

//統計GV3所勾選的資料列
function GV3State() {
    elm = document.forms[0];
    hRemoveData.value = '';
    var num = 0;
    var data = '';

    for (var i = 0; i < elm.length; i++) {
        if (elm[i].type == "checkbox" && elm[i].id != spanChk3.id) {
            var str = elm[i].name.split('$');
            if (str[4] == "RowCheckState3") {
                if (elm.elements[i].checked == true) {
                    data += num + '|';
                }
                num++;
            }
        }
    }

    hRemoveData.value += num + '|' + data;
}

//統計GV2所勾選的資料列
function GV2State() {
    elm = document.forms[0];
    hAddData.value = '';
    var num = 0;
    var data = '';

    for (var i = 0; i < elm.length; i++) {
        if (elm[i].type == "checkbox" && elm[i].id != spanChk2.id) {
            var str = elm[i].name.split('$');
            if (str[4] == "RowCheckState2") {
                if (elm.elements[i].checked == true) {
                    data += num + '|';
                }
                num++;
            }
        }
    }

    hAddData.value += num + '|' + data;
}

//統計GV4所勾選的資料列
function GV4State() {
    elm = document.forms[0];
    hAddData.value = '';
    var num = 0;
    var data = '';

    for (var i = 0; i < elm.length; i++) {
        if (elm[i].type == "checkbox" && elm[i].id != spanChk4.id) {
            var str = elm[i].name.split('$');
            if (str[4] == "RowCheckState4") {
                if (elm.elements[i].checked == true) {
                    data += num + '|';
                }
                num++;
            }
        }
    }

    hAddData.value += num + '|' + data;
}

//記錄下拉式選單index
function ddlState() {
    var gridview = document.getElementById("ContentPlaceHolder1_GridView1");
    var no = '';
    var ref = '';

    if (gridview != null) {
        for (var i = 0; i < gridview.rows.length; i++) {
            no += gridview.rows[i].cells[1].textContent + '|';
        }
        elm = document.forms[0];
        var data = '';
        for (var i = 0; i < elm.length; i++) {
            if (elm[i].type == "select-one") {
                var str = elm[i].name.split('$');
                if (str[4] == "ddlCardRule") {
                    data += elm.elements[i].selectedIndex + '|';
                }
            }
        }

        var nodata = no.split('|');
        var seldata = data.split('|');
        for (var i = 0; i < nodata.length - 1; i++) {
            ref += nodata[i] + '|' + seldata[i] + '|';
        }
    }

    hddlState.value = ref;
}

//清空所有記錄及Check勾選
function ClearAll() {
    hAddData.value = '';
    hRemoveData.value = '';
    elm = document.forms[0];
    for (var i = 0; i < elm.length; i++) {
        if (elm[i].type == "checkbox") {
            if (elm.elements[i].checked != false)
                elm.elements[i].click();
        }
    }
}

function SelectItem() {
    var item = ddl_input.options[ddl_input.selectedIndex].value;
    CallTable(item);
}

function CallTable(type) {
    switch (type) {
        case "Card":
            ChangeText(L_popName1, $("#lblCardSelect").val());
            ChangeText(popLabel_OrgList, $("#lblCardList").val());
            break;
        case "Org":
            ChangeText(L_popName1, $("#lblOrgSelect").val());
            ChangeText(popLabel_OrgList, $("#lblOrgList").val());
            break;
        case "No":
            ChangeText(L_popName1, $("#lblPsnSelect").val());
            ChangeText(popLabel_OrgList, $("#lblPsnList").val());
            break;
        case "Name":
            ChangeText(L_popName1, $("#lblNameSelect").val());
            ChangeText(popLabel_OrgList, $("#lblNameList").val());
            break;
        default:
            break;
    }

    PageMethods.SelectTable(hUserId.value, type, InitUI, onPageMethodsError);
}

function InitUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        popTxt_Query.value = "";

        while (popB_CardList1.length != 0) {
            popB_CardList1.remove(0);
        }

        while (popB_CardList2.length != 0) {
            popB_CardList2.remove(0);
        }

        for (var i = 0, j = DataList.options.length; i < j; i++) {
            popB_CardList2.add(new Option(DataList.options[i].text, DataList.options[i].value));
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        while (popB_CardList1.length != 0) {
            popB_CardList1.remove(0);
        }

        if (DataArray.length > 0) {
            for (var i = 0, k = DataArray.length; i < k; i++) {
                var data = DataArray[i].split("|");
                var option = null;
                var state = 0;
                option = document.createElement("option");
                option.text = data[0];
                option.value = data[1];

                for (var j = 0; j < popB_CardList2.length; j++) {
                    if (data[1] == popB_CardList2.options[j].value) {
                        state += 1;
                    }
                }

                if (state == 0) {
                    popB_CardList1.options.add(option);
                }
            }

            if (popB_CardList1.length != 0) {
                popB_CardList1.options[0].selected = true;
            }
        }
        else {
            alert('查無資料！')
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

/*
function SetUI(DataArray) {
    if (DataArray.d[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();

        while (popB_CardList1.length != 0) {
            popB_CardList1.remove(0);
        }

        while (popB_CardList2.length != 0) {
            popB_CardList2.remove(0);
        }

        for (var i = 0, j = DataList.options.length; i < j; i++) {
            popB_CardList2.add(new Option(DataList.options[i].text, DataList.options[i].value));
        }

        if (DataArray.d.length > 0) {
            for (var i = 0, k = DataArray.length; i < k; i++) {
                var data = DataArray.d[i].split("|");
                var option = null;
                var state = 0;
                option = document.createElement("option");
                option.text = data[0];
                option.value = data[1];

                for (var j = 0; j < popB_CardList2.length; j++) {
                    if (data[1] == popB_CardList2.options[j].value) {
                        state += 1;
                    }
                }

                if (state == 0) {
                    popB_CardList1.options.add(option);
                }
            }
        }
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray.d[1];
        Excutecomplete(sRet);
    }
}
*/

function QueryData() {
    if (popTxt_Query.value === "") {
        alert('請輸入查詢條件！')
    }
    else {
        PageMethods.QueryData(hUserId.value, popLabel_OrgList.innerText, popTxt_Query.value.trim(), SetUI, onPageMethodsError);
    }
}

function KeyDownEvent(e) {
    if (e.keyCode == 13 || e.charCode == 13) {
        QueryData();
    }
    else {
        document.getElementById('ContentPlaceHolder1_popTxt_Query').value += String.fromCharCode(e.keyCode);
    }
}

//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = '';

    if (str == 'Add') {
        for (var i = 0; i < popB_CardList1.options.length; i++) {
            if (popB_CardList1.options[i].selected) {
                popB_CardList2.add(new Option(popB_CardList1.options[i].text, popB_CardList1.options[i].value));
            }
        }

        for (var i = popB_CardList1.options.length - 1; i >= 0; i--) {
            if (popB_CardList1.options[i].selected) {
                popB_CardList1.remove(i);
            }
        }
    }
    else if (str == 'Del') {
        for (var i = 0; i < popB_CardList2.options.length; i++) {
            if (popB_CardList2.options[i].selected) {
                popB_CardList1.add(new Option(popB_CardList2.options[i].text, popB_CardList2.options[i].value));
            }
        }

        for (var i = popB_CardList2.options.length - 1; i >= 0; i--) {
            if (popB_CardList2.options[i].selected) {
                popB_CardList2.remove(i);
            }
        }
    }
}

function LoadCardData() {
    CancelTrigger1.click();

    while (DataList.length != 0)
        DataList.remove(0);

    hDataList.value = "";

    for (var i = 0; i < popB_CardList2.options.length; i++) {
        DataList.add(new Option(popB_CardList2.options[i].text, popB_CardList2.options[i].value));
        hDataList.value += popB_CardList2.options[i].text + "|" + popB_CardList2.options[i].value + "|";
    }

    hDataList.value = hDataList.value
}

function MM_openBrWindow(theURL, winName, win_width, win_height) {
    var PosX = (screen.width - win_width) / 2;
    var PosY = (screen.Height - win_height) / 2;
    features = "width=" + win_width + ",height=" + win_height + ",top=" + PosY + ",left=" + PosX;
    var newwin = window.open(theURL, winName, features);
}

function ExecProc() {
    var item = ddlOpType.options[ddlOpType.selectedIndex].value;
    var equtype = ddlEquType.options[ddlEquType.selectedIndex].value;
    var datalist = '';
    //var gv = document.getElementById('ContentPlaceHolder1_GridView1');
    //datalist = gv.rows.length;
    //datalist = gv.rows[0].cells[4].innerText;
    for (var i = 0; i < DataList.options.length; i++) {
        datalist += DataList.options[i].text + '|' + DataList.options[i].value + '|';
    }

    PageMethods.ExecCardProc(item, hFinalEquData.value, datalist, hddlState.value, hFinalFloorData.value, hUserId.value, hSelectState.value, equtype, ShowMsg, onPageMethodsError);
}

function ShowMsg(data) {
    if (data.length > 0) {
        if (data[0] != 'Saho_SysErrorMassage') {
            for (var i = 0; i < data.length; i++) {
                List_Msg.add(new Option(data[i], 'none'));
                List_Msg.selectedIndex = List_Msg.length - 1;
            }
        }
        else
            alert(data[1]);
    }
}

function SelectStateProc() {
    hSelectState.value = ddl_input.options[ddl_input.selectedIndex].value;

    while (DataList.length != 0)
        DataList.remove(0);
}

function EquData(Equid) {
    hFinalEquData.value = Equid;
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            alert(sRet.message);
            break;
        default:
            break;
    }
}