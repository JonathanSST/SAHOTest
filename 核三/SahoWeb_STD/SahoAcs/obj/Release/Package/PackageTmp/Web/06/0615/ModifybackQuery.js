// 執行新增動作
function SaveExcute() {
    PageMethods.SaveChange(hideUserID.value, Input_OldPWD.value, Input_NewPWD.value, Input_CheckPWD.value, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "SaveChange":
                    Input_OldPWD.value = "";
                    Input_NewPWD.value = "";
                    Input_CheckPWD.value = "";
                    alert('密碼變更完成');
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
