/*
=========================================================================================
// 產生 TAB BAR
=========================================================================================*/
function JsFunTAB_BAR_CREATE(I_OBJ) {
    var oCONTENT_TAB = null;
    var COLS_PARAM = new Array();

    //假設 TAB BAR 不存在, 產生
    oCONTENT_TAB = $("#CONTENT_TAB");
    if ($(oCONTENT_TAB).length == 0) {
        oCONTENT_TAB = $("<DIV id='CONTENT_TAB' class='BLOCK_T10' />");
        $("#CONTENT_PAGE").append(oCONTENT_TAB);
    }

    //
    $(I_OBJ).find('[TAB_NAME]').each(function (index) {
        COLS_PARAM.push({ TAB_NAME: $(this).attr("TAB_NAME"), COL: this });
    });

    //建立 TAB BAR
    JsFunTAB_BAR_INIT(oCONTENT_TAB, COLS_PARAM);
}

/*
=========================================================================================
說明 : 建立 TAB BAR
參數 : 
I_DIV : 要建立的根DIV
I_COLS_PARAM : 參數
I_COLS_PARAM[0][TAB_NAME] : 分頁名稱
I_COLS_PARAM[0][COL] : 分頁內容
=========================================================================================*/
function JsFunTAB_BAR_INIT(I_DIV, I_COLS_PARAM) {
    var oCONTENT_TAB = I_DIV;
    var oCOL = null;
    var oUL = null;
    var oTAB_GROUP = null;
    var strTAB_ID = "";
    var strTAB_NAME = "";

    //取得[頁簽標頭]
    oUL = $(oCONTENT_TAB).find('UL[TAB_NAV]');
    if ($(oUL).length == 0) {
        oUL = $("<UL TAB_NAV='' />");
        $(oCONTENT_TAB).append(oUL);
    }

    $(I_COLS_PARAM).each(function (i) {
        strTAB_ID = JsFunGetRound();
        strTAB_NAME = I_COLS_PARAM[i]["TAB_NAME"];
        oCOL = I_COLS_PARAM[i]["COL"];

        //取得[頁簽內容]
        oTAB_GROUP = $(oCONTENT_TAB).find("DIV[TAB_GROUP='" + strTAB_NAME + "']");
        if ($(oTAB_GROUP).length == 0) {
            oTAB_GROUP = $("<DIV id='GP_" + strTAB_ID + "' TAB_GROUP='" + strTAB_NAME + "' style='margin:0;padding:0;' />");

            //新增 TAB 標籤
            $(oUL).append("<LI><A href='#GP_" + strTAB_ID + "'>" + strTAB_NAME + "</A></LI>");

            //新增 TAB 內容
            $(oCONTENT_TAB).append(oTAB_GROUP);
        }

        if ($(oCOL).prop('offsetWidth') > $('body').prop('offsetWidth'))
            $(oCOL).css('width', 'auto');

        $(oTAB_GROUP).append(oCOL);
    });

    //產生 TAB BAR
    if ($(oUL).find('LI').length > 0) {
        if ($(oCONTENT_TAB).prop('INIT') != true) {
            $(oCONTENT_TAB).tabs();
            $(oCONTENT_TAB).prop('INIT', true);
        }
    }
}