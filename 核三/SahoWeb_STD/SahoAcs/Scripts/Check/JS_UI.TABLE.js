/*
=========================================================================================
// 設定事件(點選 TR)
=========================================================================================*/
function JsFunTR_EVENT_CREATE() {
    var oBODY = $("body");

    if ($(oBODY).attr('TR_EVENT_INIT') == undefined) {
        $(document).bind('mouseover.TR_EVENT, mousedown.TR_EVENT', function () {
            var oTR;
            var oTD;
            var strNEXT_URL;

            if (event.srcElement == undefined)
                return;

            if (event.srcElement.tagName == 'LABEL') {
                oTD = event.srcElement.parentNode;
            } else
                oTD = event.srcElement;

            switch (oTD.tagName) {
                case "TD":
                    oTR = oTD.parentNode;
                    strNEXT_URL = $(oTR).attr("NEXT_URL");

                    if (strNEXT_URL != undefined && strNEXT_URL != '') {
                        if (event.type == 'mousedown') {
                            var strPG_ID = $('input[name="PG_ID"]:eq(0)').val();
                            var strKEY_COL = $(oTR).find("input[name*='KEY_COL_']").val();

                            //顯示讀取中, 避免重複按
                            JsFunLOADING_SHOW(true);

                            if (strNEXT_URL.indexOf("?") >= 0)
                                window.location.href = strNEXT_URL + "&PG_ID=" + strPG_ID + "&KEY_COL=" + strKEY_COL;
                            else
                                window.location.href = strNEXT_URL + "?PG_ID=" + strPG_ID + "&KEY_COL=" + strKEY_COL;
                        } else {
                            $(oTD).css("cursor", "hand");
                        }
                    }
                    break;
            }
        });

        $(oBODY).attr('TR_EVENT_INIT', '1');
    }
}

///*
//=========================================================================================
////新增抬頭列
//=========================================================================================*/
//function JsFunTABLE_CREATE_PAGE_BAR_RESET(I_TABLE) {
//    $(I_TABLE).find("tr[PAGE_BAR]").remove();
//}
//function JsFunTABLE_PAGE_BAR_CREATE(I_TABLE) {
//    $(I_TABLE).each(function () {
//        //prepend 為將項目加到 TABLE 第一列
//        var HTML = "";
//        var TABLE = this;
//        var oPAGE_BAR = null;
//        var chaFORM = "#FORM_X";
//        var PAGE_INDEX = $(chaFORM).find(":input[name='PAGE_INDEX']").val();
//        var PAGE_COUNT = $(TABLE).attr("PAGE_COUNT");
//        var TABLE_ROWS = $(TABLE).attr("TABLE_ROWS");
//        var NEXT_URL = $(TABLE).attr("NEXT_URL");
//        var strTITLE = $(TABLE).attr('TITLE');
//        var oBAR_LEFT = $("<UL id='BAR_LEFT' style='position:absolute;padding:0;margin:-5px;' onclick='JsFunPAGE_BAR_COMMAND(this);' onmouseover='JsFunPAGE_BAR_COMMAND(this);' />");
//        var blnCHECK_ROW = false;
//        //
//        PAGE_INDEX = (PAGE_INDEX == undefined || PAGE_INDEX == '') ? '1' : PAGE_INDEX;

//        //不需要 PAGE_BAR
//        if ($(TABLE).attr("TOOL_BAR") == "NONE")
//            return;

//        //清除
//        JsFunTABLE_CREATE_PAGE_BAR_RESET(TABLE);

//        HTML = "<CAPTION class='PAGE_BAR'>&nbsp;</CAPTION>";

//        oPAGE_BAR = $(HTML);
//        $(oPAGE_BAR).append(oBAR_LEFT);
//        $(TABLE).prepend(oPAGE_BAR);

//        HTML = "";

//        //新增抬頭
//        if (strTITLE != '')
//            HTML += '<LI style="margin-left:2em;margin-right:2em;FONT-WEIGHT:bold;">【' + strTITLE + '】</LI>';

//        //新增目前資料筆數
//        if (TABLE_ROWS != undefined)
//            HTML += '<LI style=\'color:red\'>' + JsFunML(ML_JS_UI, "共") + ' ' + TABLE_ROWS + ' ' + JsFunML(ML_JS_UI, "筆") + '</LI>';

//        //如果有 checkbox
//        if ($(TABLE).has('[name*="CHK_COL_"]')) {
//            HTML += '<LI id=\'SEL_ALL\'>' + JsFunML(ML_JS_UI, "全部點選") + '</LI>';
//            HTML += '<LI id=\'SEL_NONE\'>' + JsFunML(ML_JS_UI, "取消點選") + '</LI>';
//        }

//        //如果需要[新增一筆]
//        if ($(TABLE).attr("ADD_ROW") == "1") {
//            HTML += '<LI id=\'SEL_ADD\'>' + JsFunML(ML_JS_UI, "新增一筆") + '</LI>';
//            blnCHECK_ROW = true;
//        }

//        //如果需要[複製一筆]
//        if ($(TABLE).attr("COPY_ROW") == "1") {
//            HTML += '<LI id=\'SEL_COPY\'>' + JsFunML(ML_JS_UI, "複製一筆") + '</LI>';
//            blnCHECK_ROW = true;
//        }

//        //快取一列
//        if (blnCHECK_ROW == true)
//            JsFunTABLE_ROW_CACHE(TABLE);

//        //
//        $(oBAR_LEFT).append(HTML);

//        //新增自訂按鈕, 如果有宣告的話
//        if (typeof (JsFunEVENT_COMMAND) == 'function')
//            JsFunEVENT_COMMAND("PAGE_BAR_BUTTON_CREATED", { sender: TABLE, EventArgs: $(oPAGE_BAR).find("#BAR_LEFT") });

//        //如果需要換頁
//        if (TABLE_ROWS != undefined) {
//            HTML = '';
//            HTML += '<LI>' + JsFunML(ML_JS_UI, "到") + ' ';
//            HTML += '<SELECT>';
//            for (var i = 0; i < PAGE_COUNT; i++) {
//                if (i == PAGE_INDEX - 1)
//                    HTML += "<OPTION selected>" + (i + 1) + "</OPTION>";
//                else
//                    HTML += "<OPTION>" + (i + 1) + "</OPTION>";
//            }
//            HTML += '</SELECT>';
//            HTML += ' ' + JsFunML(ML_JS_UI, "頁") + ' / ' + JsFunML(ML_JS_UI, "共") + ' ' + PAGE_COUNT + ' ' + JsFunML(ML_JS_UI, "頁") + '</LI>';
//            {
//                if (PAGE_INDEX != "1")
//                    HTML += '<LI id=\'PAGE_BEGIN\'>' + JsFunML(ML_JS_UI, "第一頁") + '</LI>';

//                if (PAGE_INDEX != "1")
//                    HTML += '<LI id=\'PAGE_LAST\'>' + JsFunML(ML_JS_UI, "上一頁") + '</LI>';

//                if (PAGE_INDEX != PAGE_COUNT)
//                    HTML += '<LI id=\'PAGE_NEXT\'>' + JsFunML(ML_JS_UI, "下一頁") + '</LI>';

//                if (PAGE_INDEX != PAGE_COUNT)
//                    HTML += '<LI id=\'PAGE_END\'>' + JsFunML(ML_JS_UI, "最後頁") + '</LI>';
//            }
//            $(oBAR_LEFT).append(HTML);

//            //            //設定排序
//            //            $(TABLE).find('tr[UI-WIDGET-HEADER=""] LABEL[SORT]').each(function (index) {
//            //                $(this).css("cursor", "hand");

//            //                $(this).bind("click", function (index) {
//            //                    var chaSORT = $(this).attr("SORT");

//            //                    //如果沒有排序項目, 補上
//            //                    if ($(chaFORM).find(":input[name='ORDER_COL']").length == 0) {
//            //                        $(chaFORM).append("<INPUT type='hidden' name='ORDER_COL' />");
//            //                        $(chaFORM).append("<INPUT type='hidden' name='R1' value='V2'");
//            //                    }

//            //                    //設定排序欄位與遞增方式
//            //                    $(":input[name='ORDER_COL']:eq(0)").val(chaSORT);
//            //                    $(":input[name='R1']").val(function () {
//            //                        return ($(this).val() == "V2") ? "V1" : "V2";
//            //                    });

//            //                    //傳送變更
//            //                    $(chaFORM).submit();
//            //                });
//            //            });
//        }

//        //設定下拉換頁
//        $(oPAGE_BAR).find("SELECT").each(function (index) {
//            $(this).bind({
//                change: function () {
//                    $(":input[name='PAGE_INDEX']").val($(this).val());
//                    $(chaFORM).submit();
//                }
//            });
//        });

//        //設定按鈕樣式
//        $(oPAGE_BAR).find("#BAR_LEFT > li").each(function (index) {
//            if ($(this).attr("id")) {
//                $(this).css({
//                    "cursor": "hand"
//                });
//            }

//            if (index > 0) {
//                $(this).css({
//                    "border-left": "1px solid #666666"
//                });
//            }
//        });
//    });
//}

/*
=========================================================================================
//新增抬頭列
=========================================================================================*/
function JsFunTABLE_CREATE_PAGE_BAR_RESET(I_TABLE) {
    $(I_TABLE).find("tr[PAGE_BAR]").remove();
}
function JsFunTABLE_PAGE_BAR_CREATE(I_TABLE) {
    $(I_TABLE).each(function () {
        //prepend 為將項目加到 TABLE 第一列
        var HTML = "";
        var TABLE = this;
        var oPAGE_BAR = null;
        var chaFORM = "#FORM_X";
        var PAGE_INDEX = $(chaFORM).find(":input[name='PAGE_INDEX']").val();
        var PAGE_COUNT = $(TABLE).attr("PAGE_COUNT");
        var TABLE_ROWS = $(TABLE).attr("TABLE_ROWS");
        var NEXT_URL = $(TABLE).attr("NEXT_URL");
        var strTITLE = $(TABLE).attr('TITLE');
        var oBAR_LEFT = $("<DIV class='slick-header ui-state-default' style='width:100%' onclick='JsFunPAGE_BAR_COMMAND(this);' onmouseover='JsFunPAGE_BAR_COMMAND(this);' />");
        var blnCHECK_ROW = false;
        //
        PAGE_INDEX = (PAGE_INDEX == undefined || PAGE_INDEX == '') ? '1' : PAGE_INDEX;

        //不需要 PAGE_BAR
        if ($(TABLE).attr("TOOL_BAR") == "NONE")
            return;

        //清除
        JsFunTABLE_CREATE_PAGE_BAR_RESET(TABLE);

        HTML = "<CAPTION class='PAGE_BAR'>&nbsp;</CAPTION>";

        oPAGE_BAR = $(HTML);
        $(oPAGE_BAR).append(oBAR_LEFT);
        $(TABLE).prepend(oPAGE_BAR);

        HTML = "";

        //新增抬頭
        if (strTITLE != '')
            HTML += '<DIV style="margin-left:2em;margin-right:2em;FONT-WEIGHT:bold;">【' + strTITLE + '】</DIV>';

        //新增目前資料筆數
        if (TABLE_ROWS != undefined)
            HTML += '<DIV style=\'color:red\'>' + JsFunML(ML_JS_UI, "共") + ' ' + TABLE_ROWS + ' ' + JsFunML(ML_JS_UI, "筆") + '</DIV>';

        //如果有 checkbox
        if ($(TABLE).has('[name*="CHK_COL_"]')) {
            HTML += '<DIV id=\'SEL_ALL\'>' + JsFunML(ML_JS_UI, "全部點選") + '</DIV>';
            HTML += '<DIV id=\'SEL_NONE\'>' + JsFunML(ML_JS_UI, "取消點選") + '</DIV>';
        }

        //如果需要[新增一筆]
        if ($(TABLE).attr("ADD_ROW") == "1") {
            HTML += '<DIV id=\'SEL_ADD\'>' + JsFunML(ML_JS_UI, "新增一筆") + '</DIV>';
            blnCHECK_ROW = true;
        }

        //如果需要[複製一筆]
        if ($(TABLE).attr("COPY_ROW") == "1") {
            HTML += '<DIV id=\'SEL_COPY\'>' + JsFunML(ML_JS_UI, "複製一筆") + '</DIV>';
            blnCHECK_ROW = true;
        }

        //快取一列
        if (blnCHECK_ROW == true)
            JsFunTABLE_ROW_CACHE(TABLE);

        //
        $(oBAR_LEFT).append(HTML);

        //新增自訂按鈕, 如果有宣告的話
        if (typeof (JsFunEVENT_COMMAND) == 'function')
            JsFunEVENT_COMMAND("PAGE_BAR_BUTTON_CREATED", { sender: TABLE, EventArgs: oBAR_LEFT });

        //如果需要換頁
        if (TABLE_ROWS != undefined) {
            HTML = '';
            HTML += '<DIV>' + JsFunML(ML_JS_UI, "到") + ' ';
            //            HTML += '<SELECT>';
            //            for (var i = 0; i < PAGE_COUNT; i++) {
            //                if (i == PAGE_INDEX - 1)
            //                    HTML += "<OPTION selected>" + (i + 1) + "</OPTION>";
            //                else
            //                    HTML += "<OPTION>" + (i + 1) + "</OPTION>";
            //            }
            //            HTML += '</SELECT>';
            HTML += ' ' + JsFunML(ML_JS_UI, "頁") + ' / ' + JsFunML(ML_JS_UI, "共") + ' ' + PAGE_COUNT + ' ' + JsFunML(ML_JS_UI, "頁") + '</DIV>';
            {
                if (PAGE_INDEX != "1")
                    HTML += '<DIV id=\'PAGE_BEGIN\'>' + JsFunML(ML_JS_UI, "第一頁") + '</DIV>';

                if (PAGE_INDEX != "1")
                    HTML += '<DIV id=\'PAGE_LAST\'>' + JsFunML(ML_JS_UI, "上一頁") + '</DIV>';

                if (PAGE_INDEX != PAGE_COUNT)
                    HTML += '<DIV id=\'PAGE_NEXT\'>' + JsFunML(ML_JS_UI, "下一頁") + '</DIV>';

                if (PAGE_INDEX != PAGE_COUNT)
                    HTML += '<DIV id=\'PAGE_END\'>' + JsFunML(ML_JS_UI, "最後頁") + '</DIV>';
            }
            $(oBAR_LEFT).append(HTML);

            //            //設定排序
            //            $(TABLE).find('tr[UI-WIDGET-HEADER=""] LABEL[SORT]').each(function (index) {
            //                $(this).css("cursor", "hand");

            //                $(this).bind("click", function (index) {
            //                    var chaSORT = $(this).attr("SORT");

            //                    //如果沒有排序項目, 補上
            //                    if ($(chaFORM).find(":input[name='ORDER_COL']").length == 0) {
            //                        $(chaFORM).append("<INPUT type='hidden' name='ORDER_COL' />");
            //                        $(chaFORM).append("<INPUT type='hidden' name='R1' value='V2'");
            //                    }

            //                    //設定排序欄位與遞增方式
            //                    $(":input[name='ORDER_COL']:eq(0)").val(chaSORT);
            //                    $(":input[name='R1']").val(function () {
            //                        return ($(this).val() == "V2") ? "V1" : "V2";
            //                    });

            //                    //傳送變更
            //                    $(chaFORM).submit();
            //                });
            //            });
        }

        //        //設定下拉換頁
        //        $(oPAGE_BAR).find("SELECT").each(function (index) {
        //            $(this).bind({
        //                change: function () {
        //                    $(":input[name='PAGE_INDEX']").val($(this).val());
        //                    $(chaFORM).submit();
        //                }
        //            });
        //        });

        //設定按鈕樣式
        $(oBAR_LEFT).find("div").each(function (index) {
            if ($(this).attr("id")) {
                $(this).css({
                    "cursor": "hand"
                }).addClass('slick-cell');
            }

//            if (index > 0) {
//                $(this).css({
//                    "border-left": "1px solid #666666"
//                });
//            }
        });


    });
}

/*
=========================================================================================
//[PAGE_BAR]點選
=========================================================================================*/
function JsFunPAGE_BAR_COMMAND(I_OBJ) {
    var chaFORM = "#FORM_X";
    var srcElement = $(event.srcElement);
    var srcID = $(srcElement).attr('id');

    if (srcID == undefined || srcID == '')
        return;

    //alert(event.type);
    switch (event.type) {
        case "mouseover":
            if ($(I_OBJ).prop('INIT') == undefined) {
                $(I_OBJ).prop('INIT', true);
                $(I_OBJ).find("li[id]").each(function (index) {
                    $(this).hover(
                    function () {
                        $(this).addClass('ui-state-hover');
                    },
                    function () {
                        $(this).removeClass('ui-state-hover');
                    }
                );
                });
                $(srcElement).trigger('mouseenter');
            }
            break;

        case "click":
            {
                var oTABLE = JsFunFindParent(srcElement[0], "TABLE");
                var objPAGE_INDEX = $(chaFORM).find(":input[name='PAGE_INDEX']");
                var intPAGE_INDEX = Number($(objPAGE_INDEX).val());
                var PAGE_COUNT = $(oTABLE).attr("PAGE_COUNT");

                //觸發按鈕事件[開始]
                //觸發按鈕事件, 如果回傳為 true, 就不執行預設按鈕事件
                //備註:(當自訂按鈕行為時, 如果不想要執行預設的方法, 請回傳 true)
                if (typeof (JsFunEVENT_COMMAND) == 'function') {
                    if (JsFunEVENT_COMMAND('PAGE_BAR_BUTTON_ONCLICK_B', { sender: srcElement, EventArgs: oTABLE }) == true)
                        return;
                }

                switch (srcID) {
                    case "PAGE_BEGIN":
                        //第一頁
                        $(objPAGE_INDEX).val(1);
                        $(chaFORM).submit();
                        break;

                    case "PAGE_LAST":
                        //上一頁
                        $(objPAGE_INDEX).val(intPAGE_INDEX - 1);
                        $(chaFORM).submit();
                        break;

                    case "PAGE_NEXT":
                        //下一頁
                        $(objPAGE_INDEX).val(intPAGE_INDEX + 1);
                        $(chaFORM).submit();
                        break;

                    case "PAGE_END":
                        //最後頁
                        $(objPAGE_INDEX).val(PAGE_COUNT);
                        $(chaFORM).submit();
                        break;

                    case "SEL_ALL":
                        //全部點選
                        $(oTABLE).find('[name*="CHK_COL_"]:not(:disabled)').attr("checked", true);
                        break;

                    case "SEL_NONE":
                        //取消點選
                        $(oTABLE).find('[name*="CHK_COL_"]:not(:disabled)').attr("checked", false);
                        break;

                    case "SEL_ADD":
                        //新增一筆
                        JsFunTABLE_ROW_ADD(srcElement[0]);
                        break;

                    case "SEL_COPY":
                        //複製一筆
                        JsFunTABLE_ROW_COPY(srcElement[0]);
                        break;
                }

                //觸發按鈕事件[結束]
                if (typeof (JsFunEVENT_COMMAND) == 'function')
                    JsFunEVENT_COMMAND('PAGE_BAR_BUTTON_ONCLICK_E', { sender: srcElement, EventArgs: oTABLE });
            }
            break;
    }
}

/*
=========================================================================================
// 快取 TABLE 一列, 當新增一列時使用
=========================================================================================*/
function JsFunTABLE_ROW_CACHE(I_TABLE) {
    //將有新增一筆的最後一列快取
    $(I_TABLE).each(function () {
        var oTABLE = this;
        var NEW_ROW = JsFunGET_PARAM('NEW_ROW');
        var strM_SEQ_NO = $(oTABLE).attr('M_SEQ_NO');
        var strGROUP_NUM = $(oTABLE).attr('GROUP_NUM');
        var strGROUP_KEY = strM_SEQ_NO + "_" + strGROUP_NUM;
        var oTR;

        if (NEW_ROW[strGROUP_KEY] == undefined) {
            oTR = $($(oTABLE).prop('rows')).eq(-1);

            //設定序號
            $(oTABLE).attr('ROW_NUM', $(oTR[0].childNodes[0]).text());
            $(oTABLE).attr('CHK_NUM', $(oTR[0].childNodes[1]).find(':input[name="CHK_COL_' + strM_SEQ_NO + '"]').val());

            oTR = $(oTR[0].outerHTML);
            if ($(oTR).length > 0) {
                $(oTR).find(":input").each(function (index) {
                    var strDEFAULTVALUE = $.trim($(this).attr("DEFAULT_VALUE"));

                    //清空值
                    $(this).val(strDEFAULTVALUE);
                });

                NEW_ROW[strGROUP_KEY] = oTR[0].outerHTML;
            }
        }
    });
}

/*
=========================================================================================
// 新增一筆
=========================================================================================*/
function JsFunTABLE_ROW_ADD(I_OBJ) {
    var oOBJ = $(I_OBJ)[0];
    var oTABLE = JsFunFindParent(oOBJ, "TABLE");
    var oTR = null;
    var strM_SEQ_NO = $(oTABLE).attr("M_SEQ_NO");
    var strGROUP_NUM = "";
    var strGROUP_KEY = "";

    if ($(oTABLE).length > 0) {
        var oCHK_COL = null;
        var numCHK_COL = 0;
        var numINDEX = 0;

        //取得TABLE 參考
        strM_SEQ_NO = $(oTABLE).attr("M_SEQ_NO");
        strGROUP_NUM = $(oTABLE).attr('GROUP_NUM');
        strGROUP_KEY = strM_SEQ_NO + "_" + strGROUP_NUM;

        //複製表格最後一列
        oTR = $(JsFunGET_PARAM('NEW_ROW')[strGROUP_KEY]);

        //序號 +1
        var CHK_NUM = Number($(oTABLE).attr('CHK_NUM')) + 1;
        var ROW_NUM = Number($(oTABLE).attr('ROW_NUM')) + 1;
        $(oTABLE).attr('CHK_NUM', CHK_NUM);
        $(oTABLE).attr('ROW_NUM', ROW_NUM);

        //重設複製列內容, 將複製的欄位值清空, 並變更ID代號
        $(oTR).find(":input").each(function (index) {
            var strVALUE = $(this).val();
            var strNAME = $(this).attr("name");
            strNAME = (strNAME == undefined) ? '' : strNAME;

            //設定 ID
            $(this).attr("id", JsFunGetRound() + '_' + strNAME);

            //設定 CHK_COL
            oCHK_COL = $(oTR[0].childNodes[1]).find(':input[name="CHK_COL_' + strM_SEQ_NO + '"]');
            if ($(oCHK_COL).length > 0) {
                $(oCHK_COL).prop('checked', true);
                $(oCHK_COL).val(CHK_NUM);
                $(oTR[0].childNodes[0]).text(ROW_NUM);
            }
        });

        //處理 TAB 分頁標籤
        $(oTR).find('DIV[class*="ui-tabs"]').each(function () {
            $(this).find('DIV[TAB_GROUP]').each(function () {
                var strGP_ID = $(this).attr('id');
                var newGP_ID = "GP_" + JsFunGetRound();

                //新增 TAB 標籤
                var oA = $(oTR).find("A[href$='" + strGP_ID + "']");
                var strHREF = $(oA).attr('href');

                $(this).attr('id', newGP_ID);
                $(oA).attr('href', strHREF.replace(strGP_ID, newGP_ID));
            });
        }).removeClass("ui-tabs ui-widget ui-widget-content ui-corner-all").tabs();

        //將資料列新增至 TABLE
        $(oTR).appendTo(oTABLE);

        //新增按鈕新增後事件, 如果有宣告的話
        if (typeof (JsFunEVENT_COMMAND) == 'function')
            JsFunEVENT_COMMAND("TABLE_ROW_ADD", { sender: this, EventArgs: oTR });

        //初始化控制項
        JsFunREF_INIT(oTR);
    }

    return oTR;
}

/*
=========================================================================================
// 複製
=========================================================================================*/
function JsFunTABLE_ROW_COPY(I_OBJ) {
    var oTR = JsFunTABLE_ROW_ADD(I_OBJ);
    var COLS_INPUT = $(oTR).prev().find(':input');

    $(oTR).find(':input:not([name*="CHK_COL_"], [name*="KEY_COL_"])').each(function () {
        var oINPUT = this;

        $(COLS_INPUT).each(function () {
            if ($(oINPUT).attr('name') == $(this).attr('name')) {
                $(oINPUT).val($(this).val());
                return;
            }
        });
    });

    //新增按鈕新增後事件, 如果有宣告的話
    if (typeof (JsFunEVENT_COMMAND) == 'function')
        JsFunEVENT_COMMAND("TABLE_ROW_COPY", { sender: this, EventArgs: oTR });

}