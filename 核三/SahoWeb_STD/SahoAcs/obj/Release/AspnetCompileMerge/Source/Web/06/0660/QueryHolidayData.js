$(document).ready(function () {
    //setTimeout('SetHighLifthEnabled()', 500); 
    //BindEvent();
    //if ($("#PsnID").val() != "") {
    //    $(".ShowPsnInfo").css("display", "none");
    //}
    $("#PrintButton").click(function () {
        console.log('test');
        SetPrint();
    });
});

//呼叫新增視窗
function CallAdd() {    
    $.ajax({
        type: "POST",
        url: "HolidayEdit.aspx",
        data: {},        
        success: function (data) {            
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
            $("#BtnLeave").click(function () {
                DoCancel();
            });
            $("#popB_Cancel").click(function () {
                DoCancel();
            });
            $("#popB_Add").click(function () {
                SaveExcute();
            });
        }
    });
}




//執行新增動作
function SaveExcute() {
    $("#StartTime").val($('[name*="MainStartTime"]').val());
    $("#EndTime").val($('[name*="MainEndTime"]').val());
    $("#DoAction").val("Save");
    if ($('#StartTime').val() >= $("#EndTime").val()) {
        alert('休假起始時間不得大於訖止時間');
        return false;
    }
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: 'HolidayEdit.aspx',
            data: $('#ParaExtDiv').find("input,select,textarea").serialize(),
            dataType: "json",
            success: function (data) {                
                if (data.result) {
                    $('[name*="QueryTimeE"]').val($("#EndTime").val());
                    DoCancel();
                    SetMode(1);
                } else {
                    alert(data.message);
                }
                console.log(data);
            }
        });
        //console.log($("#ParaExtDiv").find("input,select,textarea").serialize());
    }
}

//呼叫編輯視窗
function CallEdit() {
    let SelectValue = $('#SelectValue').val();
    if (IsEmpty(SelectValue) || SelectValue == "0")
        NotSelectForDelete(NoselectMsg);
    else {
        $.ajax({
            type: "POST",
            url: "HolidayEdit.aspx",
            data: { "DoAction": "Edit", "RecordID": $("#SelectValue").val() },            
            success: function (data) {
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
                $("#BtnLeave").click(function () {
                    DoCancel();
                });
                $("#popB_Cancel").click(function () {
                    DoCancel();
                });
                $("#popB_Add").click(function () {
                    SaveExcute();
                });
                $("#popB_Delete").click(function () {
                    DeleteExcute();
                });
            }
        });
    } // end else
}



function ShowOver() {
    $("#popOverlay" + val_key).css("display", "block");
    $("#ParaExtDiv" + val_key).css("display", "block");
    $("#popOverlay" + val_key).width($(document).width());
    //$("#popOverlay" + val_key).height($("#ParaExtDiv" + val_key).height() + 130);
    $("#ParaExtDiv" + val_key).css("left", ($(document).width() - $("#ParaExtDiv" + val_key).width()) / 2);
    $("#ParaExtDiv" + val_key).css("top", $(document).scrollTop() + 20);
    //$("#ParaExtDiv" + val_key).css("top", 20);
}

function DoCancel(val_key) {
    $("#popOverlay" + val_key).css("display", "none");
    $("#ParaExtDiv" + val_key).css("display", "none");
}



// 依照模式設定各按鈕的啟用狀態
function SetbtnMode(sMode) {
    //$('.IsDel').css('display', 'none');
    switch (sMode) {
        case 'Add':
            $('#TpPsnNo').removeAttr('disabled')
            $('#TPPsnName').removeAttr('disabled')
            $('.IsMgaVacationDate').css('display', 'none')
            $('.IsOrgName').css('display', 'none')
            $('.IsVacationDate').css("display", 'block');
            $('#Remark').removeAttr('disabled')
            $("#popB_Add").css('display', 'inline');
            $("#popB_Edit").css('display', 'none');
            $("#popB_Delete").css('display', 'none');
            break;
        case 'Edit':
            $('#TpPsnNo').attr('disabled', 'disabled')
            $('#TPPsnName').attr('disabled', 'disabled')
            $('.IsMgaVacationDate').css('display', 'none')
            $('.IsOrgName').css('display', 'none')
            $('.IsVacationDate').css("display", 'block');
            $('#Remark').removeAttr('disabled')
            $("#popB_Add").css('display', 'none');
            $("#popB_Edit").css('display', 'inline');
            $("#popB_Delete").css('display', 'none');
            break;
        case 'Delete':
            $('#TpPsnNo').attr('disabled', 'disabled')
            $('#TPPsnName').attr('disabled', 'disabled')
            $('#MgaVacationDate').attr('disabled', 'disabled')
            $('#MgaOrgName').attr('disabled', 'disabled')
            $('#Remark').attr('disabled', 'disabled')
            $('.IsVacationDate').css("display", 'none');
            $("#popB_Edit").css('display', 'none');
            $("#popB_Add").css('display', 'none');
            $("#popB_Delete").css('display', 'inline');
            //popB_Delete.style.display = "inline";
            break;
        case '':

            break;
    }
}



function CancelOneLogArea() {
    $("#OneLogArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}

function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, $("#SelectNowNo").val());
}





function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        //$("#CardDateE").val($('[name*="CalendarE"]').val());
        //$("#CardDateS").val($('[name*="CalendarS"]').val());        
        $('#QryTimeS').val($('[name*="QueryTimeS"]').val());
        $('#QryTimeE').val($('[name*="QueryTimeE"]').val());
    }
    ShowPage(1);
}

function SetSort(sort_name) {
    $("#SortName").val(sort_name);
    if ($("#SortType").val() == "ASC") {
        $("#SortType").val("DESC");
    } else {
        $("#SortType").val("ASC");
    }
    SetQuery();
}

function ShowPage(page_index) {
    $("#PageIndex").val(page_index);
    SetQuery();
}


function SetPrint() {
    $('#QryTimeS').val($('[name*="QueryTimeS"]').val());
    $('#QryTimeE').val($('[name*="QueryTimeE"]').val());
    $("#PageEvent").val("Print");
    $("#form1").attr("target", "_blank");
    $("#form1").submit();
}

function SetChange(obj) {
    var empno = $(obj).next();
    var psnid = $(empno).next();
    $(empno).val($(psnid).val());
}



function CheckMax() {
    if (parseInt($("#CurrentMobile").val()) >= parseInt($("#MaxMobile").val())) {        
        $('[name="EmpNo"]').each(function () {
            var that = $(this);
            if ($(that).val() == "0") {
                console.log($(that).prev().val());
                $(that).prev().prop("disabled", true);
            }
        });
    }
}

function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: location.href,
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            //CheckMax();
            //BindEvent();
            $.unblockUI();
        }
    });
}

function BindEvent() {
    $('.GVStyle').find('th').each(function (index) {
        if (index < $('.GVStyle').find('th').length - 1) {
            $(this).css("width", $(this).find('[name*="TitleCol"]').val());
        }
    });
}


function DoCancel() {
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}

