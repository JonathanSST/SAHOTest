
function OnLoad(){    
    $(".GV_Row2").each(function () {
        //var card_no = $(this).find("td:eq(2)").html();
        
    });
}

function SetCheckAll(obj){
    $('input[name*="ChkCardNo"]').prop("checked", $(obj).prop("checked"));
}

function Query() {
    if ($("#QueryName").val() === "") {
        $("#QueryName").focus();
        alert("必須輸入查詢條件!!");
        return false;
    }
    Block();
    $.ajax({
        type: "POST",
        url: "CardAuthMode.aspx",
        data: { 'QueryType': $("#QueryType").val(), 'QueryName': $("#QueryName").val(), 'PageEvent': 'Query', 'UserID': $('input[name*="hideUserID"]').val() },
        success: function (data) {
            //alert('UpdateOk');
            //location.href = location.href;
            //console.log($(data).find("#ContentPlaceHolder1_tablePanel").html());
            $("#ContentPlaceHolder1_tablePanel").html($(data).find("#ContentPlaceHolder1_tablePanel").html());
            OnLoad();
            $.unblockUI();
        }
    });
}

    

function DoReset() {
    Block();
    console.log($("#form1:eq(0)").find('input[name="ChkCardNo"],input[name="EquID"],select[name="VerifiMode"]').serialize());
    $.ajax({
        type: "POST",
        url: "CardAuthMode.aspx",
        data: $("#form1:eq(0)").find('input[name="ChkCardNo"],input[name="EquID"],select[name="VerifiMode"]').serialize()+"&PageEvent=Save&UserID="+$('input[name*="hideUserID"]').val(),
        success: function (data) {
            //alert('UpdateOk');
            alert("更新完成!!");
            //location.href = location.href;
            //$("#ContentPlaceHolder1_tablePanel").html($(data).find("#ContentPlaceHolder1_tablePanel").html());
            //OnLoad();
            $.unblockUI();
        }
    });
}