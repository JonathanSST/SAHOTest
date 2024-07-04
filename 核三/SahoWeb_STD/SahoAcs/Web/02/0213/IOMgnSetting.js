$(document).ready(function () {
    $("#BtnCopyLight").click(function () {
        SetCopyLight();
    });


});

// 設定整體Div顯示
function SetDivMode(sMode) {
    switch (sMode) {
        case 'DeviceConnInfo':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "";
            document.getElementById('ContentPlaceHolder1_Div_IOMaster').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOModule').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Sensor').style.display = "none";

            DciUI_Title.style.background = "#3B5998";
            IOMasterUI_Title.style.background = "#BBBBBB";
            IOModuleUI_Title.style.background = "#BBBBBB";
            SensorUI_Title.style.background = "#BBBBBB";
            break;
        case 'IOMaster':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOMaster').style.display = "";
            document.getElementById('ContentPlaceHolder1_Div_IOModule').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Sensor').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            IOMasterUI_Title.style.background = "#3B5998";
            IOModuleUI_Title.style.background = "#BBBBBB";
            SensorUI_Title.style.background = "#BBBBBB";
            break;
        case 'IOModule':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOMaster').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOModule').style.display = "";
            document.getElementById('ContentPlaceHolder1_Div_Sensor').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            IOMasterUI_Title.style.background = "#BBBBBB";
            IOModuleUI_Title.style.background = "#3B5998";
            SensorUI_Title.style.background = "#BBBBBB";

            IomDDLCtrlK3.style.display = "none";
            IomDDLCtrlK3_Other.style.display = "";
            IomDDLCtrlK3.disabled = true
            IomDDLCtrlK3_Other.disabled = true

            break;
        case 'Sensor':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOMaster').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOModule').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Sensor').style.display = "";

            DciUI_Title.style.background = "#BBBBBB";
            IOMasterUI_Title.style.background = "#BBBBBB";
            IOModuleUI_Title.style.background = "#BBBBBB";
            SensorUI_Title.style.background = "#3B5998";
            break;
        default:
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOMaster').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_IOModule').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Sensor').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            IOMasterUI_Title.style.background = "#BBBBBB";
            IOModuleUI_Title.style.background = "#BBBBBB";
            SensorUI_Title.style.background = "#BBBBBB";
            break;
    }
}

// ParamDiv顯示
function SetParamDiv(sMode) {
    switch (sMode) {
        case 'IPParam':
            document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";
            //document.getElementById('ContentPlaceHolder1_ComPortParam').style.display = "none";
            break;
        case 'ComPortParam':
            //document.getElementById('ContentPlaceHolder1_IPParam').style.display = "none";
            //document.getElementById('ContentPlaceHolder1_ComPortParam').style.display = "";
            break;
    }
}

//function SetAddReaderDisplay(sMode) {
//    switch (sMode) {
//        case 'Add':
//            th_Controller_AddReader_1.style.display = "inline";
//            th_Controller_AddReader_2.style.display = "inline";
//            th_Controller_AddReader_3.style.display = "inline";
//            th_Controller_AddReader_4.style.display = "inline";

//            th_Controller_FwVer_1.style.display = "none";
//            th_Controller_FwVer_2.style.display = "none";
//            th_Controller_FwVer_3.style.display = "none";
//            th_Controller_FwVer_4.style.display = "none";
//            break;
//        case 'Edit':
//            th_Controller_AddReader_1.style.display = "none";
//            th_Controller_AddReader_2.style.display = "none";
//            th_Controller_AddReader_3.style.display = "none";
//            th_Controller_AddReader_4.style.display = "none";

//            th_Controller_FwVer_1.style.display = "none";
//            th_Controller_FwVer_2.style.display = "none";
//            th_Controller_FwVer_3.style.display = "none";
//            th_Controller_FwVer_4.style.display = "none";
//            break;
//        case 'Del':
//            th_Controller_AddReader_1.style.display = "none";
//            th_Controller_AddReader_2.style.display = "none";
//            th_Controller_AddReader_3.style.display = "none";
//            th_Controller_AddReader_4.style.display = "none";

//            th_Controller_FwVer_1.style.display = "none";
//            th_Controller_FwVer_2.style.display = "none";
//            th_Controller_FwVer_3.style.display = "none";
//            th_Controller_FwVer_4.style.display = "none";
//            break;
//    }
//}

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

    ind = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
    s = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");

    event_srcElement_id.value = s;

    var noteItemType = txt_NodeTypeList.value.split(',');
    var noteID = txt_NodeIDList.value.split(',');

    if (noteItemType[parseInt(s)] != "NONE") {
        if (noteItemType[parseInt(s)] == "SMS") {
            SetDivMode('');
            $.unblockUI();
        }
        else if (noteItemType[parseInt(s)] == "IODCI") {
            Block();
            SetDivMode("DeviceConnInfo");
            LoadDeviceConnInfo(noteID[s]);
            SetDeviceConnInfoMode("Edit");
        }
        else if (noteItemType[parseInt(s)] == "IOMASTER") {
            Block();
            SetDivMode("IOMaster");
            LoadIOMaster(noteID[s]);
            SetIOMasterMode("Edit");
        }
        else if (noteItemType[parseInt(s)] == "IOMODULE") {
            Block();
            SetDivMode("IOModule");
            SetIOModuleMode("Edit");
            LoadIOModule(noteID[s]);
        }
        else if (noteItemType[parseInt(s)] == "SENSOR") {
            Block();
            SetDivMode("Sensor");
            SetSensorMode("Edit");
            LoadSensor(noteID[s]);
        }
    }
}

//呼叫設備關鍵字搜尋
function CallSearch(Title) {
    ChangeText(L_popName1, Title);
    txtKeyWord.value = "";
    PopupTrigger1.click();
}

//關鍵字搜尋
//function SearchEquData() {
//    PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
//    CancelTrigger1.click();
//}

function LoadAffterAction(sClass, sAct, sID, sUp) {
    // 移動TreeView_Panel的scrollTop
    document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = TreeView_Panel_scrollTop.value;

    // sClass = DeviceConnInfo、Master、Controller、Reader
    // sAct = Insert、Update、Delete
    if (sAct == 'Insert') {
        SetDivMode(sClass);

        switch (sClass) {
            case "DeviceConnInfo":
                LoadDeviceConnInfo(sID);
                SetDeviceConnInfoMode('Edit');

                sLoc = sUp * 19.5;
                document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = Math.round(sLoc);

                ShowDialog('message', '連線', '新增連線資料成功！');
                break;
            case "IOMaster":
                LoadIOMaster(sID);
                SetIOMasterMode('Edit');

                sLoc = sUp * 19.5;
                document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = Math.round(sLoc);

                ShowDialog('message', 'I/O連線裝置', '新增I/O連線裝置資料成功！');
                break
            case "IOModule":
                CtrlIDValue.value = sID;
                LoadIOModule(sID);
                SetIOModuleMode('Edit');
                ShowDialog('message', 'I/O模組', '新增I/O模組資料成功！');
                break;
            case "Sensor":
                hfReaderID.value = sID;

                LoadSensor(sID);
                SetSensorMode('Edit');

                ShowDialog('message', '偵測器', '新增偵測器資料成功！');
                break;
        }
    }
    else if (sAct == 'Update') {
        SetDivMode(sClass);

        switch (sClass) {
            case "DeviceConnInfo":
                LoadDeviceConnInfo(sID);
                SetDeviceConnInfoMode('Edit');

                ShowDialog('message', '連線', '更新連線資料成功！');
                break;
            case "IOMaster":
                LoadIOMaster(sID);
                SetIOMasterMode('Edit');

                ShowDialog('message', 'I/O連線裝置', '更新I/O連線裝置資料成功！');
                break
            case "IOModule":
                LoadIOModule(sID);
                SetIOModuleMode('Edit');

                ShowDialog('message', 'I/O模組', '更新I/O模組資料成功！');
                break;
            case "Sensor":
                LoadSensor(sID);
                SetSensorMode('Edit');

                ShowDialog('message', '偵測器', '更新偵測器資料成功！');
                break;
        }
    }
    else if (sAct == 'Delete') {
        SetDivMode("");

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
                case "InsertDeviceConnInfo":
                    hfDciID.value = objRet.message;
                    DciIDValue.value = objRet.message;

                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_DeviceConnInfo_Insert_' + objRet.message + '_' + objRet.sUp);
                    break;
                case "UpdateDeviceConnInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_DeviceConnInfo_Update_' + objRet.message);
                    break;
                case "DeleteDeviceConnInfo":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_DeviceConnInfo_Delete_' + objRet.message);
                    break;
                case "InsertIOMaster":
                    IOMstIDValue.value = objRet.message;

                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOMaster_Insert_' + objRet.message + '_' + objRet.sUp);
                    break;
                case "UpdateIOMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOMaster_Update_' + objRet.message);
                    break;
                case "DeleteIOMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOMaster_Delete_' + objRet.message);
                    break;
                case "InsertIOModule":
                    CtrlIDValue.value = objRet.message;
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOModule_Insert_' + objRet.message);
                    break;
                case "UpdateIOModule":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOModule_Update_' + objRet.message);
                    break;
                case "DeleteIOModule":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_IOModule_Delete_' + objRet.message);
                    break;
                case "InsertSensor":
                    hfReaderID.value = objRet.message;
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Sensor_Insert_' + objRet.message);
                    break;
                case "UpdateSensor":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Sensor_Update_' + objRet.message);
                    break;
                case "DeleteSensor":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Sensor_Delete_' + objRet.message);
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

/*DeviceConnInfo操作相關------ Start ------*/

// DeviceConnInfo操作
function SetDeviceConnInfoMode(sMode) {
    switch (sMode) {
        case 'Add':
            Dci_No.value = ''          // 設備連線編碼　　DciNo
            Dci_Name.value = ''        // 設備連線名稱　　DciName
            Dci_PassWD.value = ''      // 連線密碼　　　　DciPassWD
            Dci_Ip.value = ''          // IP位置　　　　　IpAddress
            Dci_Port.value = ''        // 主動回傳端口　　TcpPort    
            Dci_IsAssign.value = '0'   // 限制IP位置　　　IsAssignIP

            Dci_No.disabled = false;          // 設備連線編碼　　DciNo
            Dci_Name.disabled = false;        // 設備連線名稱　　DciName
            Dci_PassWD.disabled = false;      // 連線密碼　　　　DciPassWD
            Dci_Ip.disabled = false;          // IP位置　　　　　IpAddress
            Dci_Port.disabled = false;        // 主動回傳端口　　TcpPort  
            Dci_IsAssign.disabled = false;    // 限制IP位置　　　IsAssignIP

            Dci_B_Add.style.display = "inline";
            Dci_B_Edit.style.display = "none";
            Dci_B_Delete.style.display = "none";

            break;
        case 'Edit':
            Dci_No.disabled = false;          // 設備連線編碼　　DciNo
            Dci_Name.disabled = false;        // 設備連線名稱　　DciName
            Dci_PassWD.disabled = false;      // 連線密碼　　　　DciPassWD
            Dci_Ip.disabled = false;          // IP位置　　　　　IpAddress
            Dci_Port.disabled = false;        // 主動回傳端口　　TcpPort  
            Dci_IsAssign.disabled = false;    // 限制IP位置　　　IsAssignIP

            Dci_B_Add.style.display = "none";
            Dci_B_Edit.style.display = "inline";
            Dci_B_Delete.style.display = "none";

            break;
        case 'Delete':
            Dci_No.disabled = true;          // 設備連線編碼　　DciNo
            Dci_Name.disabled = true;        // 設備連線名稱　　DciName
            Dci_PassWD.disabled = true;      // 連線密碼　　　　DciPassWD
            Dci_Ip.disabled = true;          // IP位置　　　　　IpAddress
            Dci_Port.disabled = true;        // 主動回傳端口　　TcpPort
            Dci_IsAssign.disabled = true;    // 限制IP位置　　　IsAssignIP

            Dci_B_Add.style.display = "none";
            Dci_B_Edit.style.display = "none";
            Dci_B_Delete.style.display = "inline";

            break;
        default:
            break;
    }
}

// 讀取DeviceConnInfo資料
function LoadDeviceConnInfo(DciID) {
    PageMethods.LoadDeviceConnInfo(DciID, SetDeviceConnInfoUI, onPageMethodsError);
}

// 讀取DeviceConnInfo資料回填UI
function SetDeviceConnInfoUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        Dci_No.value = DataArray[0]          // 設備連線編碼　　DciNo
        Dci_Name.value = DataArray[1]        // 設備連線名稱　　DciName
        Dci_PassWD.value = DataArray[5]      // 連線密碼　　　　DciPassWD
        Dci_Ip.value = DataArray[3]          // IP位置　　　　　IpAddress
        Dci_Port.value = DataArray[4]        // 主動回傳端口　　TcpPort    

        // 限制IP位置　　　IsAssignIP
        for (var j = 0; j < Dci_IsAssign.options.length; j++) {
            if (Dci_IsAssign.options[j].value == DataArray[2]) {
                Dci_IsAssign.options[j].selected = true;
            }
            else {
                Dci_IsAssign.options[j].selected = false;
            }
        }

        hfDciID.value = DataArray[6]        // 設備連線識別碼　　DciID  
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

//取得DeviceConnInfo畫面資料
function GetDciDataArray() {
    var DataArray = [];
    DataArray[0] = Dci_No.value;        // 設備連線編碼　　DciNo
    DataArray[1] = Dci_Name.value;      // 設備連線名稱　　DciName
    DataArray[2] = Dci_IsAssign.value;  // 限制IP位置　　　IsAssignIP
    DataArray[3] = Dci_Ip.value;        // IP位置　　　　　IpAddress
    DataArray[4] = Dci_Port.value;      // 主動回傳端口　　TcpPort    
    DataArray[5] = Dci_PassWD.value;    // 連線密碼　　　　DciPassWD
    DataArray[6] = hfDciID.value;       // 設備連線識別碼　DciID 

    return DataArray;
}

//執行新增DeviceConnInfo動作
function InsertDciExcute() {
    var DataArray = GetDciDataArray();
    PageMethods.InsertDeviceConnInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯DeviceConnInfo動作
function UpdateDciExcute() {
    var DataArray = GetDciDataArray();
    PageMethods.UpdateDeviceConnInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除DeviceConnInfo動作
function DeleteDciExcute() {
    var DataArray = GetDciDataArray();
    PageMethods.DeleteDeviceConnInfo(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

/*DeviceConnInfo操作相關------ End   ------*/


/*Master操作相關------ Start ------*/
// Master操作
function SetIOMasterMode(sMode) {
    switch (sMode) {
        case 'Add':
            IOMaster_Input_No.value = '';
            IOMaster_Input_Name.value = '';
            IOMaster_Input_Dci.innerText = '';
            IOMaster_Input_IPParam_IP.value = '';
            IOMaster_Input_IPParam_Port.value = '';

            document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";

            IOMaster_Input_Model.value = '';
            IOMaster_Input_Status.value = '1';
            IOMaster_Input_CtrlModel.value = '';
            IOMaster_Input_AutoRerun_N.checked = true;

            IOMaster_Input_No.disabled = false;
            IOMaster_Input_Name.disabled = false;
            IOMaster_Input_IPParam_IP.disabled = false;
            IOMaster_Input_IPParam_Port.disabled = false;
            IOMaster_Input_Model.disabled = false;
            IOMaster_Input_Status.disabled = false;
            IOMaster_Input_CtrlModel.disabled = false;
            IOMasterDDLArea.disabled = false;
            IOMasterDDLBuilding.disabled = false;
            IOMasterDDLFloor.disabled = false;

            IOMaster_Input_AutoRerun_Y.disabled = false;
            IOMaster_Input_AutoRerun_N.disabled = false;
            IOMaster_B_Add.style.display = "inline";
            IOMaster_B_Edit.style.display = "none";
            IOMaster_B_Delete.style.display = "none";

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            //SetLocationItem
            SetLocationListItem();

            break;
        case 'Edit':
            IOMaster_Input_IPParam_IP.value = '';
            IOMaster_Input_IPParam_Port.value = '';

            IOMaster_Input_No.disabled = false;
            IOMaster_Input_Name.disabled = false;
            IOMaster_Input_IPParam_IP.disabled = false;
            IOMaster_Input_IPParam_Port.disabled = false;
            IOMaster_Input_Model.disabled = false;
            IOMaster_Input_Status.disabled = false;
            IOMaster_Input_AutoRerun_Y.disabled = false;
            IOMaster_Input_AutoRerun_N.disabled = false;

            IOMaster_B_Add.style.display = "none";
            IOMaster_B_Edit.style.display = "inline";
            IOMaster_B_Delete.style.display = "none";
            break;
        case 'Delete':

            IOMaster_Input_IPParam_IP.value = '';
            IOMaster_Input_IPParam_Port.value = '';

            IOMaster_Input_No.disabled = true;
            IOMaster_Input_Name.disabled = true;
            IOMaster_Input_IPParam_IP.disabled = true;
            IOMaster_Input_IPParam_Port.disabled = true;
            IOMaster_Input_Model.disabled = true;
            IOMaster_Input_Status.disabled = true;
            IOMaster_Input_CtrlModel.disabled = true;
            IOMaster_Input_AutoRerun_Y.disabled = true;
            IOMaster_Input_AutoRerun_N.disabled = true;

            IOMaster_B_Add.style.display = "none";
            IOMaster_B_Edit.style.display = "none";
            IOMaster_B_Delete.style.display = "inline";
            break;
        default:
            break;
    }
}

// 呼叫Master新增視窗
function CallCreateIOMaster(FromDciID) {
    PageMethods.GetDciInfo(FromDciID, SetCreateIOMasterUI, onPageMethodsError);
}

// CreateMasterUI將資料帶回畫面
function SetCreateIOMasterUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        DciIDValue.value = DataArray[0];
        IOMaster_Input_Dci.innerText = DataArray[1];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
    $.unblockUI();
}

// 讀取Master資料
function LoadIOMaster(IOMstID) {
    PageMethods.LoadIOMaster(IOMstID, SetIOMasterUI, onPageMethodsError);
}

// 讀取Master資料回填UI
function SetIOMasterUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        IOMaster_Input_No.value = DataArray[0];               // 連線裝置編號
        IOMaster_Input_Name.value = DataArray[1];             // 連線裝置說明
        IOMaster_Input_Dci.innerText = DataArray[2];          // 使用連線

        document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";

        // 判斷IPV4或IPV6
        var IpArray = DataArray[3].split(':');

        if (IpArray.length > 2) {
            IpArray = DataArray[3].split(',');
        }

        IOMaster_Input_IPParam_IP.value = IpArray[0];         // IP
        IOMaster_Input_IPParam_Port.value = IpArray[1];       // PORT

        // 自動回傳
        if (DataArray[4] == "1") {
            IOMaster_Input_AutoRerun_Y.checked = true;
        }
        else if (DataArray[4] == "0") {
            IOMaster_Input_AutoRerun_N.checked = true;
        }

        IOMasterDDLArea.value = DataArray[5];           //區域
        sLocArea.value = DataArray[5];
        tmpLocArea.value = DataArray[5];

        IOMasterDDLBuilding.value = DataArray[6];       //棟別
        sLocBuilding.value = DataArray[6];
        tmpLocBuilding.value = DataArray[6];

        IOMasterDDLFloor.value = DataArray[7];          //樓層
        sLocFloor.value = DataArray[7];

        IOMaster_Input_Model.value = DataArray[8];            // 連線裝置機型
        IOMaster_Input_Status.value = DataArray[9];           // 狀態
        IOMaster_Input_CtrlModel.value = DataArray[10];        // 控制器機型

        DciIDValue.value = DataArray[11];                   // 暫存 DciID
        IOMstIDValue.value = DataArray[12];                   // 暫存 MstID

        if (DataArray[10] !== "0") {
            IOMaster_Input_CtrlModel.disabled = true;
        }
        else {
            IOMaster_Input_CtrlModel.disabled = false;
        }

        SetLocationListItem();
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

//取得Master畫面資料
function GetIOMasterDataArray() {
    var DataArray = [];
    DataArray[0] = IOMaster_Input_No.value;           // 連線裝置編號
    DataArray[1] = IOMaster_Input_Name.value;         // 連線裝置名稱
    DataArray[2] = IOMaster_Input_Dci.innerText;      // 使用連線

    // IP + PORT
    DataArray[3] = IOMaster_Input_IPParam_IP.value + "_" + IOMaster_Input_IPParam_Port.value;

    //自動回傳
    if (IOMaster_Input_AutoRerun_Y.checked) {
        DataArray[4] = "1";
    }
    else if (IOMaster_Input_AutoRerun_N.checked) {
        DataArray[4] = "0";
    }

    DataArray[5] = IOMasterDDLArea.value;    //區域
    DataArray[6] = IOMasterDDLBuilding.value;    //棟別
    DataArray[7] = IOMasterDDLFloor.value;    //樓層


    DataArray[8] = IOMaster_Input_Model.value;        // 連線裝置機型
    DataArray[9] = IOMaster_Input_Status.value;       // 狀態
    DataArray[10] = IOMaster_Input_CtrlModel.value;    // 控制器機型

    DataArray[11] = DciIDValue.value;               // 暫存 DciID
    DataArray[12] = IOMstIDValue.value;               // 暫存 MstID

    return DataArray;
}

//執行新增Master動作
function InsertIOMasterExcute() {
    var DataArray = GetIOMasterDataArray();
    PageMethods.InsertIOMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Master動作
function UpdateIOMasterExcute() {
    var DataArray = GetIOMasterDataArray();
    PageMethods.UpdateIOMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Master動作
function DeleteIOMasterExcute() {
    var DataArray = GetIOMasterDataArray();
    PageMethods.DeleteIOMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}
/*Master操作相關------ End ------*/

/*IOModule操作相關------ Start ------*/
// IOModule操作
function SetIOModuleMode(sMode) {
    $("#BtnCopyLight").hide();
    switch (sMode) {
        case 'Add':
            Iom_Input_No.value = '';
            Iom_Input_Name.value = '';
            Iom_Input_Addr.value = '1';
            Iom_Input_Ctrl.innerText = '';
            ContentPlaceHolder1_Iom_Input_Usage_0.checked = true;
            //ContentPlaceHolder1_Iom_Input_Usage_1.checked = false;
            Iom_Input_IOMst.innerText = '';
            Iom_Input_Status.value = '';
            Iom_Input_Desc.value = '';
            Iom_Input_Bits.value = '16';
            IomDDLCtrlK3.value = '';

            Iom_Input_No.disabled = false;
            Iom_Input_Name.disabled = false;
            Iom_Input_Addr.disabled = false;
            IomDDLCtrlK3.disabled = true
            IomDDLCtrlK3_Other.disabled = true
            Iom_Input_Bits.disabled = false;
            IomDDLArea.disabled = false;
            IomDDLBuilding.disabled = false;
            IomDDLFloor.disabled = false;
            Iom_Input_Status.disabled = false;
            Iom_Input_Desc.disabled = false;
            ContentPlaceHolder1_Iom_Input_Usage_0.disabled = false;
            //ContentPlaceHolder1_Iom_Input_Usage_1.disabled = false;

            Iom_B_Add.style.display = "inline";
            Iom_B_Edit.style.display = "none";
            Iom_B_Delete.style.display = "none";

            CtrlParaButton.style.display = "none";
            SetButton.style.display = "none";

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            SetLocationListItem();

            break;
        case 'Edit':
            $("#BtnCopyLight").hide();

            Iom_Input_No.value = '';
            Iom_Input_Name.value = '';
            Iom_Input_Addr.value = '1';
            Iom_Input_Ctrl.innerText = '';
            ContentPlaceHolder1_Iom_Input_Usage_0.checked = true;
            //ContentPlaceHolder1_Iom_Input_Usage_1.checked = false;
            Iom_Input_IOMst.innerText = '';
            Iom_Input_Status.value = '';
            Iom_Input_Desc.value = '';
            Iom_Input_Bits.value = '';
            IomDDLCtrlK3.value = '';

            Iom_Input_No.disabled = false;
            Iom_Input_Name.disabled = false;
            Iom_Input_Addr.disabled = false;
            IomDDLCtrlK3.disabled = true
            IomDDLCtrlK3_Other.disabled = true
            Iom_Input_Bits.disabled = true;
            IomDDLArea.disabled = false;
            IomDDLBuilding.disabled = false;
            IomDDLFloor.disabled = false;
            Iom_Input_Status.disabled = false;
            Iom_Input_Desc.disabled = false;

            ContentPlaceHolder1_Iom_Input_Usage_0.disabled = true;
            //ContentPlaceHolder1_Iom_Input_Usage_1.disabled = true;

            Iom_B_Add.style.display = "none";
            Iom_B_Edit.style.display = "inline";
            Iom_B_Delete.style.display = "none";

            CtrlParaButton.style.display = "none";
            SetButton.style.display = "none";


            break;
        case 'Delete':
            $("#BtnCopyLight").hide();

            Iom_Input_No.value = '';
            Iom_Input_Name.value = '';
            Iom_Input_Addr.value = '1';
            Iom_Input_Ctrl.innerText = '';
            ContentPlaceHolder1_Iom_Input_Usage_0.checked = true;
            //ContentPlaceHolder1_Iom_Input_Usage_1.checked = false;
            Iom_Input_IOMst.innerText = '';
            Iom_Input_Status.value = '';
            Iom_Input_Desc.value = '';

            Iom_Input_No.disabled = true;
            Iom_Input_Name.disabled = true;
            Iom_Input_Addr.disabled = true;
            IomDDLCtrlK3.disabled = true
            IomDDLCtrlK3_Other.disabled = true
            Iom_Input_Bits.disabled = true;
            IomDDLArea.disabled = true;
            IomDDLBuilding.disabled = true;
            IomDDLFloor.disabled = true;
            Iom_Input_Status.disabled = true;
            Iom_Input_Desc.disabled = true;

            ContentPlaceHolder1_Iom_Input_Usage_0.disabled = true;
            //ContentPlaceHolder1_Iom_Input_Usage_1.disabled = true;

            Iom_B_Add.style.display = "none";
            Iom_B_Edit.style.display = "none";
            Iom_B_Delete.style.display = "inline";

            CtrlParaButton.style.display = "none";
            SetButton.style.display = "none";



            break;
        default:
            break;
    }
}

// 呼叫IOModule新增視窗
function CallCreateIOModule(FromDciID) {
    PageMethods.GetIOMstInfo(FromDciID, SetCreateIOModuleUI, onPageMethodsError);
}

// CreateControllerUI將資料帶回畫面
function SetCreateIOModuleUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        DciIDValue.value = DataArray[0]
        IOMstIDValue.value = DataArray[2];
        Iom_Input_Ctrl.innerText = DataArray[4];
        Iom_Input_IOMst.innerText = DataArray[1] + " / " + DataArray[3];

    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

// 讀取IOModule資料
function LoadIOModule(IomID) {
    PageMethods.LoadIOModule(hideUserID.value, IomID, SetIOModuleUI, onPageMethodsError);
}

// 讀取IOModule資料回填UI
function SetIOModuleUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Iom_Input_No.value = DataArray[0];
        Iom_Input_Name.value = DataArray[1];
        Iom_Input_Addr.value = DataArray[2];
        Iom_Input_Ctrl.innerText = DataArray[3];

        ContentPlaceHolder1_Iom_Input_Usage_0.checked = true;
        //if (DataArray[4] == 0) {
        //    ContentPlaceHolder1_Iom_Input_Usage_0.checked = true;
        //    ContentPlaceHolder1_Iom_Input_Usage_1.checked = false;
        //} else if (DataArray[4] == 1) {
        //    ContentPlaceHolder1_Iom_Input_Usage_0.checked = false;
        //    ContentPlaceHolder1_Iom_Input_Usage_1.checked = true;
        //}

        Iom_Input_IOMst.innerText = DataArray[5];

        for (i = 0; i < IomDDLCtrlK3.options.length; i++) {
            if (IomDDLCtrlK3.options[i].value == DataArray[6]) {
                IomDDLCtrlK3.options[i].selected = true;
            }
            else {
                IomDDLCtrlK3.options[i].selected = false;
            }
        }

        Iom_Input_Bits.value = DataArray[7];
        sLocArea.value = DataArray[8];
        tmpLocArea.value = DataArray[8];
        sLocBuilding.value = DataArray[9];
        tmpLocBuilding.value = DataArray[9];
        sLocFloor.value = DataArray[10];

        Iom_Input_Status.value = DataArray[11];
        Iom_Input_Desc.value = DataArray[12];

        DciIDValue.value = DataArray[13];
        IOMstIDValue.value = DataArray[14];
        IOModuleIDValue.value = DataArray[15];

        SetLocationListItem();

    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

// 取得IOModule畫面資料
function GetIOModuleDataArray() {
    var DataArray = [];

    DataArray[0] = Iom_Input_No.value;               // IO編號
    DataArray[1] = Iom_Input_Name.value;             // IO名稱
    DataArray[2] = Iom_Input_Addr.value;             // 機號
    DataArray[3] = Iom_Input_Ctrl.innerText;         // 設備機型

    // IO用途
    DataArray[4] = "0";
    //if (ContentPlaceHolder1_Iom_Input_Usage_0.checked) {
    //    DataArray[4] = "0";
    //}
    //else if (ContentPlaceHolder1_Iom_Input_Usage_1.checked) {
    //    DataArray[4] = "1";
    //}

    DataArray[5] = IOMstIDValue.value;               // IO連線裝置
    DataArray[6] = IomDDLCtrlK3.value;               // 控制器ID
    DataArray[7] = Iom_Input_Bits.value;             // 接點數
    DataArray[8] = IomDDLArea.value;                 // 區域
    DataArray[9] = IomDDLBuilding.value;             // 棟別
    DataArray[10] = IomDDLFloor.value;               // 樓層
    DataArray[11] = Iom_Input_Status.value;          // 狀態
    DataArray[12] = Iom_Input_Desc.value;            // 說明
    DataArray[13] = DciIDValue.value;                // DciID
    DataArray[14] = IOMstIDValue.value;              // IOMasterID
    DataArray[15] = IOModuleIDValue.value            // IOMID

    return DataArray;
}

//執行新增IOModule動作
function InsertIOModuleExcute() {
    var DataArray = GetIOModuleDataArray();
    PageMethods.InsertIOModule(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯IOModule動作
function UpdateIOModuleExcute() {
    var DataArray = GetIOModuleDataArray();
    PageMethods.UpdateIOModule(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除IOModule動作
function DeleteIOModuleExcute() {
    var DataArray = GetIOModuleDataArray();
    PageMethods.DeleteIOModule(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}
/*IOModule操作相關------ End ------*/

/*Sensor操作相關------ Start ------*/
//Sendor操作
function SetSensorMode(sMode) {
    switch (sMode) {
        case 'Add':
            Sen_Input_No.value = '';
            Sen_Input_Name.value = '';
            Sen_Input_Bit.value = '';
            Sen_Input_ActiveSignal.value = '';
            SenDDLAlmType.value = '';
            Sen_Input_Status.value = '';
            Sen_Input_Desc.value = '';
            Sen_Input_AlmSeconds.value = '';

            Sen_Input_No.disabled = false;
            Sen_Input_Name.disabled = false;
            Sen_Input_Bit.disabled = false;
            Sen_Input_ActiveSignal.disabled = false;
            SenDDLAlmType.disabled = false;
            SenDDLArea.disabled = false;
            SenDDLBuilding.disabled = false;
            SenDDLFloor.disabled = false;
            Sen_Input_Status.disabled = false;
            Sen_Input_Desc.disabled = false;
            Sen_Input_AlmSeconds.disabled = false;

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            Sen_B_Add.style.display = "inline";
            Sen_B_Edit.style.display = "none";
            Sen_B_Delete.style.display = "none";
            ParaButton.style.display = "none";

            break;
        case 'Edit':

            Sen_Input_No.value = '';
            Sen_Input_Name.value = '';
            Sen_Input_Bit.value = '';
            Sen_Input_ActiveSignal.value = '';
            SenDDLAlmType.value = '';
            Sen_Input_Status.value = '';
            Sen_Input_Desc.value = '';
            Sen_Input_AlmSeconds.value = '';

            Sen_Input_No.disabled = false;
            Sen_Input_Name.disabled = false;
            Sen_Input_Bit.disabled = false;
            Sen_Input_ActiveSignal.disabled = false;
            SenDDLAlmType.disabled = false;
            SenDDLArea.disabled = false;
            SenDDLBuilding.disabled = false;
            SenDDLFloor.disabled = false;
            Sen_Input_Status.disabled = false;
            Sen_Input_Desc.disabled = false;
            Sen_Input_AlmSeconds.disabled = false;

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            Sen_B_Add.style.display = "none";
            Sen_B_Edit.style.display = "inline";
            Sen_B_Delete.style.display = "none";
            ParaButton.style.display = "none";

            break;
        case 'Delete':

            Sen_Input_No.value = '';
            Sen_Input_Name.value = '';
            Sen_Input_Bit.value = '';
            Sen_Input_ActiveSignal.value = '';
            SenDDLAlmType.value = '';
            Sen_Input_Status.value = '';
            Sen_Input_Desc.value = '';
            Sen_Input_AlmSeconds.value = '';

            Sen_Input_No.disabled = true;
            Sen_Input_Name.disabled = true;
            Sen_Input_Bit.disabled = true;
            Sen_Input_ActiveSignal.disabled = true;
            SenDDLAlmType.disabled = true;
            SenDDLArea.disabled = true;
            SenDDLBuilding.disabled = true;
            SenDDLFloor.disabled = true;
            Sen_Input_Status.disabled = true;
            Sen_Input_Desc.disabled = true;
            Sen_Input_AlmSeconds.disabled = true;

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            Sen_B_Add.style.display = "none";
            Sen_B_Edit.style.display = "none";
            Sen_B_Delete.style.display = "inline";
            ParaButton.style.display = "none";

            break;
        default:
            break;
    }
}

//呼叫Sensor新增視窗
function CallCreateSensor(FromIOMID) {
    PageMethods.GetIOMInfo(FromIOMID, SetCreateSensorUI, onPageMethodsError);
}

//CreateSenSorUI將資料帶回畫面
function SetCreateSensorUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Sen_Input_No.value = DataArray[3] + "_" + String(DataArray[5]).padStart(2, "0");
        Sen_Input_Name.value = DataArray[3] + "_" + String(DataArray[5]).padStart(2, "0");
        Sen_Input_Bit.value = DataArray[5];
        Sen_Input_Status.value = DataArray[9];
        Sen_Input_ActiveSignal.value = "1";
        Sen_Input_AlmSeconds.value = "10";

        for (i = 0; i < SenDDLAlmType.options.length; i++) {
            if (SenDDLAlmType.options[i].value == DataArray[4]) {
                SenDDLAlmType.options[i].selected = true;
            }
            else {
                SenDDLAlmType.options[i].selected = false;
            }
        }

        DciIDValue.value = DataArray[0];
        IOMstIDValue.value = DataArray[1];
        IOModuleIDValue.value = DataArray[2];
        sLocArea.value = DataArray[6];
        tmpLocArea.value = DataArray[6];
        sLocBuilding.value = DataArray[7];
        tmpLocBuilding.value = DataArray[7];
        sLocFloor.value = DataArray[8];

        SetSenLocListItem();
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

//讀取Sensor資料
function LoadSensor(SenID) {
    PageMethods.LoadSensor(hideUserID.value, SenID, SetSensorUI, onPageMethodsError);
}

//讀取Sensor資料回填UI
function SetSensorUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Sen_Input_No.value = DataArray[0];
        Sen_Input_Name.value = DataArray[1];
        Sen_Input_Bit.value = DataArray[2];
        Sen_Input_ActiveSignal.value = DataArray[3];

        for (i = 0; i < SenDDLAlmType.options.length; i++) {
            if (SenDDLAlmType.options[i].value == DataArray[4]) {
                SenDDLAlmType.options[i].selected = true;
            }
            else {
                SenDDLAlmType.options[i].selected = false;
            }
        }

        Sen_Input_AlmSeconds.value = DataArray[14];
        Sen_Input_Status.value = DataArray[8];
        Sen_Input_Desc.value = DataArray[9];

        DciIDValue.value = DataArray[10];
        IOMstIDValue.value = DataArray[11];
        IOModuleIDValue.value = DataArray[12];
        SenIDValue.value = DataArray[13];

        sLocArea.value = DataArray[5];
        tmpLocArea.value = DataArray[5];
        sLocBuilding.value = DataArray[6];
        tmpLocBuilding.value = DataArray[6];
        sLocFloor.value = DataArray[7];

        SetSenLocListItem();

    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

//取得Sensor畫面資料
function GetSensorDataArray() {
    var DataArray = [];

    DataArray[0] = Sen_Input_No.value;
    DataArray[1] = Sen_Input_Name.value;
    DataArray[2] = Sen_Input_Bit.value;
    DataArray[3] = Sen_Input_ActiveSignal.value;
    DataArray[4] = SenDDLAlmType.value;
    DataArray[5] = SenDDLArea.value;
    DataArray[6] = SenDDLBuilding.value;
    DataArray[7] = SenDDLFloor.value;
    DataArray[8] = Sen_Input_Status.value;
    DataArray[9] = Sen_Input_Desc.value;
    DataArray[10] = DciIDValue.value;
    DataArray[11] = IOMstIDValue.value;
    DataArray[12] = IOModuleIDValue.value;
    DataArray[13] = SenIDValue.value;
    DataArray[14] = Sen_Input_AlmSeconds.value;

    return DataArray;
}
//執行新增Sensor動作
function InsertSensorExcute() {
    var DataArray = GetSensorDataArray();
    PageMethods.InsertSensor(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Sensor動作
function UpdateSensorExcute() {
    var DataArray = GetSensorDataArray();
    PageMethods.UpdateSensor(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Sensor動作
function DeleteSensorExcute() {
    var DataArray = GetSensorDataArray();
    PageMethods.DeleteSensor(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//動態產生DropDownList
function CreateDDL(DataArray) {
    while (Reader_Input_EquNo.options.length > 0)
        Reader_Input_EquNo.options.remove(0);

    var DataStr = null;
    var option = null;

    option = document.createElement("option");
    option.text = "- 請選擇 -";
    option.value = "";
    Reader_Input_EquNo.options.add(option);

    for (i = 0; i < DataArray.length; i++) {
        option = document.createElement("option");
        DataStr = DataArray[i].split("/");
        option.text = DataStr[0];
        option.value = DataStr[1];
        Reader_Input_EquNo.options.add(option);
    }

    PageMethods.LoadSensor(ClickItem.value, SetSensorUI, onPageMethodsError);
}

//動態產生DropDownList不回傳Reader
function UpdateDDL(DataArray) {
    while (Reader_Input_EquNo.options.length > 0)
        Reader_Input_EquNo.options.remove(0);

    var DataStr = null;
    var option = null;

    option = document.createElement("option");
    option.text = "- 請選擇 -";
    option.value = "";
    Reader_Input_EquNo.options.add(option);

    for (i = 0; i < DataArray.length; i++) {
        option = document.createElement("option");
        DataStr = DataArray[i].split("/");
        option.text = DataStr[0];
        option.value = DataStr[1];
        Reader_Input_EquNo.options.add(option);
    }
}

/*Sensor操作相關------ End ------*/


// 呼叫參數設定視窗
function CallParaSetting() {
    //var sURL = "../../ParaSetting.aspx?EquNo=" + txtEquNo.value + "&ParaType=Reader";
    //var vArguments = "";
    //var sFeatures = "dialogHeight:440px;dialogWidth:730px;center:yes;scroll:no;";
    //window.showModalDialog(sURL, vArguments, sFeatures);
    $.post("/Web/02/ParaSettingBox.aspx",
        { "EquNo": txtEquNo.value, "ParaType": "Reader" },
        function (data) {
            OverlayContent(data);
        });
}

// 呼叫參數設定視窗
function CallCtrlParaSetting() {
    //var sURL = "../../ParaSetting.aspx?CtrlNo=" + Iom_Input_No.value + "&ParaType=Controller";
    //var vArguments = "";
    //var sFeatures = "dialogHeight:440px;dialogWidth:730px;center:yes;scroll:no;";
    //window.showModalDialog(sURL, vArguments, sFeatures);    
    $.post("/Web/02/ParaSettingBox.aspx",
        { "EquNo": Iom_Input_No.value, "ParaType": "Controller" },
        function (data) {
            OverlayContent(data);
        });
}


// 新增列印QrCode的功能
function SetPrintQR() {
    var EquNo = txtEquNo.value;
    //alert(EquNo);
    window.open("QRCodeReport.aspx?EquNo=" + EquNo);
}

// 新增列印QrCode的功能
function SetPrintQR1() {
    var EquNo = txtEquNo.value;
    //alert(EquNo);
    window.open("ReaderQRCode.aspx?EquNo=" + EquNo);
}

// 呼叫參數設定視窗 2
function CallParaSettingPage() {

    $.ajax({
        type: 'POST',
        url: '../../ParaSetting.aspx',
        data:
        {
            'EquNo': txtEquNo.value,
            'RemotePageCall': 'EquMapSetting3.aspx',    // 不在同一層資料呼叫頁面，發起點為所傳的值 
        },
        async: false,
        success: function (data) {

            $(document.body).append('<div id="baseOverlay00" style="position:absolute; top:0; left:0; z-index:1000; overflow:hidden;opacity:0.9;'
                + ' -webkit-transform: translate3d(0,0,0);width:100%;height:100%;background:#fff"></div>');
            $(document.body).append('<div id="ParaDiv00" style="position:absolute;left:20px;top:30px;z-index:1001;background-color:#1275BC" ></div>');

            $('#baseOverlay00').height($(document).height());

            $("#ParaDiv00").html(data);
            $("#ParaDiv00").css("left", ($(document).width() - $("#ParaDiv00").width()) / 2);
            $("#ParaDiv00").css("top", $(document).scrollTop() + 50);

            $("#popB_Cancel").click(function () {
                $("#ParaDiv00").remove();
                $("#baseOverlay00").remove();
            });
        }
    });

    return false;

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




//20171129加入的，for chrome使用
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


function ChangeIOUsage() {
    IomDDLCtrlK3.style.display = "none";
    IomDDLCtrlK3_Other.style.display = "";
    Iom_Input_Bits.value = "16";
    //if (ContentPlaceHolder1_Iom_Input_Usage_0.checked) {
    //    IomDDLCtrlK3.style.display = "none";
    //    IomDDLCtrlK3_Other.style.display = "";
    //    Iom_Input_Bits.value = "16";
    //} else if (ContentPlaceHolder1_Iom_Input_Usage_1.checked) {
    //    IomDDLCtrlK3.style.display = "";
    //    IomDDLCtrlK3_Other.style.display = "none";
    //    Iom_Input_Bits.value = "3";
    //}
}


function SetLocationListItem() {
    //IOMaster
    while (IOMasterDDLArea.options.length > 0)
        IOMasterDDLArea.options.remove(0);

    while (IOMasterDDLBuilding.options.length > 0)
        IOMasterDDLBuilding.options.remove(0);

    while (IOMasterDDLFloor.options.length > 0)
        IOMasterDDLFloor.options.remove(0);

    //IOModule
    while (IomDDLArea.options.length > 0)
        IomDDLArea.options.remove(0);

    while (IomDDLBuilding.options.length > 0)
        IomDDLBuilding.options.remove(0);

    while (IomDDLFloor.options.length > 0)
        IomDDLFloor.options.remove(0);

    //IOMaster
    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLFloor.options.add(option);

    //IOModule
    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLFloor.options.add(option);

    var AreaItem = txtAreaList.value.split(',');
    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");
        //IOMaster
        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        IOMasterDDLArea.options.add(option);

        //IOModule
        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        IomDDLArea.options.add(option);
    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == tmpLocArea.value) {
            //IOMaster
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IOMasterDDLBuilding.options.add(option);

            //IOModule
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IomDDLBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == tmpLocBuilding.value) {
            //IOMaster
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IOMasterDDLFloor.options.add(option);

            //IOModule
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IomDDLFloor.options.add(option);
        }
    }

    //IOMaster
    for (i = 0; i < IOMasterDDLArea.options.length; i++) {
        IOMasterDDLArea.options[i].selected = false;

        if (IOMasterDDLArea.options[i].value == sLocArea.value) {
            IOMasterDDLArea.options[i].selected = true;
        }
    }
    for (i = 0; i < IOMasterDDLBuilding.options.length; i++) {
        IOMasterDDLBuilding.options[i].selected = false;

        if (IOMasterDDLBuilding.options[i].value == sLocBuilding.value) {
            IOMasterDDLBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < IOMasterDDLFloor.options.length; i++) {
        IOMasterDDLFloor.options[i].selected = false;

        if (IOMasterDDLFloor.options[i].value == sLocFloor.value) {
            IOMasterDDLFloor.options[i].selected = true;
        }
    }

    //IOModule
    for (i = 0; i < IomDDLArea.options.length; i++) {
        IomDDLArea.options[i].selected = false;

        if (IomDDLArea.options[i].value == sLocArea.value) {
            IomDDLArea.options[i].selected = true;
        }
    }
    for (i = 0; i < IomDDLBuilding.options.length; i++) {
        IomDDLBuilding.options[i].selected = false;

        if (IomDDLBuilding.options[i].value == sLocBuilding.value) {
            IomDDLBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < IomDDLFloor.options.length; i++) {
        IomDDLFloor.options[i].selected = false;

        if (IomDDLFloor.options[i].value == sLocFloor.value) {
            IomDDLFloor.options[i].selected = true;
        }
    }
}

function SelectIOMstArea() {
    tmpLocArea.value = IOMasterDDLArea.value;
    SetLocBuildingListItem();
    $.unblockUI();
}

function SelectIomArea() {
    tmpLocArea.value = IomDDLArea.value;
    SetIomLocBuildingListItem();
    $.unblockUI();
}

function SelectIOMstBuilding() {
    tmpLocArea.value = IOMasterDDLArea.value;
    tmpLocBuilding.value = IOMasterDDLBuilding.value;
    SetLocFloorListItem();
    $.unblockUI();
}

function SelectIomBuilding() {
    tmpLocArea.value = IomDDLArea.value;
    tmpLocBuilding.value = IomDDLBuilding.value;
    SetIomLocFloorListItem();
    $.unblockUI();
}

function SetLocBuildingListItem() {
    while (IOMasterDDLBuilding.options.length > 0)
        IOMasterDDLBuilding.options.remove(0);

    while (IOMasterDDLFloor.options.length > 0)
        IOMasterDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLFloor.options.add(option);

    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IOMasterDDLBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IOMasterDDLFloor.options.add(option);
        }
    }
}

function SetIomLocBuildingListItem() {
    while (IomDDLBuilding.options.length > 0)
        IomDDLBuilding.options.remove(0);

    while (IomDDLFloor.options.length > 0)
        IomDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLFloor.options.add(option);

    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IomDDLBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IomDDLFloor.options.add(option);
        }
    }
}

function SetLocFloorListItem() {
    while (IOMasterDDLFloor.options.length > 0)
        IOMasterDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IOMasterDDLFloor.options.add(option);

    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IOMasterDDLFloor.options.add(option);
        }
    }
}

function SetIomLocFloorListItem() {
    while (IomDDLFloor.options.length > 0)
        IomDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    IomDDLFloor.options.add(option);

    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            IomDDLFloor.options.add(option);
        }
    }
}


function SetSenLocListItem() {
    //Sensor
    while (SenDDLArea.options.length > 0)
        SenDDLArea.options.remove(0);

    while (SenDDLBuilding.options.length > 0)
        SenDDLBuilding.options.remove(0);

    while (SenDDLFloor.options.length > 0)
        SenDDLFloor.options.remove(0);

    //Sensor
    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLFloor.options.add(option);

    var AreaItem = txtAreaList.value.split(',');
    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");
        //Sensor
        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        SenDDLArea.options.add(option);

    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == tmpLocArea.value) {
            //Sensor
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            SenDDLBuilding.options.add(option);

        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == tmpLocBuilding.value) {
            //Sensor
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            SenDDLFloor.options.add(option);
        }
    }
    //Sensor
    for (i = 0; i < SenDDLArea.options.length; i++) {
        SenDDLArea.options[i].selected = false;

        if (SenDDLArea.options[i].value == sLocArea.value) {
            SenDDLArea.options[i].selected = true;
        }
    }
    for (i = 0; i < SenDDLBuilding.options.length; i++) {
        SenDDLBuilding.options[i].selected = false;

        if (SenDDLBuilding.options[i].value == sLocBuilding.value) {
            SenDDLBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < SenDDLFloor.options.length; i++) {
        SenDDLFloor.options[i].selected = false;

        if (SenDDLFloor.options[i].value == sLocFloor.value) {
            SenDDLFloor.options[i].selected = true;
        }
    }
}

function SelectSenArea() {
    tmpLocArea.value = SenDDLArea.value;
    SetSenLocBuildingListItem();
    $.unblockUI();
}

function SelectSenBuilding() {
    tmpLocArea.value = SenDDLArea.value;
    tmpLocBuilding.value = SenDDLBuilding.value;
    SetSenLocFloorListItem();
    $.unblockUI();
}

function SetSenLocBuildingListItem() {
    while (SenDDLBuilding.options.length > 0)
        SenDDLBuilding.options.remove(0);

    while (SenDDLFloor.options.length > 0)
        SenDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLFloor.options.add(option);

    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            SenDDLBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            SenDDLFloor.options.add(option);
        }
    }
}

function SetSenLocFloorListItem() {
    while (SenDDLFloor.options.length > 0)
        SenDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    SenDDLFloor.options.add(option);

    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            SenDDLFloor.options.add(option);
        }
    }
}