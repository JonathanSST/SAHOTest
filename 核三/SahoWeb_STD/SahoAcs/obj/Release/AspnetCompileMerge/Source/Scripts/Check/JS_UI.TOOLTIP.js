/*
=========================================================================================
// 建立提示文字
=========================================================================================*/
function JsFunTOOLTIP_CREATE() {
    var oTOOLTIP = $("#RSL_TOOLTIP");

    if (oTOOLTIP.length == 0) {
        oTOOLTIP = $("<DIV id='RSL_TOOLTIP' class='tooltip' tooltip='' style='position:absolute;padding:10px;width:200px;filter:Alpha(Opacity=85)' />");
        $('body').append(oTOOLTIP);

        $(document).bind('focusin.TOOLTIP, focusout.TOOLTIP, mouseover.TOOLTIP', function () {
            var oINPUT;
            var strID;
            var strNAME;
            var strFIELD_NAME;
            var strMAXLENGTH;
            var strDEBUG;

            if (event == null || event.srcElement == undefined)
                return;

            switch (event.srcElement.tagName) {
                case "INPUT":
                case "TEXTAREA":
                case "SELECT":
                    oINPUT = event.srcElement;

                    if (event.type == 'mouseover') {
                        //當滑鼠指向時, 顯示目前欄位值
                        $(oINPUT).attr("title", function () {
                            return $(this).val();
                        });
                        return;
                    }

                    strID = $(oINPUT).attr('id');
                    strNAME = $(oINPUT).attr('NAME');
                    strFIELD_NAME = $.trim($(oINPUT).attr('FIELD_NAME'));
                    strMAXLENGTH = $(oINPUT).attr('MAXLENGTH');
                    strDEBUG = "";

                    if (
                    ($(oINPUT).prop('readonly') == true && blnDEBUG != 'True') ||
                    strFIELD_NAME == '')
                        return;

                    if (event.type == 'focusin') {
                        //新增焦點
                        if ($(oINPUT).prop('readonly') != true && $(oINPUT).prop('disabled') != true)
                            $(oINPUT).addClass("ui-state-active");

                        //除錯模式顯示
                        if (blnDEBUG == 'True') {
                            strDEBUG = ' <BR>NAME:[' + strNAME + ']<BR>ID:[' + strID + ']<BR>';
                        }

                        //設定提示內容
                        $(oTOOLTIP).html('<SPAN>' +
                            strFIELD_NAME + strDEBUG + ' ' +
                            ((strMAXLENGTH == undefined) ? '' : JsFunML(ML_JS_INIT, "長度") + '[' + strMAXLENGTH + ']') +
                            '</SPAN>');

                        //設定目前位置,
                        $(oTOOLTIP).css({
                            "left": $(oINPUT).offset().left + (oINPUT.offsetWidth * 0.8),
                            "top": $(oINPUT).offset().top + $(oINPUT).innerHeight() + 2
                        });

                        $(oTOOLTIP).show();
                    } else {
                        //移除焦點
                        $(oINPUT).removeClass("ui-state-active");
                        $(oTOOLTIP).hide();
                    }
                    break;
            }
        });


    }
}