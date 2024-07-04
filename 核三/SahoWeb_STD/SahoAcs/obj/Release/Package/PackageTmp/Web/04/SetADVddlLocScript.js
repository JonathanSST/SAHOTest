function SetADVLocListItem() {

    while (ContentPlaceHolder1_ADVddlLocArea.options.length > 0)
        ContentPlaceHolder1_ADVddlLocArea.options.remove(0);

    while (ContentPlaceHolder1_ADVddlLocBuilding.options.length > 0)
        ContentPlaceHolder1_ADVddlLocBuilding.options.remove(0);

    while (ContentPlaceHolder1_ADVddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ADVddlLocFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocArea.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocFloor.options.add(option);

    var AreaItem = ContentPlaceHolder1_txtAreaList.value.split(',');
    var BuildingItem = ContentPlaceHolder1_txtBuildingList.value.split(',');
    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < AreaItem.length; i++) {
        DataStr = AreaItem[i].split("|");

        option = document.createElement("option");
        option.value = DataStr[0];
        option.text = "[" + DataStr[1] + "]" + DataStr[2];
        ContentPlaceHolder1_ADVddlLocArea.options.add(option);

    }

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");
        if (DataStr[3] == ContentPlaceHolder1_tmpLocArea.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ADVddlLocBuilding.options.add(option);

        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");
        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {

            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ADVddlLocFloor.options.add(option);
        }
    }


    for (i = 0; i < ContentPlaceHolder1_ADVddlLocArea.options.length; i++) {
        ContentPlaceHolder1_ADVddlLocArea.options[i].selected = false;

        if (ContentPlaceHolder1_ADVddlLocArea.options[i].value == ContentPlaceHolder1_sLocArea.value) {
            ContentPlaceHolder1_ADVddlLocArea.options[i].selected = true;
        }
    }
    for (i = 0; i < ContentPlaceHolder1_ADVddlLocBuilding.options.length; i++) {
        ContentPlaceHolder1_ADVddlLocBuilding.options[i].selected = false;

        if (ContentPlaceHolder1_ADVddlLocBuilding.options[i].value == ContentPlaceHolder1_sLocBuilding.value) {
            ContentPlaceHolder1_ADVddlLocBuilding.options[i].selected = true;
        }
    }
    for (i = 0; i < ContentPlaceHolder1_ADVddlLocFloor.options.length; i++) {
        ContentPlaceHolder1_ADVddlLocFloor.options[i].selected = false;

        if (ContentPlaceHolder1_ADVddlLocFloor.options[i].value == ContentPlaceHolder1_sLocFloor.value) {
            ContentPlaceHolder1_ADVddlLocFloor.options[i].selected = true;
        }
    }
}

function SelectADVArea() {
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ADVddlLocArea.value;
    SetADVLocBuildingListItem();
    $.unblockUI();
}

function SelectADVBuilding() {
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ADVddlLocArea.value;
    ContentPlaceHolder1_tmpLocBuilding.value = ContentPlaceHolder1_ADVddlLocBuilding.value;
    SetADVLocFloorListItem();
    $.unblockUI();
}

function SetADVLocBuildingListItem() {
    while (ContentPlaceHolder1_ADVddlLocBuilding.options.length > 0)
        ContentPlaceHolder1_ADVddlLocBuilding.options.remove(0);

    while (ContentPlaceHolder1_ADVddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ADVddlLocFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocBuilding.options.add(option);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocFloor.options.add(option);

    var BuildingItem = ContentPlaceHolder1_txtBuildingList.value.split(',');
    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < BuildingItem.length; i++) {
        DataStr = BuildingItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocArea.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ADVddlLocBuilding.options.add(option);
        }
    }

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ADVddlLocFloor.options.add(option);
        }
    }
}

function SetADVLocFloorListItem() {
    while (ContentPlaceHolder1_ADVddlLocFloor.options.length > 0)
        ContentPlaceHolder1_ADVddlLocFloor.options.remove(0);

    option = document.createElement("option");
    option.value = '';
    option.text = "選取資料";
    ContentPlaceHolder1_ADVddlLocFloor.options.add(option);

    var FloorItem = ContentPlaceHolder1_txtFloorList.value.split(',');

    for (i = 0; i < FloorItem.length; i++) {
        DataStr = FloorItem[i].split("|");

        if (DataStr[3] == ContentPlaceHolder1_tmpLocBuilding.value) {
            option = document.createElement("option");
            option.value = DataStr[0];
            option.text = "[" + DataStr[1] + "]" + DataStr[2];
            ContentPlaceHolder1_ADVddlLocFloor.options.add(option);
        }
    }
}
