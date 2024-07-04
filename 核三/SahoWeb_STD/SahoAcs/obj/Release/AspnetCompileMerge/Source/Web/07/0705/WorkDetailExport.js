$(document).ready(function () {
    $("#BtnCalc").click(function () {
        Block();
        NowDate = $('[name*="CalcDayS"]').val();
        CalcData();
    });
});
var NowDate = "";
var DateVal = new Date();
function CalcData() {
    var dateE = $('[name*="CalcDayE"]').val();
    console.log(NowDate);
    if (NowDate <= dateE) {
        $.ajax({
            type: "POST",
            url: window.location.href,
            data: {
                'WorkDate': NowDate, 'WorkDateE': NowDate,
                'PageEvent': 'Calc'
            },
            dataType: 'json',
            success: function (data) {
                DateVal = new Date(NowDate);
                DateVal.setDate(DateVal.getDate() + 1);
                var dd = String(DateVal.getDate()).padStart(2, '0');
                var mm = String(DateVal.getMonth() + 1).padStart(2, '0'); //January is 0!
                var yyyy = DateVal.getFullYear();
                NowDate = yyyy + "/" + mm + "/" + dd;
                CalcData();
            },
            fail: function () {
                alert("計算轉換失敗");
                $.unblockUI();
            }
        });

    } else {
        $.unblockUI();
        $("#PageIndex").val("1");
        SetQuery();
    }


}

function ShowPage(val) {
    $("#PageIndex").val(val);
    SetQuery();
}

//設定資料查詢
function SetQuery() { 
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            "DateS": $('[name*="CalcDayS"]').val(),
            "DateE": $('[name*="CalcDayE"]').val(),
            "PageEvent": "Query",            
            "PageIndex": $("#PageIndex").val()
        },
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            //BindEvent();
            $.unblockUI();
            //SetUserLevel();
        }
    });
}
