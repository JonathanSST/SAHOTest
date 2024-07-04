// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');
    
    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    __doPostBack(UpdatePanel1.id, 'StarePage');
}

//儲存畫面InputComponent資料
function SaveBuzzerCard() {

    var objForm = document.forms["form1"];
    var objLen = objForm.length;
    var CardFormatValue = "";
    var BuzzerValue = "";

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].name != null && (objForm.elements[iCount].name == "CardFormat" || objForm.elements[iCount].name == "Buzzer")) {
            if (objForm.elements[iCount].checked && objForm.elements[iCount].name == "CardFormat")
                CardFormatValue = objForm.elements[iCount].value;
            if (objForm.elements[iCount].checked && objForm.elements[iCount].name == "Buzzer")
                BuzzerValue = objForm.elements[iCount].value;
        }
    }

    PageMethods.SaveData(hideUserID.value, hideEquID.value, hideEquParaID.value, CardFormatValue, BuzzerValue, Excutecomplete, onPageMethodsError);
}

// 各動作完成
function Excutecomplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case "SaveData":
                    window.returnValue = objRet.message;
                    window.close();
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

// Radio選取
function RadioSelected(Obj) {
    document.getElementById(Obj).checked = true;
}


//儲存資料
function DoSave() {
    //組合所有卡片規則
    var CardBuzzerModeRule = "";
    CardBuzzerModeRule = $('input[name*="CardFormat"]:checked').val() + $('input[name*="Buzzer"]:checked').val();
    $(target_para_obj).val(CardBuzzerModeRule);
    $("#ParaPopContent").remove();
}


function DoClose() {
    $("#ParaPopContent").remove();
}