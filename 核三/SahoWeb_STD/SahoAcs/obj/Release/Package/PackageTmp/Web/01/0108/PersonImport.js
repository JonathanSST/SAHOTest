// 顯示Dci錯誤資料
function ShowDciErrorData() {
    PageMethods.ShowDciErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Mst錯誤資料
function ShowMstErrorData() {
    PageMethods.ShowMstErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Ctrl錯誤資料
function ShowCtrlErrorData() {
    PageMethods.ShowCtrlErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Reader錯誤資料
function ShowReaderErrorData() {
    PageMethods.ShowReaderErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Reader錯誤資料
function ShowEquGroupErrorData() {
    PageMethods.ShowEquGroupErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Org錯誤資料
function ShowOrgErrorData() {
    PageMethods.ShowOrgErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示OrgStruc錯誤資料
function ShowOrgStrucErrorData() {
    PageMethods.ShowOrgStrucErrorData(Excutecomplete, onPageMethodsError);
}

// 顯示Psn錯誤資料
function ShowPsnCardErrorData() {
    PageMethods.ShowPsnCardErrorData(Excutecomplete, onPageMethodsError);
}

//// 顯示Card錯誤資料
//function ShowCardErrorData() {
//    PageMethods.ShowCardErrorData(Excutecomplete, onPageMethodsError);
//}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            break;
        case false:
            switch (objRet.act) {
                case "ShowDciError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowDciError');
                    break;
                case "ShowMstError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowMstError');
                    break;
                case "ShowCtrlError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowCtrlError');
                    break;
                case "ShowReaderError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowReaderError');
                    break;
                case "ShowEquGroupError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowEquGroupError');
                    break;
                case "ShowOrgError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowOrgError');
                    break;
                case "ShowOrgStrucError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowOrgStrucError');
                    break;
                case "ShowPsnError":
                    hideErrorData.value = objRet.message;
                    __doPostBack(EquMsg_UpdatePanel.id, 'ShowPsnError');
                    break;
            }
            break;
        default:

            break;
    }
}

function ScrollBottom() {
    window.scroll(0, document.body.scrollHeight);
}