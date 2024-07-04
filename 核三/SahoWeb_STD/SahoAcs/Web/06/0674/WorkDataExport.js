$(document).ready(function () {
    $("#BtnCalc").click(function () {
        Block();
        NowDate = $('[name*="$ExportDate$"]').val();
        CalcData();
    });
});
var NowDate = "";
var DateVal = new Date();
function CalcData() {
    $("#Date").val($('[name*="$ExportDate$"]').val());
    var date = $("#Date").val();    
    var dateE = $('[name*="$ExportDate2$"]').val();
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
    }
    

}

function ExportData() {
    $("#Date").val($('[name*="$ExportDate$"]').val());
    var date = $("#Date").val(); 
    Block();
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: {
            'WorkDate': date, 'WorkDateE': $('[name*="$ExportDate2$"]').val(),
            'PageEvent': 'Export'
        },
        dataType: 'json',
        success: function (data) {
            if (data.success) {
                $.unblockUI();
                alert(data.message);
            }
        },
        fail: function () {
            alert("匯入失敗");
            $.unblockUI();
        }
    });
}