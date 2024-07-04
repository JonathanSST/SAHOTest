/*
=========================================================================================
// JS_UTIL
=========================================================================================*/
function JsFunML(I_OBJ, I_VALUE) {
    var strVALUE = I_OBJ[I_VALUE];

    if (strVALUE == undefined || strVALUE == '')
        strVALUE = I_VALUE;

    return strVALUE;
}

/*
=========================================================================================
' // 說明 : 傳回一格式化的數字
' // 作者 : RD
' // 範例 : JsFunFormatNumber()		
' // 日期 : 2011/11/30
' // 輸出 : 無
' // 輸入 : 

Expression 
必要項。欲被格式化的運算式。 

NumDigitsAfterDecimal 
選擇項。此數值表示有多少小數位數。預設值為 0

IncludeLeadingDigit 
選擇項。以 Tristate 常數表示小數點前是否顯示前導零。 
=========================================================================================*/
function JsFunFormatNumber(Expression, NumDigitsAfterDecimal, IncludeLeadingDigit) {
    var numVALUE = 0;

    //檢查傳入參數
    if (arguments.length == 1)
        NumDigitsAfterDecimal = INI_DECIMAL_POINT;

    //處理小數位
    try {
        NumDigitsAfterDecimal = Number(NumDigitsAfterDecimal).toFixed(0);
    } catch (e) {
        NumDigitsAfterDecimal = 0;
    }

    //開始轉換
    try {
        numVALUE = Number(Expression).toFixed(NumDigitsAfterDecimal);
    }
    catch (e) {
        numVALUE = NaN;
    }

    return numVALUE;
}

/*
=========================================================================================
// 取得亂數
=========================================================================================*/
function JsFunGetRound() {
    return "A" + Math.random().toString().replace(".", "");
}

/*
=========================================================================================
// 尋找父控制像
=========================================================================================*/
function JsFunFindParent(I_obj, I_TagName) {
    var oSRC = null;

    if (I_obj == undefined || I_obj == null)
        return;

    oSRC = (I_obj.length) ? I_obj[0] : I_obj;

    while (true) {
        if (oSRC["tagName"] == I_TagName)
            return oSRC

        if (oSRC.parentNode)
            oSRC = oSRC.parentNode;
        else
            break;
    }
    return null;
}

/*
=========================================================================================
// 按 ENTER 跳下一個欄位
=========================================================================================*/
function HotKey() {    
    var I_Event = window.event;
    var chaCode = I_Event.keyCode;
    var strVAL = '';
    //alert(chaCode);
    if (chaCode == 13) {        
        switch (I_Event.srcElement.type) {            
            case "textarea":
            case "submit":                
                break;

            default:                                
                I_Event.keyCode = 9;
                event.keyCode = 9;                
                break;
        }
    } else {
        //當多筆輸入時, 有KEY資料時, 打勾啟用
        switch (I_Event.srcElement.tagName) {
            case "INPUT":
            case "TEXTAREA":
                var oTABLE = JsFunFindParent(I_Event.srcElement, "TABLE");
                var oTR = JsFunFindParent(I_Event.srcElement, "TR");
                var strM_SEQ_NO = $(oTABLE).attr('M_SEQ_NO');
                var strSTYLE_M = $(oTABLE).attr('STYLE_M');
                if (strSTYLE_M == '02') {
                    var oCHK_COL = $(oTR.cells[1]).find(':input[name="CHK_COL_' + strM_SEQ_NO + '"]');

                    if ($(oCHK_COL).length > 0 && $(oCHK_COL).prop('disabled') == false)
                        $(oCHK_COL).prop('checked', true);
                }
                break;
        }
    }
    return true;
}

/*
=========================================================================================
// 重設登出時間
=========================================================================================*/
function SetLogoutTime() {
    if (typeof top.FrameMenuBar != 'undefined') {
        if (typeof top.FrameMenuBar.document.all['hidLogoutTime'] != 'undefined') {
            top.FrameMenuBar.document.all['hidLogoutTime'].value = '9999';
        }
    }
    else {
        if (typeof opener.top.FrameMenuBar != 'undefined') {
            if (typeof opener.top.FrameMenuBar.document.all['hidLogoutTime'] != 'undefined') {
                opener.top.FrameMenuBar.document.all['hidLogoutTime'].value = '9999';
            }
        }
    }
}

/*
=========================================================================================
// 畫面展開縮放
=========================================================================================*/
function SetAllFrame() {
    if (typeof parent.FrameMenu == 'undefined') {
        alert(JsFunML(ML_JS_UTIL, "你無法於此畫面執行 『展開縮放』功能") + '!');
        return false;
    }
    var AllScreenObj = top.FrameLeftButton.document.all['AllScreen'];
    var chaCurFrameIDObj = top.FrameLeftButton.document.all['chaCurFrameID'];

    if (AllScreenObj == 'undefined' || chaCurFrameIDObj == 'undefined') {
        alert(JsFunML(ML_JS_UTIL, "你無法於此畫面執行 『展開縮放』功能") + '!');
        return false;
    }
    var AllScreen = AllScreenObj.value * 1;
    var chaCurFrameID = chaCurFrameIDObj.value;
    if (AllScreen) {
        var tmps = '';
        if (chaCurFrameID == 'main_0') {
            tmps = '0,0,*,0,0,0';
        }
        else {
            if (chaCurFrameID == 'main_1') {
                tmps = '0,0,0,*,0,0';
            }
            else {
                tmps = '0,0,0,0,*,0';
            }
        }

        top.document.all['idMainFrame'].rows = '0,0,0,*';
        top.document.all['idMainFrame'].frameSpacing = 0;
        top.document.all['multiF'].cols = tmps;
        top.document.all['multiF'].frameSpacing = 0;
        top.FrameLeftButton.document.all['AllScreen'].value = 0;
    } else {		// to show 
        SetToolBarHeight();

        SetBodyWidth();

        top.FrameLeftButton.document.all['AllScreen'].value = 1;
    }
    return false;
}

/*
=========================================================================================
// 
=========================================================================================*/
function SetToolBarHeight() {
    var SelectToolbar = top.FrameLeftButton.document.all['SelectToolbar'].value * 1;

    if (SelectToolbar) {
        top.document.all['idMainFrame'].rows = '91,30,0,*';
    }
    else {
        top.document.all['idMainFrame'].rows = '91,0,0,*';
    }
    top.document.all['idMainFrame'].frameSpacing = 0;
}

/*
=========================================================================================
// 
=========================================================================================*/
function SetBodyWidth() {
    var SelectMainMenu = top.FrameLeftButton.document.all['SelectMainMenu'].value * 1;
    var SelectMenu = top.FrameLeftButton.document.all['SelectMenu'].value * 1;
    var SelectFavority = top.FrameLeftButton.document.all['SelectFavority'].value * 1;

    var SelectMessage = top.FrameLeftButton.document.all['SelectMessage'].value * 1;

    var ShowMain_0 = top.FrameLeftButton.document.all['ShowMain_0'].value * 1;
    var ShowMain_1 = top.FrameLeftButton.document.all['ShowMain_1'].value * 1;
    var ShowMain_2 = top.FrameLeftButton.document.all['ShowMain_2'].value * 1;

    var tmps = '64';

    if (SelectMainMenu || SelectMenu || SelectFavority)
        tmps = tmps + ',20%';
    else
        tmps = tmps + ',0';

    var tmpRate1 = 58;
    if (!SelectMessage)
        tmpRate1 = 78;
    var tmpRate2 = Math.round(tmpRate1 / 2)
    var tmpRate3 = Math.round(tmpRate1 / 3)

    if (ShowMain_0 && !ShowMain_1 && !ShowMain_2)
        tmps = tmps + ',*,0,0';
    else {
        if (ShowMain_0 && ShowMain_1 && !ShowMain_2)
            tmps = tmps + ',' + tmpRate2 + '%,*,0';
        else {
            if (ShowMain_0 && !ShowMain_1 && ShowMain_2)
                tmps = tmps + ',' + tmpRate2 + '%,0,*';
            else {
                if (ShowMain_0 && ShowMain_1 && ShowMain_2)
                    tmps = tmps + ',' + tmpRate3 + '%,' + tmpRate3 + '%,*';
                else {
                    if (!ShowMain_0 && ShowMain_1 && !ShowMain_2)
                        tmps = tmps + ',0,*,0';
                    else {
                        if (!ShowMain_0 && ShowMain_1 && ShowMain_2)
                            tmps = tmps + ',0,' + tmpRate2 + '%,*';
                        else
                            tmps = tmps + ',0,0,*';
                    }
                }
            }
        }
    }

    if (SelectMessage)
        tmps = tmps + ',20%';
    else
        tmps = tmps + ',0';

    top.document.all['multiF'].cols = tmps;
    top.document.all['multiF'].frameSpacing = 0;
}


/*
=====================================
說明 : 檔案物件
=====================================*/
function JsFSO() {
    var ForReading = 1, ForWriting = 2;
    this._FSO = null;

    /*
    =====================================
    說明 : 取得檔案物件
    =====================================*/
    this.GetFSO = function () {
        if (this._FSO == null) {
            //建立檔案物件
            this._FSO = new ActiveXObject("Scripting.FileSystemObject");
        }
        return this._FSO;
    };

    /*
    =====================================
    說明 : 讀取文字檔
    I_FILE_NAME : 檔名
    =====================================*/
    this.READ_ALL = function (I_FILE_NAME) {
        var f, strVALUE = '';

        try {
            f = this.GetFSO().OpenTextFile(I_FILE_NAME, ForReading);
            strVALUE = f.ReadAll();
        } catch (e) {
            strVALUE = '';
        }

        f = undefined;
        return strVALUE;
    };

    /*
    =====================================
    說明 : 寫文字檔
    I_FILE_NAME : 檔名
    =====================================*/
    this.WRITE_ALL = function (I_FILE_NAME, I_strVAL) {
        var f, strVALUE = I_strVAL;

        try {
            f = this.GetFSO().OpenTextFile(I_FILE_NAME, ForWriting, true);
            f.Write(I_strVAL);
            f.Close();
        } catch (e) {
            strVALUE = '';
        }

        f = undefined;
        return strVALUE;
    };

    /*
    =====================================
    說明 : 讀取檔案清單
    I_PATH : 檔案路徑
    I_FILTER : 檔案過濾字串
    =====================================*/
    this.GetFILE_LIST = function (I_PATH, I_FILTER) {
        var f, fc, s;
        var ARR = new Array();

        f = this.GetFSO().GetFolder(I_PATH);
        fc = new Enumerator(f.files);
        for (; !fc.atEnd(); fc.moveNext()) {
            var re = new RegExp(I_FILTER, "ig");
            var strVAL = '' + fc.item();

            if (re.test(strVAL))
                ARR.push(strVAL);
        }

        return ARR;
    };
}