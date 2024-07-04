function Call_PickDate(objText) {
    var obj = document.getElementById(objText);
    var sPara = obj.value;
    var sRet = window.showModalDialog('/DialogBox/SelectDate.aspx', sPara, "dialogHeight:260px; dialogWidth:250px; center: Yes; help: No; resizable: No; scroll: No ");

    if (sRet != null) {
        obj.value = sRet;
    }
}

function CheckDate(objText) {
    var re = new RegExp("^([0-9]{4})[./]{1}([0-9]{1,2})[./]{1}([0-9]{1,2})$");
    var obj = document.getElementById(objText);
    var flag = true;

    if (obj.value != "") {
        //判斷格式
        if (!re.test(obj.value)) {
            flag = false;
        }

        if (flag) {
            var datestr = obj.value.split('/');
            var i = parseFloat(datestr[0]);
            if (i <= 0 || i > 9999) { // 年
                flag = false;
            }
            i = parseFloat(datestr[1]);
            if (i <= 0 || i > 12) { // 月
                flag = false;
            }
            i = parseFloat(datestr[2]);
            if (i <= 0 || i > 31) { // 日
                flag = false;
            }
        }

        if (!flag) {
            alert('日期格式錯誤');
            obj.value = "";
            obj.focus();
        }
    }
}

if (Sys && Sys.Application) { Sys.Application.notifyScriptLoaded(); }