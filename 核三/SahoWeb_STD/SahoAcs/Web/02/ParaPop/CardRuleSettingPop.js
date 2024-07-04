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




//新增規則
function DoAdd() {
    var input_no = $('input[name*="popInput_CardRuleIndex"]').val();
    var check_result = true;
    if ($("#popInput_CardRule").val() == "") {
        alert('必須選取時區規則!!');
        check_result = false;
        return false;
    }
    if (input_no == "" || isNaN(input_no)) {
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
        if ($(this).find("td:eq(0)").html() == padLeft(input_no, 2)) {
            alert('提醒：時區編號不得重覆。');
            check_result = false;
            return false;
        }
    });
    if (check_result === true) {
        $("#CardRuleGridView").append('<tr style="background-color: #069; color: #fff">'
                                                            + '<td align="center" style="width: 41px;">'
                                                            + padLeft(input_no, 2)
                                                            + '</td>'
                                                            + '<td style="width: 232px;">'
                                                            + '<input type="hidden" value="' + $('select[name*="popInput_CardRule"]').val() + '" name="CardRule" />'
                                                            + $('select[name*="popInput_CardRule"] option:selected').text()
                                                            + '</td>'
                                                            + '<td align="center">'
                                                            + '<input type="button" value="刪除" class="IconDelete" onclick="DoRemove(this)" />'
                                                             + '<input type="hidden" value="-1" id="RemoveID" name="RemoveID" />'
                                                            + '</td>'
                                                        + '</tr>');
        $('input[name*="popInput_CardRuleIndex"]').val($("#CardRuleGridView").find("TR").length);
    }
}

//儲存資料
function DoSave() {
    //組合所有卡片規則
    var cardrule_str = "";
    $("#CardRuleGridView").find("TR").each(function () {
        cardrule_str += parseInt($(this).find("td:eq(0)").html()) + ":" + $(this).find("input[name*='CardRule']").val() + ",";
    });
    $(target_para_obj).val(cardrule_str.slice(0, -1));
    $("#ParaPopContent").remove();
}

var removelist = ""

function DoRemove(obj) {
    //$(obj).parent().parent().remove();
    if (removelist != "") {
        removelist += ",";
    }
    removelist += $(obj).parent().find("#RemoveID").val();


    $.ajax({
        type: "POST",
        url: "/Web/02/paraPop/CardRuleSettingPop.aspx",
        data: { "PageEvent": "CheckValue", "CardRule": $(obj).parent().find("#RemoveID").val(), "EquID": hideEquID.value },
        //contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.has_rule === true) {
                if (confirm("讀卡規則" + data.rule + "已被卡片定義使用，是否繼續進行規則刪除?")) {
                    $(obj).parent().parent().remove();
                }
            } else {
                $(obj).parent().parent().remove();
            }
        },
        fail: function () {
            alert("error");
        }
    });
}


function DoClose() {
    $("#ParaPopContent").remove();
}

function padLeft(str, lenght) {
    if (str.length >= lenght)
        return str;
    else
        return padLeft("0" + str, lenght);
}