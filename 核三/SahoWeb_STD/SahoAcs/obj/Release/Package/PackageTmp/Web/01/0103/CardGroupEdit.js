
function SetCardGroupEdit() {
    if ($('[name*="CardInfo"] option:selected').val() !== undefined) 
    {
        cardid = $('[name*="CardInfo"] option:selected').val();
        var d = new Date();
        var n = d.getHours() + d.getMinutes() + d.getSeconds();
        CheckGlobalSessionStatus().then(function (answer) {
            if (answer) {
                $.post("CardGroupEdit.aspx",
                    { DoAction: "Query", card_id: cardid, random: n },
                    function (data) {
                        $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                            + ' -webkit-transform: translate3d(0,0,0);"></div>');
                        $("#popOverlay").css("background", "#000");
                        $("#popOverlay").css("opacity", "0.5");
                        $("#popOverlay").width($(document).width());
                        $("#popOverlay").height($(document).height());
                        $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                        $("#ParaExtDiv").html(data);
                        $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                        $(document).scrollTop(0);
                        $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                        $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
                            var equ_id = $(this).find('input[name*="EquID"]').val();
                            var ext_obj = $(this).find('input[name*="CardExtData"]:eq(0)');
                            var equ_name = $(this).find('input[name*="EquName"]').val();
                            var hid = $(this).find('input[name="Mode_hid"]:eq(0)').val();
                            var that = $(this);
                            //設定讀卡規則欄位
                            //if ($(this).find('input[name="CardRuleID"]:eq(0)').val() != "" && $(this).find('input[name="CardRuleID"]:eq(0)').val()!="0") {
                            //    $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
                            //}
                            if ($(this).find('select[name*="CardRule_"]:eq(0) option[value="' + $(this).find('input[name="CardRuleID"]:eq(0)').val() + '"]').length > 0) {
                                $(this).find('select[name*="CardRule_"]:eq(0)').val($(this).find('input[name="CardRuleID"]:eq(0)').val());
                            }
                            if ($(this).find('input[name="VerifiMode"]').length > 0) {
                                $(this).find('select[name*="VerifiMode_"]:eq(0)').val($(this).find('input[name="VerifiMode"]:eq(0)').val());
                            }
                            $(this).find('select[name*="CardRule_"]:eq(0)').change(function () {
                                $(that).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                            });
                            if ($(this).find("td:eq(2)").find('input[name*="HasAuth"]').val() == "V") {
                                $(this).find("td").css("background-color", "NAVY").css("color", "#fff");
                            }
                            if ($(this).find('input[name*="EquClass"]').val() != "Elevator") {
                                $(this).find('.IconGroupSet').prop("disabled", true);
                            }
                            if (hid == "-") {
                                $(this).find('input[name*="OpMode_"][value="0"]').prop("checked", true);
                            } else if (hid == "+" || hid == "*") {
                                $(this).find('input[name*="OpMode_"][value="1"]').prop("checked", true);
                            } else {
                                $(this).find('input[name*="OpMode_"][value="2"]').prop("checked", true);
                            }
                            $(this).find('input[name*="OpMode_"]').click(function () {
                                //$('input[name*="ChkEquID"]:eq(' + index + ')').prop("checked", true);
                            });

                            $(this).find('.IconGroupSet').first().click(function () {
                                OpenFloor(this, equ_id, equ_name, index);
                            });
                        });

                        if (CodeStatus == 1) {
                            $("#BtnCodeSetting").prop("disabled", true);
                        }

                    }//end function
                );//end ajax post
            }
        });
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}


function GroupSetting(action) {
    if (action == "add") {
        $("#InCardEquGroup").append($("#OutCardEquGroup option:selected"));
        $("#OutCardEquGroup option:selected").remove();
    } else {
        $("#OutCardEquGroup").append($("#InCardEquGroup option:selected"));
        $("#InCardEquGroup option:selected").remove();
    }
    return false;
}

function SelectAll(obj)
{
    var check = $(obj).prop("checked");    
    $('input[name*="ChkEquID"]').each(function () {
        $(this).prop("checked", check);
    });
}


function SetVerifiMode(action) {
    $("#DoAction").val("Setting");
    var form_data = $("#form_adj").serialize();    
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: "CardGroupEdit.aspx",
                dataType: "json",
                data: form_data,
                success: function (data) {
                    alert($("#ResultMsg").val());
                    CodeStatus = 1;
                    $("#BtnCodeSetting").prop("disabled", true);
                },
                fail: function () {
                    alert("failed");
                }
            });
        }
    });

}


function SaveSetting(action) {    
    $("#InCardEquGroup option").each(function () {
        $(this).prop("selected", true);
    });
    $("#OutCardEquGroup option").each(function () {
        $(this).prop("selected", true);
    });
    var equall = "";
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {
        var hid = $(this).find('input[name*="OpMode_"]:checked').val();
        if (hid != "2") {
            equall+=$(this).find('input[name="EquID"]').val() + ",";
        }
    });
    equall = equall.slice(0, -1);
    console.log(equall);
    $("#EquIDAll").val(equall);
    $("#DoAction").val("Save");
    var form_data = $("#form_adj").serialize();
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.ajax({
                type: "POST",
                url: "CardGroupEdit.aspx",
                dataType: "json",
                data: form_data,
                success: function (data) {
                    alert($("#ResultMsg").val());
                    DoCancel();
                    SetCardGroupEdit();
                },
                fail: function () {
                    alert('failed');
                }
            });
        }
    });
        /*
        .success(function (data) {
        alert($("#ResultMsg").val());
        DoCancel();
        SetCardGroupEdit();
    }).fail(function () {
        alert("failed");
    });
    */
}

//開啟電梯控制UI
function OpenFloor(obj, val, val3, val4) {
    //console.log($('input[name*="CardExtData"]:eq(' + val4 + ')').val());
    $.post("EquFloorSetting.aspx", {
        equ_id: val,        
        card_id: $('#CardID').val(),
        card_ext_data:$('input[name*="CardExtData"]:eq(' + val4 + ')').val(),
        equ_name: val3, equ_index: val4
    }, function (data) {
        /*
        $('<div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000'
           + ';background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;" ></div>').appendTo("#EquArea");
        $("#dvContent").html(data);
        */

        $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:30000; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
        $("#popOverlay2").css("background", "#000");
        $("#popOverlay2").css("opacity", "0.5");
        $("#popOverlay2").width($(document).width());
        $("#popOverlay2").height($(document).height());
        $(document.body).append('<div id="ParaExtDiv2" style="position:absolute;z-index:30001;background-color:#1275BC;'
            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
        $("#ParaExtDiv2").html(data);
        $("#ParaExtDiv2").css("left", ($(document).width() - $("#ParaExtDiv2").width()) / 2);
        $(document).scrollTop(0);
        $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);

    });
}

//開啟設消碼狀態
function OpenStatus() {
    //console.log($('input[name*="CardExtData"]:eq(' + val4 + ')').val());
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.post("CardAuthStatus.aspx", {
                "CardID": $('select[name*="List_CardInfo"]:eq(0)').val()
            }, function (data) {
                /*
                $('<div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000'
                   + ';background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;" ></div>').appendTo("#EquArea");
                $("#dvContent").html(data);
                */

                $(document.body).append('<div id="popOverlay2" style="position:absolute; top:0; left:0; z-index:30000; overflow:hidden;'
                    + ' -webkit-transform: translate3d(0,0,0);"></div>');
                $("#popOverlay2").css("background", "#000");
                $("#popOverlay2").css("opacity", "0.5");
                $("#popOverlay2").width($(document).width());
                $("#popOverlay2").height($(document).height());
                $(document.body).append('<div id="ParaExtDiv2" style="position:absolute;z-index:30001;background-color:#1275BC;'
                    + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                $("#ParaExtDiv2").html(data);
                $("#ParaExtDiv2").css("left", ($(document).width() - $("#ParaExtDiv2").width()) / 2);
                $(document).scrollTop(0);
                $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);

            });
        }
    });
}

function CloseStatus() {
    $("#popOverlay2").remove();
    $("#ParaExtDiv2").remove();
}


function OpenOnReady() {

}


function ChangeCard2(TypeNo) {
    if ($('[name*="CardInfo"] option:selected') !== undefined) {
        cardid = $('[name*="CardInfo"] option:selected').val();
        var d = new Date();
        var n = d.getHours() + d.getMinutes() + d.getSeconds();
        CheckFunSessionStatus().then(function (answer) {
            if (answer) {
                $.post("CardChange.aspx",
                    { DoAction: "Query", card_id: cardid, random: n },
                    function (data) {
                        $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                            + ' -webkit-transform: translate3d(0,0,0);"></div>');
                        $("#popOverlay").css("background", "#000");
                        $("#popOverlay").css("opacity", "0.5");
                        $("#popOverlay").width($(document).width());
                        $("#popOverlay").height($(document).height());
                        $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                        $("#ParaExtDiv").html(data);
                        $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                        $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                        $("#btnCancel").click(function () {
                            DoCancel();
                        });
                        //$("#NewCardNo").val(ToNarrowValue($("#NewCardNo").val()));
                        $("#NewCardNo").keydown(function (e) {
                            if (e.keyCode == 13) {
                                if (TypeNo == "1") {
                                    SaveCardChange0();
                                } else {
                                    SaveCardChange();
                                }
                            }
                        });
                        $("#btnUpdate").click(function () {
                            if (TypeNo == "1") {
                                SaveCardChange0();
                            } else {
                                SaveCardChange();
                            }

                        });
                        $("#NewCardNo").focus();
                    }
                );
            }
        });
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}

function ChangeCardVer(TypeNo) {
    if ($('[name*="CardInfo"] option:selected') !== undefined) {
        cardid = $('[name*="CardInfo"] option:selected').val();
        var d = new Date();
        var n = d.getHours() + d.getMinutes() + d.getSeconds();
        CheckFunSessionStatus().then(function (answer) {
            if (answer) {
                $.post("CardVerChange.aspx",
                    { DoAction: "Query", card_id: cardid, random: n },
                    function (data) {
                        $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                            + ' -webkit-transform: translate3d(0,0,0);"></div>');
                        $("#popOverlay").css("background", "#000");
                        $("#popOverlay").css("opacity", "0.5");
                        $("#popOverlay").width($(document).width());
                        $("#popOverlay").height($(document).height());
                        $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                            + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                        $("#ParaExtDiv").html(data);
                        $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                        $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                        $("#btnCancel").click(function () {
                            DoCancel();
                        });
                        //$("#NewCardNo").val(ToNarrowValue($("#NewCardNo").val()));
                        $("#NewCardNo").keydown(function (e) {
                            if (e.keyCode == 13) {
                                if (TypeNo == "1") {
                                    SaveCardVerChange0();
                                } else {
                                    SaveCardVerChange();
                                }
                            }
                        });
                        $("#btnUpdate").click(function () {
                            //if (TypeNo == "1") {
                            //    SaveCardVerChange0();
                            //} else {
                            //    SaveCardVerChange();
                            //}
                            SaveCardVerChange();

                        });
                        $("#NewCardNo").focus();
                    }
                );
            }
        });
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}

function UploadPicSource() {

    var d = new Date();
    var n = d.getHours() + d.getMinutes() + d.getSeconds();
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            $.post("UploadPic.aspx",
                { DoAction: "Query", "PsnID": $('[name*="SelectValue"]').val(), random: n },
                function (data) {
                    $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                        + ' -webkit-transform: translate3d(0,0,0);"></div>');
                    $("#popOverlay").css("background", "#000");
                    $("#popOverlay").css("opacity", "0.5");
                    $("#popOverlay").width($(document).width());
                    $("#popOverlay").height($(document).height());
                    $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                        + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
                    $("#ParaExtDiv").html(data);
                    $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                    $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                    $("#btnCancel").click(function () {
                        DoCancel();
                    });
                    $("#btnUpload").click(function () {
                        var formData = new FormData();
                        formData.append('file', $('#file')[0].files[0]);
                        formData.append('PageEvent', 'Save');
                        formData.append("PsnID", $("#PsnID").val());
                        formData.append("PsnIDNum", $("#PsnIDNum").val());
                        $.ajax({
                            url: 'UploadPic.aspx',
                            type: 'POST',
                            cache: false,
                            data: formData,
                            processData: false,
                            contentType: false
                        }).done(function (res) {
                            DoCancel();
                        }).fail(function (res) { });
                    });
                }
            );
        }
    });
}

function CheckData(e) {
   
}

function SaveCardChange() {
    var checkend = true;
    if ($("#NewCardNo").val().length != parseInt($("#CardLength").val())) {
        alert("卡號長度必須為" + $("#CardLength").val() + "碼");
        checkend = false;
    } else {
        if ($("#NewCardNo").val() == parseInt($("#CardNo").val())) {
            alert("新舊卡號必須為不同卡號!!");
            checkend = false;
        }
        var re = /^[A-F0-9]+$/;
        if (re.test($("#NewCardNo").val()) == false) {
            alert("卡號須為16進制數字");
            checkend = false;
        }
        if ($("#NewCardNo").val() <= "0000010000") {
            alert("新卡號得大於「0000010000」!! ");
            checkend = false;
        }
    }
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            checkend = false;
        }
    });
    if (checkend) {
        $.ajax({
            type: "POST",
            url: "CardChange.aspx",
            dataType: "json",
            data: { "CardID": $("#CardID").val(), "NewCardNo": $("#NewCardNo").val(), "CardNo": $("#CardNo").val(), "PageEvent": "Save" },
            success: function (data) {
                if (data.ErrorMessage.length > 0) {
                    alert(data.ErrorMessage);
                } else {
                    alert("變更完成");
                    DoCancel();
                    CallEdit(SelectNowNo.value);
                }
            },
            fail: function () {
                alert("failed");
            }
        });
    }
}

function SaveCardVerChange() {
    var checkend = true;
    if ($("#NewCardVer").val().length != 1) {
        alert("新版次必填");
        checkend = false;
    } else {
        if ($("#NewCardVer").val() == parseInt($("#CardVer").val())) {
            alert("新舊版次必須為不同!!");
            checkend = false;
        }
        var re = /^[A-F0-9]+$/;
        if (re.test($("#NewCardVer").val()) == false) {
            alert("卡號須為16進制數字");
            checkend = false;
        }
    }
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            checkend = false;
        }
    });
    if (checkend) {
        $.ajax({
            type: "POST",
            url: "CardVerChange.aspx",
            dataType: "json",
            data: { "CardID": $("#CardID").val(), "NewCardVer": $("#NewCardVer").val(), "CardNo": $("#CardNo").val(), "PageEvent": "Save" },
            success: function (data) {
                if (data.ErrorMessage.length > 0) {
                    alert(data.ErrorMessage);
                } else {
                    alert("變更完成");
                    DoCancel();
                    CallEdit(SelectNowNo.value);
                }
            },
            fail: function () {
                alert("failed");
            }
        });
    }
}

function CreateOverlay() {
    $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
                + ' -webkit-transform: translate3d(0,0,0);"></div>');
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
        + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
}


function CardNoUpdate() {
    if (List_CardInfo.options[List_CardInfo.selectedIndex] !== undefined) {
        cardid = List_CardInfo.options[List_CardInfo.selectedIndex].value;
        var d = new Date();
        var n = d.getHours() + d.getMinutes() + d.getSeconds();
        CheckFunSessionStatus().then(function (answer) {
            if (answer) {
                $.post("CardNoUpdate.aspx",
                    { DoAction: "Query", card_id: cardid, random: n },
                    function (data) {
                        CreateOverlay();
                        $("#ParaExtDiv").html(data);
                        $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                        $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
                        $("#btnCancel").click(function () {
                            DoCancel();
                        });
                        $("#NewCardNo").keydown(function (e) {
                            if (e.keyCode == 13) {
                                SaveCardChange();
                            }
                        });
                        $("#btnUpdate").click(function () {
                            SaveCardChange0();
                        });
                        $("#NewCardNo").focus();
                    }
                );
            }
        });
    } else {
        alert("請選取欲設定卡片資料");
    }
    return false;
}



function SaveCardChange0() {
    var checkend = true;    
    if ($("#NewCardNo").val().length != parseInt($("#CardLength").val())) {
        alert("卡號長度必須為" + $("#CardLength").val() + "碼");
        checkend = false;
    } else {
        if ($("#NewCardNo").val() == parseInt($("#CardNo").val())) {
            alert("新舊卡號必須為不同卡號!!");
            checkend = false;
        }
        var re = /^[A-F0-9]+$/;
        if (re.test($("#NewCardNo").val()) == false) {
            alert("卡號須為16進制數字");
            checkend = false;
        }
    }
    CheckFunSessionStatus().then(function (answer) {
        if (answer) {
            checkend = false;
        }
    });
    if (checkend) {
        $.ajax({
            type: "POST",
            url: "CardChange.aspx",
            dataType: "json",
            data: { "CardID": $("#CardID").val(), "NewCardNo": $("#NewCardNo").val(), "CardNo": $("#CardNo").val(), "PageEvent": "Save" },
            success: function (data) {
                if (data.ErrorMessage.length > 0) {
                    alert(data.ErrorMessage);
                } else {
                    alert("變更完成");
                    DoCancel();
                    if (location.href.indexOf("PersonManage") > 0) {
                        CallEdit($("#SelectValue").val());
                    } else {
                        CallEdit(SelectNowNo.value);
                    }
                    
                }
            },
            fail: function () {
                alert("failed");
            }
        });
    }
}

/*
function SaveFloor() {
    var ext_index=$("#EquIndex").val();
    var floor_bin = "";
    $("#ContentPlaceHolder1_GridViewFloor").find("tr").each(function (index) {
        if($(this).find('input[name*="CheckIO"]').prop("checked")==true){
            floor_bin = "1" + floor_bin;
        } else {
            floor_bin = "0" + floor_bin;
        }        
    });
    for (var s = floor_bin.length; s < 48; s++) {
        floor_bin = "0" + floor_bin;
    }    
    var temp = parseInt(floor_bin, 2); // 先將字串以2進位方式解析為數值
    var result = temp.toString(16); // 將數值轉為16進位
    for (var s = result.length; s < 12; s++) {
        result = "0" + result;
    }
    //console.log(result);
    $('input[name*="CardExtData"]:eq(' + ext_index + ')').val(result);
    $('input[name*="OpMode_"][value="1"]:eq('+ext_index+')').prop("checked", true);
    closeAllCompont();
}
*/




function ToNarrowValue(val) {
    var v = val;
    if (v == 'NaN' || v == null || v == '') {
        return 0;
    } else {
        //全行轉半型
        result = "";
        console.log("啟始" + String.fromCharCode(65249) + "End")
        for (i = 0; i <= v.length; i++) {
            if (v.charCodeAt(i) == 12288) {
                result += " ";
            } else {
                if (v.charCodeAt(i) > 65280 && v.charCodeAt(i) < 65375) {
                    result += String.fromCharCode(v.charCodeAt(i) - 65248);
                } else {
                    result += String.fromCharCode(v.charCodeAt(i));
                }
            }
        }
        val.value = result;
        v = result;
        return v;
    }
}