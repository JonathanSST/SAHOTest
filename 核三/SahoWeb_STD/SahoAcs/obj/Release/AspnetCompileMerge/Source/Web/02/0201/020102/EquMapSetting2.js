// 設定整體Div顯示
function SetDivMode(sMode) {
    switch (sMode) {
        case 'Master':
            document.getElementById('ContentPlaceHolder1_MasterUI').style.display = "";
            document.getElementById('ContentPlaceHolder1_ControllerUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ReaderUI').style.display = "none";
            document.getElementById('MasterUI_Title').style.background = "#3B5998";
            document.getElementById('ControllerUI_Title').style.background = "#BBBBBB";
            document.getElementById('ReaderUI_Title').style.background = "#BBBBBB";
            break;
        case 'Controller':
            document.getElementById('ContentPlaceHolder1_MasterUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ControllerUI').style.display = "";
            document.getElementById('ContentPlaceHolder1_ReaderUI').style.display = "none";
            document.getElementById('MasterUI_Title').style.background = "#BBBBBB";
            document.getElementById('ControllerUI_Title').style.background = "#3B5998";
            document.getElementById('ReaderUI_Title').style.background = "#BBBBBB";
            break;
        case 'Reader':
            document.getElementById('ContentPlaceHolder1_MasterUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ControllerUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ReaderUI').style.display = "";
            document.getElementById('MasterUI_Title').style.background = "#BBBBBB";
            document.getElementById('ControllerUI_Title').style.background = "#BBBBBB";
            document.getElementById('ReaderUI_Title').style.background = "#3B5998";
            break;
        default:
            document.getElementById('ContentPlaceHolder1_MasterUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ControllerUI').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ReaderUI').style.display = "none";
            document.getElementById('MasterUI_Title').style.background = "#BBBBBB";
            document.getElementById('ControllerUI_Title').style.background = "#BBBBBB";
            document.getElementById('ReaderUI_Title').style.background = "#BBBBBB";
            break;
    }
}

// ParamDiv顯示
function SetParamDiv(sMode) {
    switch (sMode) {
        case 'IPParam':
            document.getElementById('ContentPlaceHolder1_IPParam').style.display = "";
            document.getElementById('ContentPlaceHolder1_ComPortParam').style.display = "none";
            break;
        case 'ComPortParam':
            document.getElementById('ContentPlaceHolder1_IPParam').style.display = "none";
            document.getElementById('ContentPlaceHolder1_ComPortParam').style.display = "";
            break;
    }
}

function SetAddReaderDisplay(sMode) {
    switch (sMode) {
        case 'Add':
            Controller_Label_AddReader.style.display = "";
            Controller_Input_AddReader.style.display = "";
            break;
        case 'Edit':
            Controller_Label_AddReader.style.display = "none";
            Controller_Input_AddReader.style.display = "none";
            break;
    }
}

function OpenWin() {
    var s = event.srcElement.id;
    var ind = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
    s = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
    var noteItemType = txt_NodeTypeList.value.split(',');
    var noteID = txt_NodeIDList.value.split(',');

    if (noteItemType[parseInt(s)] != "NONE" && noteItemType[parseInt(s)] != "DCI" && noteItemType[parseInt(s)] != "SMS") {
        if (noteItemType[parseInt(s)] == "MASTER") {
            Block();
            SetDivMode("Master");
            SetMasterMode("Edit");
            LoadMaster(noteID[s]);
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

// 各動作完成
function Excutecomplete(objRet) {
    $.unblockUI();
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "InsertMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "UpdateMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "DeleteMaster":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "InsertController":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "UpdateController":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "DeleteController":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "InsertReader":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "UpdateReader":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "DeleteReader":
                    __doPostBack(TreeView_UpdatePanel.id, 'Refalsh');
                    break;
                case "Edit":
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
                    break;
                case "Delete":
                    __doPostBack(UpdatePanel1.id, 'popPagePost');
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
            Master_Input_ComPortParam_ComPort.value = '';
            Master_Input_ComPortParam_BaudRate.value = '9600';
            Master_Input_ComPortParam_Parity.value = 'N';
            Master_Input_ComPortParam_DataBits.value = '8';
            Master_Input_ComPortParam_StopBits.value = '1';
            Master_Input_Type_TCPIP.checked = true;
            SetParamDiv('IPParam');
            Master_Input_Model.value = '';
            Master_Input_Status.value = '1';
            Master_Input_CtrlModel.value = '';
            Master_Input_LinkMode_Always.checked = true;
            Master_Input_AutoRerun_Y.checked = true;
            Master_Input_No.disabled = false;
            Master_Input_Desc.disabled = false;
            Master_Input_Type_TCPIP.disabled = false;
            Master_Input_Type_COMPort.disabled = false;
            Master_Input_IPParam_IP.disabled = false;
            Master_Input_IPParam_Port.disabled = false;
            Master_Input_ComPortParam_ComPort.disabled = false;
            Master_Input_ComPortParam_BaudRate.disabled = false;
            Master_Input_ComPortParam_Parity.disabled = false;
            Master_Input_ComPortParam_DataBits.disabled = false;
            Master_Input_ComPortParam_StopBits.disabled = false;
            Master_Input_Model.disabled = false;
            Master_Input_Status.disabled = false;
            Master_Input_CtrlModel.disabled = false;
            Master_Input_LinkMode_OneShot.disabled = false;
            Master_Input_LinkMode_Always.disabled = false;
            Master_Input_AutoRerun_Y.disabled = false;
            Master_Input_AutoRerun_N.disabled = false;
            Master_B_Add.style.display = "inline";
            Master_B_Edit.style.display = "none";
            Master_B_Delete.style.display = "none";
            break;
        case 'Edit':
            Master_Input_IPParam_IP.value = '';
            Master_Input_IPParam_Port.value = '';
            Master_Input_ComPortParam_ComPort.value = '';
            Master_Input_ComPortParam_BaudRate.value = '9600';
            Master_Input_ComPortParam_Parity.value = 'N';
            Master_Input_ComPortParam_DataBits.value = '8';
            Master_Input_ComPortParam_StopBits.value = '1';
            Master_Input_No.disabled = false;
            Master_Input_Desc.disabled = false;
            Master_Input_Type_TCPIP.disabled = false;
            Master_Input_Type_COMPort.disabled = false;
            Master_Input_IPParam_IP.disabled = false;
            Master_Input_IPParam_Port.disabled = false;
            Master_Input_ComPortParam_ComPort.disabled = false;
            Master_Input_ComPortParam_BaudRate.disabled = false;
            Master_Input_ComPortParam_Parity.disabled = false;
            Master_Input_ComPortParam_DataBits.disabled = false;
            Master_Input_ComPortParam_StopBits.disabled = false;
            Master_Input_Model.disabled = false;
            Master_Input_Status.disabled = false;
            Master_Input_CtrlModel.disabled = true;
            Master_Input_LinkMode_OneShot.disabled = false;
            Master_Input_LinkMode_Always.disabled = false;
            Master_Input_AutoRerun_Y.disabled = false;
            Master_Input_AutoRerun_N.disabled = false;
            Master_B_Add.style.display = "none";
            Master_B_Edit.style.display = "inline";
            Master_B_Delete.style.display = "none";
            break;
        case 'Delete':
            Master_Input_IPParam_IP.value = '';
            Master_Input_IPParam_Port.value = '';
            Master_Input_ComPortParam_ComPort.value = '';
            Master_Input_ComPortParam_BaudRate.value = '9600';
            Master_Input_ComPortParam_Parity.value = 'None';
            Master_Input_ComPortParam_DataBits.value = '8';
            Master_Input_ComPortParam_StopBits.value = 'One';
            Master_Input_No.disabled = true;
            Master_Input_Desc.disabled = true;
            Master_Input_Type_TCPIP.disabled = true;
            Master_Input_Type_COMPort.disabled = true;
            Master_Input_IPParam_IP.disabled = true;
            Master_Input_IPParam_Port.disabled = true;
            Master_Input_ComPortParam_ComPort.disabled = true;
            Master_Input_ComPortParam_BaudRate.disabled = true;
            Master_Input_ComPortParam_Parity.disabled = true;
            Master_Input_ComPortParam_DataBits.disabled = true;
            Master_Input_ComPortParam_StopBits.disabled = true;
            Master_Input_Model.disabled = true;
            Master_Input_Status.disabled = true;
            Master_Input_CtrlModel.disabled = true;
            Master_Input_LinkMode_OneShot.disabled = true;
            Master_Input_LinkMode_Always.disabled = true;
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
}

// 讀取Master資料
function LoadMaster(MstID) {
    ClickItem.value = MstID;
    PageMethods.LoadMaster(MstID, SetMasterUI, onPageMethodsError);
}

// 讀取Master資料回填UI
function SetMasterUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        Master_Input_No.value = DataArray[0];
        Master_Input_Desc.value = DataArray[1];
        Master_Input_Dci.innerText = DataArray[2];
        if (DataArray[3] == "T") {
            SetParamDiv('IPParam');
            Master_Input_Type_TCPIP.checked = true;
            var IpArray = DataArray[4].split(':');
            Master_Input_IPParam_IP.value = IpArray[0];
            Master_Input_IPParam_Port.value = IpArray[1];
        }
        else if (DataArray[3] == "C") {
            SetParamDiv('ComPortParam');
            Master_Input_Type_COMPort.checked = true;
            var ComArray = DataArray[4].split(':');
            var ParamArray = ComArray[1].split(',');
            Master_Input_ComPortParam_ComPort.value = ComArray[0];
            Master_Input_ComPortParam_BaudRate.value = ParamArray[0];
            Master_Input_ComPortParam_Parity.value = ParamArray[1];
            Master_Input_ComPortParam_DataBits.value = ParamArray[2];
            Master_Input_ComPortParam_StopBits.value = ParamArray[3];
        }
        Master_Input_Model.value = DataArray[5];
        Master_Input_Status.value = DataArray[6];
        Master_Input_CtrlModel.value = DataArray[7];
        if (DataArray[8] == "0") {
            Master_Input_LinkMode_OneShot.checked = true;
        }
        else if (DataArray[8] == "1") {
            Master_Input_LinkMode_Always.checked = true;
        }

        if (DataArray[9] == "1") {
            Master_Input_AutoRerun_Y.checked = true;
        }
        else if (DataArray[9] == "0") {
            Master_Input_AutoRerun_N.checked = true;
        }
        DciIDValue.value = DataArray[10];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

//取得Master畫面資料
function GetMasterDataArray() {
    var DataArray = [];
    DataArray[0] = Master_Input_No.value;
    DataArray[1] = Master_Input_Desc.value;
    DataArray[2] = DciIDValue.value;
    if (Master_Input_Type_TCPIP.checked) {
        DataArray[3] = "T";
        DataArray[4] = Master_Input_IPParam_IP.value + ":" + Master_Input_IPParam_Port.value;
    }
    else if (Master_Input_Type_COMPort.checked) {
        DataArray[3] = "C";
        DataArray[4] = Master_Input_ComPortParam_ComPort.value + ":" + Master_Input_ComPortParam_BaudRate.value + "," +
            Master_Input_ComPortParam_Parity.value + "," + Master_Input_ComPortParam_DataBits.value + "," + Master_Input_ComPortParam_StopBits.value;
    }
    DataArray[5] = Master_Input_Model.value;
    DataArray[6] = Master_Input_Status.value;
    DataArray[7] = Master_Input_CtrlModel.value;
    if (Master_Input_LinkMode_OneShot.checked) {
        DataArray[8] = "0";
    }
    else if (Master_Input_LinkMode_Always.checked) {
        DataArray[8] = "1";
    }
    if (Master_Input_AutoRerun_Y.checked) {
        DataArray[9] = "1";
    }
    else if (Master_Input_AutoRerun_N.checked) {
        DataArray[9] = "0";
    }
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
    PageMethods.UpdateMaster(hideUserID.value, ClickItem.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Master動作
function DeleteMasterExcute() {
    PageMethods.DeleteMaster(hideUserID.value, ClickItem.value, Excutecomplete, onPageMethodsError);
}
/*Master操作相關------ End ------*/

/*Controller操作相關------ Start ------*/
// Controller操作
function SetControllerMode(sMode) {
    switch (sMode) {
        case 'Add':
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
            Controller_B_Add.style.display = "inline";
            Controller_B_Edit.style.display = "none";
            Controller_B_Delete.style.display = "none";
            SetAddReaderDisplay('Add');
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
            SetAddReaderDisplay('Edit');
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
    ClickItem.value = CtrlID;
    PageMethods.LoadController(CtrlID, SetControllerUI, onPageMethodsError);
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
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

// 取得Controller畫面資料
function GetControllerDataArray() {
    var DataArray = [];
    DataArray[0] = Controller_Input_No.value;
    DataArray[1] = Controller_Input_Name.value;
    DataArray[2] = Controller_Input_Desc.value;
    DataArray[3] = MstIDValue.value;
    DataArray[4] = Controller_Input_Model.innerText;
    DataArray[5] = Controller_Input_Status.value;
    DataArray[6] = Controller_Input_Addr.value;
    DataArray[7] = Controller_Input_AddReader.value;
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
    PageMethods.UpdateController(hideUserID.value, ClickItem.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Controller動作
function DeleteControllerExcute() {
    PageMethods.DeleteController(hideUserID.value, ClickItem.value, Excutecomplete, onPageMethodsError);
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
            Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = false;
            Reader_Input_Name.disabled = false;
            Reader_Input_Desc.disabled = false;
            Reader_Input_EquNo.disabled = false;
            Reader_Input_Dir.disabled = false;
            Reader_B_Add.style.display = "inline";
            Reader_B_Edit.style.display = "none";
            Reader_B_Delete.style.display = "none";
            break;
        case 'Edit':
            Reader_Input_No.value = '';
            Reader_Input_Name.value = '';
            Reader_Input_Desc.value = '';
            Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = false;
            Reader_Input_Name.disabled = false;
            Reader_Input_Desc.disabled = false;
            Reader_Input_EquNo.disabled = false;
            Reader_Input_Dir.disabled = false;
            Reader_B_Add.style.display = "none";
            Reader_B_Edit.style.display = "inline";
            Reader_B_Delete.style.display = "none";
            break;
        case 'Delete':
            Reader_Input_No.value = '';
            Reader_Input_Name.value = '';
            Reader_Input_Desc.value = '';
            Reader_Input_EquNo.value = '';
            Reader_Input_Dir.value = '進';
            Reader_Input_CtrlName.innerText = '';
            Reader_Input_CtrlModel.innerText = '';
            Reader_Input_No.disabled = true;
            Reader_Input_Name.disabled = true;
            Reader_Input_Desc.disabled = true;
            Reader_Input_EquNo.disabled = true;
            Reader_Input_Dir.disabled = true;
            Reader_B_Add.style.display = "none";
            Reader_B_Edit.style.display = "none";
            Reader_B_Delete.style.display = "inline";
            break;
        default:
            break;
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
    ClickItem.value = ReaderID;
    PageMethods.Reader_GetDropDownList(ReaderID, CreateDDL);
}

//讀取Reader資料回填UI
function SetReaderUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        if (DataArray[6].indexOf('SC300') > -1) {
            //Reader_Label_EquNo.innerText = "對應設備 (SC300系列將同步修改)";
        }
        else {
            //Reader_Label_EquNo.innerText = "對應設備";
        }
        Reader_Input_No.value = DataArray[0];
        Reader_Input_Name.value = DataArray[1];
        Reader_Input_Desc.value = DataArray[2];
        Reader_Input_EquNo.value = DataArray[3];
        Reader_Input_Dir.value = DataArray[4];
        Reader_Input_CtrlName.innerText = DataArray[5];
        Reader_Input_CtrlModel.innerText = DataArray[6];
        CtrlIDValue.value = DataArray[7];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }

    $.unblockUI();
}

//取得Reader畫面資料
function GetReaderDataArray() {
    var DataArray = [];
    DataArray[0] = Reader_Input_No.value;
    DataArray[1] = Reader_Input_Name.value;
    DataArray[2] = Reader_Input_Desc.value;
    DataArray[3] = Reader_Input_EquNo.value;
    DataArray[4] = Reader_Input_Dir.value;
    DataArray[5] = CtrlIDValue.value;
    DataArray[6] = Reader_Input_CtrlModel.innerText;
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
    PageMethods.UpdateReader(hideUserID.value, ClickItem.value, DataArray, Excutecomplete, onPageMethodsError);
}

//執行刪除Reader動作
function DeleteReaderExcute() {
    PageMethods.DeleteReader(hideUserID.value, ClickItem.value, Excutecomplete, onPageMethodsError);
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
