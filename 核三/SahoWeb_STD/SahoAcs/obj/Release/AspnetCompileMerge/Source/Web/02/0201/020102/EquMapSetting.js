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
            document.getElementById('ContentPlaceHolder1_Div_Master').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Controller').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Reader').style.display = "none";

            DciUI_Title.style.background = "#3B5998";
            MasterUI_Title.style.background = "#BBBBBB";
            ControllerUI_Title.style.background = "#BBBBBB";
            ReaderUI_Title.style.background = "#BBBBBB";
            break;
        case 'Master':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Master').style.display = "";
            document.getElementById('ContentPlaceHolder1_Div_Controller').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Reader').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            MasterUI_Title.style.background = "#3B5998";
            ControllerUI_Title.style.background = "#BBBBBB";
            ReaderUI_Title.style.background = "#BBBBBB";
            break;
        case 'Controller':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Master').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Controller').style.display = "";
            document.getElementById('ContentPlaceHolder1_Div_Reader').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            MasterUI_Title.style.background = "#BBBBBB";
            ControllerUI_Title.style.background = "#3B5998";
            ReaderUI_Title.style.background = "#BBBBBB";

            break;
        case 'Reader':
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Master').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Controller').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Reader').style.display = "";

            DciUI_Title.style.background = "#BBBBBB";
            MasterUI_Title.style.background = "#BBBBBB";
            ControllerUI_Title.style.background = "#BBBBBB";
            ReaderUI_Title.style.background = "#3B5998";
            break;
        default:
            document.getElementById('ContentPlaceHolder1_Div_Dci').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Master').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Controller').style.display = "none";
            document.getElementById('ContentPlaceHolder1_Div_Reader').style.display = "none";

            DciUI_Title.style.background = "#BBBBBB";
            MasterUI_Title.style.background = "#BBBBBB";
            ControllerUI_Title.style.background = "#BBBBBB";
            ReaderUI_Title.style.background = "#BBBBBB";
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

function SetAddReaderDisplay(sMode) {
    switch (sMode) {
        case 'Add':
            th_Controller_AddReader_1.style.display = "inline";
            th_Controller_AddReader_2.style.display = "inline";
            th_Controller_AddReader_3.style.display = "inline";
            th_Controller_AddReader_4.style.display = "inline";

            th_Controller_FwVer_1.style.display = "none";
            th_Controller_FwVer_2.style.display = "none";
            th_Controller_FwVer_3.style.display = "none";
            th_Controller_FwVer_4.style.display = "none";
            break;
        case 'Edit':
            th_Controller_AddReader_1.style.display = "none";
            th_Controller_AddReader_2.style.display = "none";
            th_Controller_AddReader_3.style.display = "none";
            th_Controller_AddReader_4.style.display = "none";

            th_Controller_FwVer_1.style.display = "none";
            th_Controller_FwVer_2.style.display = "none";
            th_Controller_FwVer_3.style.display = "none";
            th_Controller_FwVer_4.style.display = "none";
            break;
        case 'Del':
            th_Controller_AddReader_1.style.display = "none";
            th_Controller_AddReader_2.style.display = "none";
            th_Controller_AddReader_3.style.display = "none";
            th_Controller_AddReader_4.style.display = "none";

            th_Controller_FwVer_1.style.display = "none";
            th_Controller_FwVer_2.style.display = "none";
            th_Controller_FwVer_3.style.display = "none";
            th_Controller_FwVer_4.style.display = "none";
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

    ind = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
    s = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");

    event_srcElement_id.value = s;
    
    var noteItemType = txt_NodeTypeList.value.split(',');
    var noteID = txt_NodeIDList.value.split(',');

    if (noteItemType[parseInt(s)] != "NONE" ) {
        if (noteItemType[parseInt(s)] == "SMS")
        {
            SetDivMode('');
            $.unblockUI();
        }
        else if (noteItemType[parseInt(s)] == "DCI") {
            Block();
            SetDivMode("DeviceConnInfo");
            LoadDeviceConnInfo(noteID[s]);
            SetDeviceConnInfoMode("Edit");
        }
        else if (noteItemType[parseInt(s)] == "MASTER") {
            Block();
            SetDivMode("Master");
            LoadMaster(noteID[s]);
            SetMasterMode("Edit");
        }
        else if (noteItemType[parseInt(s)] == "CONTROLLER") {
            Block(); 
            SetDivMode("Controller");
            SetControllerMode("Edit");
            LoadController(noteID[s]);
        }
        else if (noteItemType[parseInt(s)] == "READER") {
            Block();
            SetDivMode("Reader");
            SetReaderMode("Edit");
            LoadReader(noteID[s]);
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
function SearchEquData() {
    PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
    CancelTrigger1.click();
}

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
            case "Master":
                LoadMaster(sID);
                SetMasterMode('Edit');

                sLoc = sUp * 19.5;
                document.getElementById('ContentPlaceHolder1_TreeView_Panel').scrollTop = Math.round(sLoc);

                ShowDialog('message', '連線裝置', '新增連線裝置資料成功！');
                break
            case "Controller":
                CtrlIDValue.value = sID;
                LoadController(sID);
                SetControllerMode('Edit');
                ShowDialog('message', '控制器', '新增控制器資料成功！');
                break;
            case "Reader":
                hfReaderID.value = sID;

                LoadReader(sID);
                SetReaderMode('Edit');

                ShowDialog('message', '讀卡器&設備', '新增讀卡器&設備資料成功！');
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
            case "Master":
                LoadMaster(sID);
                SetMasterMode('Edit');

                ShowDialog('message', '連線裝置', '更新連線裝置資料成功！');
                break
            case "Controller":
                LoadController(sID);
                SetControllerMode('Edit');

                ShowDialog('message', '控制器', '更新控制器資料成功！');
                break;
            case "Reader":
                LoadReader(sID);
                SetReaderMode('Edit');

                ShowDialog('message', '讀卡器&設備', '更新讀卡器&設備資料成功！');
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
                case "InsertMaster":
                    MstIDValue.value = objRet.message;

                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Master_Insert_' + objRet.message + '_' + objRet.sUp);
                    break;
                case "UpdateMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Master_Update_' + objRet.message);
                    break;
                case "DeleteMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Master_Delete_' + objRet.message);
                    break;
                case "InsertController":
                    CtrlIDValue.value = objRet.message;
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Controller_Insert_' + objRet.message);
                    break;
                case "UpdateController":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Controller_Update_' + objRet.message);
                    break;
                case "DeleteController":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Controller_Delete_' + objRet.message);
                    break;
                case "InsertReader":
                    hfReaderID.value = objRet.message;
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Reader_Insert_' + objRet.message);
                    break;
                case "UpdateReader":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Reader_Update_' + objRet.message);
                    break;
                case "DeleteReader":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh_Reader_Delete_' + objRet.message);
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
function SetMasterMode(sMode) {
    switch (sMode) {
        case 'Add':
            Master_Input_No.value = '';
            Master_Input_Desc.value = '';
            Master_Input_Dci.innerText = '';
            Master_Input_IPParam_IP.value = '';
            Master_Input_IPParam_Port.value = '';
            //Master_Input_ComPortParam_ComPort.value = '';
            //Master_Input_ComPortParam_BaudRate.value = '9600';
            //Master_Input_ComPortParam_Parity.value = 'N';
            //Master_Input_ComPortParam_DataBits.value = '8';
            //Master_Input_ComPortParam_StopBits.value = '1';
            //Master_Input_Type_TCPIP.checked = true;
            //SetParamDiv('IPParam');

            document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";

            Master_Input_Model.value = '';
            Master_Input_FwVer.value = '';
            Master_Input_Status.value = '1';
            Master_Input_CtrlModel.value = '';
            //Master_Input_LinkMode_Always.checked = true;
            Master_Input_AutoRerun_N.checked = true;
            Master_Input_No.disabled = false;
            Master_Input_Desc.disabled = false;
            //Master_Input_Type_TCPIP.disabled = false;
            //Master_Input_Type_COMPort.disabled = false;
            Master_Input_IPParam_IP.disabled = false;
            Master_Input_IPParam_Port.disabled = false;
            //Master_Input_ComPortParam_ComPort.disabled = false;
            //Master_Input_ComPortParam_BaudRate.disabled = false;
            //Master_Input_ComPortParam_Parity.disabled = false;
            //Master_Input_ComPortParam_DataBits.disabled = false;
            //Master_Input_ComPortParam_StopBits.disabled = false;
            Master_Input_Model.disabled = false;
            Master_Input_FwVer.disabled = false;

            Master_Input_Status.disabled = false;
            Master_Input_CtrlModel.disabled = false;

            //Master_Input_LinkMode_Always.disabled = false;
            Master_Input_AutoRerun_Y.disabled = false;
            Master_Input_AutoRerun_N.disabled = false;
            Master_B_Add.style.display = "inline";
            Master_B_Edit.style.display = "none";
            Master_B_Delete.style.display = "none";
            break;
        case 'Edit':
            Master_Input_IPParam_IP.value = '';
            Master_Input_IPParam_Port.value = '';
            //Master_Input_ComPortParam_ComPort.value = '';
            //Master_Input_ComPortParam_BaudRate.value = '9600';
            //Master_Input_ComPortParam_Parity.value = 'N';
            //Master_Input_ComPortParam_DataBits.value = '8';
            //Master_Input_ComPortParam_StopBits.value = '1';
            Master_Input_No.disabled = false;
            Master_Input_Desc.disabled = false;
            //Master_Input_Type_TCPIP.disabled = false;
            // Master_Input_Type_COMPort.disabled = false;
            Master_Input_IPParam_IP.disabled = false;
            Master_Input_IPParam_Port.disabled = false;
            //Master_Input_ComPortParam_ComPort.disabled = false;
            //Master_Input_ComPortParam_BaudRate.disabled = false;
            //Master_Input_ComPortParam_Parity.disabled = false;
            //Master_Input_ComPortParam_DataBits.disabled = false;
            //Master_Input_ComPortParam_StopBits.disabled = false;
            Master_Input_Model.disabled = false;
            Master_Input_FwVer.disabled = false;

            Master_Input_Status.disabled = false;

            //Master_Input_LinkMode_Always.disabled = false;

            Master_Input_AutoRerun_Y.disabled = false;
            Master_Input_AutoRerun_N.disabled = false;
            Master_B_Add.style.display = "none";
            Master_B_Edit.style.display = "inline";
            Master_B_Delete.style.display = "none";
            break;
        case 'Delete':

            Master_Input_IPParam_IP.value = '';
            Master_Input_IPParam_Port.value = '';
            //Master_Input_ComPortParam_ComPort.value = '';
            //Master_Input_ComPortParam_BaudRate.value = '9600';
            //Master_Input_ComPortParam_Parity.value = 'None';
            //Master_Input_ComPortParam_DataBits.value = '8';
            //Master_Input_ComPortParam_StopBits.value = 'One';
            Master_Input_No.disabled = true;
            Master_Input_Desc.disabled = true;
            //Master_Input_Type_TCPIP.disabled = true;
            //Master_Input_Type_COMPort.disabled = true;
            Master_Input_IPParam_IP.disabled = true;
            Master_Input_IPParam_Port.disabled = true;
            //Master_Input_ComPortParam_ComPort.disabled = true;
            //Master_Input_ComPortParam_BaudRate.disabled = true;
            //Master_Input_ComPortParam_Parity.disabled = true;
            //Master_Input_ComPortParam_DataBits.disabled = true;
            //Master_Input_ComPortParam_StopBits.disabled = true;
            Master_Input_Model.disabled = true;
            Master_Input_FwVer.disabled = true;

            Master_Input_Status.disabled = true;
            Master_Input_CtrlModel.disabled = true;

            //Master_Input_LinkMode_Always.disabled = true;
            Master_Input_AutoRerun_Y.disabled = true;
            Master_Input_AutoRerun_N.disabled = true;
            Master_B_Add.style.display = "none";
            Master_B_Edit.style.display = "none";
            Master_B_Delete.style.display = "inline";
            break;
        default:
            break;
    }
}

// 呼叫Master新增視窗
function CallCreateMaster(FromDciID) {
    PageMethods.GetDciInfo(FromDciID, SetCreateMasterUI, onPageMethodsError);
}

// CreateMasterUI將資料帶回畫面
function SetCreateMasterUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        DciIDValue.value = DataArray[0];
        Master_Input_Dci.innerText = DataArray[1];
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
function LoadMaster(MstID) {
    PageMethods.LoadMaster(MstID, SetMasterUI, onPageMethodsError);
}

// 讀取Master資料回填UI
function SetMasterUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        Master_Input_No.value = DataArray[0];               // 連線裝置編號
        Master_Input_Desc.value = DataArray[1];             // 連線裝置說明
        Master_Input_Dci.innerText = DataArray[2];          // 使用連線
        
        // 連線類型：現在固定為TCPIP (DataArray[3] == "T")
        document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";

        // 判斷IPV4或IPV6
        var IpArray = DataArray[4].split(':');

        if (IpArray.length > 2) {
            IpArray = DataArray[4].split(',');
        }

        Master_Input_IPParam_IP.value = IpArray[0];         // IP
        Master_Input_IPParam_Port.value = IpArray[1];       // PORT
        
        Master_Input_Model.value = DataArray[5];            // 連線裝置機型
        Master_Input_Status.value = DataArray[6];           // 狀態
        Master_Input_CtrlModel.value = DataArray[7];        // 控制器機型

        // DataArray[8];        該控制項目前不用

        // 自動回傳
        if (DataArray[9] == "1") {
            Master_Input_AutoRerun_Y.checked = true;
        }
        else if (DataArray[9] == "0") {
            Master_Input_AutoRerun_N.checked = true;
        }

        DciIDValue.value = DataArray[10];                   // 暫存 DciID
        MstIDValue.value = DataArray[11];                   // 暫存 MstID

        Master_Input_FwVer.value = DataArray[12];           // 韌體版本

        /*  wei 20170209
            得到目前該連線裝置底下所用的控制器數量，若不等於0則disabled=true

            原本Master_Input_CtrlModel的disabled的狀態是在SetMasterMode運作
            但這邊需要在RUN AT SERVER後變更disabled，所以移到這邊執行
        */
        ControlCount.value = DataArray[13];
        if (ControlCount.value !== "0") {
            Master_Input_CtrlModel.disabled = true;
        }
        else {
            Master_Input_CtrlModel.disabled = false;
        }
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
function GetMasterDataArray() {
    var DataArray = [];
    DataArray[0] = Master_Input_No.value;           // 連線裝置編號
    DataArray[1] = Master_Input_Desc.value;         // 連線裝置說明
    DataArray[2] = Master_Input_Dci.innerText;      // 使用連線

    DataArray[3] = "T";         // 連線模式現在永遠為 TCPIP 模式，(MstType="T")

    // IP + PORT
    DataArray[4] = Master_Input_IPParam_IP.value + "_" + Master_Input_IPParam_Port.value;

    DataArray[5] = Master_Input_Model.value;        // 連線裝置機型
    DataArray[6] = Master_Input_Status.value;       // 狀態
    DataArray[7] = Master_Input_CtrlModel.value;    // 控制器機型

    DataArray[8] = "1";         // 連線模式現在永遠為 Always (LinkMode=1)
        
    // 連線模式
    if (Master_Input_AutoRerun_Y.checked) {
        DataArray[9] = "1";
    }
    else if (Master_Input_AutoRerun_N.checked) {
        DataArray[9] = "0";
    }

    DataArray[10] = DciIDValue.value;               // 暫存 DciID
    DataArray[11] = MstIDValue.value;               // 暫存 MstID
    DataArray[12] = Master_Input_FwVer.value;       // 韌體版本

    return DataArray;
}

//執行新增Master動作
function InsertMasterExcute() {
    var DataArray = GetMasterDataArray();
    PageMethods.InsertMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Master動作
function UpdateMasterExcute() {
    var DataArray = GetMasterDataArray();
    PageMethods.UpdateMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Master動作
function DeleteMasterExcute() {
    var DataArray = GetMasterDataArray();
    PageMethods.DeleteMaster(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}
/*Master操作相關------ End ------*/

/*Controller操作相關------ Start ------*/
// Controller操作
function SetControllerMode(sMode) {
    $("#BtnCopyLight").hide();
    switch (sMode) {
        case 'Add':
            Controller_Input_No.value = '';
            Controller_Input_Name.value = '';
            Controller_Input_Desc.value = '';
            Controller_Input_Addr.value = '1';
            Controller_Input_Model.innerText = '';
            Controller_Input_Status.value = '1';
            Controller_Input_No.disabled = false;
            Controller_Input_Name.disabled = false;
            Controller_Input_Desc.disabled = false;
            Controller_Input_Status.disabled = false;
            Controller_Input_Addr.disabled = false;
            Controller_B_Add.style.display = "inline";
            Controller_B_Edit.style.display = "none";
            Controller_B_Delete.style.display = "none";

            Controller_Input_EquClass.disabled = false;

            CtrlParaButton.style.display = "none";
            SetButton.style.display = "none";

            SetAddReaderDisplay('Add');

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            SetLocListItem();
            break;
        case 'Edit':
            Controller_Input_No.value = '';
            Controller_Input_Name.value = '';
            Controller_Input_Desc.value = '';
            Controller_Input_Addr.value = '';
            Controller_Input_Model.innerText = '';
            Controller_Input_Status.value = '1';
            Controller_Input_No.disabled = false;
            Controller_Input_Name.disabled = false;
            Controller_Input_Desc.disabled = false;
            Controller_Input_Status.disabled = false;
            Controller_Input_Addr.disabled = false;
            Controller_B_Add.style.display = "none";
            Controller_B_Edit.style.display = "inline";
            Controller_B_Delete.style.display = "none";

            Controller_Input_EquClass.disabled = false;

            CtrlParaButton.style.display = "inline";
            SetButton.style.display = "inline";

            SetAddReaderDisplay('Edit');

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            break;
        case 'Delete':

            Controller_Input_No.value = '';
            Controller_Input_Name.value = '';
            Controller_Input_Desc.value = '';
            Controller_Input_Addr.value = '';
            Controller_Input_Model.innerText = '';
            Controller_Input_Status.value = '1';
            Controller_Input_No.disabled = true;
            Controller_Input_Name.disabled = true;
            Controller_Input_Desc.disabled = true;
            Controller_Input_Status.disabled = true;
            Controller_Input_Addr.disabled = true;
            Controller_B_Add.style.display = "none";
            Controller_B_Edit.style.display = "none";
            Controller_B_Delete.style.display = "inline";

            CtrlParaButton.style.display = "none";
            SetButton.style.display = "none";

            SetAddReaderDisplay('Del');

            Controller_Input_EquClass.disabled = true;

            break;
        default:
            break;
    }
}

// 呼叫Controller新增視窗
function CallCreateController(FromDciID) {
    PageMethods.GetMstInfo(FromDciID, SetCreateControllerUI, onPageMethodsError);
}

// CreateControllerUI將資料帶回畫面
function SetCreateControllerUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        MstIDValue.value = DataArray[0];
        Controller_Input_Mst.innerText = DataArray[1];
        Controller_Input_Model.innerText = DataArray[2];

        // 產生 設備類別 項目
        GetEquClassListItem();
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

// 讀取Controller資料
function LoadController(CtrlID) {
    PageMethods.LoadController(hideUserID.value, CtrlID, SetControllerUI, onPageMethodsError);
}

// 讀取Controller資料回填UI
function SetControllerUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {

        Controller_Input_No.value = DataArray[0];
        Controller_Input_Name.value = DataArray[1];
        Controller_Input_Desc.value = DataArray[2];
        Controller_Input_Mst.innerText = DataArray[3];
        Controller_Input_Model.innerText = DataArray[4];
        Controller_Input_Status.value = DataArray[5];
        Controller_Input_Addr.value = DataArray[6];
        Controller_Input_FwVer.innerText = DataArray[7];
        MstIDValue.value = DataArray[8];
        tmpEquClass.value = DataArray[9];       // 暫存EquClass
        DciIDValue.value = DataArray[10];
        CtrlIDValue.value = DataArray[11];        
        if ($('[name*="tmpEquClass"]').val() == "TRT") {
            $("#BtnCopyLight").show();            
        }
        GetEquClassListItem();  // 取得以 [Controller_Input_Model] 為基準的 [EquClass] 項目

        sLocArea.value = DataArray[12];
        tmpLocArea.value = DataArray[12];
        sLocBuilding.value = DataArray[13];
        tmpLocBuilding.value = DataArray[13];
        sLocFloor.value = DataArray[14];

        SetLocListItem();

    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

function SetCopyLight() {
    $.post("/Web/02/LightInfoCopy.aspx",
         { "EquNo": Controller_Input_No.value, "ParaType": "Controller" },
         function (data) {
             OverlayContent(data);
         });
}

// 取得以 [Controller_Input_Model] 為基準的 [EquClass] 項目
function GetEquClassListItem() {
    // 從控制器機型(Controller_Input_Model)得到其所有的設備類別
    PageMethods.GetEquClassListItem(Controller_Input_Model.innerText, SetEquClassListItem, onPageMethodsError);
}

function SetEquClassListItem(DataArray) {
    while (Controller_Input_EquClass.options.length > 0)
        Controller_Input_EquClass.options.remove(0);

    for (i = 0; i < DataArray.length; i++) {
        option = document.createElement("option");
        DataStr = DataArray[i].split("/");
        option.text = DataStr[0];
        option.value = DataStr[1];

        Controller_Input_EquClass.options.add(option);
    }

    for (i = 0; i < Controller_Input_EquClass.options.length; i++) {
        Controller_Input_EquClass.options[i].selected = false;

        if (Controller_Input_EquClass.options[i].value == tmpEquClass.value) {
            Controller_Input_EquClass.options[i].selected = true;
        }
    }
}

// 取得Controller畫面資料
function GetControllerDataArray() {
    var DataArray = [];

    DataArray[0] = Controller_Input_No.value;               // 控制器編號
    DataArray[1] = Controller_Input_Name.value;             // 控制器名稱
    DataArray[2] = Controller_Input_Desc.value;
    DataArray[3] = Controller_Input_Mst.innerText;
    DataArray[4] = Controller_Input_Model.innerText;
    DataArray[5] = Controller_Input_Status.value;
    DataArray[6] = Controller_Input_Addr.value;
    DataArray[7] = Controller_Input_FwVer.innerText;
    DataArray[8] = MstIDValue.value;                                                    

    DataArray[9] = Controller_Input_EquClass.value;             // 設備類別 = 暫存EquClass
    tmpEquClass.value = Controller_Input_EquClass.value;

    DataArray[10] = DciIDValue.value;
    DataArray[11] = CtrlIDValue.value;
    DataArray[12] = Controller_Input_AddReader.value;

    DataArray[13] = ControllerDDLArea.value;
    DataArray[14] = ControllerDDLBuilding.value;
    DataArray[15] = ControllerDDLFloor.value;

    return DataArray;
}

//執行新增Controller動作
function InsertControllerExcute() {
    var DataArray = GetControllerDataArray();
    PageMethods.InsertController(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Controller動作
function UpdateControllerExcute() {
    var DataArray = GetControllerDataArray();
    PageMethods.UpdateController(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Controller動作
function DeleteControllerExcute() {
    var DataArray = GetControllerDataArray();
    PageMethods.DeleteController(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}
/*Controller操作相關------ End ------*/

/*Reader操作相關------ Start ------*/
//Reader操作
function SetReaderMode(sMode) {
    switch (sMode) {
        case 'Add':
            Reader_Input_No.value = '';
            Reader_Input_Name.value = '';
            Reader_Input_Desc.value = '';
            //Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = false;
            Reader_Input_Name.disabled = false;
            Reader_Input_Desc.disabled = false;
            //Reader_Input_EquNo.disabled = false;
            Reader_Input_Dir.disabled = false;
            Reader_B_Add.style.display = "inline";
            Reader_B_Edit.style.display = "none";
            Reader_B_Delete.style.display = "none";
            $("#BtnPrint").css("display", "none");
            // 設備部份
            ddlEquClass.disabled = true;
            txtEquNo.disabled = false;
            txtEquModel.disabled = true;
            txtCardNoLen.disabled = false;
            ddlLocationArea.disabled = false;
            ddlLocationBuilding.disabled = false;
            ddlLocationFloor.disabled = false;
            txtEquName.disabled = false;
            txtEquEName.disabled = false;
            ddlIsShowName.disabled = false;
            ddlInToCtrlAreaID.disabled = false;
            ddlOutToCtrlAreaID.disabled = false;
            ddlIsAndTrt.disabled = false;

            sLocArea.value = '';
            sLocBuilding.value = '';
            sLocFloor.value = '';
            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            ParaButton.style.display = "none";

            SetEquLocListItem();

            break;
        case 'Edit':
            Reader_Input_No.value = '';
            Reader_Input_Name.value = '';
            Reader_Input_Desc.value = '';
            //Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = false;
            Reader_Input_Name.disabled = false;
            Reader_Input_Desc.disabled = false;
            //Reader_Input_EquNo.disabled = false;
            Reader_Input_Dir.disabled = false;
            Reader_B_Add.style.display = "none";
            $("#BtnPrint").css("display", "inline");
            Reader_B_Edit.style.display = "inline";
            Reader_B_Delete.style.display = "none";

            // 設備部份
            ddlEquClass.disabled = true;
            txtEquNo.disabled = false;
            txtEquModel.disabled = true;
            txtCardNoLen.disabled = false;
            ddlLocationArea.disabled = false;
            ddlLocationBuilding.disabled = false;
            ddlLocationFloor.disabled = false;
            txtEquName.disabled = false;
            txtEquEName.disabled = false;
            ddlIsShowName.disabled = false;
            ddlInToCtrlAreaID.disabled = false;
            ddlOutToCtrlAreaID.disabled = false;
            ddlIsAndTrt.disabled = false;

            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            ParaButton.style.display = "inline";
            
            break;
        case 'Delete':

            Reader_Input_No.value = '';
            Reader_Input_Name.value = '';
            Reader_Input_Desc.value = '';
            //Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = true;
            Reader_Input_Name.disabled = true;
            Reader_Input_Desc.disabled = true;
			//Reader_Input_EquNo.disabled = true;
            Reader_Input_Dir.disabled = true;
            Reader_B_Add.style.display = "none";
            Reader_B_Edit.style.display = "none";
            Reader_B_Delete.style.display = "inline";

            // 設備部份
            ddlEquClass.disabled = true;
            txtEquNo.disabled = true;
            txtEquModel.disabled = true;
            txtCardNoLen.disabled = true;
            ddlLocationArea.disabled = true;
            ddlLocationBuilding.disabled = true;
            ddlLocationFloor.disabled = true;
            txtEquName.disabled = true;
            txtEquEName.disabled = true;
            ddlIsShowName.disabled = true;
            ddlInToCtrlAreaID.disabled = true;
            ddlOutToCtrlAreaID.disabled = true;
            ddlIsAndTrt.disabled = true;

            tmpLocArea.value = '';
            tmpLocBuilding.value = '';

            ParaButton.style.display = "none";

            break;
        default:
            break;
    }
	// 設備相關 值的指派
    //hfEquID.value = '';
    //txtEquClass.value = '';


    // 設備新增

    // 設備類別
    for (i = 0; i < ddlEquClass.options.length; i++) {
        if (ddlEquClass.options[i].value == "Door Access") {
            ddlEquClass.options[i].selected = true;
        }
        else {
            ddlEquClass.options[i].selected = false;
        }
    }

    txtEquNo.value = '';

    for (i = 0; i < ddlLocationArea.options.length; i++) {
        if (ddlLocationArea.options[i].value == "") {
            ddlLocationArea.options[i].selected = true;
        }
        else {
            ddlLocationArea.options[i].selected = false;
        }
    }

    for (i = 0; i < ddlLocationBuilding.options.length; i++) {
        if (ddlLocationBuilding.options[i].value == "") {
            ddlLocationBuilding.options[i].selected = true;
        }
        else {
            ddlLocationBuilding.options[i].selected = false;
        }
    }

    for (i = 0; i < ddlLocationFloor.options.length; i++) {
        if (ddlLocationFloor.options[i].value == "") {
            ddlLocationFloor.options[i].selected = true;
        }
        else {
            ddlLocationFloor.options[i].selected = false;
        }
    }

    txtEquName.value = txtEquNo.value;
    txtEquEName.value = txtEquNo.value;

    txtCardNoLen.value = '';

    for (i = 0; i < ddlInToCtrlAreaID.options.length; i++) {
        if (ddlInToCtrlAreaID.options[i].value == "") {
            ddlInToCtrlAreaID.options[i].selected = true;
        }
        else {
            ddlInToCtrlAreaID.options[i].selected = false;
        }
    }

    for (i = 0; i < ddlOutToCtrlAreaID.options.length; i++) {
        if (ddlOutToCtrlAreaID.options[i].value == "") {
            ddlOutToCtrlAreaID.options[i].selected = true;
        }
        else {
            ddlOutToCtrlAreaID.options[i].selected = false;
        }
    }

    // 卡鐘設定
    for (i = 0; i < ddlIsAndTrt.options.length; i++) {
        if (ddlIsAndTrt.options[i].value == "0") {
            ddlIsAndTrt.options[i].selected = true;
        }
        else {
            ddlIsAndTrt.options[i].selected = false;
        }
    }

    // 顯示姓名
    for (i = 0; i < ddlIsShowName.options.length; i++) {
        if (ddlIsShowName.options[i].value == "0") {
            ddlIsShowName.options[i].selected = true;
        }
        else {
            ddlIsShowName.options[i].selected = false;
        }
    }

}

//呼叫Reader新增視窗
function CallCreateReader(FromCtrlID) {
    PageMethods.GetCtrlInfo(FromCtrlID, SetCreateReaderUI, onPageMethodsError);
}

//CreateReaderUI將資料帶回畫面
function SetCreateReaderUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        CtrlIDValue.value = DataArray[0];
        Reader_Input_CtrlName.innerText = DataArray[1];
        Reader_Input_CtrlModel.innerText = DataArray[2];

        DciIDValue.value = DataArray[3];

        // 2016.11.28
        txtEquModel.value = DataArray[2];

        // CardNoLen
        txtCardNoLen.value = DataArray[4];

        // EquClass：指派預設值
        for (i = 0; i < ddlEquClass.options.length; i++)
        {
            if (ddlEquClass.options[i].value == DataArray[5]) {
                ddlEquClass.options[i].selected = true;
            }
            else {
                ddlEquClass.options[i].selected = false;
            }
        }

        // 讀卡機編號
        Reader_Input_No.value = DataArray[6];

        // 讀卡機名稱
        Reader_Input_Name.value = DataArray[7] + '_' + Reader_Input_No.value;

        // 讀卡機說明
        Reader_Input_Desc.value = Reader_Input_Name.value;

        // 設備編號
        txtEquNo.value = Reader_Input_Desc.value;

        // 設備名稱
        txtEquName.value = Reader_Input_Name.value;

        // 設備英文名稱
        txtEquEName.value = Reader_Input_Name.value;

        SetControlStatus();     // 依照 ddlEquClass.Value 決定那些設備項目需要被顯示
        //GetLocationListItem();
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}

//讀取Reader資料
function LoadReader(ReaderID) {

    PageMethods.LoadReader(hideUserID.value, ReaderID, SetReaderUI, onPageMethodsError);
}

//讀取Reader資料回填UI
function SetReaderUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        /*
        if (DataArray[6].indexOf('SC300') > -1) {
            Reader_Label_EquNo.innerText = "對應設備 (SC300系列將同步修改)";
        }
        else {
            Reader_Label_EquNo.innerText = "對應設備";
        }
        */


        Reader_Input_No.value = DataArray[0];
        Reader_Input_Name.value = DataArray[1];
        Reader_Input_Desc.value = DataArray[2];
        //Reader_Input_EquNo.value = DataArray[3];
        Reader_Input_Dir.value = DataArray[4];
        Reader_Input_CtrlName.innerText = DataArray[5];
        Reader_Input_CtrlModel.innerText = DataArray[6];
        CtrlIDValue.value = DataArray[7];

        // -- 2016.11.25 add --
        txtEquModel.value = DataArray[8];
        txtEquNo.value = DataArray[9];

        //txtEquClass.value = DataArray[10];
        //txtEquClass.Title = DataArray[11];

        for (var j = 0; j < ddlEquClass.options.length; j++) {
            if (ddlEquClass.options[j].value == DataArray[11]) {
                ddlEquClass.options[j].selected = true;
            }
            else {
                ddlEquClass.options[j].selected = false;
            }
        }

        sLocBuilding.value = DataArray[12];
        tmpLocBuilding.value = DataArray[12];

        sLocFloor.value = DataArray[13];

        txtEquName.value = DataArray[14];
        txtEquEName.value = DataArray[15];

        DciIDValue.value = DataArray[16];

        txtCardNoLen.value = DataArray[17];

        for (i = 0; i < ddlInToCtrlAreaID.options.length; i++) {
            if (ddlInToCtrlAreaID.options[i].value == DataArray[18]) {
                ddlInToCtrlAreaID.options[i].selected = true;
            }
            else {
                ddlInToCtrlAreaID.options[i].selected = false;
            }
        }

        for (i = 0; i < ddlOutToCtrlAreaID.options.length; i++) {
            if (ddlOutToCtrlAreaID.options[i].value == DataArray[19]) {
                ddlOutToCtrlAreaID.options[i].selected = true;
            }
            else {
                ddlOutToCtrlAreaID.options[i].selected = false;
            }
        }

        for (i = 0; i < ddlIsAndTrt.options.length; i++) {
            if (ddlIsAndTrt.options[i].value == DataArray[20]) {
                ddlIsAndTrt.options[i].selected = true;
            }
            else {
                ddlIsAndTrt.options[i].selected = false;
            }
        }

        hfEquID.value = DataArray[21];      // 設備識別碼

        // 顯示姓名
        for (i = 0; i < ddlIsShowName.options.length; i++) {
            if (ddlIsShowName.options[i].value == DataArray[22]) {
                ddlIsShowName.options[i].selected = true;
            }
            else {
                ddlIsShowName.options[i].selected = false;
            }
        }

        hfReaderID.value = DataArray[23];
        $('[name*="ddlLinkEquNoList"]').val(DataArray[24]);

        sLocArea.value = DataArray[25];
        tmpLocArea.value = DataArray[25];

        SetEquLocListItem();
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    SetControlStatus();
    $.unblockUI();
}

//取得Reader畫面資料
function GetReaderDataArray() {
    var DataArray = [];
    DataArray[0] = Reader_Input_No.value;               // ReaderNo
    DataArray[1] = Reader_Input_Name.value;             // ReaderName
    DataArray[2] = Reader_Input_Desc.value;             // ReaderDesc
    DataArray[3] = txtEquNo.value;                      // EquNo
    DataArray[4] = Reader_Input_Dir.value;              // Dir
    DataArray[5] = CtrlIDValue.value;                   // 
    DataArray[6] = Reader_Input_CtrlModel.innerText;
    DataArray[7] = ddlEquClass.value;                   // 設備類別 EquClass

    // RD.ReaderNo, RD.ReaderName, RD.ReaderDesc, RD.EquNo, RD.Dir,
    //                CR.CtrlName, CR.CtrlModel, CR.CtrlID, 7

    // 從 index=8 開始，就是用在 設備(Equipment) 的資料處理了

    DataArray[8] = txtEquModel.value;               // 設備型號 EquModel
    DataArray[9] = txtEquNo.value;                  // 設備編號 EquNo
    DataArray[10] = '';                             // ChtEquClass
    DataArray[11] = ddlEquClass.value;              // 設備類別 EquClass
    DataArray[12] = ddlLocationBuilding.value;              // 棟別 Building
    DataArray[13] = ddlLocationFloor.value;               // 樓層 Floor
    DataArray[14] = txtEquName.value;               // 設備名稱 EquName
    DataArray[15] = txtEquEName.value;              // 設備英文名稱 EquEName
    DataArray[16] = DciIDValue.value;               // 設備連線 DciID
    DataArray[17] = txtCardNoLen.value;             // 卡號長度 CardNoLen
    DataArray[18] = ddlInToCtrlAreaID.value;        // 進入管制區 InToCtrlAreaID
    DataArray[19] = ddlOutToCtrlAreaID.value;       // 離開管制區 OutToCtrlAreaID
    DataArray[20] = ddlIsAndTrt.value;              // 卡鐘模式 IsAndTrt
    DataArray[21] = hfEquID.value;                  // 設備識別碼 EquID    
    DataArray[22] = ddlIsShowName.value;            // 顯示姓名 IsShowName
    DataArray[23] = hfReaderID.value;               // hfReaderID 
    DataArray[24] = $('[name*="ddlLinkEquNoList"]').val();      //連動設備
    DataArray[25] = ddlLocationArea.value;          // 區域
    //ED.EquModel, 8
    //ED.EquNo,    9

    //CASE 
    //WHEN ED.EquClass = 'Door Access' THEN '門禁設備'
    //WHEN ED.EquClass = 'TRT' THEN '考勤設備'
    //WHEN ED.EquClass = 'Elevator' THEN '電梯設備'
    //ELSE '' 
    //END AS ChtEquClass, 10

    //ED.EquClass, 11
    //ED.Building, 12
    //ED.[Floor],  13 
    //ED.EquName,   14
    //ED.EquEName,  15
    //ED.DciID,     16 
    //ED.CardNoLen, 17
    //ED.InToCtrlAreaID,  18
    //ED.OutToCtrlAreaID,  19
    //ED.IsAndTrt, 20
    //ED.EquID,  21
    //ED.IsShowName 22  

    return DataArray;
}
//執行新增Reader動作
function InsertReaderExcute() {
    var DataArray = GetReaderDataArray();
    PageMethods.InsertReader(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行編輯Reader動作
function UpdateReaderExcute() {
    var DataArray = GetReaderDataArray();
    PageMethods.UpdateReader(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Reader動作
function DeleteReaderExcute() {
    var DataArray = GetReaderDataArray();
    PageMethods.DeleteReader(hideUserID.value, DataArray, Excutecomplete, onPageMethodsError);
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

    PageMethods.LoadReader(ClickItem.value, SetReaderUI, onPageMethodsError);
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

/*Reader操作相關------ End ------*/


// 根據 設備類別 來決定那些控制項需要顯示
function SetControlStatus() {
    switch (ddlEquClass.value) {
        case 'Door Access':
            //// 樓層相關
            //document.getElementById('sFloor').style.display = "inline";
            //labFloor.style.display = "inline";
            //txtFloor.style.display = "inline";

            // 顯示姓名相關
            document.getElementById('sIsShowName').style.display = "none";
            labIsShowName.style.display = "none";
            ddlIsShowName.style.display = "none";

            // 進入管制區、離開管制區、卡鐘模式
            tr_IOA.style.display = "block";
            $(".AreaIo").show();
            break;
        case 'Elevator':
            //// 樓層相關
            //document.getElementById('sFloor').style.display = "none";
            //labFloor.style.display = "none";
            //txtFloor.style.display = "none";

            // 顯示姓名相關
            document.getElementById('sIsShowName').style.display = "none";
            labIsShowName.style.display = "none";
            ddlIsShowName.style.display = "none";

            // 進入管制區、離開管制區、卡鐘模式
            //document.getElementById('tb_IOA').style.visibility = "hidden";
            tr_IOA.style.display = "none";
           

            break;
        case 'TRT':
            //// 樓層相關
            //document.getElementById('sFloor').style.display = "inline";
            //labFloor.style.display = "inline";
            //txtFloor.style.display = "inline";

            // 顯示姓名相關
            document.getElementById('sIsShowName').style.display = "inline";
            labIsShowName.style.display = "inline";
            ddlIsShowName.style.display = "inline";

            // 進入管制區、離開管制區、卡鐘模式
            //document.getElementById('tb_IOA').style.visibility = "hidden";
            tr_IOA.style.display = "block";
            $(".AreaIo").hide();
            break;
        default:
            break;
    }
}

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
    //var sURL = "../../ParaSetting.aspx?CtrlNo=" + Controller_Input_No.value + "&ParaType=Controller";
    //var vArguments = "";
    //var sFeatures = "dialogHeight:440px;dialogWidth:730px;center:yes;scroll:no;";
    //window.showModalDialog(sURL, vArguments, sFeatures);    
    $.post("/Web/02/ParaSettingBox.aspx",
        { "EquNo": Controller_Input_No.value, "ParaType": "Controller" },
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

    /* 原先透過 window.open 的方式*/
    //var sURL = "../ParaSetting.aspx?EquNo=" + SelectValue.value;
    //var vArguments = "";
    //var sFeatures = "dialogHeight:440px;dialogWidth:550px;center:yes;scroll:no;";
    //window.showModalDialog(sURL, vArguments, sFeatures);
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

function SetLocListItem() {
    while (ControllerDDLArea.options.length > 0)
        ControllerDDLArea.options.remove(0);

    while (ControllerDDLBuilding.options.length > 0)
        ControllerDDLBuilding.options.remove(0);

    while (ControllerDDLFloor.options.length > 0)
        ControllerDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLFloor.options.add(option);

    var AreaItem = txtAreaList.value.split(',');
    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");

        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        ControllerDDLArea.options.add(option);

    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == tmpLocArea.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ControllerDDLBuilding.options.add(option);

        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == tmpLocBuilding.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ControllerDDLFloor.options.add(option);
        }
    }

    for (i = 0; i < ControllerDDLArea.options.length; i++) {
        ControllerDDLArea.options[i].selected = false;

        if (ControllerDDLArea.options[i].value == sLocArea.value) {
            ControllerDDLArea.options[i].selected = true;
        }
    }
    for (i = 0; i < ControllerDDLBuilding.options.length; i++) {
        ControllerDDLBuilding.options[i].selected = false;

        if (ControllerDDLBuilding.options[i].value == sLocBuilding.value) {
            ControllerDDLBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < ControllerDDLFloor.options.length; i++) {
        ControllerDDLFloor.options[i].selected = false;

        if (ControllerDDLFloor.options[i].value == sLocFloor.value) {
            ControllerDDLFloor.options[i].selected = true;
        }
    }
}

function SelectCtrlArea() {
    tmpLocArea.value = ControllerDDLArea.value;
    SetControllerLocBuildingListItem();
    $.unblockUI();
}

function SelectCtrlBuilding() {
    tmpLocArea.value = ControllerDDLArea.value;
    tmpLocBuilding.value = ControllerDDLBuilding.value;
    SetControllerLocFloorListItem();
    $.unblockUI();
}

function SetControllerLocBuildingListItem() {
    while (ControllerDDLBuilding.options.length > 0)
        ControllerDDLBuilding.options.remove(0);

    while (ControllerDDLFloor.options.length > 0)
        ControllerDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLFloor.options.add(option);

    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ControllerDDLBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ControllerDDLFloor.options.add(option);
        }
    }
}

function SetControllerLocFloorListItem() {
    while (ControllerDDLFloor.options.length > 0)
        ControllerDDLFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ControllerDDLFloor.options.add(option);

    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ControllerDDLFloor.options.add(option);
        }
    }
}

//Reader
function SetEquLocListItem() {
    while (ddlLocationArea.options.length > 0)
        ddlLocationArea.options.remove(0);

    while (ddlLocationBuilding.options.length > 0)
        ddlLocationBuilding.options.remove(0);

    while (ddlLocationFloor.options.length > 0)
        ddlLocationFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationFloor.options.add(option);

    var AreaItem = txtAreaList.value.split(',');
    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");

        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        ddlLocationArea.options.add(option);

    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == tmpLocArea.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ddlLocationBuilding.options.add(option);

        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == tmpLocBuilding.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ddlLocationFloor.options.add(option);
        }
    }


    for (i = 0; i < ddlLocationArea.options.length; i++) {
        ddlLocationArea.options[i].selected = false;

        if (ddlLocationArea.options[i].value == sLocArea.value) {
            ddlLocationArea.options[i].selected = true;
        }
    }
    for (i = 0; i < ddlLocationBuilding.options.length; i++) {
        ddlLocationBuilding.options[i].selected = false;

        if (ddlLocationBuilding.options[i].value == sLocBuilding.value) {
            ddlLocationBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < ddlLocationFloor.options.length; i++) {
        ddlLocationFloor.options[i].selected = false;

        if (ddlLocationFloor.options[i].value == sLocFloor.value) {
            ddlLocationFloor.options[i].selected = true;
        }
    }
}
function SelectArea() {
    tmpLocArea.value = ddlLocationArea.value;
    SetEquLocBuildingListItem();
    $.unblockUI();
}

function SelectBuilding() {
    tmpLocArea.value = ddlLocationArea.value;
    tmpLocBuilding.value = ddlLocationBuilding.value;
    SetEquLocFloorListItem();
    $.unblockUI();
}

function SetEquLocBuildingListItem() {
    while (ddlLocationBuilding.options.length > 0)
        ddlLocationBuilding.options.remove(0);

    while (ddlLocationFloor.options.length > 0)
        ddlLocationFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationFloor.options.add(option);

    var BuildingItem = txtBuildingList.value.split(',');
    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ddlLocationBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ddlLocationFloor.options.add(option);
        }
    }
}

function SetEquLocFloorListItem() {
    while (ddlLocationFloor.options.length > 0)
        ddlLocationFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ddlLocationFloor.options.add(option);

    var FloorItem = txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ddlLocationFloor.options.add(option);
        }
    }
}