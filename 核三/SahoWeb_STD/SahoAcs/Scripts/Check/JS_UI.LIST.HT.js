/*
=====================================
說明 : HT上傳
=====================================*/
function JsFunLIST_HT_UP() {
    //取得檔案物件
    var FSO = new JsFSO();
    var WshShell = new ActiveXObject("WScript.Shell");
    var strFILE_NAME = '';

    var strTYPE = $(oETEK_FIELD).attr('FILE_TYPE');
    var strTXT = '';
    var strProgramFile = WshShell.ExpandEnvironmentStrings('%ProgramFiles%');
    var strProgramFile1 = strProgramFile.substr(0, strProgramFile.length - 6);
    var strEXE_PATH = strProgramFile + '\\unitech\\sungching\\';
    var strEXE_PATH1 = strProgramFile1 + '\\unitech\\sungching\\';

    var strEXE_FILE = strEXE_PATH + 'sungching_PC_Trans.exe';
    var strEXE_FILE1 = strEXE_PATH1 + 'sungching_PC_Trans.exe';

    var strTXT_PATH = 'C:\\DT800\\';
    var strTXT_FILE = strTYPE + '.*?\.TXT';
    var strDEL_FILE = strTYPE + '*.TXT';

    var realEXE_FILE = "";
    if (FSO.GetFSO().FileExists(strEXE_FILE)) {
        realEXE_FILE = strEXE_FILE;
    }
    else {
        if (FSO.GetFSO().FileExists(strEXE_FILE1)) {
            realEXE_FILE = strEXE_FILE1;
        }
    }

    if (realEXE_FILE != "") {
        //刪除暫存
        try {
            //alert(strTXT_PATH + strDEL_FILE);
            FSO.GetFSO().DeleteFile(strTXT_PATH + strDEL_FILE, true);
        } catch (e) {
            //alert('<%=UTIL.FunML("無法刪除") %>' + strTXT_PATH + strDEL_FILE);
        }

        //執行HT程式
        WshShell.Run('"' + realEXE_FILE + '"', 1, true);

        //設定 HT 上傳檔路徑
        $('input[name="SELECT_FILES"]').val(strTXT_PATH);

        //取得檔案清單
        var ARR_FILES = FSO.GetFILE_LIST(strTXT_PATH, strTXT_FILE);

        //alert(ARR_FILES);

        //讀取值
        for (var i = 0; i < ARR_FILES.length; i++) {
            strFILE_NAME = ARR_FILES[i];

            if (i > 0)
                strTXT += '\n';

            strTXT += FSO.READ_ALL(strFILE_NAME);

        }

        //設定回傳欄位
        $(':input[name="SELECT_FILES_VAL"]').val(strTXT);

        if (strTXT == undefined || strTXT == '') {
            //   alert('<%=UTIL.FunML("無內容")%>!');
        } else {
            LIST_HT_AJAX_INVOKE(strTYPE, strFILE_NAME, strTXT);
        }

    } else {
        alert('<%=UTIL.FunML("HT上傳程式不存在以下路徑")%>!\n' + strEXE_FILE);
    }

    FSO = WshShell = undefined;
}

/*
=====================================
說明 : 手動上傳
=====================================*/
function JsFunLIST_CU_UP() {
    var FSO = null;
    var strTXT = '';
    var oDLG = document.getElementById('HtmlDlgHelper');
    var strTYPE = $(oETEK_FIELD).attr('FILE_TYPE');
    var strFILE_FILTER = $(oETEK_FIELD).attr('FILE_FILTER');
    var strFILE_NAME = oDLG.OpenFileDlg("", "", strFILE_FILTER, "");

    if (strFILE_NAME != undefined || strFILE_NAME != '') {
        //當 oDLG.OpenFileDlg 取得的檔名時, 會有亂碼, 需要特別處理
        var oSELECT_FILES = $('input[name="SELECT_FILES"]')[0];
        $(oSELECT_FILES).val(strFILE_NAME);
        strFILE_NAME = $(oSELECT_FILES).val();

        //取得檔案物件
        FSO = new JsFSO();

        //alert(strFILE_NAME);

        strTXT = FSO.READ_ALL(strFILE_NAME);

        //設定回傳欄位
        $(':input[name="SELECT_FILES_VAL"]').val(strTXT);

        if (strTXT == undefined || strTXT == '') {
            //   alert('<%=UTIL.FunML("無內容")%>!');
        }
        else {
            LIST_HT_AJAX_INVOKE(strTYPE, strFILE_NAME, strTXT);
        }
    }

    FSO = WshShell = undefined;
}

/*
=====================================
說明 : 使用 AJAX
參數 : I_OBJ:目前被觸發的按鈕參考('<LI id=... />')
I_USER_CONTROL_ID:伺服器控制項 ClientID
=====================================*/
function LIST_HT_AJAX_INVOKE(I_strTYPE, I_strFILE_NAME, I_strVAL) {
    //Anthem 套件的 AJAX 方法, 可以呼叫 .net 伺服端的方法

    //alert(I_strVAL);

    if (I_strVAL == undefined || I_strVAL == '')
        return;

    Anthem_InvokeControlMethod(AJAX_LIST_HT_CLIENT_ID, 'FunHT_UP', [I_strTYPE, I_strFILE_NAME, I_strVAL], function (result) {
        var JSON_ARR = {};

        //alert(result.value);

        eval("JSON_ARR = " + result.value);

        //觸發回傳事件
        if (typeof (opener.JsFunEVENT_COMMAND) != undefined) {
            try {
                opener.JsFunEVENT_COMMAND("LIST_HT_AJAX_INVOKE", { sender: this, EventArgs: JSON_ARR });
            } catch (ev) { }
        }

        close();
    });
}