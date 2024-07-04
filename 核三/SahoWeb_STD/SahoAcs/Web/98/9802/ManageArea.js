$(document).ready(function () {
    $('[name*="AddButton"]').click(function () {
        CallAdd($("#msgUserAdd").val());
        //return false;
    });
    $('[name*="EditButton"]').click(function () {
        CallEdit($("#AreaEdit").val(), $("#msgNonSelect").val().replace(/\|/g, '\n'));
        //return false;
    });
    $('[name*="DeleteButton"]').click(function () {
        CallDelete('刪除管理區', $('#msgDelete').val(), $('#msgNonDelete').val().replace(/\|/g, '\n'));
        //return false;
    });
    $('#AuthButton').click(function () {
        CallAuth($("#msgNonSelect").val().replace(/\|/g, '\n'));
        //return false;
    });
    $('[name*="AuthButton2"]').click(function () {
        CallAuth2($("#msgNonSelect").val().replace(/\|/g, '\n'));
        //return false;
    });
    $('[name*="QueryButton"]').click(function () {
        $("#SelectValue").val("");
        SetQueryWithCondition();
        return false;
    });
});




// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    $('.IsDel').css('display', 'none');
    switch (sMode) {
        case 'Add':
            $('#MgaNo').removeAttr('disabled')
            $('#MgaName').removeAttr('disabled')
            $('#MgaEName').removeAttr('disabled')
            $('#MgaEmail').removeAttr('disabled')
            $('#MgaDesc').removeAttr('disabled')
            $('#Remark').removeAttr('disabled')
            $("#popB_Add").css('display', 'inline');
            $("#popB_Edit").css('display', 'none');
            $("#popB_Delete").css('display', 'none');
            break;
        case 'Edit':
            $('#MgaNo').removeAttr('disabled')
            $('#MgaName').removeAttr('disabled')
            $('#MgaEName').removeAttr('disabled')
            $('#MgaEmail').removeAttr('disabled')
            $('#MgaDesc').removeAttr('disabled')
            $('#Remark').removeAttr('disabled')
            $("#popB_Add").css('display', 'none');
            $("#popB_Edit").css('display', 'inline');
            $("#popB_Delete").css('display', 'none');
            break;
        case 'Delete':
            $('#MgaNo').attr('disabled', 'disabled')
            $('#MgaName').attr('disabled', 'disabled')
            $('#MgaEName').attr('disabled', 'disabled')
            $('#MgaEmail').attr('disabled', 'disabled')
            $('#MgaDesc').attr('disabled', 'disabled')
            $('#Remark').attr('disabled', 'disabled')
            $('.IsDel').css("display", 'block');
            $("#popB_Edit").css('display', 'none');
            $("#popB_Add").css('display', 'none');
            $("#popB_Delete").css('display', 'inline');
            //popB_Delete.style.display = "inline";
            break;
        case 'Auth':
            
            ChangeText(DeleteLableText3, '');
            break;
        case 'Auth2':
            
            ChangeText(DeleteLableText4, '');
            break;
        case '':
            
            break;
    }
}


function SetUserLevel(str) {
    $("#AddButton").prop('disabled', true);
    $("#EditButton").prop('disabled', true);
    $("#DeleteButton").prop('disabled', true);
    $("#AuthButton").prop('disabled', true);
    $("#AuthButton2").prop('disabled', true);
    if (str != '') {
        var data = str.split(",");
        for (var i = 0; i < data.length; i++) {
            if(data[i] == 'Add')
                $("#AddButton").prop('disabled', false);
            if(data[i] == 'Edit')
                $("#EditButton").prop('disabled', false);
            if(data[i] == 'Del')
                $("#DeleteButton").prop('disabled', false);
            if (data[i] == 'Auth') {
                $("#AuthButton").prop('disabled', false);
                $("#AuthButton2").prop('disabled', false);
            }
        }
    }
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, $("#SelectNowNo").val());
}

// 呼叫新增視窗
function CallAdd() {
    $('[id*="DeleteLableText"]').text("");
    $('[id*="L_popName1"]').text($("#AreaAdd").val());
    SetMode('Add');
    ShowOver('1');
    //$('#ParaExtDiv1').find("input,select").val("");
    $("#MgaNo").val("");
    $("#MgaName").val("");
    $("#MgaEName").val("");
    $("#MgaEmail").val("");
    $("#MgaDesc").val("");
    $("#Remark").val("");
    $("#SelectValue").val("0");
}

function SetQueryWithCondition() {
    $.ajax({
        type: "POST",
        url: "ManageArea.aspx",
        data: { MgaName: $("#Input_Name").val(), MgaNo: $("#Input_No").val() },
        success: function (data) {
            $("#MainDataArea").html($(data).find("#MainDataArea").html());
            GridSelect();
            //SetCheckMax();
        }
    });
}

function SetQuery() {
    $.ajax({
        type: "POST",
        url: "ManageArea.aspx",
        data: {},
        success: function (data) {
            $("#MainDataArea").html($(data).find("#MainDataArea").html());
            GridSelect();
            //SetCheckMax();
        }
    });
}


//執行新增動作
function AddExcute(UserID) {
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: window.location,
            data: $('#ParaExtDiv1').find("input,select,textarea").serialize() + "&DoAction=Insert",
            dataType: "json",
            success: function (data) {
                console.log(data)
                if (data.resp.result) {
                    $("#SelectValue").val(data.resp.message);
                    $("#SelectNowNo").val($("#MgaNo").val());
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

// 呼叫編輯視窗
function CallEdit(Msg1, Msg) {    
    //ChangeText(, $("#AreaEdit").val());
    $('[id*="L_popName1"]').text(Msg1);
    $('[id*="DeleteLableText"]').text("");
    if (IsEmpty($("#SelectValue").val()) || $("#SelectValue").val() == "0")
        NotSelectForEdit(Msg);
    else {        
        $.ajax({
            
            type: "POST",
            url: "ManageArea.aspx",
            data: { "DoAction": "Edit", "MgaID": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                   // console.log(data.resp)
                    $("#SelectValue").val(data.resp.MgaID);
                    //設定資料到UI欄位
                    $("#MgaNo").val(data.resp.MgaNo);
                    $("#MgaName").val(data.resp.MgaName);
                    $("#MgaEName").val(data.resp.MgaEName);
                    $("#MgaDesc").val(data.resp.MgaDesc);
                    $("#Remark").val(data.resp.Remark);
                    $("#MgaEmail").val(data.resp.Email);
                    SetMode('Edit');
                    ShowOver('1');
                } else {
                    alert(data.message)
                }
            }
        });
    }
}

//執行更新動作
function EditExcute(UserID) {
    if (JsFunBASE_VERTIFY()) {
        $.ajax({
            type: "POST",
            url: window.location,
            data: $('#ParaExtDiv1').find("input,select,textarea").serialize() + "&DoAction=Update&MgaID="+$("#SelectValue").val(),
            dataType: "json",
            success: function (data) {
                if (data.resp.result) {
                    //$("#SelectValue").val(data.resp.message);
                    $("#SelectNowNo").val($("#MgaNo").val());
                    DoCancel('1');
                    SetQuery();
                } else {
                    alert(data.resp.message)
                }
            }
        });
    }
}

// 呼叫刪除視窗
function CallDelete(Msg,Msg2,Msg3) {
    $('[id*="L_popName1"]').text($("#AreaDel").val());
    $('[id*="DeleteLableText"]').text(Msg2);
    if (IsEmpty($("#SelectValue").val()) || $("#SelectValue").val() == "0")
        NotSelectForDelete(Msg3);
    else {        
        $.ajax({
            type: "POST",
            url: "ManageArea.aspx",
            data: { "DoAction": "Edit", "MgaID": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                if (data.success) {
                    $("#SelectValue").val(data.resp.MgaID);
                    //設定資料到UI欄位
                    $("#MgaNo").val(data.resp.MgaNo);
                    $("#MgaName").val(data.resp.MgaName);
                    $("#MgaEName").val(data.resp.MgaEName);
                    $("#MgaDesc").val(data.resp.MgaDesc);
                    $("#Remark").val(data.resp.Remark);
                    $("#MgaEmail").val(data.resp.Email);
                    SetMode('Delete');                    
                    ShowOver('1');
                } else {
                    alert(data.message)
                }
            }
        });
    }
}

//執行刪除動作
function DeleteExcute() {
    $.ajax({
        type: "POST",
        url: "ManageArea.aspx",
        data: { "DoAction": "Delete", "MgaID": $("#SelectValue").val() },
        dataType: "json",
        success: function (data) {
            if (data.resp.result) {
                $("#SelectValue").val("0");
                DoCancel('1');
                SetQuery();
            } else {
                alert(data.resp.message)
            }
        }
    });
}

// 呼叫組織設定視窗
function CallAuth(Msg) {
    if (IsEmpty($("#SelectValue").val()) || $("#SelectValue").val() == "0")
        NotSelectForDelete(Msg);
    else {
        $.ajax({
            type: "POST",
            url: "ManageArea.aspx",
            data: { "DoAction": "OrgList", "MgaID": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                $('[name*="popInput_No3"]').val(data.resp.MgaNo);
                $('[id*="L_popName3"]').text("「" + data.resp.MgaName + "」" + $("#AuthEdit").val());
                $("#popB_OrgList1 option").remove();
                $("#popB_OrgList2 option").remove();
                if (data.success) {
                    var arr = Array.prototype.slice.call(data.OrgList1);
                    arr.forEach(function (ele) {
                        $('#popB_OrgList1').append(new Option(ele.OrgNameList, ele.OrgStrucID));
                    });
                    arr = Array.prototype.slice.call(data.OrgList2);
                    arr.forEach(function (ele) {
                        $('#popB_OrgList2').append(new Option(ele.OrgNameList, ele.OrgStrucID));
                    });
                    ShowOver('2');
                } else {
                    alert(data.message)
                }
            }
        });
    }
}

// 呼叫設備群組視窗
function CallAuth2(Msg) {
    if (IsEmpty($("#SelectValue").val()) || $("#SelectValue").val() == "0")
        NotSelectForDelete(Msg);
    else {
        $.ajax({
            type: "POST",
            url: "ManageArea.aspx",
            data: { "DoAction": "EquGrList", "MgaID": $("#SelectValue").val() },
            dataType: "json",
            success: function (data) {
                $('[name*="popInput_No4"]').val(data.resp.MgaNo);
                $('[id*="L_popName4"]').text("「" + data.resp.MgaName + "」" + $("#AuthEdit").val());
                $("#popB_EquGrList1 option").remove();
                $("#popB_EquGrList2 option").remove();
                if (data.success) {
                    var arr = Array.prototype.slice.call(data.EquGrList1);
                    arr.forEach(function (ele) {
                        $('#popB_EquGrList1').append(new Option(ele.EquGrpName, ele.EquGrpID));
                    });
                    arr = Array.prototype.slice.call(data.EquGrList2);
                    arr.forEach(function (ele) {
                        $('#popB_EquGrList2').append(new Option(ele.EquGrpName, ele.EquGrpID));
                    });
                    ShowOver('3');
                } else {
                    alert(data.message)
                }
            }
        });
    }
}

//執行組織架構更新動作
function AuthExcute(UserID) {
    var values = $.map($('#popB_OrgList2 option'), function (ele) {
        return ele.value;
    });
    $.ajax({
        type: "POST",
        url: "ManageArea.aspx",
        data: { "DoAction": "AuthUpdate", "MgaID": $("#SelectValue").val(), "OrgList": values.join(",") },
        dataType: "json",
        success: function (data) {
            if (data.resp.result) {                
                DoCancel('2');
            } else {
                alert(data.resp.message)
            }
        }
    });
}

//執行設備群組更新動作
function AuthExcute2(UserID) {
    var values = $.map($('#popB_EquGrList2 option'), function (ele) {
        return ele.value;
    });
    $.ajax({
        type: "POST",
        url: "ManageArea.aspx",
        data: { "DoAction": "AuthUpdate2", "MgaID": $("#SelectValue").val(), "EquGrList": values.join(",") },
        dataType: "json",
        success: function (data) {
            if (data.resp.result) {
                DoCancel('3');
            } else {
                alert(data.resp.message)
            }
        }
    });
}


function SelectState() {
    //SelectValue.value = '';
    //__doPostBack(QueryButton.id, 'NewQuery');
}

//控制項加入與移除的動作
function DataEnterRemove(str) {
    var option = null;
    var num = null;
    if (str == 'Add') {        
        $("#popB_OrgList2").append($("#popB_OrgList1 option:selected"));
        $("#popB_OrgList1  option:selected").remove();
    }
    else if (str == 'Del') {        
        $("#popB_OrgList1").append($("#popB_OrgList2 option:selected"));
        $("#popB_OrgList2 option:selected").remove();
    }
}

//控制項加入與移除的動作
function DataEnterRemove2(str) {
    var option = null;
    var num = null;
    if (str == 'Add') {
        $("#popB_EquGrList2").append($("#popB_EquGrList1 option:selected"));
        $("#popB_EquGrList1.id  option:selected").remove();
    }
    else if (str == 'Del') {
        $("#popB_EquGrList1").append($("#popB_EquGrList2 option:selected"));
        $("#popB_EquGrList2 option:selected").remove();
    }
}



function SetNowName(NowName) {
    //SelectNowName.value = NowName;
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
