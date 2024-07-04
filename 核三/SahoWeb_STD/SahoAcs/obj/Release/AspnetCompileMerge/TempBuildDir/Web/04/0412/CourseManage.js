$(document).ready(function () {
    BindEvent();
    if($("#PsnID").val()!="")
    {
        $(".ShowPsnInfo").css("display", "none");
    }
});


function PopURL(obj) {
   
}


function SetAdd() {
    $.post("CourseEdit.aspx",{"PageEvent": "Add" },
    function (data) {
        OverlayContent(data);
        /*
        $("#BtnCancel,#BtnSave").click(function () {
            DoCancel();
        });
        */
        $("#BtnCancel").click(function () {
            DoCancel();
        });
        $("#BtnSave").click(function () {
            SaveData();
        });
    });
}


function SetEdit(courseid) {
    $.post("CourseEdit.aspx", { "PageEvent": "Edit","CourseID":courseid },
    function (data) {
        OverlayContent(data);
        $("#City").val($("#CityName").val());
        $("#EquID").val($("#EquName").val());
        /*
        $("#BtnCancel,#BtnSave").click(function () {
            DoCancel();
        });
        */
        $("#BtnCancel").click(function () {
            DoCancel();
        });
        $("#BtnSave").click(function () {
            SaveData();
        });
    });
}


function SetMode(mode) {
    $("#QueryMode").val(mode);

    //進行查詢處理
    if (mode == 1) {
        $("#CardTimeE").val($('[name*="$Calendar_CardTimeEDate"]').val());
        $("#CardTimeS").val($('[name*="$Calendar_CardTimeSDate"]').val());
        $("#DepID").val($('[name*="$ddlDept"]').val());
    } else {
        
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


function SetQuery() {    
    $("#PageEvent").val("Query");
    Block();
    $.ajax({
        type: "POST",
        url: "CourseManage.aspx",
        //dataType: 'json',
        data: $("#form1").find("input,select").serialize(),
        success: function (data) {
            //alert(data);
            $("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());
            BindEvent();
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




function OverlayContent(data) {
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
}

function DoCancel() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
}

function SaveData()
{
    console.log($("#CourseEdit").find("input,select").serialize());
    if (JsFunBASE_VERTIFY()) {
        //alert("儲存完成!!");
        $.ajax({
            type: "POST",
            url: "CourseEdit.aspx",
            //dataType: 'json',
            data: $("#CourseEdit").find("input,select").serialize(),
            success: function (data) {
                console.log(data);
                SetQuery();
            }
        });
        DoCancel();
    }
}


