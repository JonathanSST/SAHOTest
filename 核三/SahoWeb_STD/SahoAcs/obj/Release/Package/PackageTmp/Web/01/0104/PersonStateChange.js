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
    let poptype = $('#ContentPlaceHolder1_popddltype option:selected').val()
    $('#ContentPlaceHolder1_popB_PsnList2').empty()
    //console.log(poptype)
    while (popB_PsnList1.length != 0)
        popB_PsnList1.remove(0);
    PageMethods.QueryPsnData(Input_TxtQuery.value, hUserId.value, poptype, LoadPsnData, onPageMethodsError);
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
    var statetype = ddlType.options[ddlType.selectedIndex].value;
    for (var i = 0; i < DataList.length; i++) {
        psndata += DataList.options[i].value + '|' + DataList.options[i].text + '|';
    }
    PageMethods.ExecProcData(psndata, statetype, hUserId.value, ExecProcMsg, onPageMethodsError);
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

$(document).ready(function () {
    $("#btSelectOrgData").click(function () {
        $.post("OrgStrucSelect.aspx",
           { DoAction: "Query"},
           function (data) {
               $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
               + ' -webkit-transform: translate3d(0,0,0);"></div>');
               $("#popOverlay").css("background", "#000");
               $("#popOverlay").css("opacity", "0.5");
               $("#popOverlay").width("100%");
               $("#popOverlay").height($(document).height());
               $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                   + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
               $("#ParaExtDiv").html(data);
               $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
               $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);               
           }
       );
    });
});