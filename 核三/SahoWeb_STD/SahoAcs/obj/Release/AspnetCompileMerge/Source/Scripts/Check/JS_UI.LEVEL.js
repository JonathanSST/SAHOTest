/*
=========================================================================================
//[階層]建立
=========================================================================================*/
function JsFunLEVEL_BUTTON_CREATE(I_ROOT) {
    var oBUT = null;
    var OP_PARAM = JsFunGET_PARAM('LEVEL');
    var LV_PARAM = null;
    var oTABLE = null;
    var oTR = null;

    //取階層設定, 並將階層列快取
    $(I_ROOT).find("table[LV_OP]").each(function () {
        oTABLE = this;

        $(oTABLE).css('visibility', 'hidden');
        var M_SEQ_NO = $(oTABLE).attr('M_SEQ_NO');
        var oLV_OP = $(oTABLE).attr('LV_OP').split('^');
        var LV_NO = oLV_OP[0];
        var LV_KEY_FIELD = oLV_OP[1] + "_" + LV_NO;
        var TR_KEY_FIELD = oLV_OP[1] + "_" + M_SEQ_NO;

        if (OP_PARAM[LV_NO] == undefined)
            OP_PARAM[LV_NO] = {};

        if (OP_PARAM[LV_NO][M_SEQ_NO] == undefined)
            OP_PARAM[LV_NO][M_SEQ_NO] = { KEY_FIELD: LV_KEY_FIELD };

        //快取相同 KEY_FIELD 的資料列 
        $(oTABLE).find(':input[name="' + TR_KEY_FIELD + '"]').each(function () {
            var TR_KEY_VAL = $(this).val();

            if (TR_KEY_VAL != '') {
                if (OP_PARAM[LV_NO][M_SEQ_NO][TR_KEY_VAL] == undefined)
                    OP_PARAM[LV_NO][M_SEQ_NO][TR_KEY_VAL] = new Array();

                oTR = JsFunFindParent(this, "TR");
                OP_PARAM[LV_NO][M_SEQ_NO][TR_KEY_VAL].push(oTR);
            }
        });
    });

    //建立階層按鈕
    for (var o in OP_PARAM) {
        $(I_ROOT).find('table[M_SEQ_NO="' + o + '"]').find(':checkbox').each(function () {
            //尋找階層按鈕
            var oCHECKBOX = this;

            //$(oCHECKBOX).attr('onchange', 'JsFunLEVEL_SET_CHECKED(this);');
            //            $(oCHECKBOX).bind("change", function () {
            //                JsFunLEVEL_SET_CHECKED(this);
            //            });

            oBUT = $(oCHECKBOX).parent().find('input[LIST_TYPE="CIRCLE_PLUS"]');
            if ($(oBUT).length == 0) {
                //如果沒有就新增
                oBUT = JsFunCREATE_BUTTON("CIRCLE_PLUS", "");
                $(this).parent().append(oBUT);
            }

            //設定 checkbox
            var oOBJ = JsFunATTR_BIND(oCHECKBOX, 'onclick', "JsFunLEVEL_SET_CHECKED(this);", false);
            this.outerHTML = oOBJ.outerHTML;

        });
    }
}

/*
=========================================================================================
//當階層選擇時處理
=========================================================================================*/
function JsFunLEVEL_SET_CHECKED(I_THIS) {
    var oTR = JsFunFindParent(I_THIS, 'TR');
    var oBUT = JsFunLEVEL_GET_BUTTON(oTR);
    var strVALUE = $(I_THIS).prop('checked');

    if (JsFunLEVEL_GET_BUTTON_STATUS(oBUT))
        $(oTR).next().find(':checkbox[name*="CHK_COL_"]').prop('checked', strVALUE);
    else
        $(oTR).find(':checkbox[name*="CHK_COL_"]').prop('checked', strVALUE);
}

/*
=========================================================================================
//取得[階層]按鈕
=========================================================================================*/
function JsFunLEVEL_GET_BUTTON(I_TR) {
    var I_BUT = $(I_TR).find('input[LIST_TYPE="CIRCLE_PLUS"]:eq(0)');
    return I_BUT;
}

/*
=========================================================================================
//取得[階層]按鈕狀態
=========================================================================================*/
function JsFunLEVEL_GET_BUTTON_STATUS(I_BUTTON) {
    var blnOPEN = false;

    if ($(I_BUTTON).hasClass('ui-icon-circle-minus'))
        blnOPEN = true;

    return blnOPEN;
}
/*
=========================================================================================
//新增
=========================================================================================*/
function JsFunLEVEL_ROW_ADD(I_ROOT) {
    var oBUT = null;
    var OP_PARAM = JsFunGET_PARAM('LEVEL');
    var LV_PARAM = null;
    var oTABLE = null;
    var oTR = null;

    for (var i in OP_PARAM) {
        $(I_ROOT).find('table[M_SEQ_NO="' + i + '"]').find('input[LIST_TYPE="CIRCLE_PLUS"]').each(function () {
            //尋找階層按鈕
            oTR = JsFunFindParent(this, 'TR');
            JsFunLEVEL_CHILD_ADD(oTR);
        });
    }
}

/*
=========================================================================================
//新增
=========================================================================================*/
function JsFunLEVEL_CHILD_ADD(I_TR) {
    var oTR = I_TR;
    var oTD = $($(oTR).prop('cells')).eq(-1);
    var OP_PARAM = JsFunGET_PARAM('LEVEL');
    var strKEY_VAL = "";
    var oDIV = $(oTD).find('LABEL[PLUS=""] > DIV:eq(0)');

    if (oDIV.length == 0) {
        var oLABEL = $('<LABEL PLUS="" style="position:absolute;visibility:hidden" />');
        oDIV = $('<DIV class="BLOCK_T10" />');
        $(oLABEL).append(oDIV);
        $(oTD).append(oLABEL);
    }

    //尚未初始化
    if ($(oDIV).prop('INIT') != true) {
        var oTABLE = JsFunFindParent(I_TR, "TABLE"); ;
        var M_SEQ_NO = $(oTABLE).attr("M_SEQ_NO");
        for (var i in OP_PARAM[M_SEQ_NO]) {
            var oNEW_TABLE = null;
            var LV_PARAM = OP_PARAM[M_SEQ_NO][i];

            //建立空白容器
            if (LV_PARAM["COL"] == undefined) {
                oNEW_TABLE = $($('table[M_SEQ_NO="' + i + '"]')[0].outerHTML)
                        .find('tr:has(:checkbox)').remove().end().css({ 'visibility': '', 'width': '' });

                LV_PARAM["COL"] = oNEW_TABLE[0].outerHTML;
            }

            oNEW_TABLE = $(LV_PARAM["COL"]);
            strKEY_VAL = $(oTR).find('[name="' + LV_PARAM["KEY_FIELD"] + '"]:eq(0)').val();

            if (LV_PARAM[strKEY_VAL] != undefined) {
                for (var j = 0; j < LV_PARAM[strKEY_VAL].length; j++) {
                    var oLEV_TR = LV_PARAM[strKEY_VAL][j];
                    $(oNEW_TABLE).append(oLEV_TR);

                    //設定序號
                    $(oLEV_TR).find('td:eq(0)').text(j + 1);
                    if (j == LV_PARAM[strKEY_VAL].length - 1) {
                        $(oNEW_TABLE).attr('CHK_NUM', j);
                        $(oNEW_TABLE).attr('ROW_NUM', j + 1);
                    }
                }
            }

            $(oDIV).append(oNEW_TABLE);
        }
        $(oDIV).prop('INIT', true);
    }

    return oDIV;
}

/*
=========================================================================================
//[階層]開啟或關閉
=========================================================================================*/
function JsFunCIRCLE_PLUS(I_BUTTON) {
    var M_SEQ_NO = "";
    var oTABLE = null;
    var oTR = JsFunFindParent(I_BUTTON, 'TR');
    var oDIV = null;
    var oTD = $($(oTR).prop('cells')).eq(-1);
    var oTR_T = null;
    var oTD_T = null;

    if ($(I_BUTTON).hasClass('ui-icon-circle-plus')) {
        //開啟
        $(I_BUTTON).removeClass('ui-icon-circle-plus');
        $(I_BUTTON).addClass('ui-icon-circle-minus');
        oDIV = JsFunLEVEL_CHILD_ADD(oTR);
        $(oDIV).css('visibility', 'visible');

        oTR_T = $('<TR></TR>');
        //oTD_T = $('<TD colspan="999" style="background-color:brown;"></TD>');
        oTD_T = $('<TD colspan="999" style="background-color:#999999;"></TD>');
        $(oTR_T).append(oTD_T);
        $(oTD_T).append(oDIV);
        $(oTR).after(oTR_T);
    } else {
        //關閉
        $(I_BUTTON).removeClass('ui-icon-circle-minus');
        $(I_BUTTON).addClass('ui-icon-circle-plus');

        oTR_T = $(oTR).next();
        $(oTD).find('LABEL[PLUS=""]').append($(oTR_T).find('DIV:eq(0)').css('visibility', 'hidden'));
        $(oTR_T).remove();
    }
}