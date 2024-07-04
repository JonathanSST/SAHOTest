/*
=========================================================================================
//建立通用按鈕
=========================================================================================*/
function JsFunCREATE_BUTTON(I_TYPE, I_HTML) {
    var button = $("<INPUT TYPE='button' TABINDEX='9999' class='button ui-icon' " +
    "style='width:20px;height:20px;border:1px solid #000000;margin:0 1px 0 1px;display:inline;' " +
    I_HTML + " />");

    $(button).attr("LIST_TYPE", I_TYPE);

    switch (I_TYPE) {
        case "ETEK_LIST":
            //ETEK 選單按鈕
            $(button).addClass('ui-icon-circle-arrow-s');
            break;

        case "DATE_LIST":
            //
            $(button).css({ 'background-image': 'url(' + INI_APP_PATH + '/images/Calendar_scheduleHS.png)' });
            break;

        case "ADDR_LIST":
            //地址選擇按鈕
            $(button).css({ 'background-image': 'url(' + INI_APP_PATH + '/images/addr.gif)' });
            break;

        case "CIRCLE_PLUS":
            //樹狀選擇按鈕
            $(button).addClass('ui-icon-circle-plus');
            button = JsFunATTR_BIND(button, 'onclick', "JsFunCIRCLE_PLUS(this);", true);
            break;
    }

    return button;
}

/*
=========================================================================================
//地址按鈕事件
=========================================================================================*/
function JsFunOPEN_ADDR_LIST(I_OBJ) {
    //視窗
    try {
        frmLOV.close();
    } catch (e) { }

    frmLOV = open(INI_APP_PATH + "/ASP/Kernel/ADDR_CITY_LIST.asp?FrmRetID=" + $(I_OBJ).prev().attr('id') + "",
    "winLOV", "scrollbars=1,resizable=1,width=500,height=400");
    if (frmLOV.opener == null)
        frmLOV.opener = self;
}
