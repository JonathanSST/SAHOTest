// 執行新增動作
function SaveExcute() {
    //PageMethods.SaveChange(hideUserID.value, hidePsnID.value, Input_OldPWD.value, Input_NewPWD.value, Input_CheckPWD.value, Excutecomplete, onPageMethodsError);
    if (!JsFunBASE_VERTIFY()) {
        return false;
    }
    var regExp = RegExp("^((?=.{12,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{12,}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@[\]\^_`{|}~-]).*)");
    var str = $("#NewPwd").val();
    if (!regExp.test(str)) {
        alert($("#msgRegRule").val());
        return false;
    }
    if ($("#CheckPwd").val() != $("#NewPwd").val()) {
        alert($("#msgConfirm").val());
        return false;
    }
    console.log($('.Content').find('input,select').serialize());
    $.ajax({
        type: "POST",
        url: "ChangePWD2.aspx",
        data: $('.Content').find("input,select").serialize() + "&DoAction=Update",
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.resp.result) {
                $('[name*="Pwd"]').val("");
                alert($("#msgSuccess").val());
            } else {
                alert(data.resp.message)
            }
        }
    });
    

}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "SaveChange":
                    
                    alert(document.getElementById("msgSuccess").value);
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
