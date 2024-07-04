function SourceQueryState() {
    __doPostBack(SourceQueryButton.id, '');
}

function TargetQueryState() {
    __doPostBack(TargetQueryButton.id, '');
}

// 顯示EquList
function ShowEquList() {
    __doPostBack(EquListUpdatePanel.id, SelectValue.value);
}

// CheckBox選取
function CheckBoxSelected(Obj) {
    if (document.getElementById(Obj).checked) {
        document.getElementById(Obj).checked = false;
    }
    else {
        document.getElementById(Obj).checked = true;
    }
}

// RadioButton控制
function AuthActSelected(Obj) {
    if (Input_AuthAct1.id == Obj) {
        Input_ConflictAct1.disabled = true;
        Input_ConflictAct2.disabled = true;
        Input_ConflictAct1.checked = false;
        Input_ConflictAct2.checked = false;
    }
    else {
        Input_ConflictAct1.disabled = false;
        Input_ConflictAct2.disabled = false;
    }
}

function ShowLabel(Msg, Count, Type) {
    switch (Type) {
        case "Source":
            lblSource.innerHTML = Msg + Count + "筆)";
            break;
        case "Target":
            lblTarget.innerHTML = Msg + Count + "筆)";
            break;
        case "EquList":
            lblEquList.innerHTML = Msg + Count + "筆)";
            break;
    }
}

function ButtonDisabled(ctrlstatus) {
    if (ctrlstatus == 'true') {
        AuthCopyButton.disabled = true;
        AuthAddButton.disabled = true;
    }
    else {
        AuthCopyButton.disabled = false;
        AuthAddButton.disabled = false;
    }
}

function ShowDetail(CardNo, Msg) {

    if (IsEmpty(CardNo))
    {
        NotSelectForEdit(Msg);
    }
    else
    {
        $.ajax({
            type: "POST",
            url: "CopyCardAuthDetail.aspx",
            data:
                {
                    "CardNo": CardNo
                },
            async: true,
            success: function (data)
            {
                var top = $("#colorbox").css("top");
                var left = $("#colorbox").css("left");
                $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden; -webkit-transform: translate3d(0,0,0);"></div>');
                $("#popOverlay").css("background", "#000");
                $("#popOverlay").css("opacity", "0.5");
                $("#popOverlay").width("100%");
                $("#popOverlay").height($(document).height());
                $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC; border-style:solid; border-width:2px; border-color:#069" ></div>');
                $("#ParaExtDiv").html(data);
                $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
                $("#ParaExtDiv").css("top", $("#popOverlay").scrollTop() + 150);
                $("#btnClose").click(function () {
                    CancelOne();
                });
            }
            }).fail(function ()
            {
                alert("failed");
            });
    }

    //return false;
}


function CancelOne() {
    $("#ParaExtDiv").remove();
    $("#popOverlay").remove();
    //return false;

}