
var sh;

// SignalR專用，連結HUB
function ConnectSignalRHub() {
    sh = $.connection.SignalRHub;

    // 與伺服器連線註冊
    $.connection.hub.start().
        done(function () {
            // 與伺服器連線註冊成功後要執行的作業

        }).
        fail(function () {
            alert('Connect Server Failed...');
        });

    // 註冊給伺服端呼叫的方法
    sh.client.SqlHasChange = function () {
        //alert('偵測到伺服器資料改變');
        GetGridView();
    };
}

// SignalR專用，為了選擇下拉式選單後更新GridView
function GetGridViewForButton() {
    var ddlCtrlArea = $('#ContentPlaceHolder1_ddlCtrlArea option:selected').val();

    // 當下拉選單ddlCtrlArea和暫存hCtrlArea相同值時，則不需更新SqlDependency
    if (ddlCtrlArea != $('#ContentPlaceHolder1_hCtrlArea').val()) {
        $('#ContentPlaceHolder1_hCtrlArea').val(ddlCtrlArea);

        sh.server.setState(ddlCtrlArea);
    }

    __doPostBack('ContentPlaceHolder1_UpdatePanel1', '');
}

// SignalR專用，為了更新GridView
function GetGridView() {
    var ddlCtrlArea = $('#ContentPlaceHolder1_ddlCtrlArea option:selected').val();

    GridView_Panel_scrollTop.value = document.getElementById('ContentPlaceHolder1_tablePanel').scrollTop;

    sh.server.setState(ddlCtrlArea);
    __doPostBack('ContentPlaceHolder1_UpdatePanel1', '');
}

function GetGridViewForButtonAndNoSiganlR() {
    __doPostBack('ContentPlaceHolder1_UpdatePanel1', '');
}

function MoveScroll() {
    document.getElementById('ContentPlaceHolder1_tablePanel').scrollTop = GridView_Panel_scrollTop.value;
}

//*主要作業畫面：執行「匯出」動作
function ExportQuery() {
    __doPostBack(ExportButton.id, '');
}