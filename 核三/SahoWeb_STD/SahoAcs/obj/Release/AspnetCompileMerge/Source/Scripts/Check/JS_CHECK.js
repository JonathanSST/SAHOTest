var ML_JS_CHECK = {};
//ML_JS_CHECK
ML_JS_CHECK["天"] = '天';
ML_JS_CHECK["日期格式錯誤，應為 YYYY(西元年)/MM/DD"] = '日期格式錯誤，應為 YYYY(西元年)/MM/DD';
ML_JS_CHECK["該月僅有"] = '該月僅有';
ML_JS_CHECK["輸入YYYY(西元年)/MM/DD"] = '輸入YYYY(西元年)/MM/DD';
ML_JS_CHECK["日期需輸入西元年格式"] = '日期需輸入西元年格式';
ML_JS_CHECK["西元年範圍需大於 1911 年"] = '西元年範圍需大於 1911 年';
ML_JS_CHECK["注意！ *  欄位不能為空"] = '注意！ *  欄位不能為空';
ML_JS_CHECK["注意！欄位是數字型態"] = '注意！欄位是數字型態';
ML_JS_CHECK["注意！日期有誤，終止日期不能小於起始日期"] = '注意！日期有誤，終止日期不能小於起始日期';
ML_JS_CHECK["注意！此欄位數字太大"] = '注意！此欄位數字太大';
ML_JS_CHECK["注意！此欄位數字太小"] = '注意！此欄位數字太小';


////////////////////////////////////////////////////////////
// 說明 : 檢查輸入字串是否為空值
////////////////////////////////////////////////////////////
function JsFunCheckNull(strVALUE) {
    if (strVALUE.length == 0) {
        return false;
    }
    return true;
}

////////////////////////////////////////////////////////////
// 說明 : 檢查輸入字串是否為數字型態
////////////////////////////////////////////////////////////
function JsFunCheckNum(strVALUE) {
    if (isNaN(strVALUE)) {
        return false;
    }
    return true;
}

////////////////////////////////////////////////////////////
// 說明 : 檢查輸入字串是否為數字型態及其範圍
// 輸入 : 文字方塊物件，設定之最小值，設定之最大值
// 輸出 : 如果文字方塊物件內的資料之數字小於所設定之最小值或大於所設定之最大值，則回傳 false，否則回傳 true。
// 範例 :
// if ( !JsFunCheckNumRange(document.forms[0].FrmAmt, 0, 30000) ) {
//     return false;
// }
// 作著 : Robby Lee, Angus
// 日期 : 2001/04/16, 2006/08/21
////////////////////////////////////////////////////////////
function JsFunCheckNumRange(I_VAL, minNumber, maxNumber) {
    if (isNaN(I_VAL)) {
        alert(JsFunML(ML_JS_CHECK, "注意！此欄位是數字型態") + '！');
        return false;
    } else {
        if (I_VAL > maxNumber) {
            alert(JsFunML(ML_JS_CHECK, "注意！此欄位數字太大") + '！');
            return false;
        } else {
            if (I_VAL < minNumber) {
                alert(JsFunML(ML_JS_CHECK, "注意！此欄位數字太小") + '！');
                return false;
            }
        }
    }

    return true;
}

////////////////////////////////////////////////////////////
// 說明 : 檢查輸入字串終止日期不得小於起始日期
// 輸入 : 文字方塊物件
// 輸出 : 如果文字方塊物件內的字串終止日期小於起始日期，則回傳 false，否則回傳 true。
// 範例 :
// if ( !JsFunCHECK_BET_DATE('2011/01/01','2010/01/01') ) {
//     return false;
// }
////////////////////////////////////////////////////////////
function JsFunCHECK_BET_DATE(I_DATE_B, I_DATE_E) {
    DATE_B = new Date(I_DATE_B);
    DATE_E = new Date(I_DATE_E);

    if (DATE_B.getTime() > DATE_E.getTime())
        return true;

    //alert(JsFunML(ML_JS_CHECK, "注意！日期有誤，終止日期不能小於起始日期"));
    return false;
}

/*
////////////////////////////////////////////////////////////
// 說明 : 檢查輸入字串是否為日期型態(YYYY/MM/DD)
// 輸入 : 字串
// 輸出 : 物件
STATUS : 如果文字方塊物件內的資料之日期格式不為 YYYY/MM/DD 格式，則回傳 false，否則回傳 true。
ERROR : 錯誤訊息
VALUE : 回傳值
////////////////////////////////////////////////////////////*/
function JsFunCheckDate(strVALUE) {
    var str_i = strVALUE;
    var str_p = "YYYY/MM/DD";
    var strERROR = "";

    p_element = new Array(3);
    i_element = new Array(3);
    reg_year = /yyyy|yyy|yy|rr/i;
    reg_ycnt = /y|r/gi;
    arr_mon = ["", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    reg_mon = /Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec/i;
    reg_month = /mon/i;
    reg_s = /\-|\/|\,|\./;
    reg_ss = /\-|\/|\,|\./g;

    if (strVALUE.length == 0) {
        return {
            STATUS: true,
            ERROR: '',
            VALUE: strVALUE
        };
    }
    else {
        // 判斷是否為8碼
        if (strVALUE.length == 8) {
            if (isNaN(strVALUE)) {
                window.status = JsFunML(ML_JS_CHECK, "輸入YYYY(西元年)/MM/DD");
            } else {
                datY = str_i.substr(0, 4);
                datM = str_i.substr(4, 2);
                datD = str_i.substr(6, 2);
                datYMD = datY + '/' + datM + '/' + datD;
                str_i = datYMD;
            }
        }

        tmpDate = str_i.split("/");
        tmpYearLen = tmpDate[0].length;
        if (tmpYearLen - 4 != 0) {
            return {
                STATUS: false,
                ERROR: JsFunML(ML_JS_CHECK, "日期需輸入西元年格式") + '!',
                VALUE: strVALUE
            };
        }
        else {
            if (tmpDate[0] - 1911 < 0) {
                return {
                    STATUS: false,
                    ERROR: JsFunML(ML_JS_CHECK, "西元年範圍需大於 1911 年") + '!',
                    VALUE: strVALUE
                };
            }
        }
    }

    if (str_p.match(reg_month)) {
        if (!str_i.match(reg_mon)) {
            return {
                STATUS: false,
                ERROR: JsFunML(ML_JS_CHECK, "日期格式錯誤，應為 YYYY(西元年)/MM/DD") + '!',
                VALUE: strVALUE
            };
        } else {
            for (i = 1; i <= 12; i++) {
                if (str_i.match(new RegExp(arr_mon[i], "i"))) {
                    str_i = str_i.replace(new RegExp(arr_mon[i], "i"), i);
                }
            }
        }
    }
    // find seperate
    if (str_p.match(reg_s)) {
        sep = str_p.match(reg_s);
        seps = new RegExp("\\" + sep, "g");
        if (str_i.match("\\" + sep) && str_i.match(seps).length == 2) {
            p_element = str_p.split(sep);
            i_element = str_i.split(sep);
        } else {
            return {
                STATUS: false,
                ERROR: JsFunML(ML_JS_CHECK, "日期格式錯誤，應為 YYYY(西元年)/MM/DD") + '!',
                VALUE: strVALUE
            };
        }
        // no seperate
    } else {
        var cnt = 0, arr_cnt = 0;
        while (arr_cnt < 3) {
            p_s = str_p.substr(cnt, 1);
            p_l = str_p.match(new RegExp(p_s, "gi")).length;
            p_element[arr_cnt] = str_p.substr(cnt, p_l);
            i_element[arr_cnt] = str_i.substr(cnt, p_l);
            arr_cnt = arr_cnt + 1;
            cnt = cnt + p_l;
        }
    }
    for (i = 0; i <= 2; i++) {
        if (p_element[i].match(/y|r/i)) {
            var yy = i_element[i];
            var reg_y = "\\b\\d{2," + p_element[i].match(reg_ycnt).length + "}\\b";
        }
        if (p_element[i].match(/m/i)) {
            if (i_element[i].length < 2) {
                var mm = "0" + i_element[i];
            } else {
                var mm = i_element[i];
            }
        }
        if (p_element[i].match(/d/i)) {
            if (i_element[i].length < 2) {
                var dd = "0" + i_element[i];
            } else {
                var dd = i_element[i];
            }
        }
    }

    if (yy.match(reg_y) && (mm > 0 && mm < 13) && (dd > 0 && dd < 32)) {
        if ((mm == 4 || mm == 6 || mm == 9 || mm == 11) && dd > 30) {
            return {
                STATUS: false,
                ERROR: JsFunML(ML_JS_CHECK, "該月僅有") + ' 30 ' + JsFunML(ML_JS_CHECK, "天") + ' !',
                VALUE: strVALUE
            };
        } else if (mm == 2) {
            if ((parseInt(yy)) % 4 > 0 && dd > 28) {
                return {
                    STATUS: false,
                    ERROR: JsFunML(ML_JS_CHECK, "該月僅有") + ' 28 ' + JsFunML(ML_JS_CHECK, "天") + ' !',
                    VALUE: strVALUE
                };
            } else if ((parseInt(yy)) % 4 == 0 && dd > 29) {
                return {
                    STATUS: false,
                    ERROR: JsFunML(ML_JS_CHECK, "該月僅有") + ' 29 ' + JsFunML(ML_JS_CHECK, "天") + ' !',
                    VALUE: strVALUE
                };
            }
        } else if (dd > 31) {
            return {
                STATUS: false,
                ERROR: JsFunML(ML_JS_CHECK, "該月僅有") + ' 31 ' + JsFunML(ML_JS_CHECK, "天") + ' !',
                VALUE: strVALUE
            };
        }
        arrDate = str_i.split("/");
        if (arrDate[1].length == 1) {
            arrDate[1] = '0' + arrDate[1];
        }
        if (arrDate[2].length == 1) {
            arrDate[2] = '0' + arrDate[2];
        }
        str_i = arrDate[0] + '/' + arrDate[1] + '/' + arrDate[2];
        strVALUE = str_i;

        //OK
        return {
            STATUS: true,
            ERROR: '',
            VALUE: strVALUE
        };

    } else {
        return {
            STATUS: false,
            ERROR: JsFunML(ML_JS_CHECK, "日期格式錯誤，應為 YYYY(西元年)/MM/DD"),
            VALUE: strVALUE
        };
    }
}

/*
=========================================================================================
//基本驗證
=========================================================================================*/
function JsFunBASE_VERTIFY(I_OBJ) {    
    var oPARAM = {
        "CANCEL": false,
        "TITLE": '',
        "TARGET": null
    };

    //樣式檢核
    JsFunBASE_VERTIFY_CHK_NULL(oPARAM);

    //檢核必須為數字
    JsFunBASE_VERTIFY_CHK_NUMBER(oPARAM);

    //檢核必須為日期
    JsFunBASE_VERTIFY_CHK_DATE(oPARAM);

    //
    if (oPARAM["CANCEL"] == true) {
        alert(oPARAM["TITLE"]);
        $(oPARAM["TARGET"]).focus().removeClass('ui-state-active');
        $(oPARAM["TARGET"]).css({ "border": "1px solid #ff0000","background":"pink"});
        return false;
    } else {
        //將原本的,改成，
        $('input:text').each(function() {
            //alert($(this).val().replace(eval("/,/g"), "^"));
            //$(this).val($(this).val().replace(eval("/,/g"), "，"));
        });        
        return true;
    }
}

/*
=========================================================================================
//樣式檢核
=========================================================================================*/
function JsFunSET_ERROR_STYLE(I_blnOK, I_OBJ) {
    if (I_blnOK == true) {
        $(I_OBJ).css({ "border": "","background":"" });
    } else {        
        $(I_OBJ).css({ "border": "1px solid #ff0000", "background": "pink" });
    }
    return I_blnOK;
}

/*
=========================================================================================
//檢核必須輸入
=========================================================================================*/
function JsFunBASE_VERTIFY_CHK_NULL(I_PARAM) {    
    //檢核必須輸入    
    if (I_PARAM["CANCEL"] == false) {        
        $(":input[MUST_KEYIN_YN='Y']").each(function () {
            //重置驗證元件
            $(this).css({ "border": "", "background": "" });
            var blnMUST_KEYIN_YN = true;            
            //如果多筆輸入時, 必須有 CHK_COL 才檢核
            var oTR = JsFunFindParent(this, "TR");
            var oCHK_COL = $(oTR).find(":checkbox[name*='CHK_COL_']");
            if ($(oCHK_COL).length > 0 && $(oCHK_COL).prop("checked") == false)
                blnMUST_KEYIN_YN = false;

            if (blnMUST_KEYIN_YN != true)
                return;

            if (JsFunSET_ERROR_STYLE(JsFunCheckNull($(this).val()), this) != true) {                
                I_PARAM["CANCEL"] = true;                
                I_PARAM["TITLE"] = JsFunML(ML_JS_CHECK, " 注意！ *  欄位不能為空\n" + '[' + $(this).attr("FIELD_NAME") + ']');
                //if (I_PARAM["TARGET"] == null)
                    I_PARAM["TARGET"] = this;

                //
                $(this).bind("focusout.CHECK_NULL", function() {
                    JsFunSET_ERROR_STYLE(JsFunCheckNull($(this).val()), this);
                });
            } else {
                $(this).unbind('.CHECK_NULL');                
            }
        });
    }
}



/*
=========================================================================================
//檢核必須為數字
=========================================================================================*/
function JsFunBASE_VERTIFY_CHK_NUMBER(I_PARAM) {    
    if (I_PARAM["CANCEL"] == false) {        
        $(":input[DATA_TYPE='N'], :input[DATA_TYPE='M']").each(function () {
            //重置驗證元件
            $(this).css({ "border": "", "background": "" });
            if (JsFunSET_ERROR_STYLE(JsFunCheckNum($(this).val()), this) != true) {
                I_PARAM["CANCEL"] = true;
                I_PARAM["TITLE"] = JsFunML(ML_JS_CHECK, "注意！欄位是數字型態\n" + '[' + $(this).attr("FIELD_NAME") + ']');                
                //if (I_PARAM["TARGET"] == null)
                    I_PARAM["TARGET"] = this;

                $(this).bind("focusout.CHECK_NUM", function () {
                    JsFunSET_ERROR_STYLE(JsFunCheckNum($(this).val()), this);
                });

            } else {
                $(this).unbind('.CHECK_NUM');
            }
        });
    }
}

/*
=========================================================================================
//檢核必須為日期
=========================================================================================*/
function JsFunBASE_VERTIFY_CHK_DATE(I_PARAM) {
    //alert(I_PARAM["CANCEL"]);
    if (I_PARAM["CANCEL"] == false) {        
        $(":input[DATA_TYPE='D']").each(function () {
            //重置驗證元件
            $(this).css({ "border": "", "background": "" });
            var CHECK = JsFunCheckDate($(this).val());
            if (JsFunSET_ERROR_STYLE(CHECK["STATUS"], this) != true) {
                I_PARAM["CANCEL"] = true;
                I_PARAM["TITLE"] += "[" + $(this).attr("FIELD_NAME") + "] " + CHECK["ERROR"] + "\n";                
                //if (I_PARAM["TARGET"] == null)
                    I_PARAM["TARGET"] = this;

                //
                $(this).bind("focusout.CHECK_DATE", function () {
                    var CHECK = JsFunCheckDate($(this).val());
                    JsFunSET_ERROR_STYLE(CHECK["STATUS"], this);
                });
            } else {
                $(this).unbind('.CHECK_DATE');
            }

            $(this).val(CHECK["VALUE"]);
        });
    }
}