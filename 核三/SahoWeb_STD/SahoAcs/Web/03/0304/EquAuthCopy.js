function SelectState() {
    __doPostBack(QueryButton.id, '');
}

// 顯示PersonList
function ShowPersonList() {
    __doPostBack(PersonListUpdatePanel.id, SelectValue.value);
}

// CheckBox選取
function CheckBoxSelected(Obj) {
    if (document.getElementById(Obj).checked) {
        document.getElementById(Obj).checked = false;
    }
    else {
        document.getElementById(Obj).checked = true;
    }
}

// RadioButton控制
function AuthActSelected(Obj) {
    if (Input_AuthAct1.id == Obj) {
        Input_ConflictAct1.disabled = true;
        Input_ConflictAct2.disabled = true;
        Input_ConflictAct1.checked = false;
        Input_ConflictAct2.checked = false;
    }
    else {
        Input_ConflictAct1.disabled = false;
        Input_ConflictAct2.disabled = false;
    }
}

$(document).ready(function () {
    $('input[name*="AuthActType"]').click(function () {
        if ($('input[name*="AuthActType"]:checked').val() == "Cover") {
            $('input[name*="ConflictType"]').prop("disabled", true);
        } else {
            $('input[name*="ConflictType"]').prop("disabled", false);
        }
    });
});


function ShowLabel(Msg, Count, Type) {
    switch (Type) {
        case "Source":
            lblSource.innerHTML = Msg + Count + "筆)";
            break;
        case "Target":
            lblTarget.innerHTML = Msg + Count + "筆)";
            break;
        case "PersonList":
            lblPersonList.innerHTML = Msg + Count + "筆)";
            break;
    }
}


var count = 0;
var error_list = "";
var all_count = 0;
var doUnBlock = false;
var end_msg = "";
function SetCardAuthProcBach() {
    count = 0;
    var CardArray = $("input[name='CardNoSrc']").serializeArray();
    if (CardArray.length == 0) {
        $.unblockUI();
    }
    CardArray.forEach(function (i) {
        var cards = new Array();
        cards.push(i.value);
        var post_data = { "page_event": "Reset", "cards": cards };
        $.ajax({
            type: "POST",
            url: "EquAuthCopy.ashx",
            data: JSON.stringify(post_data),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                if (data.message != "OK") {
                    $("textarea[name*='MsgResultTextBox']:eq(0)").append(data.cards_ok + ".....Error ");
                } else {
                    error_list += data.cards_error;
                    $("textarea[name*='MsgResultTextBox']:eq(0)").append(data.cards_ok + ".....Success ");
                }
            },
            complete: function () {
                count++;
                $("textarea[name*='MsgResultTextBox']:eq(0)").append("已處理:" + count + "筆資料...\n");
                if (count == CardArray.length) {
                    $.unblockUI();
                }
            }
        });// 進行post
    });//進行卡號 loop
}


function SetAuthCopy(){
   
    var targets = new Array();

    if ($('input[name*="AuthActType"]:checked').length<=0) {
        alert("權限複製動作 必須指定");
        return;
    }

    if ($('input[name*="AuthActType"]:checked').val() == "Add" && $('input[name*="ConflictType"]:checked').length <= 0) {
        alert("權限附加進階動作 必須指定");
        return;
    }

    if ($('input[name*="SelectValue"]:eq(0)').val() == "") {
        alert("來源設備必須指定");
        return;
    }

    if ($('input[name*="SelectControl"]:checked').length <=0 ) {
        alert("目的設備必須選擇");
        return;
    }

   
    $("#ContentPlaceHolder1_TargetGridView").find("tr").each(function (index) {
        if ($(this).find('input[name*="SelectControl"]:checked').length > 0) {
            targets.push($(this).find('input[name*="EquID"]').val())
        }
    });
    Block();
    //執行來源資料的卡片權限重整
    SetCardAuthProcBach();
    //執行權限複製
    var post_data = {
        "page_event": "AuthCopy", "SourceID": $('input[name*="SelectValue"]:eq(0)').val(), "targets": targets,
        "AuthActMode": $('input[name*="AuthActType"]:checked').val(), "ConflictActMode": $('input[name*="ConflictType"]:checked').val()
    };
    $.ajax({
        type: "POST",
        url: "EquAuthCopy.ashx",
        data: JSON.stringify(post_data),
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.isSuccess == "OK") {
                count = 0;
                Block();
                end_msg = data.message;
                SetCardAuthProcBach();
            } else {

            }
        },
        fail: function () {
            console.log("Error....");
            $.unblockUI();
        }
    }).done(function(){
        //$.unblockUI();
    });
}