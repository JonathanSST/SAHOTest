$(document).ready(function () {

});

// 設定整體Div顯示
function SetDivMode(sMode) {
    switch (sMode) {
        case 'Loc':
            document.getElementById('ContentPlaceHolder1_Div_Loc').style.display = "";
            LocUI_Title.style.background = "#3B5998";
            break;

        default:
            document.getElementById('ContentPlaceHolder1_Div_Loc').style.display = "none";
            LocUI_Title.style.background = "#BBBBBB";
            break;
    }
}

function OpenWin(tmp) {
    event_srcElement_id.value = '';

    var s;
    var ind;

    if (tmp != undefined) {
        s = tmp;
    }
    else {
        s = event.srcElement.id;
    }

    ind = s.replace("ContentPlaceHolder1_Location_TreeViewt", "");
    s = s.replace("ContentPlaceHolder1_Location_TreeViewt", "");

    event_srcElement_id.value = s;

    var noteItemType = txt_NodeTypeList.value.split(',');
    var noteID = txt_NodeIDList.value.split(',');

    if (noteItemType[parseInt(s)] != "NONE") {
        if (noteItemType[parseInt(s)] == "SMS") {
            SetDivMode('');
            $.unblockUI();

        } else if (noteItemType[parseInt(s)] == "AREA" || noteItemType[parseInt(s)] == "BUILDING" || noteItemType[parseInt(s)] == "FLOOR") {
            Block();
            SetDivMode("Loc");
            LoadLocInfo(noteID[s]);
            SetLocMode("Edit", '', '');
        }
    }
}

function LoadAffterAction(sClass, sAct, sID, sUp) {
    // 移動TreeView_Panel的scrollTop
    document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = TreeView_Panel_scrollTop.value;

    //更新上層位置
    UpdatePListDDL();

    // sClass = UpdateLocInfo
    // sAct = Insert、Update、Delete
    if (sAct == 'Insert') {
        SetDivMode(sClass);
        LoadLocInfo(sID);
        SetLocMode('Edit', '', '');

        //sLoc = sUp * 19.5;
        //document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = Math.round(sLoc);

        ShowDialog('message', '設定', '新增資料成功！');
    }
    else if (sAct == 'Update') {
        SetDivMode(sClass);
        LoadLocInfo(sID);
        SetLocMode('Edit', '', '');

        ShowDialog('message', '設定', '更新資料成功！');
    }
    else if (sAct == 'Delete') {
        SetDivMode('');
        ShowDialog('message', 'General', '刪除資料成功');
    }

    $.unblockUI();
}

// 各動作完成
function Excutecomplete(objRet) {
    $.unblockUI();
    Block();
    // TreeView_Panel的scrollbar定位
    TreeView_Panel_scrollTop.value = TreeView_Panel.scrollTop;

    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "UpdateLocInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Loc_Update_' + objRet.message);
                    break;

                case "InsertLocInfo":
                    hfLocID.value = objRet.message
                    LocIDValue.value = objRet.message
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Loc_Insert_' + objRet.message + '_' + objRet.sUp);
                    break;

                case "DeleteLocInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Loc_Delete_' + objRet.message);
                    break;
            }
            break;
        case false:
            ShowDialog('alert', '警告訊息', objRet.message);

            $.unblockUI();
            break;
        default:
            $.unblockUI();
            break;
    }
}



/* Location相關操作 */
function SetLocMode(sMode, sType, sPID) {

    switch (sMode) {
        case 'Add':
            Loc_Input_No.value = '';
            Loc_Input_Name.value = '';

            if (sType == "SMS") {
                Loc_Input_Type.value = '區域';
            } else if (sType == "AREA") {
                Loc_Input_Type.value = '棟別';
            } else if (sType == "BUILDING") {
                Loc_Input_Type.value = '樓層';
            }

            $('[name*="Loc_Input_PList"]').val(sPID);
            Loc_Input_Map.value = '';
            Loc_Input_Desc.value = '';

            hfLocID.value = '';

            Loc_B_Add.style.display = "inline";
            Loc_B_Edit.style.display = "none";
            Loc_B_Delete.style.display = "none";

            break;
        case 'Edit':
        case 'Update':
            Loc_B_Add.style.display = "none";
            Loc_B_Edit.style.display = "inline";
            Loc_B_Delete.style.display = "none";
            break;
        case 'Delete':
            Loc_B_Add.style.display = "none";
            Loc_B_Edit.style.display = "none";
            Loc_B_Delete.style.display = "inline";
            break;
        default:
            break;
    }

}

// 讀取Location資料
function LoadLocInfo(LocID = 0) {
    PageMethods.LoadLocInfo(LocID, SetLocInfoUI, onPageMethodsError);
}

//取得Location畫面資料
function GetLocDataArray() {
    var DataArray = [];
    DataArray[0] = hfLocType.value;           //類型
    DataArray[1] = Loc_Input_Type.value;      //類型名稱
    DataArray[2] = Loc_Input_No.value;        // 編號
    DataArray[3] = Loc_Input_Name.value;      // 名稱
    DataArray[4] = Loc_Input_Map.value;       // Map圖號
    DataArray[5] = Loc_Input_Desc.value;      // 備註
    DataArray[6] = hfLocID.value;             // 識別碼　LocID 
    DataArray[7] = Loc_Input_PList.value;     // 上層位置

    if (Loc_Input_Type.value == "區域") DataArray[0] = "AREA";
    if (Loc_Input_Type.value == "棟別") DataArray[0] = "BUILDING";
    if (Loc_Input_Type.value == "樓層") DataArray[0] = "FLOOR";

    return DataArray;
}

//執行新增動作
function InsertLocExcute() {
    var DataArray = GetLocDataArray();
    PageMethods.InsertLocInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Location動作
function UpdateLocExcute() {
    var DataArray = GetLocDataArray();
    PageMethods.UpdateLocInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除動作
function DeleteLocExcute() {
    var DataArray = GetLocDataArray();
    PageMethods.DeleteLocInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

function UpdatePListDDL() {
    PageMethods.LoadNewPList(UpdateDDL, onPageMethodsError);
}

function UpdateDDL(DataArray) {
    while (Loc_Input_PList.options.length > 0)
        Loc_Input_PList.options.remove(0);

    var DataStr = null;
    var option = null;

    option = document.createElement("option");
    option.text = "===最上層===";
    option.value = 0;
    Loc_Input_PList.options.add(option);

    for (i = 0; i < DataArray.length; i++) {
        option = document.createElement("option");
        DataStr = DataArray[i].split(",");
        option.text = DataStr[1];
        option.value = DataStr[0];
        Loc_Input_PList.options.add(option);
    }
}


// 讀取Location資料回填UI
function SetLocInfoUI(DataArray) {

    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Loc_Input_Type.value = DataArray[8];
        Loc_Input_No.value = DataArray[2];
        Loc_Input_Name.value = DataArray[3];
        Loc_Input_Map.value = DataArray[4];
        Loc_Input_Desc.value = DataArray[5];

        $('[name*="Loc_Input_PList"]').val(DataArray[7]);
        hfLocType.value = DataArray[1]
        hfLocID.value = DataArray[0]        // 識別碼 LocID
        LocIDValue.value = DataArray[0]
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];

        $.unblockUI();
        Excutecomplete(objRet);
    }

    $.unblockUI();
}


// Dialog
function ShowDialog(strType, strTitle, strMsg) {
    var vIcon;

    if (strType == 'message') {
        vIcon = "<img src='/Img/info.png' width='30px' />";
    }
    else if (strType == 'alert') {
        vIcon = "<img src='/Img/Close.png' width='30px' />";
    }

    var vHtml = '';
    vHtml += "<div align='center' valign='center'>";
    vHtml += "<table style='width: 90%'>";
    vHtml += "<tr>";
    vHtml += "<td style='text-align: middle; vertical-align: middle; width: 50px'>" + vIcon + "</td>";
    vHtml += "<td style='text-align: left; vertical-align: middle'><span>" + strMsg + "</span></td>";
    vHtml += "</tr>";
    vHtml += "</table>";
    vHtml += "</div>";

    $(function () {
        $("#dialog").html(vHtml);
        $("#dialog").dialog({
            autoOpen: true,
            draggable: true,
            resizable: false,
            title: strTitle,
            closeOnEscape: true,
            buttons: {
                "確認": function () { $(this).dialog('close'); }
            },
            modal: true
            //hide: { effect: "blind", duration: 1000 }
        });
    });
}