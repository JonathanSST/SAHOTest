/*
=========================================================================================
// ETEK_LIST
=========================================================================================*/
var frmLOV = null;

/*
=========================================================================================
//開啟自訂LIST
=========================================================================================*/
function JsFunOPEN_ETEK_LIST_CUSTOM(I_TYPE, I_PARAM) {
    var URL = "";

    //設定網址
    URL += "../Kernel/ETEK_LIST.aspx";
    //使用 Math.random() 避免使用快取
    URL += "?RDM=" + Math.random();

    if (I_PARAM["PG_ID"] != undefined)
        URL += "&PG_ID=" + I_PARAM["PG_ID"];

    if (I_PARAM["PAGE_ID"] != undefined)
        URL += "&PAGE_ID=" + I_PARAM["PAGE_ID"];

    if (I_PARAM["FUN"] != undefined)
        URL += "&FUN=" + I_PARAM["FUN"];

    if (I_PARAM["M_SEQ_NO"] != undefined)
        URL += "&M_SEQ_NO=" + I_PARAM["M_SEQ_NO"];

    if (I_PARAM["FIELD_ID"] != undefined)
        URL += "&FIELD_ID=" + I_PARAM["FIELD_ID"];

    if (I_PARAM["CID"] != undefined)
        URL += "&CID=" + I_PARAM["CID"];

    if (I_PARAM["RID"] != undefined)
        URL += "&RID=" + I_PARAM["RID"];

    //自訂 ETEK LIST
    if (I_PARAM["ASCX"] != undefined)
        URL += "&ASCX=" + I_PARAM["ASCX"];

    //視窗
    try {
        frmLOV.close();
    } catch (e) { }

    frmLOV = open(URL, "winLOV", "dependent=yes, status=1, scrollbars=1,resizable=1,width=600,height=400");
    if (frmLOV.opener == null)
        frmLOV.opener = self;
}

function JsFunOPEN_ETEK_LIST_ADDR(I_TYPE, I_COL, I_FIELD_ID) {

    var oETEK = $(I_COL).prev()[0];
    var URL = "";

    //建立參數
    var oETEK_PARAM = {
        'CANCEL': false,
        'TABLE': JsFunFindParent(oETEK, "TABLE"),
        'ROW': JsFunFindParent(oETEK, "TR"),
        'FIELD': oETEK,
        'STYLE_M': ''
    };

    var idx = 1;
    var chk = 1;
    if ($(oETEK_PARAM['TABLE']).prop('STYLE_M') == "02") {
        idx = 0;
        chk = 0;
    }

    var tmpID = I_FIELD_ID.split("^");
    var mseqno = $(oETEK).attr("M_SEQ_NO");

    for (var i = 0; i < tmpID.length; i++) {
        var tmpOBJ;
        if (chk == 0) {
            tmpOBJ = $(oETEK_PARAM['ROW']).find("[CID='" + tmpID[i] + "_" + mseqno + "']");
            idx = $(oETEK_PARAM['ROW']).prop('rowIndex');
        }
        else {
            tmpOBJ = $("[CID='" + tmpID[i] + "_" + mseqno + "']");
            idx = 1;
        }
        $(tmpOBJ).attr("RID", "txt" + tmpID[i] + "_" + mseqno + '_' + idx);
    }
    //設定 RID
//    $(oETEK).attr('RID', function () {
//        return I_FIELD_ID + '_' + $(oETEK_PARAM['ROW']).prop('rowIndex');
//    });

    //設定網址
    URL += "../ETEK_LIST/ADDR_CITY_LIST.aspx";
    URL += "?List_Prog_ID=" + $(oETEK).attr("PROG_ID");
    URL += "&List_Page_ID=" + $(oETEK).attr("PAGE_ID");
    URL += "&List_Field_Id=" + $(oETEK).attr("RID");
    URL += "&M_SEQ_NO=" + $(oETEK).attr("M_SEQ_NO");
    URL += "&RETURNFIELD=" + $(oETEK).attr("RETURN_FIELD");
    URL += "&RETURN_CONTROL=" + $(oETEK).attr("RETURN_CONTROL") + "_" + mseqno + '_' + idx;

    frmLOV = open(URL, "winLOV", "dependent=yes, status=1, scrollbars=1,resizable=1,width=500,height=400");
    if (frmLOV.opener == null)
        frmLOV.opener = self;
}
/*
=========================================================================================
//設定[ETEK_LIST] 按鈕點選
=========================================================================================*/
var frmLOV = null;
function JsFunOPEN_ETEK_LIST(I_TYPE, I_COL) {
    var COLS_FIELD_NAME = {};
    var oETEK = $(I_COL).prev()[0];
    var URL = "";
    var strWHERE_VALUE = "";
    
    //建立參數
    var oETEK_PARAM = {
        'CANCEL': false,
        'TABLE': JsFunFindParent(oETEK, "TABLE"),
        'ROW': JsFunFindParent(oETEK, "TR"),
        'FIELD': oETEK,
        'STYLE_M': ''
    };

    oETEK_PARAM['STYLE_M'] = $(oETEK_PARAM['TABLE']).attr("STYLE_M");

    //設定 RID
    $(oETEK).attr('RID', function () {
        $(this).attr('CID', $(this).attr('ID'));
        return $(this).attr('name') + '_' + $(oETEK_PARAM['ROW']).prop('rowIndex');
    });

    //觸發回傳事件
    if (typeof (JsFunEVENT_COMMAND) != undefined) {
        try {
            JsFunEVENT_COMMAND("ETEK_LIST_BEFORE", { sender: oETEK, EventArgs: oETEK_PARAM });
            if (oETEK_PARAM['CANCEL'] == true)
                return;
        } catch (ev) { }
    }

    var COLS_WHERE = $(oETEK).attr("WHERE_CONTROL");

    //取 WHERE 值
    strWHERE_VALUE = JsFunGET_ETEK_WHERE(
        oETEK_PARAM['TABLE'],
        oETEK_PARAM['ROW'],
        $(oETEK).attr("WHERE_CONTROL")
    );

    //alert(oETEK.outerHTML);

    //設定網址
    URL += "../Kernel/ETEK_LIST.aspx";
    //使用 Math.random() 避免使用快取
    URL += "?RDM=" + Math.random();
    URL += "&PG_ID=" + $(oETEK).attr("PROG_ID");
    URL += "&PAGE_ID=" + $(oETEK).attr("PAGE_ID");
    URL += "&M_SEQ_NO=" + $(oETEK).attr("M_SEQ_NO");
    URL += "&FIELD_ID=" + $(oETEK).attr("FIELD_ID");
    URL += "&RID=" + $(oETEK).attr("RID");
    URL += "&BTN_ETEK_SQL_P=" + $(oETEK).attr("BTN_ETEK_SQL");
    
    //自訂 SQL
    COLS_FIELD_NAME['IS_CUSTOM'] = "";

    //自訂 ETEK LIST
    COLS_FIELD_NAME['ASCX'] = "";

    //是否[多選輸入]
    COLS_FIELD_NAME['MULTIPLE_INPUT'] = "";

    //是否顯示[選擇全部]
    COLS_FIELD_NAME['SHOW_SET_ALL'] = "";

    //
    COLS_FIELD_NAME['CID'] = "";

    for (var strFIELD_NAME in COLS_FIELD_NAME) {
        if ($(oETEK).attr(strFIELD_NAME) != undefined)
            URL += "&" + strFIELD_NAME + "=" + $(oETEK).attr(strFIELD_NAME);
    }

    //WHERE 條件
    URL += "&ETEK_WHERE=" + strWHERE_VALUE;

    //新增參數
    if ($("#ETEK_PARAM").length == 0) {
        $("body").append("<INPUT type='hidden' ID='ETEK_PARAM' />");
    }

    //設定列參考
    document.getElementById("ETEK_PARAM")["TR_EVENT"] = oETEK_PARAM;

    //視窗
    try {
        frmLOV.close();
    } catch (e) { }

    try {
        frmLOV = open(URL, "winLOV", "dependent=yes, status=1, scrollbars=1,resizable=1,width=600,height=400");
        if (frmLOV.opener == null)
            frmLOV.opener = self;
    } catch (e) { }
}

/*
=========================================================================================
// 初始化
=========================================================================================*/
function JsFunETEK_SET_STYLE() {
    var chaFORM = '#FORM_X';

    if ($(chaFORM).find('[name="PAGE_INDEX"]').length == 0)
        $(chaFORM).append('<input type="hidden" name="PAGE_INDEX" value="1" />');

    $('body').addClass('styPopupWinBody');
    $(':input[type="text"]').addClass('inputbox10');
    $('DIV[id$="CONTENT_ETEK_BUTTON"]').addClass("t10").css({ "margin": '0.5em' })
        .find(':button').button();
}
function JsFunETEK_INIT() {
    JsFunETEK_SET_STYLE();

    $('DIV[id$="CONTENT_ETEK_BUTTON"]').each(function () {
        $(this).find(":button").bind("click", function () {
            var chaFORM = "#FORM_X";

            switch ($(this).attr("id")) {
                case "BTN_SELECT":
                    //查詢
                    if ($(chaFORM).find("[name='ETEK_FILTER']").length == 0)
                        $(chaFORM).append("<INPUT type='hidden' name='ETEK_FILTER' />");

                    var Filter = $("#ETEK_FILTER").val();
//                    var Filter = $(chaFORM).find("[name='ETEK_FILTER']").val();
//                    var Filter_NOW = $("#ETEK_FILTER").val();
//                    if (Filter_NOW == "") {
//                        Filter = "";
//                    }
//                    else {
//                        if (Filter == "")
//                            Filter = Filter_NOW;
//                        else
//                            Filter += "," + Filter_NOW;
//                    }

                    $(chaFORM).find("[name='ETEK_FILTER']").val(Filter);
                    $(chaFORM).submit();
                    break;

                case "BTN_SELECT_CLOSE":
                    //多選
                    var oTABLE = $('[id$="CONTENT_ETEK_DEFAULT"]').find("TABLE");
                    JsFunETEK_EVENT_CMD("BTN_SELECT_CLOSE", oTABLE);
                    break;

                case "BTN_CLOSE":
                    //關閉
                    close();
                    break;

                case "BTN_SET_ALL":
                    //選擇全部
                    JsFunETEK_EVENT_CMD("ETEN_SET_ALL", this);

                    close();
                    break;

                case "BTN_SET_CLEAR":
                    //清除選擇
                    JsFunETEK_EVENT_CMD("ETEN_SET_CLEAR", this);

                    close();
                    break;
            }
        });
    });

    //設定事件
    JsFunETEK_EVENT_SET();
}

/*
=========================================================================================
// 設定列點選
=========================================================================================*/
function JsFunETEK_EVENT_SET() {
    var oTABLE = $('[id$="CONTENT_ETEK_DEFAULT"]').find("TABLE");
    
    if ($(oTABLE).attr("LAYOUT_TYPE") == "ETEK_LIST" && $(oTABLE).attr("MULTIPLE_INPUT") != "1") {
        $(oTABLE).find('tr:not([UI-WIDGET-HEADER=])').each(function (index) {
            //如果非輸入項目, 可以點選
            $(this).find('td:not(:has(:input))').bind('click', function () {
                var oTR = JsFunFindParent(this, "TR"); ;
                JsFunETEK_EVENT_CMD("ETEN_ROW_CLICK", oTR);
            });
        });
    }
}

/*
=========================================================================================
// 取 ETEK WHERE 值
=========================================================================================*/
function JsFunGET_ETEK_WHERE(I_TABLE, I_TR, I_WHERE_CONTROL) {
    var oTABLE = I_TABLE;
    var oTR = I_TR;
    var strSTYLE_M = $(oTABLE).attr("STYLE_M");
    var COLS_WHERE;
    var oWHERE_CONTROL;
    var strWHERE_VALUE = '';

    if (I_WHERE_CONTROL != undefined && I_WHERE_CONTROL != '') {
        COLS_WHERE = I_WHERE_CONTROL.split(",");
        for (var i = 0; i < COLS_WHERE.length; i++) {
            if (strSTYLE_M == "01")
                COLS_WHERE[i] = $(oTABLE).find(":input[name='" + COLS_WHERE[i] + "']:eq(0)").val();
            else {
                //多筆輸入時, 預設抓本身所在 TR, 如果沒有就抓其他區塊
                oWHERE_CONTROL = $(oTR).find(":input[name='" + COLS_WHERE[i] + "']:eq(0)");
                if ($(oWHERE_CONTROL).length > 0)
                    COLS_WHERE[i] = $(oWHERE_CONTROL).val();
                else
                    COLS_WHERE[i] = $(":input[name='" + COLS_WHERE[i] + "']:eq(0)").val();
            }
        }
        strWHERE_VALUE = COLS_WHERE.join(",");
    }

    return strWHERE_VALUE;
}

/*
=========================================================================================
// 設定事件(點選 ETEK_LIST)
=========================================================================================*/
function JsFunETEK_EVENT_CMD(I_TYPE, I_OBJ) {
    var oTR = I_OBJ;

    //事件參數
    var oEVENT_PARAM = { blnKEEP_OPEN: false, blnMULTIPLE: false };
    var oPR = (opener) ? opener.document : parent.document;
    var oTABLE = $('[id$="CONTENT_ETEK_DEFAULT"]').find("TABLE");
    var COLS_RETURN_FIELD = $(oTABLE).attr("RETURN_FIELD").split(",");
    var COLS_RETURN_CONTROL = $(oTABLE).attr("RETURN_CONTROL").split(",");

    //ETEK 欄位 ClientID
    var strETEK_FIELD_ID = $(oTABLE).attr("ETEK_FIELD_ID");

    //取得參數設定
    var oETEK_PARAM = $(oPR).find("#ETEK_PARAM")[0]["TR_EVENT"];

    //
    var strSRC_FIELD_VAL = $(oETEK_PARAM["FIELD"]).val();
    var strSTYLE_M = $(oETEK_PARAM["TABLE"]).attr("STYLE_M");
    var strM_SEQ_NO = $(oETEK_PARAM["FIELD"]).attr("M_SEQ_NO");
    var oTARGET = (strSTYLE_M == "01") ? oETEK_PARAM["TABLE"] : oETEK_PARAM["ROW"];

    switch (I_TYPE) {
        case "ETEN_ROW_CLICK":
            //列選取
            for (var i = 0; i < COLS_RETURN_CONTROL.length; i++) {
                var strKEY = COLS_RETURN_FIELD[i].toString();
                var strCONTROL = COLS_RETURN_CONTROL[i].toString();
                var strVALUE = $.trim($(oTR).find(":input[name$='" + strKEY + "']").val());

                strCONTROL = strCONTROL.replace("txt", "");
                $(oTARGET).find(":input[name='" + strCONTROL + "_" + strM_SEQ_NO + "']").val(strVALUE);
            }
            break;

        case "BTN_SELECT_CLOSE":
            var arrVALUE = new Array(COLS_RETURN_CONTROL.length);
            for (var i = 0; i < arrVALUE.length; i++) {
                arrVALUE[i] = "";
            }

            $(oTABLE).find("tr:has(:checkbox)").each(function (index) {
                var CHK_COL = $(this).find(":input[name='CHK_COL_1']");
                if ($(CHK_COL).prop('checked') == true) {
                    var oTR = JsFunFindParent(CHK_COL, "TR");
                    for (var i = 0; i < COLS_RETURN_CONTROL.length; i++) {
                        var strKEY = COLS_RETURN_FIELD[i].toString();
                        var strVALUE = $.trim($(oTR).find(":input[name$='" + strKEY + "']").val());

                        if (arrVALUE[i] != "")
                            arrVALUE[i] += ",";
                        arrVALUE[i] += strVALUE;
                    }
                }
            });
            for (var i = 0; i < COLS_RETURN_CONTROL.length; i++) {
                var strCONTROL = COLS_RETURN_CONTROL[i].toString();
                strCONTROL = strCONTROL.replace("txt", "");
                $(oTARGET).find(":input[name='" + strCONTROL + "_" + strM_SEQ_NO + "']").val(arrVALUE[i]);
            }
            break;

        case "ETEN_SET_ALL":    //選擇全部
        case "ETEN_SET_CLEAR":  //清除選擇
            for (var i = 0; i < 2; i++) {
                var strKEY = COLS_RETURN_FIELD[i].toString();
                var strCONTROL = COLS_RETURN_CONTROL[i].toString();
                var strVALUE = '';

                if (I_TYPE == "ETEN_SET_ALL") {
                    //選擇全部
                    strVALUE = (i == 0) ? 'xx' : strVALUE;
                    strVALUE = (i == 1) ? '全部' : strVALUE;
                } else {
                    //清除選擇
                    strVALUE = (i == 0) ? '' : strVALUE;
                    strVALUE = (i == 1) ? '' : strVALUE;
                }

                strCONTROL = strCONTROL.replace("txt", "");
                $(oTARGET).find(":input[name='" + strCONTROL + "_" + strM_SEQ_NO + "']").val(strVALUE);
            }
            break;
    }

    //當選取時觸發事件

    //觸發回傳事件
    if (typeof (opener.JsFunEVENT_COMMAND) != undefined) {
        try {
            oEVENT_PARAM = opener.JsFunEVENT_COMMAND("ETEK_LIST_SELECT", { sender: oTR, EventArgs: oETEK_PARAM });
        } catch (ev) { }
    }

    //觸發焦點
    $(oETEK_PARAM["FIELD"]).focus();

    close();
}

/*
=========================================================================================
// 變更顯示筆數
=========================================================================================*/
function JsFunETEK_PAGE_ROWS_CHANGE(I_THIS) {
    var oFORM = JsFunETEK_GET_FORM();
    var oPAGE_ROWS = $(oFORM).find("input[name='PAGE_ROWS_1']");
    
    if ($(oPAGE_ROWS).length == 0) {
        oPAGE_ROWS = $("<INPUT type='hidden' name='PAGE_ROWS_1' value='1' />");
        $(oFORM).append(oPAGE_ROWS);
    }

    $(oPAGE_ROWS).val($(I_THIS).val());
    $(oFORM).submit();
}

/*
=========================================================================================
// 取得作用中的 FORM
=========================================================================================*/
function JsFunETEK_GET_FORM() {
    var chaFORM = '#FORM_X';
    return $(chaFORM);
}

/*
=========================================================================================
// ETEK_BG
=========================================================================================*/

/*
=====================================
說明 : 使用 AJAX
參數 : I_OBJ:目前被觸發的按鈕參考('<LI id=... />')
I_USER_CONTROL_ID:伺服器控制項 ClientID
=====================================*/
function AJAX_ETEK_BG_INVOKE(I_OBJ) {
    
    //建立參數
    var oETEK_BG_PARAM = {
        'CANCEL': false,
        'TABLE': JsFunFindParent(I_OBJ, "TABLE"),
        'ROW': JsFunFindParent(I_OBJ, "TR"),
        'FIELD': I_OBJ,
        'STYLE_M': ''
    };
    
    oETEK_BG_PARAM['STYLE_M'] = $(oETEK_BG_PARAM['TABLE']).attr('STYLE_M');

    //觸發回傳事件
    if (typeof (JsFunEVENT_COMMAND) != undefined) {
        try {
            JsFunEVENT_COMMAND("ETEK_BG_BEFORE", { sender: I_OBJ, EventArgs: oETEK_BG_PARAM });
            if (oETEK_BG_PARAM['CANCEL'] == true)
                return;
        } catch (ev) { }
    }

    var oTAG = null;
    var PROG_ID = $(I_OBJ).attr("PROG_ID");
    var PAGE_ID = $(I_OBJ).attr("PAGE_ID");
    var M_SEQ_NO = $(I_OBJ).attr("M_SEQ_NO");
    var FIELD_ID = $(I_OBJ).attr("FIELD_ID");
    var CLIENT_ID = $(I_OBJ).attr("id");
    var COLS_WHERE = $(I_OBJ).attr("WHERE_CONTROL");
    var strWHERE_VALUE = "";

    //取 ETEK_BG 的欄位名稱
    if (COLS_WHERE == undefined || COLS_WHERE.length == 0)
        COLS_WHERE = $(I_OBJ).attr("name");
    else
        COLS_WHERE = $(I_OBJ).attr("name") + "," + COLS_WHERE;

    //取 WHERE 值
    strWHERE_VALUE = JsFunGET_ETEK_WHERE(oETEK_BG_PARAM['TABLE'], oETEK_BG_PARAM['ROW'], COLS_WHERE);

    //Anthem 套件的 AJAX 方法, 可以呼叫 .net 伺服端的方法
    Anthem_InvokeControlMethod(AJAX_ETEK_BG_CLIENT_ID, 'ETEK_AJAX', [PROG_ID, PAGE_ID, M_SEQ_NO, FIELD_ID, CLIENT_ID, strWHERE_VALUE,
    $.trim($(I_OBJ).attr("FORM_TITLE")),
    $.trim($(I_OBJ).attr("LIST_TITLE")),
    $.trim($(I_OBJ).attr("LIST_FIELD")),
    $.trim($(I_OBJ).attr("RETURN_FIELD")),
    $.trim($(I_OBJ).attr("RETURN_CONTROL")),
    $.trim($(I_OBJ).attr("WHERE_FIELD")),
    $.trim($(I_OBJ).attr("WHERE_CONTROL")),
    $.trim($(I_OBJ).attr("BTN_ETEK_SQL"))], function (result) {
        //呼叫AP FunGET_DB_SCHEMA 方法
        var oTAG = null;
        var oTABLE = null;
        var oTR = null;
        var oCLIENT = null;
        var chaFIELD_NAME = "";
        var chaFIELD_DATA = "";
        var COLS_RETURN_FIELD = null;
        var COLS_RETURN_CONTROL = null;
        var oETEK_BG_PARAM = {};
        //alert(result.value);

        //將回傳轉為 JSON 陣列
        eval("var JSON = " + result.value + ";");

        //取得 ETEK_BG 參考
        oCLIENT = $("#" + JSON["CLIENT_ID"])[0];

        //設定參數
        oETEK_BG_PARAM = {
            'CANCEL': false,
            "CANCEL_ALERT": false,
            "CLEAR_DATA": true,
            'OLD_VAL': $(oCLIENT).val(),
            "DATA_ROWS": JSON["DATA_ROWS"],
            'TABLE': JsFunFindParent(oCLIENT, "TABLE"),
            'ROW': JsFunFindParent(oCLIENT, "TR"),
            'FIELD': oCLIENT,
            'STYLE_M': ''
        };

        oETEK_BG_PARAM['STYLE_M'] = $(oETEK_BG_PARAM['TABLE']).attr('STYLE_M');

        //設定作用範圍, 單筆輸入作用範圍是 TABLE, 否則為 TR
        if (oETEK_BG_PARAM['STYLE_M'] == '01')
            oTAG = oETEK_BG_PARAM['TABLE']
        else
            oTAG = oETEK_BG_PARAM['ROW'];

        if (JSON["RETURN_FIELD"] != null) {
            COLS_RETURN_FIELD = JSON["RETURN_FIELD"].split(",");
            COLS_RETURN_CONTROL = JSON["RETURN_CONTROL"].split(",");
            for (var i = 0; i < COLS_RETURN_FIELD.length; i++) {
                var strKEY = COLS_RETURN_FIELD[i].toString();
                var strCONTROL = COLS_RETURN_CONTROL[i].toString();
                var strVALUE = JSON[strKEY];
                var oINPUTS = null;

                strCONTROL = strCONTROL.replace("txt", "") + "_" + JSON["M_SEQ_NO"];
                oINPUTS = $(oTAG).find(":input[name='" + strCONTROL + "']").eq(0);
                $(oINPUTS).val(strVALUE);
            }

            //觸發回傳事件
            if (typeof (JsFunEVENT_COMMAND) != undefined) {
                try {
                    JsFunEVENT_COMMAND("ETEK_BG_SELECT", { sender: oCLIENT, EventArgs: oETEK_BG_PARAM });
                } catch (ev) { }
            }

            //顯示查無資料
            if (oETEK_BG_PARAM['CANCEL_ALERT'] != true) {
                try {
                    if (oETEK_BG_PARAM['OLD_VAL'] != undefined && oETEK_BG_PARAM['OLD_VAL'] != '') {
                        if (JSON["DATA_ROWS"] == "0")
                            alert('[' + oETEK_BG_PARAM['OLD_VAL'] + '] ' + JsFunML(ML_JS_ETEK, "查無資料") + '!');
                    }
                } catch (e) { }
            }
        }


        //                //假設查無資料, 清除資料
        //                try {
        //                    if (oETEK_BG_PARAM['CLEAR_DATA'] == true && JSON["DATA_ROWS"] == "0")
        //                        $(oCLIENT).val('');
        //                } catch (e) { }
    });
}