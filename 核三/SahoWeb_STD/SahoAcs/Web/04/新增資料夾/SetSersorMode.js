$(document).ready(function () {
    SetLocListItem();

    $("#HeaderCheckState").click(function () {
        if ($("#HeaderCheckState").prop("checked")) {
            $("input[name='RowCheckState[]']").each(function () {
                $(this).prop("checked", true);
            });
        } else {
            $("input[name='RowCheckState[]']").each(function () {
                $(this).prop("checked", false);
            });
        }
    });

});

function SetStatus() {
    ContentPlaceHolder1_sSaveCheckList.value = '';

    $("input[name='RowCheckState[]']").each(function () {
        // 判斷是否被勾選
        if ($(this).prop('checked')) {
            // 取的已勾選的checkbox值
            ContentPlaceHolder1_sSaveCheckList.value += $(this).attr("value") + "|";
        }
    })

    ContentPlaceHolder1_sSenStatus.value = "1";
    if (ContentPlaceHolder1_SenStatus_0.checked) {
        ContentPlaceHolder1_sSenStatus.value = "0";
    } else if (ContentPlaceHolder1_SenStatus_2.checked) {
        ContentPlaceHolder1_sSenStatus.value = "2";
    }

    $("#PageEvent").val("SetStatus");

    Block();

    $.ajax({
        type: "POST",
        url: "SetSersorMode.aspx",
        data: $("#form1").find("input").serialize(),
        success: function (data) {
            var oResult = JSON.parse(data).result;
            var oMsg = JSON.parse(data).message;
            var oAct = JSON.parse(data).act;

            alert(oMsg);
            if (oAct == "Y") {
                SetMode(1);
            } else {
                $.unblockUI();
            }
           
        }
    });
}


function SetMode(mode) {
    ContentPlaceHolder1_sCtrlModel.value = ContentPlaceHolder1_ddlCtrlMode.value;

    ContentPlaceHolder1_sLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    ContentPlaceHolder1_sLocBuilding.value = ContentPlaceHolder1_ddlLocBuilding.value;
    ContentPlaceHolder1_tmpLocBuilding.value = ContentPlaceHolder1_ddlLocBuilding.value;
    ContentPlaceHolder1_sLocFloor.value = ContentPlaceHolder1_ddlLocFloor.value;

    $("#QueryMode").val(mode);
    ShowPage(1);
}

function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    SetQuery();
}


function SetQuery() {
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "SetSersorMode.aspx",
        //dataType: 'json',
        data: $("#form1").find("input").serialize(),
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
            $.unblockUI();
        }
    });
}

///設定拖曳功能設定
function BindEvent() {
    $('.GVStyle').find('th').each(function (index) {
        if (index < $('.GVStyle').find('th').length - 1) {
            $(this).css("width", $(this).find('[name*="TitleCol"]').val());
        }
    });
    $('.DataRow').each(function (outIndex) {
        var that = $(this);
        $(that).find('td').each(function (index) {
            if (index < $(that).find('td').length - 1) {
                $(this).css("width", $(this).find('[name*="DataCol"]').val());
            }
        });
    });
    $(".GVStyle").sortable({
        opacity: 0.6,    //拖曳時透明
        cursor: 'pointer',  //游標設定
        axis: 'x,y',       //只能垂直拖曳
        update: function () {
            SetQuery();
        }
    });
}

function SetLocListItem() {

    while (ContentPlaceHolder1_ddlLocArea.options.length > 0)
        ContentPlaceHolder1_ddlLocArea.options.remove(0);

    while (ContentPlaceHolder1_ddlLocBuilding.options.length > 0)
        ContentPlaceHolder1_ddlLocBuilding.options.remove(0);

    while (ContentPlaceHolder1_ddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ddlLocFloor.options.remove(0);


    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocFloor.options.add(option);

    var AreaItem = ContentPlaceHolder1_txtAreaList.value.split(',');
    var BuildingItem = ContentPlaceHolder1_txtBuildingList.value.split(',');
    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");

        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        ContentPlaceHolder1_ddlLocArea.options.add(option);

    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == ContentPlaceHolder1_tmpLocArea.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ddlLocBuilding.options.add(option);

        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ddlLocFloor.options.add(option);
        }
    }


    for (i = 0; i < ContentPlaceHolder1_ddlLocArea.options.length; i++) {
        ContentPlaceHolder1_ddlLocArea.options[i].selected = false;

        if (ContentPlaceHolder1_ddlLocArea.options[i].value == ContentPlaceHolder1_sLocArea.value) {
            ContentPlaceHolder1_ddlLocArea.options[i].selected = true;
        }
    }
    for (i = 0; i < ContentPlaceHolder1_ddlLocBuilding.options.length; i++) {
        ContentPlaceHolder1_ddlLocBuilding.options[i].selected = false;

        if (ContentPlaceHolder1_ddlLocBuilding.options[i].value == ContentPlaceHolder1_sLocBuilding.value) {
            ContentPlaceHolder1_ddlLocBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < ContentPlaceHolder1_ddlLocFloor.options.length; i++) {
        ContentPlaceHolder1_ddlLocFloor.options[i].selected = false;

        if (ContentPlaceHolder1_ddlLocFloor.options[i].value == ContentPlaceHolder1_sLocFloor.value) {
            ContentPlaceHolder1_ddlLocFloor.options[i].selected = true;
        }
    }
}

function SelectArea() {
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    SetLocBuildingListItem();
    $.unblockUI();
}

function SelectBuilding() {
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    ContentPlaceHolder1_tmpLocBuilding.value = ContentPlaceHolder1_ddlLocBuilding.value;
    SetLocFloorListItem();
    $.unblockUI();
}

function SetLocBuildingListItem() {
    while (ContentPlaceHolder1_ddlLocBuilding.options.length > 0)
        ContentPlaceHolder1_ddlLocBuilding.options.remove(0);

    while (ContentPlaceHolder1_ddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ddlLocFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocFloor.options.add(option);

    var BuildingItem = ContentPlaceHolder1_txtBuildingList.value.split(',');
    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ddlLocBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ddlLocFloor.options.add(option);
        }
    }
}

function SetLocFloorListItem() {
    while (ContentPlaceHolder1_ddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ddlLocFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ddlLocFloor.options.add(option);

    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ddlLocFloor.options.add(option);
        }
    }
}
