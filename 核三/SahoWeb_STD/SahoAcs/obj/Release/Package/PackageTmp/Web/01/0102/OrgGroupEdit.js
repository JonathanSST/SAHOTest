
function SetOrgGroupEdit() {
    if (!IsEmpty(SelectValue.value))
    {
        //cardid = List_CardInfo.options[List_CardInfo.selectedIndex].value;
        var d = new Date();
        var n = d.getHours() + d.getMinutes() + d.getSeconds();
        
        $.post("OrgGroupEdit.aspx",
            { DoAction: "Query", OrgStrucID: SelectValue.value, random: n },
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
                $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
                    var hid = $(this).find('input[name="Mode_hid"]:eq(0)').val();
                    var that = $(this);
                    //設定讀卡規則欄位
                    if ($(this).find('input[name="CardRuleID"]:eq(0)').val() != "" && $(this).find('input[name="CardRuleID"]:eq(0)').val()!="0") {
                        $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
                    }
                    $(this).find('select[name*="CardRule_"]:eq(0)').change(function () {
                        $(that).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                    });
                    if ($(this).find("td:eq(2)").find('input[name*="HasAuth"]').val() == "V") {
                        $(this).find("td").css("background-color", "NAVY").css("color","#fff");
                    }
                    if ($(this).find('input[name*="EquClass"]').val() != "Elevator") {
                        $(this).find('.IconGroupSet').prop("disabled", true);
                    }
                    if (hid == "-") {
                        $(this).find('input[name*="OpMode_"][value="0"]').prop("checked", true);
                    } else if (hid == "+"||hid=="*") {
                        $(this).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                    } else {
                        $(this).find('input[name*="OpMode_"][value="2"]').prop("checked", true);
                    }                    
                    $(this).find('input[name*="OpMode_"]').click(function () {
                        //$('input[name*="ChkEquID"]:eq(' + index + ')').prop("checked", true);
                    });
                    var equ_id = $(this).find('input[name*="EquID"]').val();
                    var ext_obj = $(this).find('input[name*="CardExtData"]:eq(0)');
                    var equ_name = $(this).find('input[name*="EquName"]').val();
                    $(this).find('.IconGroupSet').first().click(function () {
                        OpenFloor(this, equ_id, equ_name, index);
                    });
                });
            }
        );
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}


function GroupSetting(action) {
    if (action == "add") {
        $("#InCardEquGroup").append($("#OutCardEquGroup option:selected"));
        $("#OutCardEquGroup option:selected").remove();
    } else {
        $("#OutCardEquGroup").append($("#InCardEquGroup option:selected"));
        $("#InCardEquGroup option:selected").remove();
    }
    return false;
}

function SelectAll(obj)
{
    var check = $(obj).prop("checked");    
    $('input[name*="ChkEquID"]').each(function () {
        $(this).prop("checked", check);
    });
}

function SaveSetting(action) {    
    $("#InCardEquGroup option").each(function () {
        $(this).prop("selected", true);
    });
    $("#OutCardEquGroup option").each(function () {
        $(this).prop("selected", true);
    });
    var equall = "";
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        var hid = $(this).find('input[name*="OpMode_"]:checked').val();
        if (hid != "2") {
            equall+=$(this).find('input[name="EquID"]').val() + ",";
        }
    });
    equall = equall.slice(0, -1);
    $("#EquIDAll").val(equall);
    var form_data = $("#form_adj").serialize();    
    $.ajax({
        type: "POST",
        url: "OrgGroupEdit.aspx",
        dataType: "json",
        data: form_data,
        success: function (data) {
            alert($("#ResultMsg").val());
            //console.log(data);
            DoCancel();
            //SetCardGroupEdit();
        },
        fail: function () {
            alert("failed");
        }
    });
    
}

//開啟電梯控制UI
function OpenFloor(obj, val, val3, val4) {
    //console.log($('input[name*="CardExtData"]:eq(' + val4 + ')').val());
    $.post("OrgFloorSetting.aspx", {
        equ_id: val,        
        OrgStrucId: $('#OrgStrucID').val(),
        card_ext_data:$('input[name*="CardExtData"]:eq(' + val4 + ')').val(),
        equ_name: val3, equ_index: val4
    }, function (data) {
        $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:39999; overflow:hidden;'
                    + ' -webkit-transform: translate3d(0,0,0);"></div>');
        $("#popOverlay2").css("background", "#000");
        $("#popOverlay2").css("opacity", "0.5");
        $("#popOverlay2").width("100%");
        $("#popOverlay2").height($(document).height());
        $(document.body).append('<div id="ElevPop" style="position:absolute;z-index:40000;background-color:#1275BC;'
            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
        $("#ElevPop").html(data);
        $("#ElevPop").css("left", ($(document).width() - $("#ElevPop").width()) / 2);
        $("#ElevPop").css("top", $(document).scrollTop() + 50);
        $("#popOverlay2").click(function () {
            $("#ElevPop").remove();
            $("#popOverlay2").remove();
        });
    });
}

function OpenOnReady() {

}




function DoCancel() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}