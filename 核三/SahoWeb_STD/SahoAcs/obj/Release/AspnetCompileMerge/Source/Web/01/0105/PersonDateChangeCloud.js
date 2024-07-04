function Default(dt) {
    Input_Time.value = dt;
}

//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = '';
    if (str == 'Add') {
        for (var i = 0; i < popB_PsnList1.options.length; i++) {
            if (popB_PsnList1.options[i].selected) {
                popB_PsnList2.add(new Option(popB_PsnList1.options[i].text, popB_PsnList1.options[i].value));
            }
        }
        for (var i = popB_PsnList1.options.length - 1; i >= 0; i--) {
            if (popB_PsnList1.options[i].selected) {
                popB_PsnList1.remove(i);
            }
        }
    }
    else if (str == 'Del') {
        for (var i = 0; i < popB_PsnList2.options.length; i++) {
            if (popB_PsnList2.options[i].selected) {
                popB_PsnList1.add(new Option(popB_PsnList2.options[i].text, popB_PsnList2.options[i].value));
            }
        }
        for (var i = popB_PsnList2.options.length - 1; i >= 0; i--) {
            if (popB_PsnList2.options[i].selected) {
                popB_PsnList2.remove(i);
            }
        }
    }
}

function LoadPsnDataList() {
    CancelTrigger1.click();
    while (DataList.length != 0)
        DataList.remove(0);
    if (popB_PsnList2.length > 0) {
        for (var i = 0; i < popB_PsnList2.options.length; i++) {
            DataList.add(new Option(popB_PsnList2.options[i].text, popB_PsnList2.options[i].value));
        }
    }
}

function QueryPsnData() {
    while (popB_PsnList1.length != 0)
        popB_PsnList1.remove(0);
    PageMethods.QueryPsnData(Input_TxtQuery.value, hUserId.value, LoadPsnData, onPageMethodsError);
}

function LoadPsnData(data) {
    if (data[0] != 'Saho_SysErrorMassage') {
        var psnid = data[0].split("|");
        var psnno = data[1].split("|");
        var psnname = data[2].split("|");
        var psntype = data[3].split("|");
        var option = null;
        for (var i = 0; i < psnid.length; i++) {
            var state = 0;
            option = document.createElement("option");
            option.text = psnno[i] + ' ' + psnname[i];
            option.value = psnid[i];
            for (var j = 0; j < popB_PsnList2.length; j++) {
                if (psnid[i] == popB_PsnList2.options[j].value) {
                    state += 1;
                }
            }
            if (state == 0)
                popB_PsnList1.options.add(option);

        }
    }
    else
        alert(data[1]);
}

function ExecProcData() {
    var psndata = '';
    var timetype = ddlType.options[ddlType.selectedIndex].value;
    var datetime = Input_Time.value;
    for (var i = 0; i < DataList.length; i++) {
        psndata += DataList.options[i].value + '|' + DataList.options[i].text + '|';
    }
    console.log(psndata);
    console.log(timetype);
    console.log(datetime);
    PageMethods.ExecProcData(psndata, timetype, datetime, hUserId.value, ExecProcMsg, onPageMethodsError);
}

function ExecProcMsg(data) {
    if (data.length > 0) {
        if (data[0] != 'Saho_SysErrorMassage') {
            var datalist = data[1].split('|');
            for (var i = 0; i < datalist.length; i++) {
                List_Msg.add(new Option(datalist[i], 'none'));
                List_Msg.selectedIndex = List_Msg.length - 1;
            }
            List_Msg.add(new Option(data[0], 'none'));
            List_Msg.selectedIndex = List_Msg.length - 1;
        }
        else
            alert(data[1]);
    }
}