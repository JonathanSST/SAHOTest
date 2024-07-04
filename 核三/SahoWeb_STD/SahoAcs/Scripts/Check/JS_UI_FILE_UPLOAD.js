/*
=========================================================================================
//設定[FILE_UPLOAD] 點選
=========================================================================================*/
function JsFunOPEN_FILE_UPLOAD(I_TYPE, e) {
    var oTABLE = null;
    var oTR = null;
    var URL = "";

    //設定網址
    URL += "../UTIL/FILE_UPLOAD.aspx";
    //使用 Math.random() 避免使用快取
    URL += "?RDM=" + Math.random();
    URL += "&PG_ID=" + e["PROG_ID"];
    URL += "&PAGE_ID=" + e["PAGE_ID"];
    URL += "&M_SEQ_NO=" + e["M_SEQ_NO"];
    URL += "&FIELD_ID=" + e["FIELD_ID"];
    URL += "&FILE_PATH=" + e["FILE_PATH"];

    //    //新增參數
    //    if ($("#ETEK_PARAM").length == 0) {
    //        $("body").append("<INPUT type='hidden' ID='ETEK_PARAM' />");
    //    }

    //設定列參考
    //    oTABLE = JsFunFindParent(I_COL, "TABLE");
    //    oTR = JsFunFindParent(I_COL, "TR");
    //    oETEK_PARAM = document.getElementById("ETEK_PARAM");
    //    oETEK_PARAM["TR_EVENT"] = {
    //        STYLE_M: $(oTABLE).attr("STYLE_M"),
    //        TABLE: oTABLE,
    //        ROW: oTR,
    //        FIELD: oETEK
    //    };

    //視窗
    try {
        frmLOV.close();
    } catch (e) { }

    frmLOV = open(URL, "winLOV", "dependent=yes, status=1, scrollbars=1,resizable=1,width=500,height=400");
    if (frmLOV.opener == null)
        frmLOV.opener = self;
}






