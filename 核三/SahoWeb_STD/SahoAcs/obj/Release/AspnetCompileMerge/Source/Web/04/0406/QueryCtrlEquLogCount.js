var conn_count = 0;
function setEquCheck() {
    var conn_message = "";
    $.ajax({
        type: "POST",
        url: "QueryCtrlEquLogCount.aspx",
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
    if (conn_count > 10) {
        return false;
    }
    var conn_message = "";
    $.ajax({
        type: "POST",
        url: "QueryCtrlEquLogCount.aspx",
        dataType: "json",
        data: { "QueryConn": "Get" },
        success: function (data) {
            if (example1 !== undefined) {
                example1.$set('items', data.equ_lists);
            }
            if (data.equ_lists[0].EquDesc === "") {
                setEquCheck();
            }
            conn_count += 1;
        },
        fail: function () {

        }
    });
}
setInterval("getEquCheck()", 3000);

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

function CheckOpen(obj) {
    //console.log($(obj).find("td:eq(2)").html());
}