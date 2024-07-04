//預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');
    
    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    __doPostBack(UpdatePanel1.id, 'StarePage');
}


//儲存資料
function DoSave()
{
    var result_val = "";
    for (var i = 1; i <= 10; i++) {
        console.log("PickTime$PickTime");
        console.log('[name*= "PickTime$PickTime"]');
        if ($('[name*="PickTime' + i+'$PickTime"]').val() != "") {
            if (result_val != "") {
                result_val += ",";
            }
            result_val += i + ":" + $('[name*="PickTime' + i + '$PickTime"]').val().replace(":","");
        }
    }
    $(target_para_obj).val(result_val);
    DoClose();
}

function DoClose() {
    $("#ParaPopContent").remove();
}