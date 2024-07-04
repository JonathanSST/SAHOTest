// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;

    if (vArguments !== "") {
        var paraqueue = vArguments.split('&');
        hideEquID.value = paraqueue[0];
        hideEquParaID.value = paraqueue[1];
        hideParaValue.value = paraqueue[2];
    }

    __doPostBack(UpdatePanel1.id, 'StarePage');
}



//儲存資料
function DoSave() {    
    $(target_para_obj).val($("#ParaPopContent").find('#Input_Mode').val());
    //console.log($("#ParaPopContent").find('#Input_Mode').val());
    $("#ParaPopContent").remove();
}

function DoClose() {
    $("#ParaPopContent").remove();
}
