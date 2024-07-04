/*
=========================================================================================
//設定[DATE_LIST] 點選
=========================================================================================*/
function JsFunOPEN_DATE_LIST(I_TYPE, I_COL) {
    //新增日期按鈕
    var datepicker = null;

    if ($('#datepicker').length == 0) {
        datepicker = $('<div id="datepicker" style="position:absolute;"></div>');
        $(datepicker).datepicker({
            showAnim: "",
            changeMonth: true,
            changeYear: true,
            onSelect: function (dateText, inst) {
                $($(this).prop('DATEPICKER_PARAM')).val(dateText);
                $(this).hide();

                try {
                    if (typeof (JsFunEVENT_COMMAND) == 'function')
                        JsFunEVENT_COMMAND("DATE_SELECT", { sender: $(I_COL).prev(), EventArgs: dateText });
                } catch (e) { }
            }
        }).hide();

        $(document).bind('mousedown.datepicker', function () {
            var datepicker = $('#datepicker');
            var srcElement = $(event.srcElement);

            //假設未顯示
            if ($(datepicker).css('display') == 'none')
                return;

            //當點擊範圍之外
            if (srcElement.id != datepicker[0].id &&
            $(srcElement).parents('#' + datepicker[0].id).length == 0 &&
            !$(srcElement).hasClass('hasDatepicker') &&
            !$(srcElement).hasClass('ui-datepicker-trigger')
            ) {
                $(datepicker).hide();
            }
        });

        $('body').append(datepicker);
    }

    $($(I_COL).prev()).each(function () {
        var oINPUT = this;
        if ($(this).prop('readonly') == true)
            return;

        var datepicker = $('#datepicker');
        $(datepicker).prop('DATEPICKER_PARAM', oINPUT);

        //設定目前位置
        $(datepicker).css({
            "left": $(this).offset().left,
            "top": $(this).offset().top + this.offsetHeight
        });

        $(datepicker).datepicker("setDate", $(this).val());
        $(datepicker).show();
    });
}