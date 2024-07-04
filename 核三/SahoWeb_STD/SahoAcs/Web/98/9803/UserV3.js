//*** 主共用元件：相關的方法 ***
$(document).ready(function () {
    $('[name*="AddButton"]').click(function () {
        CallAdd($("#msgUserAdd").val());
        return false;
    });
    $('[name*="EditButton"]').click(function () {
        CallEdit($('#msgUserEdit').val(), $("#msgNonSelect").val().replace(/\|/g, '\n'));
        return false;
    });
    $('[name*="DeleteButton"]').click(function () {
        CallDelete($('#msgUserDel').val(), $('#msgDelete').val(), $('#msgNonDelete').val().replace(/\|/g, '\n'));
        return false;
    });
    $('[name*="MenuButton"]').click(function () {
        CallUserMenusAuth($('#msgUserAuth').val(), $("#msgNonSelect").val().replace(/\|/g,'\n'));
        return false;
    });
    $('[name*="RoleButton"]').click(function () {
        CallUserRolesAuth($('#msgUserRole').val(), $("#msgNonSelect").val().replace(/\|/g,'\n'));
        return false;
    });
    $('[name*="MgnaButton"]').click(function () {
        CallUserMgnsAuth($('#msgUserMgn').val(), $("#msgNonSelect").val().replace(/\|/g, '\n'));
        return false;
    });
    $('[name*="QueryButton"]').click(function () {
        $("#SelectValue").val("");
        SetQueryWithCondition();
        return false;
    });
});

//GridView 自動設置位置(動作在 xObj.js 中) - r####################################################################
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, $("#SelectValue").val());
}

//DropDownList 選取 - r####################################################################
function DropDownListSelected(Obj, opMode) {
    if ((Obj != "") && (Obj != null)) {
        if (opMode == "-")
            document.getElementById('ContentPlaceHolder1_' + Obj).selectedIndex = 0;
        else if (opMode == "+")
            document.getElementById('ContentPlaceHolder1_' + Obj).selectedIndex = 1;
    }
}

//CheckBox 選取 - r####################################################################
function CheckBoxSelected(Obj) {
    if ((Obj != "") && (Obj != null)) {
        if (document.getElementById('ContentPlaceHolder1_' + Obj).checked)
            document.getElementById('ContentPlaceHolder1_' + Obj).checked = false;
        else
            document.getElementById('ContentPlaceHolder1_' + Obj).checked = true;
    }
}

//CheckBox 全選或全取消 - r####################################################################
function Checkall(input1, input2) {
    var objForm = document.forms[input1];
    var objLen = objForm.length;

    for (var iCount = 0; iCount < objLen; iCount++) {
        if (objForm.elements[iCount].type == "checkbox")
            objForm.elements[iCount].checked = false;
    }
}



//******* 主作業畫面：使用者資料作業相關的方法 *******


function SetQuery()
{
    $.ajax({
        type: "POST",
        url: location.href,
        data: {},
        success: function (data) {
            $("#MainDataArea").html($(data).find("#MainDataArea").html());
            //$("#SelectValue").val("");
            GridSelect();
            SetCheckMax();
        }
    });
}

function SetSort(sort_name) {
    $("#SortName").val(sort_name);
    if ($("#SortType").val() == "ASC") {
        $("#SortType").val("DESC");
    } else {
        $("#SortType").val("ASC");
    }
    SetQueryWithCondition();
}

//主作業畫面：呼叫使用者資料新增視窗
function CallAdd(title) {
    //ChangeText(L_popName1, title);
    SetMode('Add');
    ShowOver("1");
    $('[name*="UserSTime"]').val(GetNowDate());
    $('[name*="UserETime"]').val("2099/12/31 23:59:59");
}

//主作業畫面： 呼叫使用者資料編輯視窗
function CallEdit(title, Msg) {
    if ($('[name*="EditButton"]').prop("disabled")) {
        alert("You haven't authority of Edit !!")
        return false;
    }
    if (IsEmpty($("#SelectValue").val()))
        NotSelectForEdit(Msg);
    else {
        $.ajax({
            type: "POST",
            url: location.href,
            data: { 'DoAction': 'Edit', 'UserID': $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    ShowOver("1");
                    SetMode('Edit');
                    SetUI(data.resp)
                    $('[id="DeleteLableText"]').text('');
                } else {
                    alert(data.message);
                }
            }
        });
        
    }
}

//主作業畫面： 呼叫使用者資料刪除視窗
function CallDelete(title, Msg,Msg2) {    
    if (IsEmpty($("#SelectValue").val()))
        NotSelectForDelete(Msg2);
    else {
        $.ajax({
            type: "POST",
            url: location.href,
            data: { 'DoAction': 'Edit', 'UserID': $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    SetMode('Delete');
                    ShowOver("1");
                    SetUI(data.resp);
                    $('[id="DeleteLableText"]').text(Msg);
                } else {
                    alert(data.message);
                }
            }
        });
    }
}

//主作業畫面：呼叫使用者功能清單權限設定視窗
function CallUserMenusAuth(title, Msg) {
    //ChangeText(L_popName2, title);
    if (IsEmpty($("#SelectValue").val()))
        NotSelectForEdit(Msg);
    else {
        //ShowOver("3");
        $.ajax({
            type: "POST",
            url: location.href,
            data: { 'DoAction': 'UserMenu', 'UserID': $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                ShowOver("3");
                console.log(data);
                $('[name*="Chk_"]').prop("checked", false);
                $('[name*="UserMenusOPMode_"]').val('-');
                var arr = Array.prototype.slice.call(data.auths);
                arr.forEach(function (ele) {
                    console.log($("#UserMenusOPMode_" + String(ele.MenuNo)).val());
                    $("#UserMenusOPMode_" + String(ele.MenuNo)).val(ele.OpMode);
                    var auths = ele.FunAuthSet.split(',');
                    for (var i = 0; i < auths.length; i++) {
                        $("#Chk_" + String(ele.MenuNo) + "_" + String(auths[i])).prop("checked", true);
                    }
                });
            }
        });
    }
}

//主作業畫面：呼叫使用者角色清單權限設定視窗
function CallUserRolesAuth(title, Msg) {
    //ChangeText(L_popName3, title);
    if (IsEmpty($("#SelectValue").val()))
        NotSelectForEdit(Msg);
    else {
        $.ajax({
            type: "POST",
            url: location.href,
            data: { 'DoAction': 'UserRole', 'SelectValue': $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                ShowOver('2');
                $('[name*="UserRolesAuth"]').prop("checked", false);
                var arr = Array.prototype.slice.call(data.roles);
                arr.forEach(function (ele) {
                    $('[name*="UserRolesAuth"][value="' + ele.RoleID + '"]').prop('checked', true);
                });
            }
        });
    }
}

//主作業畫面：呼叫使用者管理區清單權限設定視窗
function CallUserMgnsAuth(title, Msg) {
    //ChangeText(L_popName4, title);
    if (IsEmpty($("#SelectValue").val()))
        NotSelectForEdit(Msg);
    else {
        $.ajax({
            type: "POST",
            url: location.href,
            data: { 'DoAction': 'UserMgns', 'SelectValue': $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                console.log(data);
                ShowOver('4');
                $('[name*="UserMgaAuth"]').prop("checked", false);
                var arr = Array.prototype.slice.call(data.mgns);
                arr.forEach(function (ele) {
                    $('[name*="UserMgaAuth"][value="' + ele.MgaID + '"]').prop('checked', true);
                });
            }
        });
    }
}

//******* 次作業畫面一：使用者資料新增與編輯視窗相關的方法 *******

//次作業畫面一：執行新增動作
function AddExcute() {
    //var regExp = /^((?=.{12,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*|(?=.{12,}$)(?=.*\d)(?=.*[a-zA-Z])(?=.*[!\u0022#$%&'()*+,./:;<=>?@[\]\^_`{|}~-]).*)/;
    var regExp = new RegExp("^(?=.{12,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).*$", "g");
    //var re2 = new RegExp("^(?=.{12,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).*$", "g");
    //var re3 = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
    //var re4 = new RegExp("^(?=.{12,})(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$", "g");
    var re = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])", "g");
    var re2 = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).*$", "g");
    var re3 = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
    var re4 = new RegExp("^(?=.{12,})(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$", "g");
    var str = $("#UserPW").val();
    if (!(re.test(str) || re2.test(str) || re3.test(str) || re4.test(str))) {
        alert($("#msgRegRule").val());
        return false;
    }
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: location.href,
            data: $('#ParaExtDiv1').find("input,select").serialize() + "&DoAction=Insert",
            dataType: "json",
            success: function (data) {
                if (data.resp.result) {
                    $("#SelectValue").val(data.resp.message);
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

//次作業畫面一：執行編輯動作
function EditExcute() {
    var re = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])", "g");
    var re2 = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[A-Z])(?=.*\\W).*$", "g");
    var re3 = new RegExp("^(?=.{12,})(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
    var re4 = new RegExp("^(?=.{12,})(?=.*[A-Z])(?=.*[0-9])(?=.*\\W).*$", "g");
    var str = $("#UserPW").val();
    if (!(re.test(str) || re2.test(str) || re3.test(str) || re4.test(str))) {
        alert($("#msgRegRule").val());
        return false;
    }
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: location.href,
            data: $('#ParaExtDiv1').find("input,select").serialize() + "&DoAction=Update&SelectValue=" + $("#SelectValue").val(),
            dataType: "json",
            success: function (data) {
                if (data.resp.result) {
                    $("#SelectValue").val(data.resp.message);
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

function ResetExcute()
{
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: location.href,
            data: $('#ParaExtDiv1').find("input,select").serialize() + "&DoAction=Reset&SelectValue=" + $("#SelectValue").val(),
            dataType: "json",
            success: function (data) {
                if (data.resp.result) {
                    $("#SelectValue").val(data.resp.message);
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

//次作業畫面一：執行刪除動作
function DeleteExcute() {
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: location.href,
            data: "DoAction=Delete&SelectValue=" + $("#SelectValue").val(),
            dataType: "json",
            success: function (data) {
                if (data.resp.result) {
                    $("#SelectValue").val(data.resp.message);
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

//次作業畫面一：將資料帶回畫面UI ####################################################################
function SetUI(resp) {
    $("#UserID").val(resp.UserID);
    $("#UserName").val(resp.UserName);
    $("#UserEName").val(resp.UserEName);
    $("#UserPW").val(resp.UserPW);
    $("#IsOptAllow").val(resp.IsOptAllow);
    //var dateS = new Date(parseInt(resp.UserSTime.replace("/Date(", "").replace(")/", ""), 10));
    //console.log(dateS.format('yyyy/MM/dd hh:mm:ss'));
    $('[name*="UserSTime"]').val(convertTime(resp.UserSTime, 'yyyy/MM/dd hh:mm:ss'));
    $('[name*="UserETime"]').val(convertTime(resp.UserETime, 'yyyy/MM/dd hh:mm:ss'));

}

//次作業畫面一：依照模式設定各按鈕的啟用狀態 - ####################################################################
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            $("#UserID").val("");
            $("#UserName").val("");
            $("#UserEName").val("");
            $("#UserPW").val("");
            $("#IsOptAllow").val("1");
            //Display
            $('#popB_Add').css('display', 'inline');
            $('#popB_Edit').css('display', 'none');
            $('#popB_Reset').css('display', 'none');
            $('#popB_Delete').css('display', 'none');
            //LabelText
            //ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            //Disabled
            
            //Display
            $('#popB_Add').css('display', 'none');
            $('#popB_Edit').css('display', 'inline');
            $('#popB_Reset').css('display', 'inline');
            $('#popB_Delete').css('display', 'none');
            //LabelText
            ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            //Disabled
            
            //Display
            $('#popB_Add').css('display', 'none');
            $('#popB_Edit').css('display', 'none');
            $('#popB_Delete').css('display', 'inline');
            break;
        case '':
            //Disabled
            
            //Display
            popB_Add.style.display = "none";
            popB_Edit.style.display = "none";
            break;
    }
}

function SetQueryWithCondition() {
    $.ajax({
        type: "POST",
        url: location.href,
        data: { "Input_ID": $("#Input_ID").val(), "Input_Name": $("#Input_Name").val(), "DoAction": "Query", 'SortName': $("#SortName").val(), 'SortType': $("#SortType").val() },
        success: function (data) {
            $("#MainDataArea").html($(data).find("#MainDataArea").html());
            GridSelect();
        }
    });
}




//次作業畫面二：儲存使用者功能清單權限設定視窗相關的資料內容
function UserMenusAuthSaveExcute() {
    console.log($("#ContentPlaceHolder1_popUserMenusAuthPanel").find("input,select").serialize() + "&UserID=" + $("#SelectValue").val());
    $.ajax({
        type: "POST",
        url: location.href,
        data: $("#ContentPlaceHolder1_popUserMenusAuthPanel").find("input,select").serialize() + "&DoAction=SaveMenu&UserID=" + $("#SelectValue").val(),
        dataType: "json",
        success: function (data) {
            DoCancel('3');
        }
    });
    
}


//次作業畫面三：儲存使用者角色清單權限設定視窗相關的資料內容
function UserRolesAuthSaveExcute() {
    console.log($('[name*="UserRolesAuth"]').serialize());
    $.ajax({
        type: "POST",
        url: location.href,
        data: $('[name*="UserRolesAuth"]').serialize() + "&DoAction=SaveRole&UserID=" + $("#SelectValue").val(),
        dataType: "json",
        success: function (data) {
            DoCancel('2');
        }
    });
}


//次作業畫面四：儲存使用者管理區清單權限設定視窗相關的資料內容
function UserMgnsAuthSaveExcute() {
    console.log($('[name*="UserMgaAuth"]').serialize());
    $.ajax({
        type: "POST",
        url: location.href,
        data: $('[name*="UserMgaAuth"]').serialize() + "&DoAction=SaveMgns&UserID=" + $("#SelectValue").val(),
        dataType: "json",
        success: function (data) {
            DoCancel('4');
        }
    });
}



function SetCheckMax() {
    if (parseInt($('input[name*="CurrentUser"]:eq(0)').val()) >= parseInt($('input[name*="MaxUser"]:eq(0)').val())) {
        $('[name*="AddButton"]').prop('disabled', true);
    } else {
        $('[name*="AddButton"]').prop('disabled', false);
    }
}



//設定使用者按鈕的權限
function SetUserLevel(str) {
    var data = str.split(",");
    $('[name*="AddButton"]').prop('disabled', true);
    $('[name*="EditButton"]').prop('disabled', true);
    $('[name*="DeleteButton"]').prop('disabled', true);
    $('[name*="MenuButton"]').prop('disabled', true);
    $('[name*="RoleButton"]').prop('disabled', true);
    $('[name*="MgnaButton"]').prop('disabled', true);
    for (var i = 0; i < data.length; i++) {
        if (data[i] == 'Add') {
            $('[name*="AddButton"]').prop('disabled', false);
            SetCheckMax();
        }
        if (data[i] == 'Edit') {
            $('[name*="EditButton"]').prop('disabled', false);
        }
        if (data[i] == 'Del') {
            $('[name*="DeleteButton"]').prop('disabled', false);
        }
        if (data[i] == "Auth") {
            $('[name*="MenuButton"]').prop('disabled', false);
            $('[name*="RoleButton"]').prop('disabled', false);
            $('[name*="MgnaButton"]').prop('disabled', false);
        }
    }
}//完成權限判斷    



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

function convertTime(jsonTime, format) {
    var date = new Date(parseInt(jsonTime.replace("/Date(", "").replace(")/", ""), 10));
    var formatDate = date.format(format);
    return formatDate;
}


Date.prototype.format = function (format) {
    var date = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours(),
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S+": this.getMilliseconds()
    };

    if (/(y+)/i.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }

    for (var k in date) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? date[k] : ("00" + date[k]).substr(("" + date[k]).length));
        }
    }

    return format;
}


function GetNowDate() {
    var nt = new Date();
    var sYear = nt.getFullYear();
    var sMonth = nt.getMonth() + 1;
    var sDay = nt.getDate();
    var sHour = nt.getHours();
    var sMin = nt.getMinutes();
    var sSec = nt.getSeconds();

    if (sYear < 1911) sYear = 1911;
    if (sMonth < 10) sMonth = '0' + sMonth;
    if (sDay < 10) sDay = '0' + sDay;
    if (sHour < 10) sHour = '0' + sHour;
    if (sMin < 10) sMin = '0' + sMin;
    if (sSec < 10) sSec = '0' + sSec;

    var strNowTime = sYear + '/' + sMonth + '/' + sDay + ' ' + sHour + ':' + sMin + ':' + sSec;

    return strNowTime;
}