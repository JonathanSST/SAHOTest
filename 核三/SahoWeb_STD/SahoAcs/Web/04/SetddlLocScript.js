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
