// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');
    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    __doPostBack(UpdatePanel1.id, 'StarePage');
}

//新增規則
function AddRule() {
    __doPostBack(popB_Add.id,'');
}


function DoRemove() {

}

//新增規則
function DoAdd() {
    var input_no = $('input[name*="popInput_CardRuleIndex"]').val();
    var check_result = true;
    if (input_no == "" || isNaN(input_no))
    {
        alert('編號必須為數字');
        check_result = false;
        return false;
    }
    if (input_no < 0 || input_no > 99) {
        alert('提醒：時區規則為0~99');
        check_result = false;
        return false;
    }
    $("#CardRuleGridView").find("TR").each(function () {
        //console.log(padLeft(input_no, 2));
        if ($(this).find("td:eq(0)").html() == padLeft(input_no,2)) {
            alert('提醒：時區編號不得重覆。');
            check_result = false;
            return false;
        }
    });
    if(check_result===true){
        $("#CardRuleGridView").append('<tr style="background-color: #069; color: #fff">'
                                                            + '<td align="center" style="width: 41px;">'
                                                            + input_no
                                                            +'</td>'
                                                            + '<td style="width: 232px;">'
                                                            + '<input type="hidden" value="' + $('select[name*="popInput_CardRule"]').val() + '" name="CardRule" />'
                                                            + $('select[name*="popInput_CardRule"] option:selected').text()
                                                            +'</td>'
                                                            +'<td align="center">'
                                                            +'<input type="button" value="刪除" class="IconDelete" onclick="DoRemove()" />'
                                                            +'</td>'
                                                        +'</tr>');
    }
}

//儲存資料
function DoSave() {
    //組合所有卡片規則
    var cardrule_str = "";
    $("#CardRuleGridView").find("TR").each(function () {                
        cardrule_str += parseInt($(this).find("td:eq(0)").html())+":"+$(this).find("input[name*='CardRule']").val()+",";
    });
    $(target_para_obj).val(cardrule_str.slice(0, -1));
    $("#ParaExtDiv").html("");
}


function padLeft(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return padLeft("0" + str, lenght);
}