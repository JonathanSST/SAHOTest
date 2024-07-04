function DisplayTime() {
    const now = new Date();
    now.getHours(); // 取得本地的小時（0~23）            
    console.log(now.getTimezoneOffset());
    $('[name*="HiddenTimeOffset"]').val(now.getTimezoneOffset());
    $.ajax({
        type: "POST",
        url: "/ReCheck.ashx",
        data: { "UserName": $("#labUserName").text(), "TimeOffset": $('[name*="HiddenTimeOffset"]').val(), 'TimeOnceID': $('#TimeOnceID').val() },
        //contentType: "application/json",
        dataType: "json",
        success: function (data) {
            console.log(data);
            if (data.isSuccess == true) {
                console.log("Check ok");
                $('#TimeOnceID').val(data.time_once);
            } else {

            }
        },
        fail: function () {
            console.log("Error....");
        }
    }).done(function () {
        //$.unblockUI();
    });
}
setInterval("DisplayTime()", 60000);