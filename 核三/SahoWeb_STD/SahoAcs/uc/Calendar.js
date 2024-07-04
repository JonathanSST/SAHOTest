var target_obj;
var mainDiv;
var textObj;

function Call_Calendar(objText) {
    target_obj = objText;
    var obj = document.getElementById(objText);    
    var sPara = obj.value;    
    //20210908 有修改路徑
    $.post(
        "../../../DialogBox/DateTimeSelect.aspx",
        {
            para_date: sPara
        }, function (data) {            
            $("#CalendarShow").html(data);
            mainDiv = document.getElementById("CalendarShow");
            $("#CalendarShow").find(".IconCancel").click(function () {
                $("#CalendarShow").html("");
            });
            textObj = document.getElementById(target_obj);
            //<div id="popOverlay" style="display:none;position:absolute; top:0; left:0; z-index:29999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>
            $(document.body).append('<div id="CalendarOverlay" style="display:block;position:absolute; top:0; left:0; z-index:39999; overflow:hidden;-webkit-transform: translate3d(0,0,0);"></div>');
            $("#CalendarOverlay").css("background", "#000");
            $("#CalendarOverlay").css("opacity", "0.5");
            $("#CalendarOverlay").width($(document).width());
            $("#CalendarOverlay").height($(document).height());
            adjustAutoLocation(mainDiv, textObj, 25, 0);          //改用遮罩方式處理定位
        });
                
    //}
    
    
    /*
    
    */
}

function CheckTime(objText) {
    var re = new RegExp(/^[0-9]{4}[/]{1}[0-9]{2}[/]{1}[0-9]{2}\s[0-9]{2}[:]{1}[0-9]{2}[:]{1}[0-9]{2}$/);
    var obj = document.getElementById(objText);
    var flag = true;

    if (obj.value != "") {
        //判斷格式
        if (!re.test(obj.value)) {
            flag = false;
        }

        if (flag) {
            var objstr = obj.value.split(" ");
            var datestr = objstr[0].split("/");
            var timestr = objstr[1].split(":");

            //判斷日期
            if (datestr[1] > 12 || datestr[2] > 31) {
                flag = false;
            }
            //判斷時間
            if (timestr[0] > 23 || timestr[1] > 59 || timestr[2] > 59) {
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

if (Sys && Sys.Application) {
    Sys.Application.notifyScriptLoaded();
}

function adjustAutoLocation(divname, prodBoxName, top_distance, left_distance) {

    var top = 0;
    var left = 0;
    var objTop = cumulativeOffset(prodBoxName)[1];
    var objLeft = cumulativeOffset(prodBoxName)[0];
    var docheight = $(document).height();
    var windowWidth = $(window).width();
    //top_distance
    if (top_distance == undefined) {
        divname.style.top = objTop - realOffset(prodBoxName)[1] + document.body.scrollTop + 22 + 'px';
    } else {
        //top = top_distance;
        //divname.style.top = objTop - realOffset(prodBoxName)[1] + document.body.scrollTop + top + 'px';
        top = (objTop + $(divname).height() - docheight);       //取得差異高度
        if (top < 0) {
            top = objTop + 22;
        } else {
            top = objTop - $(divname).height();
        }
        divname.style.top = top + 'px';
    }

    //left_distance
    if (left_distance == undefined) {
        divname.style.left = objLeft - document.body.scrollLeft + 22 + 'px';
    } else {
        //left = left_distance;
        //divname.style.left = objLeft + left + 'px'; //+document.body.scrollLeft
        left = (objLeft + $(divname).width() - windowWidth);
        if (left < 0) {
            left = objLeft;
        } else {
            left = objLeft - left;
        }
        divname.style.left = left + 'px';
    }
}



document.addEventListener("onmousedown", hiddenCompont);

//隱藏開啟中的元件
function hiddenCompont(event) {
    //alert(1);
    if (mainDiv != null) {
        var offsetX = 0;
        var offsetY = 0;
        var scrollX = 0;
        var scrollY = 0;
        var cX = event.clientX;
        var cY = event.clientY;

        offsetX = cumulativeOffset(mainDiv)[0];
        offsetY = cumulativeOffset(mainDiv)[1];
        scrollX = realOffset(mainDiv)[0];
        scrollY = realOffset(mainDiv)[1];

        if (cX >= (offsetX - document.body.scrollLeft) && cX <= ((offsetX + mainDiv.offsetWidth) - document.body.scrollLeft) && cY >= (offsetY - document.body.scrollTop - scrollY)) {

        } else {
            closeAllCompont();
        }
    }
}

function realOffset(element) {
    var valueT = 0, valueL = 0;
    do {
        valueT += element.scrollTop || 0;
        valueL += element.scrollLeft || 0;
        element = element.parentNode;
    } while (element);
    return [valueL, valueT];
}

function cumulativeOffset(element) {
    var valueT = 0, valueL = 0;
    do {
        valueT += element.offsetTop || 0;
        valueL += element.offsetLeft || 0;
        element = element.offsetParent;
    } while (element);
    return [valueL, valueT];
}
