var mainDiv;
var textObj;
var i = -1;
var oldi = -1;
var EquList;

function jsFUNC_REL_TYPE(obj, val,val2) {
    $.post("EquFloorSetting.aspx", { equ_id: val, card_ext_data: val2, card_id: $('#CardID').val() }, function (data) {
        $('<div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000'
           +';background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;" ></div>').appendTo("#EquArea");
        $("#dvContent").html(data);
    });    
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


//當進行快顯工作前，須關閉作用中的選單
function closeAllCompont() {
    //$("#dvContent").html("");
    $("#dvContent").remove();
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


//mouse事件處理
//透過上下鍵帶入選項的value(onkeyup)
function OnKeyUpSelect() {
    if (mainDiv != null) {
        //設定新的i值
        if (event.keyCode == 40) {            //往下
            i++;
            if (document.getElementById("hiddenCount") != null) {
                if (i > eval(document.getElementById("hiddenCount").value)) {
                    i = 0;
                }
            }
        } else if (event.keyCode == 38 && oldi != -1) { //往上
            i--;
            if (i < 0) {
                i = document.getElementById("hiddenCount").value;
            }
        }
        if (document.getElementById("SelectMark_" + oldi) != null) {
            document.getElementById("SelectMark_" + oldi).className = "mouseOut";
        }

        if (document.getElementById("SelectMark_" + i) != null) {
            document.getElementById("SelectMark_" + i).className = "mouseOver";
        }
        //儲存舊的i值
        oldi = i;
    }
}
