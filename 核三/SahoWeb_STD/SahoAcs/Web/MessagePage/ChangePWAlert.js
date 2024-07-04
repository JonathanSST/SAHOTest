function DivMode() {
    AlertDiv.style.display = "none";
    UIDiv.style.display = "";
}

function ChangePWExcute() {
    PageMethods.ChangePW(hideUserID.value, Input_OldPWD.value, Input_NewPWD.value, Input_CheckPWD.value, Excutecomplete, onPageMethodsError);
}

function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "ChangePW":
                    alert('密碼變更完成');
                    document.location.href = "../../Default.aspx";
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