
function OnLoad(){    
    $(".GV_Row2").each(function () {
        var card_no = $(this).find("td:eq(2)").html();
        $(this).find(".IconRefresh").click(function () {
            DoReset(card_no);
        });
    });
}
    

function DoReset(card_no) {
    Block();
    $.ajax({
        type: "POST",
        url: "ResetCardAuth.aspx",
        data: { 'CardNo': card_no, 'PageEvent': 'Save' },
        success: function (data) {
            //alert('UpdateOk');
            //location.href = location.href;
            $("#ContentPlaceHolder1_tablePanel").html($(data).find("#ContentPlaceHolder1_tablePanel").html());
            OnLoad();
            $.unblockUI();
        }
    });
}