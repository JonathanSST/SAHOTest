
function SaveEquVms() {
    var equ_array = new Array();
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        if ($(this).find('input[name*="ChkOne"]:eq(0)').prop("checked") == true) {
            equ_array.push({ "EquNo": $(this).find('input[name*="EquNo"]').val() });
        }        
    });

    console.log(JSON.stringify(equ_array));    
    var post_data = {
        "PageEvent": "Save","EquVmsList": equ_array
    };
    
    $.ajax({
        type: "POST",
        url: "AntiPassList.aspx",
        data: JSON.stringify(post_data),
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.success === true) {
                alert("更新完成");
            }
        },
        fail: function () {
            alert("更新失敗");
        }
    });
    
}

function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}