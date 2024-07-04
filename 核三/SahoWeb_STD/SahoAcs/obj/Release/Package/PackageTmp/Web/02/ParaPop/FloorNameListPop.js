// 預先讀取資料
function LoadData() {
    var vArguments = window.dialogArguments;
    var paraqueue = vArguments.split('&');

    hideEquID.value = paraqueue[0];
    hideEquParaID.value = paraqueue[1];
    hideParaValue.value = paraqueue[2];
    hideFloorName.value = decodeURIComponent(paraqueue[3]);
    __doPostBack(UpdatePanel1.id, 'StarePage');
}

function SelectState() {
    SelectValue.value = '';
    __doPostBack(Input_EquModel.id, '');
}

//設定樓層數
function SetElevatorRow() {
    // __doPostBack(popB_SetElevatorButton.id, popInput_FloorCount.value);
    var floorCount = parseInt($("#FloorCount").val());
    for (var i = $("#Elevator_GridView").find("tbody tr").length; i < floorCount; i++) {
        console.log(i);
        $("#Elevator_GridView").find("tbody").append('<tr><td align="center" style="border-width:1px;border-style:solid;width:100px;">' + i + '</td>'
            + '<td style="border-width:1px;border-style:solid;">'
            + '<input name="FloorName' + i + '"  id="FloorName' + i + '" type="text" value="" style="border-width:0px;width:98%;" />'
            + '</td></tr>');
    }
    while($("#Elevator_GridView").find("tbody tr").length > floorCount) {
        $("#Elevator_GridView").find("tbody tr:last").remove();
    }
}

function ValidateNumber(e, pnumber) {
    if (!/^\d+$/.test(pnumber)) {
        e.value = /^\d+/.exec(e.value);
    }
    return false;
}




//儲存資料
function DoSave() {
    var InputNameList = new Array();

    $("#Elevator_GridView").find("tbody tr").each(function (index) {
        InputNameList.push({ "IoIndex": index, "FloorName": $(this).find('input[name*="FloorName"]').val() });
    });
    var post_data = {
        "NameList": InputNameList, "EquID": $("#hideEquID").val()
    };
    console.log(JSON.stringify(post_data));
    $.ajax({
        type: "POST",
        url: "/Web/02/paraPop/FloorNameListPop.aspx",
        data: "PageEvent=Save&PostData=" + JSON.stringify(post_data),
        //contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.success === true) {
                alert("更新完成");
                $("#ParaPopContent").remove();
            }
        },
        fail: function () {
            alert("更新失敗");
        }
    });
    
}

function DoClose() {
    $("#ParaPopContent").remove();
}