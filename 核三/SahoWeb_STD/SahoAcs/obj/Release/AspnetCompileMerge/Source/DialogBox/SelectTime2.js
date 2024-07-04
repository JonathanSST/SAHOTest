// JScript 檔
var oSelectHour;
var oSelectBMin;
var oSelectSMin;
var bClean = false;

// 本視窗啟動時
function Page_Init() {
    SetBox();
    if (document.getElementById(target_obj).value !== "" && document.getElementById(target_obj).value !== undefined) {
        var timestr = document.getElementById(target_obj).value.split(":");
        document.getElementById('Hour' + parseInt(timestr[0], 10)).click();
        document.getElementById('BMin' + timestr[1].substr(0, 1)).click();
        document.getElementById('SMin' + timestr[1].substr(1, 1)).click();
    }
    //document.getElementById('Hour' + parseInt(timestr[0], 10)).click();
    //document.getElementById('BMin' + timestr[1].substr(0, 1)).click();
    //document.getElementById('SMin' + timestr[1].substr(1, 1)).click();
}

function SetBox() {
    var oTd;
    for (var i = 0; i < 24; i++) {
        oTd = $get('Hour' + i);
        $addHandlers(oTd, { click: onHourSelect });
    }
    for (var i = 0; i <= 5; i++) {
        oTd = $get('BMin' + i);
        $addHandlers(oTd, { click: onBMinSelect });
    }
    for (var i = 0; i <= 9; i++) {
        oTd = $get('SMin' + i);
        $addHandlers(oTd, { click: onSMinSelect });
    }
    ShowSelectDateTime();
}

function onHourSelect(eventElement) {
    oTd = eventElement.target;
    LaHour.innerText = oTd.innerText;
    if (oSelectHour != null)
        oSelectHour.style.background = '#FFFFFF';
    oSelectHour = oTd;
    oSelectHour.style.background = '#999999';
    ShowSelectDateTime();
}

function onBMinSelect(eventElement) {
    oTd = eventElement.target;
    LaBMin.innerText = oTd.innerText;
    if (oSelectBMin != null)
        oSelectBMin.style.background = '#FFFFFF';
    oSelectBMin = oTd;
    oSelectBMin.style.background = '#999999';
    ShowSelectDateTime();
}

function onSMinSelect(eventElement) {
    oTd = eventElement.target;
    LaSMin.innerText = oTd.innerText;
    if (oSelectSMin != null)
        oSelectSMin.style.background = '#FFFFFF';
    oSelectSMin = oTd;
    oSelectSMin.style.background = '#999999';
    ShowSelectDateTime();
}

function ShowSelectDateTime() {
    LaHour.style.color = "#000000";
    LaBMin.style.color = "#000000";
    LaSMin.style.color = "#000000";
    bClean = false;
}

function Call_First() {
    document.getElementById('Hour0').click();
    document.getElementById('BMin0').click();
    document.getElementById('SMin0').click();
    ShowSelectDateTime();
}

function Call_00Min() {
    document.getElementById('BMin0').click();
    document.getElementById('SMin0').click();
    ShowSelectDateTime();
}

function Call_15Min() {
    document.getElementById('BMin1').click();
    document.getElementById('SMin5').click();
    ShowSelectDateTime();
}

function Call_30Min() {
    document.getElementById('BMin3').click();
    document.getElementById('SMin0').click();
    ShowSelectDateTime();
}

function Call_45Min() {
    document.getElementById('BMin4').click();
    document.getElementById('SMin5').click();
    ShowSelectDateTime();
}

function Call_Last() {
    document.getElementById('Hour23').click();
    document.getElementById('BMin5').click();
    document.getElementById('SMin9').click();
    ShowSelectDateTime();
}

function Call_Clean() {
    LaHour.style.color = "#FFFFFF";
    LaBMin.style.color = "#FFFFFF";
    LaSMin.style.color = "#FFFFFF";
    if (oSelectHour != null)
        oSelectHour.style.background = '#FFFFFF';
    if (oSelectBMin != null)
        oSelectBMin.style.background = '#FFFFFF';
    if (oSelectSMin != null)
        oSelectSMin.style.background = '#FFFFFF';
    LaHour.innerText = '00';
    LaBMin.innerText = '0';
    LaSMin.innerText = '0';
    bClean = true;
}

function Call_Cancel() {
    $("#CalendarShow").html("");
    $("#CalendarOverlay").remove();
}

function Call_Yes() {
    if (bClean != true) {
        document.getElementById(target_obj).value = LaHour.innerText + ":" + LaBMin.innerText + LaSMin.innerText;
    }
    else
        document.getElementById(target_obj).value = "";
    //close();
    $("#CalendarShow").html("");
    $("#CalendarOverlay").remove();
}
