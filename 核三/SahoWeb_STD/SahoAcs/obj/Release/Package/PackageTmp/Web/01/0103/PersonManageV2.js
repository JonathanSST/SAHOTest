var CodeStatus = 0;


$(document).ready(function () {
    $.post("ResultTable.aspx", {

    }, function (data) {
        $("#ResultArea").html(data);
        SetPageChange();
    });
    SetMode('Add');
});


function SetAuthMode() {
    $.ajax({
        url: location.href,
        type: "post",
        data: { 'VerifiMode': $('#VerifiMode').val(), 'PsnID': $('#SelectValue').val(), "PageEvent": "VerifiMode" },
        dataType: "json",
        success: function (data) {            
            alert('驗證模式更新完成!!');
        }
    });
}


//設定編輯模式介面介面
function SetMode(sMode) {
    $('#btSave').hide();
    $("#btUpdate").hide();
    $("#btDelete").hide();
    if (sMode == "Add") {
        $('#btSave').show();        
    } else if (sMode = "Edit") {        
        $("#btUpdate").show();
        $("#btDelete").show();
    }
    SetUserLevel();     //設定按鈕權限
    if (parseInt($("#CurrentCard").val()) >= parseInt($("#MaxCard").val())) {
        $("#BtnPsnAdd").prop("disabled", true);
        $("#btCardAdd").prop("disabled", true);
        if ($("#SelectValue").val() == "0")
            $("#btSave").prop("disabled", true);
    }
}



//產生新增人員介面資訊
function AddNewData() {
    $.ajax({
        url: location.href,
        type: "post",
        data: {},
        success: function (data) {
            $("#EditArea").html($(data).find("#EditArea").html());
            SetMode("Add");
            //$.unblockUI();
        }
    });
}


//儲存人員基本資料
function SaveData() {
    var re = /^[A-F0-9]+$/;
    var checkend = true;
    if ($("#PsnID").val() == "0" && re.test($("#CardNo").val()) == false) {
        alert("卡號須為16進制數字");
        checkend = false;
        $("#CardNo").focus();
        return false;
    }//end cardno check
    if ($("#PsnID").val() == "0" && $("#CardNo").val().length != parseInt($("#CardNo").prop("maxlength"))) {
        alert("卡號長度必須為" + $("#CardNo").prop("maxlength")+ "碼");
        $("#CardNo").focus();
        return false;
    }//end card length check
    if (JsFunBASE_VERTIFY() && checkend) {
        console.log($("#EditArea").find("input,select").serialize())
        //Block();
        $.ajax({
            url: location.href,
            type: "post",
            data: $("#EditArea").find("input,select,textarea").serialize(),
            success: function (data) {
                //沒有錯誤訊息再進行UI更新
                if ($(data).find("#ErrMsg").val() != "") {
                    console.log('error');
                    alert($(data).find("#ErrMsg").val());
                } else {
                    $("#EditArea").html($(data).find("#EditArea").html());
                    if ($("#PsnID").val() != "0") {
                        SetMode("Edit");
                        SetMainQuery($('.NowPage').text());
                    }
                }
            }
        });
    }//End Js Check
}//End SaveData


//刪除人員
function DeleteData() {
    if ($("#PsnID").val() == 0) {
        alert("請選擇需要刪除的人員");
        return false;
    }
    if (confirm("是否刪除[工號:" + $("#PsnNo").val() + "]人員資料")) {
        $.ajax({
            url: location.href,
            type: "post",
            data: { "PsnNo": $("#PsnNo").val(), "PsnID": $("#PsnID").val(), "PageEvent": "Delete" },
            success: function (data) {
                alert('刪除完成!!');
                AddNewData();
                SetMainQuery($(".NowPage").text());
            }
        });
    }
}



//執行資料查詢
function SetMainQuery(page) {
    $.post("ResultTable.aspx", {
        "PageIndex": page, "QueryName": $("#ContentPlaceHolder1_InputWord").val(), "Comp":$("#CompanyList").val(), "Dept":$("#DeptList").val(), PsnType:$("#PsnTypeList").val()
    }, function (data) {
        $("#ResultArea").html(data);
        SetPageChange();
        GridSelect();
    });
}


//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, "tablePanel", 'MainGridView', 0, $("#PsnNo").val());
}


//設定使用者按鈕的權限
function SetUserLevel() {
    var str = $("#AuthList").val();
    $("#btSave").prop("disabled", true);
    $("#btUpdate").prop("disabled", true);
    $("#btDelete").prop("disabled", true);
    $("#btCardInfo").prop("disabled", true);
    $("#btCardAdd").prop("disabled", true);
    $("#BtSetting").prop("disabled", true);
    $("#BtCardGroup").prop("disabled", true);
    $("#btnChange").prop("disabled", true);
    $("#BtnPsnAdd").prop("disabled", true);
    var bool_current = true;
   
    console.log(str);
    if (str != '') {
        var data = str.split(",");        
        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                $("#btSave").prop("disabled", false);
                //$("#btCardAdd").prop("disabled", false);
                $("#BtSetting").prop("disabled", false);
                $("#BtCardGroup").prop("disabled", false);
                $("#BtnPsnAdd").prop("disabled", false);
            }
            if (data[i] == 'Edit') {
                $("#btUpdate").prop("disabled", false);
                $("#btCardAdd").prop("disabled", false);
                $("#BtSetting").prop("disabled", false);
                $("#BtCardGroup").prop("disabled", false);
                $("#btnChange").prop("disabled", false);
                $("#btnPic").prop("disabled", false);
                $("#btCardInfo").prop("disabled", false);
            }
            if (data[i] == 'Del') {
                $("#btDelete").prop("disabled", false);
            }
            //btCardInfo.disabled = false;
            
        }
    }
}



//執行換頁控制項註冊
function SetPageChange() {
    $("#ResultArea").find(".PageLink").each(function () {
        var pageindex = $(this).text();
        $(this).click(function () {
            SetMainQuery(pageindex);
        });
    });
    $(".PageNext").click(function () {
        var pagename = $(".NowPage").text();
        var pageindex = parseInt(pagename);
        pageindex++;
        SetMainQuery(pageindex);
    });
    $(".PagePrev").click(function () {
        var pagename = $(".NowPage").text();
        var pageindex = parseInt(pagename);
        pageindex--;
        SetMainQuery(pageindex);
    });
    $(".PageFirst").click(function () {       
        SetMainQuery(1);
    });
    $(".PageLast").click(function () {
        var pagename = $("#PageCount").val();
        var pageindex = parseInt(pagename);
        SetMainQuery(pageindex);
    });
    //設定人員基本資料選取、修改
    $("#MainGridView").find("tr").each(function () {
        var psnid = $(this).find("#RowSelectID").val();        
        $(this).click(function () {
            SingleRowSelect(0, this, $('#PsnID')[0], psnid, '', '');
            CallEdit(psnid);
        });
    });
}


//設定人員基本資料編輯
function CallEdit(psn_id) {
    Block();
    $.ajax({
        url: location.href,
        type: "post",
        data: { "PsnID": psn_id, "PageEvent": "Edit" },
        success: function (data) {
            $("#EditArea").html($(data).find("#EditArea").html());
            SetMode('Edit');
            CodeStatus = 0;
            //設定第一筆卡號selected
            $('#CardInfo')[0].selectedIndex = 0;
            $.unblockUI();
        }
    });
}


//設定卡片資料編輯
function CallCardEdit(type) {
    var cardid = "";
    if (type == "update")
    {
        cardid = $("#CardInfo").val();
    }
    $.post("CardEditV2.aspx",
           { "DoAction": type, "CardID": cardid,"PsnID":$("#PsnID").val() },
           function (data) {
               $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
               + ' -webkit-transform: translate3d(0,0,0);"></div>');
               $("#popOverlay").css("background", "#000");
               $("#popOverlay").css("opacity", "0.5");
               $("#popOverlay").width($(document).width());
               $("#popOverlay").height($(document).height());
               $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                   + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
               $("#ParaExtDiv").html($(data).find('#PanelPopup1').html());
               $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
               $(document).scrollTop(0);
               $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
               if (type == "add")
                   $("#popCardNo").prop('disabled', false);
           }//end function
       );//end ajax post
}

//關閉遮罩區塊資訊
function DoCancel() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


//開啟卡片權限設定
function SetCardAuthEdit() {
    cardid = $("#CardInfo").val();
    var d = new Date();
    var n = d.getHours() + d.getMinutes() + d.getSeconds();

    $.post("CardGroupEdit.aspx",
        { DoAction: "Query", card_id: cardid, random: n },
        function (data) {
            $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
            + ' -webkit-transform: translate3d(0,0,0);"></div>');
            $("#popOverlay").css("background", "#000");
            $("#popOverlay").css("opacity", "0.5");
            $("#popOverlay").width($(document).width());
            $("#popOverlay").height($(document).height());
            $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
            $("#ParaExtDiv").html(data);
            $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
            $(document).scrollTop(0);
            $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
            $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
                var equ_id = $(this).find('input[name*="EquID"]').val();
                var ext_obj = $(this).find('input[name*="CardExtData"]:eq(0)');
                var equ_name = $(this).find('input[name*="EquName"]').val();
                var hid = $(this).find('input[name="Mode_hid"]:eq(0)').val();
                var that = $(this);
                //設定讀卡規則欄位
                //if ($(this).find('input[name="CardRuleID"]:eq(0)').val() != "" && $(this).find('input[name="CardRuleID"]:eq(0)').val()!="0") {
                //    $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
                //}
                if ($(this).find('select[name*="CardRule_"]:eq(0) option[value="' + $(this).find('input[name="CardRuleID"]:eq(0)').val() + '"]').length > 0) {
                    $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
                }
                if ($(this).find('input[name="VerifiMode"]').length > 0) {
                    $(this).find('select[name*="VerifiMode_"]:eq(0)').val($(this).find('input[name="VerifiMode"]:eq(0)').val());
                }
                $(this).find('select[name*="CardRule_"]:eq(0)').change(function () {
                    $(that).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                });
                if ($(this).find("td:eq(2)").find('input[name*="HasAuth"]').val() == "V") {
                    $(this).find("td").css("background-color", "NAVY").css("color", "#fff");
                }
                if ($(this).find('input[name*="EquClass"]').val() != "Elevator") {
                    $(this).find('.IconGroupSet').prop("disabled", true);
                }
                if (hid == "-") {
                    $(this).find('input[name*="OpMode_"][value="0"]').prop("checked", true);
                } else if (hid == "+" || hid == "*") {
                    $(this).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                } else {
                    $(this).find('input[name*="OpMode_"][value="2"]').prop("checked", true);
                }
                $(this).find('input[name*="OpMode_"]').click(function () {
                    //$('input[name*="ChkEquID"]:eq(' + index + ')').prop("checked", true);
                });

                $(this).find('.IconGroupSet').first().click(function () {
                    OpenFloor(this, equ_id, equ_name, index);
                });
            });

            if (CodeStatus == 1) {
                $("#BtnCodeSetting").prop("disabled", true);
            }

        }//end success
    );//end ajax post
}//end function