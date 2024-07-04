function DivMode() {
    AlertDiv.style.display = "none";
    UIDiv.style.display = "";
}

function ChangePWExcute() {
    var regExp = RegExp("^((?=.{12,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{12,}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@[\]\^_`{|}~-]).*)");
    var str = document.getElementById("Input_NewPWD").value;
    if (!regExp.test(str)) {
        alert("密碼原則：密碼必須為12碼以上英文大小寫及數字0~9組成");
        return false;
    }
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