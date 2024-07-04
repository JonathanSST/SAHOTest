var extdata;

$(document).ready(function () {
    SetBindEvent();
});

function SetBindEvent() {
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        //設定讀卡規則欄位
        if ($(this).find('select[name*="CardRule_"]:eq(0) option[value="' + $(this).find('input[name="CardRuleID"]:eq(0)').val() + '"]').length > 0) {
            $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
        }
        //if ($(this).find('input[name="CardRuleID"]:eq(0)').val() != "" && $(this).find('input[name="CardRuleID"]:eq(0)').val() != "0") {
        //    $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
        //}
        var that = $(this);
        if ($(this).find('input[name*="EquGrpID"]').val() != "0") {
            $(this).find("td").css("background-color", "NAVY").css("color", "#fff");
        }
        if ($(that).find("#BtnFloor").length > 0) {
            $(that).find("#BtnFloor").click(function () {
                extdata = $(that).find('input[name*="CardExtData_"]').first();
                var equid = $(that).find('input[name*="CHK_COL_1"]:eq(0)').val();
                var equname = $(that).find("td:eq(1)").html();
                OpenFloor(equid, equname, index);
            });
        }
        //設定讀卡規則異動事件
        $(that).find('select[name*="CardRule_"]:eq(0)').change(function () {
            $(this).closest("TR").find("[name='CHK_COL_1']").prop("checked", true);
        });
    });//Detail 的資料foreach
}

function SetSave() {
    var group_id = $("#EquGroup").val();
    var formdata = $("#GroupEdit").find("input,select").serialize();
    $.ajax({
        type: "POST",
        url: "EquGroupEdit.aspx",
        data: formdata,
        success: function (data) {
            $("#GroupEdit").html($(data).find("#GroupEdit").html());
            //$("#TableDetail").find("tr").each(function (index) {
            //    //設定讀卡規則欄位
            //    if ($(this).find('input[name="CardRuleID"]:eq(0)').val() != "" && $(this).find('input[name="CardRuleID"]:eq(0)').val() != "0") {
            //        $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
            //    }
            //});
            SetBindEvent();
            alert("更新完成");
        },
        fail: function () {
            alert("failed");
        }
    });
}



//開啟電梯控制UI
function OpenFloor(val, val3, val4) {    
    $.post("EquGroupFloorSetting.aspx", {
        equ_id: val,       
        card_ext_data: $('input[name*="CardExtData"]:eq(' + val4 + ')').val(),
        equ_name: val3,
        equ_index: val4
    }, function (data) {
        $('<div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000'
           + ';background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;" ></div>').appendTo("#GroupEdit");
        $("#dvContent").html(data);
    });
}