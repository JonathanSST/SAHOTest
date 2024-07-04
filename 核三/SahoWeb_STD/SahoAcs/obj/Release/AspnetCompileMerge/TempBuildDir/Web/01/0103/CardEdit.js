function DeleteCardData() {
    if ($("#CardID").val() == "0") {
        alert("請選擇需要刪除的卡號");
        return false;
    }
    if (confirm("是否刪除[卡號:" + $("#popCardNo").val() + "]")) {
        $.ajax({
            url: "CardEditV2.aspx",
            type: "post",
            data: { "CardID": $("#CardID").val(), "DoAction": "delete" },
            success: function (data) {
                alert('刪除完成!!');
                //重新產生人員基本資料編輯區            
                CallEdit($("#PsnID").val());
                DoCancel();
            }
        });
    }
}

function SaveCardData() {
    //儲存卡片資料
    console.log($("#ParaExtDiv").find("input,select,textarea").serialize());
    var re = /^[A-F0-9]+$/;
    var checkend = true;
    if (re.test($("#popCardNo").val()) == false) {
        alert("卡號須為16進制數字");
        checkend = false;
        $("#popCardNo").focus();
        return false;
    }//end cardno check
    if ($("#popCardNo").val().length != parseInt($("#popCardNo").prop("maxlength"))) {
        alert("卡號長度必須為" + $("#popCardNo").prop("maxlength") + "碼");
        $("#popCardNo").focus();
        return false;
    }//end card length check
    if ($("#popCardPW").val().length != parseInt($("#popCardPW").prop("maxlength"))) {
        alert("密碼長度必須為" + $("#popCardPW").prop("maxlength") + "碼");
        $("#popCardPW").focus();
        return false;
    }//end card length check
    if (JsFunBASE_VERTIFY() && checkend) {
        $.ajax({
            url: "CardEditV2.aspx",
            type: "post",
            data: $("#ParaExtDiv").find("input,select,textarea").serialize() + "&PsnID=" + $("#PsnID").val(),
            success: function (data) {
                if ($(data).find("#CardErrMsg").val() != "") {
                    alert($(data).find("#CardErrMsg").val());
                } else {
                    //重設人員基本資料的編輯畫面
                    CallEdit($("#PsnID").val());
                    DoCancel();
                }
            }
        });
        //alert('新增完成');
    }//End Js Check
}