var sNumStr = "一二三四五六七八九十";

function NotSelectMsg() {
    alert(sMsg);
}

function NotSelectForDelete(sMsg) {
    //var sMsg = '';
    //sMsg += '請注意!!  目前無法進行刪除\n\n';
    //sMsg += '可能的原因：1.沒有資料可供刪除。\n';
    //sMsg += '　　　　　　2.尚未選擇要刪除的項目。\n';
    alert(sMsg);
}

function NotSelectForEdit(sMsg) {
    //var sMsg = '';
    //sMsg += '請注意!!  目前無法進行編輯\n\n';
    //sMsg += '可能的原因：1.沒有資料可供編輯。\n';
    //sMsg += '　　　　　　2.尚未選擇要編輯的項目。\n';
    alert(sMsg);
}

function NotSelectForSend(sMsg) {
    //var sMsg = '';
    //sMsg += '請注意!!  目前無法進行傳送設定\n\n';
    //sMsg += '可能的原因：1.沒有資料可供傳送。\n';
    //sMsg += '　　　　　　2.尚未選擇要傳送的項目。\n';
    alert(sMsg);
}

function NotSelectForQuery(sMsg) {
    //var sMsg = '';
    //sMsg += '請注意!!  目前無法進行傳送查詢\n\n';
    //sMsg += '可能的原因：1.沒有資料可供傳送查詢。\n';
    //sMsg += '　　　　　　2.尚未選擇要傳送查詢的項目。\n';
    alert(sMsg);
}

// 配合 PageMethods 執行發生錯誤時，訊息顯示
function onPageMethodsError(error) {
    if ($.find('blockUI').length > 0) {
        $.unblockUI();
    }
    var msg = '';
    msg += '\nTrace:' + error.get_stackTrace();
    msg += '\nStatusCode:' + error.get_statusCode();
    msg += '\nExceptionType:' + error.get_exceptionType();
    msg += '\nMessage:' + error.get_message();
    msg += '\nTimedOut:' + error.get_timedOut();
    alert(msg);
}

function VerifyConfirmation(Msg, callback) {
    debugger;
    BootstrapDialog.confirm(Msg, function (result) {
        if (result === true) {
            var Ramdom = Math.floor((Math.random() * (8999 - 0)) + 1000);//取1000-9999亂數

            BootstrapDialog.show({
                title: 'Verify Confirmation',
                message: $('<span>驗證碼：</span><span id="VerifyCode" style="color:red;">' + Ramdom + '</span><br /><br /><input id="CheckCode" class="form-control" placeholder="Try to input multiple lines here..." maxlength="4"></input>'),
                buttons: [{
                    label: 'Confirm',
                    cssClass: 'btn-primary',
                    hotkey: 13,//Enter.
                    action: function (dialogItself) {
                        if ($('#CheckCode').val() === $('#VerifyCode').text()) {
                            dialogItself.close();//關閉驗證碼視窗
                            callback();
                        }
                        else {
                            alert('驗證碼輸入錯誤！');
                        }
                    }
                }]
            });
        }
    });
}