$(document).ready(function () {

});

// 設定整體Div顯示
function SetDivMode(sMode) {
    switch (sMode) {
        case 'AREA':
            document.getElementById('ContentPlaceHolder1_Div_CtrlArea').style.display = "";
            UI_Title.style.background = "#3B5998";
            break;

        default:
            document.getElementById('ContentPlaceHolder1_Div_CtrlArea').style.display = "none";
            UI_Title.style.background = "#BBBBBB";
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

        } else if (noteItemType[parseInt(s)] == "FIRST" || noteItemType[parseInt(s)] == "SECOND" || noteItemType[parseInt(s)] == "THRID") {
            Block();
            SetDivMode("AREA");
            LoadAreaInfo(noteID[s]);
            SetAreaMode("Edit", '', '');
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
        LoadAreaInfo(sID);
        SetAreaMode('Edit', '', '');

        //sLoc = sUp * 19.5;
        //document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = Math.round(sLoc);

        ShowDialog('message', '設定', '新增資料成功！');
    }
    else if (sAct == 'Update') {
        SetDivMode(sClass);
        LoadAreaInfo(sID);
        SetAreaMode('Edit', '', '');

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
                case "UpdateAreaInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_AREA_Update_' + objRet.message);
                    break;

                case "InsertAreaInfo":
                    hfAreaID.value = objRet.message
                    AreaIDValue.value = objRet.message
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_AREA_Insert_' + objRet.message + '_' + objRet.sUp);
                    break;

                case "DeleteAreaInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_AREA_Delete_' + objRet.message);
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



/* Area相關操作 */
function SetAreaMode(sMode, tmpLevel , sPID) {

    switch (sMode) {
        case 'Add':
            Area_Input_No.value = '';
            Area_Input_Name.value = '';
            Area_Input_Desc.value = '';
            $('[name*="ddlManage"]').val("1");
            $('[name*="Area_Input_PList"]').val(sPID);

            if (tmpLevel == "SMS") {
                hfAreaLevel.value = "1";
            } else if (tmpLevel == "FIRST") {
                hfAreaLevel.value = "2";
            } else if (tmpLevel == "SECOND") {
                hfAreaLevel.value = "3";
            } else {
                hfAreaLevel.value = "9";
            }

            hfAreaID.value = '';
            hfAreaPID.value = '';
            AreaIDValue.value = '';

            Area_B_Add.style.display = "inline";
            Area_B_Edit.style.display = "none";
            Area_B_Delete.style.display = "none";

            break;
        case 'Edit':
        case 'Update':
            Area_B_Add.style.display = "none";
            Area_B_Edit.style.display = "inline";
            Area_B_Delete.style.display = "none";
            break;
        case 'Delete':
            Area_B_Add.style.display = "none";
            Area_B_Edit.style.display = "none";
            Area_B_Delete.style.display = "inline";
            break;
        default:
            break;
    }

}

// 讀取Location資料
function LoadAreaInfo(AreaID = 1) {
    PageMethods.LoadAreaInfo(AreaID, SetAreaInfoUI, onPageMethodsError);
}

//取得Location畫面資料
function GetAreaDataArray() {
    var DataArray = [];
    DataArray[0] = hfAreaID.value;            //識別碼
    DataArray[1] = Area_Input_No.value;       //編號
    DataArray[2] = Area_Input_Name.value;     //名稱
    DataArray[3] = Area_Input_PList.value;    //上層ID
    DataArray[4] = ddlManage.value;           //是否管制
    DataArray[5] = Area_Input_Desc.value;     //備註
    DataArray[6] = hfAreaLevel.value;         //層級
    DataArray[7] = Area_Input_PList.value;    //變更後的上層ID

    return DataArray;
}

//執行新增動作
function InsertAreaExcute() {
    var DataArray = GetAreaDataArray();
    console.log(DataArray);
    PageMethods.InsertAreaInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯動作
function UpdateAreaExcute() {
    var DataArray = GetAreaDataArray();
    console.log(DataArray);
    PageMethods.UpdateAreaInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除動作
function DeleteAreaExcute() {
    var DataArray = GetAreaDataArray();
    console.log(DataArray);
    PageMethods.DeleteAreaInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

function UpdatePListDDL() {
    PageMethods.LoadNewPList(UpdateDDL, onPageMethodsError);
}

function UpdateDDL(DataArray) {
    while (Area_Input_PList.options.length > 0)
        Area_Input_PList.options.remove(0);

    var DataStr = null;
    var option = null;

    option = document.createElement("option");
    option.text = "===最上層===";
    option.value = 0;
    Area_Input_PList.options.add(option);

    for (i = 0; i < DataArray.length; i++) {
        option = document.createElement("option");
        DataStr = DataArray[i].split("|");
        option.text = DataStr[1];
        option.value = DataStr[0];
        Area_Input_PList.options.add(option);
    }
}


// 讀取Location資料回填UI
function SetAreaInfoUI(DataArray) {

    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Area_Input_No.value = DataArray[1];
        Area_Input_Name.value = DataArray[2];

        $('[name*="ddlManage"]').val(DataArray[4]);
        Area_Input_Desc.value = DataArray[5];
        $('[name*="Area_Input_PList"]').val(DataArray[3]);

        hfAreaID.value = DataArray[0];  // 識別碼 CtrlAreaID
        hfAreaPID.value = DataArray[3];  // 上層ID
        AreaIDValue.value = DataArray[0];  // 識別碼 CtrlAreaID
        hfAreaLevel.value = DataArray[6];   //層級

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