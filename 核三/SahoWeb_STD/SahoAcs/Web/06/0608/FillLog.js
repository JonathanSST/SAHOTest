function SetFillLog(val1, val2) {
    $.ajax({
        type: "POST",
        url: "FillLog.ashx",
        data: {
            "PsnNo": val1, "EquNo": val2
        },
        success: function (data) {
            alert(data);
        },
        fail: function () {
            alert("failed");
        }
    });
    //alert(val1 + "===" + val2);
}