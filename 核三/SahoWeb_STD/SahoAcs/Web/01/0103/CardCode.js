function SetCardCode() {    
    if ($('[name*="CardInfo"] option:selected') !== undefined) {
        cardid = $('[name*="CardInfo"] option:selected').val();
        CheckFunSessionStatus().then(function (answer) {
            if (answer) {
                $.post("EquCodeSetting.aspx",
                    { "card_id": cardid },
                    function (data) {
                        $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                            + ' -webkit-transform: translate3d(0,0,0);"></div>');
                        $("#popOverlay").css("background", "#000");
                        $("#popOverlay").css("opacity", "0.5");
                        $("#popOverlay").width("100%");
                        $("#popOverlay").height($(document).height());
                        $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                        $("#ParaExtDiv").html(data);
                        $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                        $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                        $("#TableCode").find("TR").each(function () {
                            var equ_id = $(this).find("input[name*='EquID']").val();
                            $(this).find(".IconPassword").click(function () {
                                DoEquSetting(equ_id);
                            });
                        });
                        if (CodeStatus == 1) {
                            $("#BtnCodeSetting").prop("disabled", true);
                        }
                    });
            }
        });
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}

//所有機器作設碼
function DoSetting() {
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.post("EquCodeSetting.aspx",
                { "card_id": $("#CardId").val(), "DoAction": "All" },
                function (data) {
                    alert($("#ResultMsg").val());
                    $("#popOverlay").remove();
                    $("#ParaExtDiv").remove();
                    CodeStatus = 1;
                });
        }
    });
}

//單一機器作設碼
function DoEquSetting(equ_id) {
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.post("EquCodeSetting.aspx",
                { "equ_id": equ_id, "card_id": $("#CardId").val(), "DoAction": "Setting" },
                function (data) {
                    alert('單機設碼完成!!');
                });
        }
    });
}


function DoCancel() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


function CheckFunSessionStatus() {
    var defer = $.Deferred();
    $.ajax({
        type: "POST",
        url: "../../../ReCheck.ashx",    //執行驗證的頁面
        data: { "StatusName": "UserID", "PageEvent": "CheckStatus" },
        dataType: "json",
        success: function (data) {
            if (data.isSuccess) {
                defer.resolve(true);
            } else {
                alert(data.message);
                //產生登出視窗
                defer.resolve(false);
            }
            return defer.promise();
        }
    });
    return defer.promise();
}