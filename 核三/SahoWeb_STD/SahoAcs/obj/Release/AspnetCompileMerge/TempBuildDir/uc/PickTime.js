function Call_PickTime(objText) {    
    var obj = document.getElementById(objText);
    var sPara = obj.value;
    
    var sRet = window.showModalDialog('/DialogBox/SelectTime.aspx', sPara, "dialogHeight:290px; dialogWidth:270px; center: Yes; help: No; resizable: No; scroll: No ");

    if (sRet != null) {
        obj.value = sRet;
    }
    
}

function CheckTime(objText) {
    var re = new RegExp(/^[0-9]{2}[:]{1}[0-9]{2}$/);
    var obj = document.getElementById(objText);
    var flag = true;

    if (obj.value != "") {
        //判斷格式
        if (!re.test(obj.value)) {
            flag = false;
        }

        if (flag) {
            var timestr = obj.value.split(':');
            //判斷時間
            if (timestr[0] > 23 || timestr[1] > 59 ) {
                flag = false;
            }
        }

        if (!flag) {
            alert('時間格式錯誤');
            obj.value = "";
            obj.focus();
        }
    }
}

if (Sys && Sys.Application) { Sys.Application.notifyScriptLoaded(); }