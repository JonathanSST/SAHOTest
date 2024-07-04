﻿var conn_count = 0;

function setEquCheck() {
    var conn_message = "";
    $.ajax({
        type: "POST",
        url: "QueryCheckAlive.aspx",
        dataType: "json",
        data: { "QueryConn": "Send" },
        success: function (data) {
            if (example1 !== undefined) {
                example1.$set('items', data.equ_lists);
            }
        },
        fail: function () {

        }
    });
}

var example1 = new Vue({
    el: '#list_each',
    data: {
        items: []
    }
});

function getEquCheck() {    
    var conn_message = "";
    $("#Waiting").css("display", "block");
    $.ajax({
        type: "POST",
        url: "QueryCheckAlive.aspx",
        dataType: "json",
        data: { "QueryConn": "Init" },
        success: function (data) {
            if (example1 !== undefined) {
                //example1.$set('items', data.equ_lists);
                example1.items = data.equ_lists;
                $("#Waiting").css("display", "none");
                //setTimeout(SendData, 1000);
            }
        },
        fail: function () {

        }
    });
}


/*
function getEquCheck() {
    var example1 = new Vue({
        el: '#list_each',
        data: {
            items: [
                { EquName: 'Test1' }, { EquName: 'Test2' }, { EquName: 'Test3' }, { EquName: 'Test4' }
            ]
        }
    });
}
*/

var EquList = Array();


$(document).ready(function () {
    //$('[name*="ChkGetTime"]').prop('checked', 'checked');
    getEquCheck();
    //setTimeout(SendData(), 1000);    
    //SendData();
});

function SendData() {
    getEquCheck();

}


function GetResult() {    
    $.ajax({
        type: "POST",
        url: "QueryCtrlEquRealtime.aspx",
        dataType: "json",
        data: { "QueryConn": "Get", "EquList": EquList },
        success: function (data) {
            example1.$set('items', data.equ_lists);
            $("#btnSend").prop("disabled", false);
            $("#Waiting").css("display", "none");
            setTimeout(function () {
                $("#ContentPlaceHolder1_MainGridView").find("tr").each(function (index) {
                    if (EquList.indexOf($(this).find(".ChkClass").val()) != -1) {
                        $(this).find(".ChkClass").prop("checked", true);
                    }
                });
            }, 100);
        },
        fail: function () {

        }
    });
}

function CheckOpen(obj) {
    //console.log($(obj).find("td:eq(2)").html());
}

function SetVueValue() {
    //example1.items[0].EquNo = example1.items[0].EquNo + "9999";
    //console.log($("#list_each").find("tr:eq(0)").find("td:eq(1)").html());
    //$("#list_each").find("tr:eq(0)").find("td:eq(1)").html($("#list_each").find("tr:eq(0)").find("td:eq(1)").html() + "999");
    example1.items[0].EquName = example1.items[0].EquNo + example1.items[0].EquID
}





