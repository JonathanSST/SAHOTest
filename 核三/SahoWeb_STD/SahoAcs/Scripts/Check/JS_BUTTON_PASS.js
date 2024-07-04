////////////////////////////////////////////////////////////
// 說明 : JavaScript Library About Button
// 作著 : RD
// 日期 : 2001/06/20
////////////////////////////////////////////////////////////
function BUTTON_PASS(I_OBJ) {
    var blnSTATUS = false;
    var strURL = $(I_OBJ).attr("URL");
    var strFLAG = $(I_OBJ).attr("FLAG");
    var strBUT_ATTR = $(I_OBJ).attr("BUTTON_ATTR");
    var strBUT_NAME = $(I_OBJ).attr("BUTTON_NAME");
    var strMSG_TYPE = $(I_OBJ).attr("MSG_TYPE");
    var strMSG_TEXT = $(I_OBJ).attr("BUTTON_MSG");
    var strMAIN_URL = $(I_OBJ).attr("MAIN_URL");

    //AJAX 新增
    var strAJAX_DB = $(I_OBJ).attr("AJAX_DB");

    //AJAX 新增, 是否換頁, 如果不想 reload, 請設定 0
    var strREDIRECT = $(I_OBJ).attr("AJAX_REDIRECT");

    //紀錄目前網址
    $('input[name="URL_PREV"]').val(window.location.href);

    //重設登出時間
    if (typeof (SetLogoutTime) == 'function') {
        try {
            SetLogoutTime();
        } catch (e) { }
    }

    // Button 提示訊息
    if (strMSG_TYPE == 'a') {
        alert(strMSG_TEXT);
    } else if (strMSG_TYPE == 'c') {
        if (!confirm(strMSG_TEXT)) {
            return false;
        }
    }

    switch (strBUT_ATTR) {
        case "N":
            window2 = window.open(strURL, "NewWindow", "scrollbars=yes,width=600,height=480")
            break;

        case "B":
            //回上一頁

            //顯示讀取中, 避免重複按
            JsFunLOADING_SHOW(true);

            history.back();
            break;

        case "Q":
            //重新查詢

            //顯示讀取中, 避免重複按
            JsFunLOADING_SHOW(true);

            window.location.href = strMAIN_URL;
            break;

        case "L":
            //重新讀取

            //顯示讀取中, 避免重複按
            JsFunLOADING_SHOW(true);

            window.location = strURL;
            break;

        default:
            //
            if (JsFunLOADING() == true) {
                alert(JsFunML(ML_JS_VERTIFY, '程式已執行，請不要重覆按鍵傳送') + '!');
                return false;
            }

            if (strAJAX_DB != '1') {
                //顯示讀取中, 避免重複按
                JsFunLOADING_SHOW(true);
            }

            //假設按鈕為刪除
            if (strBUT_NAME == "刪除") {
                if (JsFunSET_DELETE() == false) {
                    JsFunLOADING_SHOW(false);
                    return false;
                }
            }

            //處理驗證
            if (JsFunVertify(I_OBJ) == false) {
                JsFunLOADING_SHOW(false);
                return false;
            }

            //伺服器驗證
            if ($(I_OBJ).attr('SERVER_VERTIFY') == '1') {
                if (typeof (AJAX_IMS30_VERTIFY_INVOKE) == 'function') {
                    AJAX_IMS30_VERTIFY_INVOKE(I_OBJ);
                    return false;
                }
            } else {
                //清除檢視狀態避免錯誤
                $("#__VIEWSTATE").val('');

                //
                JsFunSUBMIT_B();

                $(':button').prop("disabled", true);
                if (strAJAX_DB == '1') {
                    //AJAX 新增
                    IMS30_DB_AJAX_INVOKE(I_OBJ, strREDIRECT);
                    $(':button').prop("disabled", false);
                } else {
                    document.forms[0].action = strURL;
                    document.forms[0].submit();
                }
            }
            break;
    }

    return true;
}

/*
=========================================================================================
// 進行驗證
=========================================================================================*/
function JsFunVertify(I_OBJ) {
    //啟用基本驗證
    $(I_OBJ).attr('BASE_VERTIFY', '1');

    //啟用伺服器驗證
    $(I_OBJ).attr('SERVER_VERTIFY', '0');

    //自訂驗證
    if (typeof (vertify) == 'function') {
        if (!vertify(I_OBJ)) {
            return false;
        }
    }

    //基本驗證
    if ($(I_OBJ).attr('BASE_VERTIFY') == '1') {
        if (typeof (JsFunBASE_VERTIFY) == 'function') {
            if (!JsFunBASE_VERTIFY(I_OBJ)) {
                return false;
            }
        }
    }
    return true;
}

/*
=========================================================================================
// 進行驗證
=========================================================================================*/
function JsFunSUBMIT_B() {
    var oCOLS_TABLES = {};
    var M_SEQ_NO = "";
    var oTABLE = null;
    var oTR = null;
    var oNEW_TABLE = null;

    //移除階層未選擇項目
    $('table[LV_OP]').each(function () {
        var OP_TABLE = this;
        $(OP_TABLE).find('tr:not(:has(input:checked))').remove();

        //
        $(OP_TABLE).find('tr').each(function () {
            oTR = this;
            oTABLE = JsFunFindParent(oTR, 'TABLE');
            M_SEQ_NO = $(oTABLE).attr('M_SEQ_NO');

            if (oCOLS_TABLES[M_SEQ_NO] == undefined) {
                oCOLS_TABLES[M_SEQ_NO] = $('<TABLE style="position:absolute;visibility:hidden" />');
                $('form:eq(0)').append(oCOLS_TABLES[M_SEQ_NO]);
            }

            oNEW_TABLE = oCOLS_TABLES[M_SEQ_NO];
            $(oNEW_TABLE).append(oTR);
        });

        for (var i in oCOLS_TABLES) {
            $(oCOLS_TABLES[i]).find(':input[name="CHK_COL_' + i + '"]').each(function (index) {
                $(this).val(index);
            });
        }
    });
}