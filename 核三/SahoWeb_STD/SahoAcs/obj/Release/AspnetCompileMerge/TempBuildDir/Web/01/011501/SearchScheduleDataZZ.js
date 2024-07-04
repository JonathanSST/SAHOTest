$(document).ready(function () {
    setTimeout('SetHighLifthEnabled()', 500); 
    BuildDate()
    BindEvent();
    //if ($("#PsnID").val() != "") {
    //    $(".ShowPsnInfo").css("display", "none");
    //}
});
let NoselectMsg = "請注意!!目前無法進行編輯 || 可能的原因：1.沒有資料可供編輯。 | 2.尚未選擇要編輯的項目。|";

//呼叫新增視窗
function CallAdd() {
    SetbtnMode('Add');
    ShowOver('1');
    $("#TpPsnNo").val("");
    $("#TPPsnName").val("");
    $("#MgaVacationDate").val("");
    $("#Remark").val("");
    $("#SelectValue").val("0");
}

//執行新增動作
function AddExcute() {
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: window.location,
            data: $('#ParaExtDiv1').find("input,select,textarea").serialize() + "&PageEvent=Insert",
            dataType: "json",
            success: function (data) {
                //console.log(data)
                if (data.Message=="OK") {
                    alert(data.resp)
                    $('#popDropDownList_Char2').val('0'); 
                    $("#SelectValue").val(data.SelctValue);
                    DoCancel('1');
                    SetMode(1);
                } else {
                    alert(data.resp);
                    $('#popDropDownList_Char2').val('0'); 
                    DoCancel('1');
                    SetMode(1);
                    console.log(data)
                }
            }
        });
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
            url: "SearchScheduleDataZZ.aspx",
            data: { "PageEvent": "EditChk", "VacationDate": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    // console.log(data)
                    //設定資料到UI欄位
                    $("#TpPsnNo").val(data.resp[0].employeeID);
                    $("#TPPsnName").val(data.resp[0].employeeName);
                    $("#MgaVacationDate").val(data.VacationDate);
                    $("#MgaOrgName").val(data.OrgName);
                    $("#Remark").val(data.resp[0].VacationInfo);
                    SetbtnMode('Edit');
                    ShowOver('1');
                } else {
                    alert(data.message)
                }
            }
        });
    } // end else
}

//執行編輯動作
function EditExcute() {
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: window.location,
            data: $('#ParaExtDiv1').find("input,select,textarea").serialize() + "&PageEvent=Edit&VacationDate=" + $("#SelectValue").val()
                + "&TpPsnNo=" + $('#TpPsnNo').val() + "&TPPsnName=" + $('#TPPsnName').val() + "&OldDate="+$('#MgaVacationDate').val(),
            dataType: "json",
            success: function (data) {
                //console.log(data)
                if (data.Message == "OK") {
                    alert(data.resp)
                    $('#popDropDownList_Char2').val('0');
                    $("#SelectValue").val(data.SelctValue);
                    DoCancel('1');
                    SetMode(1);
                } else {
                    alert(data.resp);
                    $('#popDropDownList_Char2').val('0');
                    DoCancel('1');
                    SetMode(1);
                    console.log(data)
                }
            }
        });
    }
}



//呼叫刪除視窗
function CallDelete() {
    let SelectValue = $('#SelectValue').val();
    if (IsEmpty(SelectValue) || SelectValue == "0")
        NotSelectForDelete(NoselectMsg);
    else {
        $.ajax({
            type: "POST",
            url: "SearchScheduleDataZZ.aspx",
            data: { "PageEvent": "DeleteChk", "VacationDate": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                   // console.log(data)
                    //設定資料到UI欄位
                    $("#TpPsnNo").val(data.resp[0].employeeID);
                    $("#TPPsnName").val(data.resp[0].employeeName);
                    $("#MgaVacationDate").val(data.VacationDate);
                    $("#MgaOrgName").val(data.OrgName);
                    $("#Remark").val(data.resp[0].VacationInfo);
                    SetbtnMode('Delete');
                    ShowOver('1');
                } else {
                    alert(data.message)
                }
            }
        });
    } // end else
} //end function

//執行刪除動作
function DeleteExcute() {
    $.ajax({
        type: "POST",
        url: "SearchScheduleDataZZ.aspx",
        data: { "PageEvent": "Delete", "VacationDate": $("#SelectValue").val() },
        dataType: "json",
        success: function (data) {
            console.log(data)
            if (data.resp.result) {
                $("#SelectValue").val('0');
                DoCancel('1');
                SetMode(1);
            } else {
                alert(data.resp.message)
            }
        }
    });
}



function ShowOver(val_key) {
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

//取得目前日期之後的月份天數
function BuildDate() {
    let today = new Date();
    //目前年份
    let CurYear = today.getFullYear();
    //目前月份
    let CurMonth = today.getMonth() + 1;
    //目前幾號
    let CurDay = today.getDate();
    //目前月份的總天數
    let CurMonthDays = new Date(CurYear, CurMonth, 0).getDate();
    console.log(CurMonthDays);
    console.log(CurDay);
    let CurYearMonth = CurYear + "/" + CurMonth + "/";
    for (di = 1; di <= CurMonthDays; di++) {
        if (CurDay < di) {
            $("#popDropDownList_Char2").append(new Option(CurYearMonth + di, CurYearMonth + di));
        }
    }
}

//設定進階查詢功能
function SetAdvArea() {    
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#AdvanceArea").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#AdvanceArea").css("left", 0);
    $("#AdvanceArea").css("top", 0);
    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
}

function CancelAdvArea() {
    $("#AdvanceArea").css("display", "none");
    $("#popOverlay").css("display", "none");
}

function SetShowCardLogDetail() {
    $("#popOverlay").width($(document).width());
    $("#popOverlay").height($(document).height());
    $("#popOverlay").css("display", "block");
    $("#AdvanceArea").css("display", "block");
    $("#popOverlay").css("background", "#000");
    $("#popOverlay").css("opacity", "0.5");
    $("#AdvanceArea").css("left", 0);
    $("#AdvanceArea").css("top", 0);
    $("#AdvanceArea").css("left", ($(document).width() - $("#AdvanceArea").width()) / 2);
    $("#AdvanceArea").css("top", $(document).scrollTop() + 50);
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
    //$("#CardTimeE").val($('[name*="Calendar_CardTimeEDate"]').val());
    //$("#CardTimeS").val($('[name*="Calendar_CardTimeSDate"]').val());
    //$("#form1").submit();    
    //window.open("0601Rpt.aspx?" + $(".Content").find("input,select").not('[name*="DataCol"]').serialize());
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
        url: "SearchScheduleDataZZ.aspx",
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //DoCancel();
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            CheckMax();
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
    $('.DataRow').each(function (outIndex) {
        var that = $(this);
        $(that).find('td').each(function (index) {
            if (index < $(that).find('td').length - 1) {
                $(this).css("width", $(this).find('[name*="DataCol"]').val());
            }
        });
    });
    $(".GVStyle").sortable({
        opacity: 0.6,    //拖曳時透明
        cursor: 'pointer',  //游標設定
        axis: 'x,y',       //只能垂直拖曳
        update: function () {            
            SetQuery();
        }
    });


}

