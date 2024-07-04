function SetMsgMode(sMode) {
    switch (sMode) {
        case '':
            Input_LogTime.innerText = '';
            Input_Source.innerText = '';
            Input_MsgContent.value = '';
            break;
    }
}

function CreateTypeList(MsgType) {
    SetMsgMode('');
    __doPostBack(TypeList.id, MsgType);
}


function LoadMsg() {
    if (IsEmpty(SelectValue.value)) NotSelectForEdit(Msg);
    else {
        PageMethods.LoadMsg(SelectValue.value, SetUI, onPageMethodsError);
    }
}


// 將資料帶回畫面UI
function SetUI(DataArray) {
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        Input_LogTime.innerText = DataArray[0];
        Input_Source.innerText = DataArray[1];
        Input_MsgContent.value = DataArray[2];
    }
    else {
        var objRet = new Object;
        objRet.result = false;
        objRet.message = DataArray[1];
        Excutecomplete(objRet);
    }
}
