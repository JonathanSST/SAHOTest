


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
            else if (noteItemType[parseInt(s)] == "DCI") {
                document.getElementById('new_childItem').innerText = "新增連線裝置";
                document.getElementById('del_childItem').innerText = "刪除連線";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "DCI";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "MASTER") {
                document.getElementById('new_childItem').innerText = "新增控制器";
                document.getElementById('del_childItem').innerText = "刪除連線裝置";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "MASTER";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "CONTROLLER") {
                document.getElementById('new_childItem').innerText = "新增讀卡機及設備";
                document.getElementById('del_childItem').innerText = "刪除控制器";
                document.getElementById('new_childItem').style.display = "";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "CONTROLLER";
                RightClickItem.value = noteID[parseInt(s)];
            }
            else if (noteItemType[parseInt(s)] == "READER") {
                document.getElementById('new_childItem').innerText = "";
                document.getElementById('del_childItem').innerText = "刪除讀卡機及設備";
                document.getElementById('new_childItem').style.display = "none";
                document.getElementById('del_childItem').style.display = "";
                NodeAct.value = "READER";
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
    else if (NodeAct.value == "DCI") {
        SetDivMode("Master");
        SetMasterMode("Add");
        CallCreateMaster(RightClickItem.value);
    }
    else if (NodeAct.value == "MASTER") {
        /*增加控制器數量的判斷*/
        if (parseInt($("#CurrentCtrls").val()) < parseInt($("#MaxCtrls").val())) {
            SetDivMode("Controller");
            SetControllerMode("Add");
            CallCreateController(RightClickItem.value);
        } else {
            alert("目前控制器已到達限制使用數量");
        }
    }
    else if (NodeAct.value == "CONTROLLER") {
        SetDivMode("Reader");
        SetReaderMode("Add");
        CallCreateReader(RightClickItem.value);
    }
}

// 點擊刪除目錄動作
function clickdelMenu() {
    if (NodeAct.value == "DCI") {
        SetDivMode("DeviceConnInfo");
        SetDeviceConnInfoMode("Delete");
       
        LoadDeviceConnInfo(RightClickItem.value);
    }
    else if (NodeAct.value == "MASTER") {
        SetDivMode("Master");
        SetMasterMode("Delete");
        
        LoadMaster(RightClickItem.value);
    }
    else if (NodeAct.value == "CONTROLLER") {
        SetDivMode("Controller");
        SetControllerMode("Delete");
        
        LoadController(RightClickItem.value);
    }
    else if (NodeAct.value == "READER") {
        SetDivMode("Reader");
        SetReaderMode("Delete");
        
        LoadReader(RightClickItem.value);
    }

    ShowConFirmDialog('確認刪除資料？資料刪除後無法恢復。').then(function (answer) {
        var ansbool = Boolean.parse(answer.toString());
        if (ansbool) {
            if (NodeAct.value == "DCI")
            {
                DeleteDciExcute(); return false;
            }
            else if (NodeAct.value == "MASTER")
            {
                DeleteMasterExcute(); return false;
            }
            else if (NodeAct.value == "CONTROLLER")
            {
                DeleteControllerExcute(); return false;
            }
            else if (NodeAct.value == "READER")
            {
                DeleteReaderExcute(); return false;
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


/*新增設備 Start*/
function AddEquData() {    
    //$.colorbox({        
    //    href: "EquDataInsert.aspx?CtrlEquModel=" + Reader_Input_CtrlModel.innerText + "&EquName=" + encodeURIComponent(Reader_Input_Name.value), overlayClose: false,        
    //    onComplete: function () {
    //        SetEquTypeArea();            
    //    }
    //});    
    $.ajax({
        type: "POST",
        url: 'EquClassFrm.aspx',
        data: {},
        async: true,
        success: function (data) {
            $(document.body).append('<div id="baseOverlay" style="position:absolute; top:0; left:0; z-index:19999; overflow:hidden;opacity:0.9;'
                + ' -webkit-transform: translate3d(0,0,0);width:100%;height:100%;background:#fff"></div>');
            $(document.body).append('<div id="ParaDiv" style="position:absolute;left:20px;top:30px;z-index:20000;background-color:#1275BC" ></div>');
            $('#baseOverlay').height($(document).height());
            $("#ParaDiv").html(data);
            $("#ParaDiv").css("left", ($(document).width() - $("#ParaDiv").width()) / 2);
            $("#ParaDiv").css("top", $(document).scrollTop() + 50);
            $("#popB_Cancel").click(function () {
                $("#ParaDiv").remove();
                $("#baseOverlay").remove();
            });
        }
    });
}

function SetEquData(equ_class) {
    $.ajax({
        type: "POST",
        url: equ_class+"Form.aspx",
        data: {
            "CtrlEquModel": Reader_Input_CtrlModel.innerText, "EquName": Reader_Input_Name.value
        },
        async: true,
        success: function (data) {
            var top = $("#colorbox").css("top");
            var left = $("#colorbox").css("left");
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
            $("#ParaExtDiv").css("top", $("#popOverlay").scrollTop() + 50);
        }
    });
}


function SaveEquData() {    
    var form_data = $("#form_insert").serialize();    
    $.ajax({
        type: "POST",
        url: "EquDataInsert.aspx",
        dataType: "json",
        data: form_data,
        success: function (data) {
            if (data.message != "") {
                //alert(data.message);



            } else {
                $.colorbox.close();
                PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
            }
        },
        fail: function () {
            ShowDialog('alert', '警告訊息', "Process failed");
        }
    });
    return false;
}

//重設EquData 的清單
function QueryEquData() {   
    PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
}

function Cancel() {
    $("#ParaDiv").remove();
    $("#baseOverlay").remove();    
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
    return false;
}

function CancelOne() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
    return false;
}


function ChangeEquType()
{
    SetEquTypeArea();
}

function SetEquTypeArea() {
    var equ_type = $('select[name*="EquClass"]:eq(0)').val();
    $(".ShowInTrt").hide();
    $(".ShowInDoor").hide();
    if (equ_type == "TRT") {
        $(".ShowInTrt").show();
    }
    if (equ_type == "Door Access") {
        $(".ShowInDoor").show();
    }    
    $.colorbox.resize();
    //$("#MainDiv").height(600);
}

/*新增設備 End*/