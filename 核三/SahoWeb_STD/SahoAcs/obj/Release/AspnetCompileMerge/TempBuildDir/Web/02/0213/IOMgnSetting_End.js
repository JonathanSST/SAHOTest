


// 顯示目錄
function showMenu() {
    if (event.srcElement.type != undefined) {
        var s = event.srcElement.id;
        var ind = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
        s = s.replace("ContentPlaceHolder1_EquOrg_TreeViewt", "");
        var noteItemType = txt_NodeTypeList.value.split(',');
        var noteID = txt_NodeIDList.value.split(',');

        //if (noteItemType[parseInt(s)] != "NONE" && noteItemType[parseInt(s)] != "SMS") 
        if (noteItemType[parseInt(s)] != "NONE") {
            if (noteItemType[parseInt(s)] == "SMS") {
                document.getElementById('new_childItem').innerText = "新增連線";
                document.getElementById('del_childItem').innerText = "";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "none";
                NodeAct.value = "SMS";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "IODCI") {
                document.getElementById('new_childItem').innerText = "新增I/O連線裝置";
                document.getElementById('del_childItem').innerText = "刪除連線";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "IODCI";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "IOMASTER") {
                document.getElementById('new_childItem').innerText = "新增I/O模組";
                document.getElementById('del_childItem').innerText = "刪除I/O連線裝置";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "IOMASTER";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "IOMODULE") {
                document.getElementById('new_childItem').innerText = "新增偵測器";
                document.getElementById('del_childItem').innerText = "刪除I/O模組";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "IOMODULE";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "SENSOR") {
                document.getElementById('new_childItem').innerText = "";
                document.getElementById('del_childItem').innerText = "刪除偵測器";
                document.getElementById('new_childItem').style.display = "none";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "SENSOR";
                RightClickItem.value = noteID[parseInt(s)];
            }

            var rightedge = document.body.clientWidth - event.clientX;
            var bottomedge = document.body.clientHeight - event.clientY;
            var Hvalue, Vvalue;

            if (rightedge < document.getElementById('ContentPlaceHolder1_RightClickmenu').offsetWidth)
                Hvalue = document.body.scrollLeft + event.clientX - document.getElementById('ContentPlaceHolder1_RightClickmenu').offsetWidth;
            else
                Hvalue = document.body.scrollLeft + event.clientX;
            if (bottomedge < document.getElementById('ContentPlaceHolder1_RightClickmenu').offsetHeight)
                Vvalue = document.body.scrollTop + event.clientY - document.getElementById('ContentPlaceHolder1_RightClickmenu').offsetHeight + document.documentElement.scrollTop;
            else
                Vvalue = document.body.scrollTop + event.clientY + document.documentElement.scrollTop;
            document.getElementById('ContentPlaceHolder1_RightClickmenu').style.left = Hvalue + "px";
            document.getElementById('ContentPlaceHolder1_RightClickmenu').style.top = Vvalue + "px";
            document.getElementById('ContentPlaceHolder1_RightClickmenu').style.visibility = "visible";
        }
        else {
            document.getElementById('ContentPlaceHolder1_RightClickmenu').style.visibility = "hidden";
        }
    }
    else {
        document.getElementById('ContentPlaceHolder1_RightClickmenu').style.visibility = "hidden";
    }

    return false;

}

// 隱藏目錄
function hideMenu() {
    document.getElementById('ContentPlaceHolder1_RightClickmenu').style.visibility = "hidden";
}

// 進入目錄風格
function highlightMenu() {
    if (event.srcElement.className == "menuitems") {
        event.srcElement.style.backgroundColor = "#999999";
        event.srcElement.style.color = "white";
    }
}

// 離開目錄風格
function lowlightMenu() {
    if (event.srcElement.className == "menuitems") {
        event.srcElement.style.backgroundColor = "";
        event.srcElement.style.color = "black";
        window.status = "";
    }
}

// 點擊新增目錄動作
function clickNewMenu() {
    if (NodeAct.value == "SMS") {
        SetDivMode("DeviceConnInfo");
        SetDeviceConnInfoMode("Add");
    }
    else if (NodeAct.value == "IODCI") {
        SetDivMode("IOMaster");
        SetIOMasterMode("Add");
        CallCreateIOMaster(RightClickItem.value);
    }
    else if (NodeAct.value == "IOMASTER") {
        SetDivMode("IOModule");
        SetIOModuleMode("Add");
        CallCreateIOModule(RightClickItem.value);
    }
    else if (NodeAct.value == "IOMODULE") {
        SetDivMode("Sensor");
        SetSensorMode("Add");
        CallCreateSensor(RightClickItem.value);
    }
}

// 點擊刪除目錄動作
function clickdelMenu() {
    if (NodeAct.value == "IODCI") {
        SetDivMode("DeviceConnInfo");
        SetDeviceConnInfoMode("Delete");
       
        LoadDeviceConnInfo(RightClickItem.value);
    }
    else if (NodeAct.value == "IOMASTER") {
        SetDivMode("IOMaster");
        SetIOMasterMode("Delete");
        
        LoadIOMaster(RightClickItem.value);
    }
    else if (NodeAct.value == "IOMODULE") {
        SetDivMode("IOModule");
        SetIOModuleMode("Delete");
        
        LoadIOModule(RightClickItem.value);
    }
    else if (NodeAct.value == "SENSOR") {
        SetDivMode("Sensor");
        SetSensorMode("Delete");
        
        LoadSensor(RightClickItem.value);
    }

    ShowConFirmDialog('確認刪除資料？資料刪除後無法恢復。').then(function (answer) {
        var ansbool = Boolean.parse(answer.toString());
        if (ansbool) {
            if (NodeAct.value == "IODCI")
            {
                DeleteDciExcute(); return false;
            }
            else if (NodeAct.value == "IOMASTER")
            {
                DeleteIOMasterExcute(); return false;
            }
            else if (NodeAct.value == "IOMODULE")
            {
                DeleteIOModuleExcute(); return false;
            }
            else if (NodeAct.value == "SENSOR")
            {
                DeleteSensorExcute(); return false;
            }
        } else {
            ShowDialog('alert', '警告訊息', '取消刪除資料');
        }
    });
}

// Dialog
function ShowConFirmDialog(strMsg) {
    var vIcon = "<img src='../../../../Img/Help.png' width='30px' />";

    var vHtml = '';
    vHtml += "<div align='center' valign='center'>";
    vHtml += "<table style='width: 90%'>";
    vHtml += "<tr>";
    vHtml += "<td style='text-align: middle; vertical-align: middle; width: 50px'>" + vIcon + "</td>";
    vHtml += "<td style='text-align: left; vertical-align: middle'>" + strMsg + "</td>";
    vHtml += "</tr>";
    vHtml += "</table>";
    vHtml += "</div>";

    var defer = $.Deferred();
    $(function () {
        $("#dialog1").html(vHtml);
        $("#dialog1").dialog({
            autoOpen: true,
            draggable: true,
            resizable: false,
            title: "確認訊息",
            closeOnEscape: true,
            buttons: {
                "確認": function () {
                    defer.resolve(true);
                    $(this).dialog('close');
                },
                "取消": function () {
                    defer.resolve(false);
                    $(this).dialog('close');
                }
            },
            close: function (dialog) {
                $(this).dialog('close');
            },
            modal: true
            //hide: { effect: "blind", duration: 1000 }
        });
    });

    return defer.promise();
}

