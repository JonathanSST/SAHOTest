
function SaveDoorAccess() {
    var oFORM1 = document.getElementById("form_insert");
    console.log(oFORM1);
    var form_data = new FormData(oFORM1);
    console.log(form_data);
    //$("#form_insert").serialize();
    /*
    $.ajax({
        type: "POST",
        url: "DoorAccessForm.aspx",
        dataType: "json",
        data: form_data
    }).success(function (data) {
        if (data.message != "") {
            alert(data.message);
        } else {
            $("#ParaDiv").remove();
            $("#baseOverlay").remove();
            $("#ParaExtDiv").remove();
            $("#popOverlay").remove();
            PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
        }
    }).fail(function () {
        alert("failed");
    });
    */

    $.ajax({
        url: "DoorAccessForm.aspx",  //Server script to process data
        type: "POST",
        dataType: "json",
        data: form_data,        
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.message != "") {
                alert(data.message);
            } else {
                $("#ParaDiv").remove();
                $("#baseOverlay").remove();
                $("#ParaExtDiv").remove();
                $("#popOverlay").remove();
                PageMethods.SearchEquData(txtKeyWord.value, UpdateDDL, onPageMethodsError);
            }
        },
        fail: function () {
            alert("failed");
        }
    });
    return false;
}