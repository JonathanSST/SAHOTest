/*
=========================================================================================
// AJAX 回呼
=========================================================================================*/
var INI_AJAX_TAG = "RSL_AJAX";
function RSL_AJAX_INVOKE_CONTROL_METHOD(I_CLIENT_ID, I_METHOD_NAME, I_METHOD_ARGS, I_CALL_BACK) {
    var form = $("form:eq(0)");
    var strPARAM = '';

    strPARAM += "&RDM=" + Math.random();
    strPARAM += '&' + INI_AJAX_TAG + '_CLIENT_ID=' + I_CLIENT_ID;
    strPARAM += '&' + INI_AJAX_TAG + '_METHOD_NAME=' + I_METHOD_NAME;

    for (var i = 0; i < I_METHOD_ARGS.length; i++)
        strPARAM += '&' + INI_AJAX_TAG + '_METHOD_ARGS=' + I_METHOD_ARGS[i];

    //清除檢視狀態避免錯誤
    $("#__VIEWSTATE").val('');
    var AJAX_PARAM = {
        type: $(form).attr('method'),
        url: $(form).attr('action'),
        data: $(form).serialize() + strPARAM
    };

    //進行 AJAX
    var request = $.ajax(AJAX_PARAM);

    //執行完成
    request.done(function (msg) {
        if (I_CALL_BACK != undefined) {
            var oHTML = $("<span>" + msg + "</span>");
            var oAJAX_RETURN = $(oHTML).find('[id$="RSL_AJAX_RETURN"]');
            var strRETURN_VAL = $(oAJAX_RETURN).find('[id="RSL_AJAX_RETURN_VAL"]').val();

            alert(oAJAX_RETURN.length);

            I_CALL_BACK({
                Response: oHTML[0].outerHTML.replace(oAJAX_RETURN[0].outerHTML, ''),
                value: strRETURN_VAL
            });
        }
    });

    //
    request.fail(function (jqXHR, textStatus) {
        //alert("Request failed: " + textStatus);
    });
}

/*
=====================================
說明 : 取得訊息視窗
=====================================*/
function JsFunGET_IMS30_DB_AJAX_DIALOG() {
    var oDIV = $('#DB_AJAX_UPDATE');
    if ($(oDIV).length == 0) {
        oDIV = $('<DIV id="DB_AJAX_UPDATE"></DIV>');
        $('body').append(oDIV);
    }
    return oDIV;
}

/*
=====================================
說明 : 關閉視窗
=====================================*/
function JsFunIMS30_DB_AJAX_DIALOG_CLOSE(I_OBJ, I_PARAM) {
    var oDIV = JsFunGET_IMS30_DB_AJAX_DIALOG();
    var oPARAM = {};

    //是否重讀頁面
    oPARAM["AJAX_REDIRECT"] = (I_PARAM.length > 0) ? I_PARAM.substr(0, 1) : '';

    //取消面板連結功能
    oPARAM["CANCEL"] = false;

    //
    try {
        if (typeof (JsFunEVENT_COMMAND) == 'function')
            JsFunEVENT_COMMAND("AJAX_DIALOG_CLOSE", { sender: I_OBJ, EventArgs: oPARAM });
    } catch (e) { }

    if (oPARAM["CANCEL"] != true) {
        if (oPARAM["AJAX_REDIRECT"] == '0') {
            $(oDIV).dialog("destroy");
        } else {
            window.location.href = window.location.href;
        }
    }
}

/*
=====================================
說明 : 使用 AJAX
參數 : I_OBJ:目前被觸發的按鈕參考('<LI id=... />')
I_USER_CONTROL_ID:伺服器控制項 ClientID
=====================================*/
function IMS30_DB_AJAX_INVOKE(I_OBJ, I_PARAM) {
    //Anthem 套件的 AJAX 方法, 可以呼叫 .net 伺服端的方法
    var oDIV = JsFunGET_IMS30_DB_AJAX_DIALOG();
    var oFORM = $('FORM:eq(0)');

    //按鈕 URL 的參數設定
    var COLS_URL_FRM = $(oFORM).attr('action').split('?');
    var COLS_URL_BTN = $(I_OBJ).attr('URL').split('?');
    if (COLS_URL_FRM.length > 1) {

        $(oFORM).attr('action', COLS_URL_FRM[0] + '?' + COLS_URL_BTN[1]);
        //        alert($(oFORM).attr('action'));
        //        return;
    }

    //開啟視窗
    $(oDIV).dialog("destroy");
    $(oDIV).html('<FONT class="n11">資料處理中...<FONT>').dialog(
        { modal: true,
            resizable: true,
            width: '80%',
            height: '80%'
        }
    );

    //隱藏關閉
    $('.ui-dialog-titlebar-close').hide();

    Anthem_InvokeControlMethod(
            IMS30_DB_AJAX_CLIENT_ID,
            'DB_AJAX_UPDATE',
            [I_PARAM],
            function (result) {
                //將回傳轉為 JSON 陣列
                var JSON = {};

                //
                try {
                    eval("JSON = " + result.value + ";");
                    $(oDIV).html(JSON['HTML']);
                } catch (e) {
                    $(oDIV).html('<FONT class="n11">資料處理發生錯誤!<FONT>');
                }

                //
                if (typeof (JsFunEVENT_COMMAND) == 'function')
                    JsFunEVENT_COMMAND("DB_AJAX_UPDATE", { sender: oDIV, EventArgs: JSON });
            }
        );
}

