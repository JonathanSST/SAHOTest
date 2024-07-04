function DoUpload() {
    $("#form1").prop('enctype', 'multipart/form-data');
    $("#form1").submit();
}

//傳入FileControl，配合在asp.net的page load當中
function UploadFileAsync() {
    var FileControl = document.getElementById('UploadFile');
    var formData = new FormData();
    //抓取File Control中的檔案            
    var file = FileControl.files[0];
    formData.append("file", file);
    formData.append("PageEvent", "Save");
    //非同步post給自己
    var uploadServerSideScriptPath = window.location.href;
    $("#SpanBusing").css("display", "block");
    $.ajax({
        type: "POST",
        url: window.location.href,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            //$("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());                    
            //$("#ResultArea").html($(data).find("#ResultArea").html());
            //SetDataScroll();
            $("#SpanBusing").css("display", "none");
            $("#ProcMessage").text(data);
            $("#form1")[0].reset();
        }
    });
}

function ClearData() {
    $("#form1")[0].reset();
}