$(document).ready(function () {
    $('#BtnBackUp').click(function () {
        DoProcess();
    });
});



function DoProcess() {
    $.ajax({
        type: "POST",
        url: location.href,
        data: { "PageEvent": "DoBackup" },
        dataType: "json",
        success: function (data) {
            if (data.is_success) {
                alert(data.message);
            } else {
                alert(data.message);
            }
        },
        fail: function () {
            alert("資料備份失敗");
        }
    });
}