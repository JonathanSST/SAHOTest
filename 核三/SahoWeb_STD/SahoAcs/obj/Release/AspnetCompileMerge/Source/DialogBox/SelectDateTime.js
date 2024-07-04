// JScript 檔

var aMonthDays = new Array();
var dToDay = new Date();
var sDay = '01';
var oSelectTd;
var bClean = false;

// 本視窗啟動時
function Page_Init() {
    //debugger;        
    var sDateTime = window.dialogArguments;
    if (sDateTime === '' || sDateTime === undefined) {
        sDateTime = Ntoc(dToDay.getFullYear(), 4) + '/' + Ntoc(dToDay.getMonth() + 1, 2) + '/' + Ntoc(dToDay.getDate(), 2) + ' ' +
                          Ntoc(dToDay.getHours(), 2) + ':' + Ntoc(dToDay.getMinutes(), 2) + ':' + Ntoc(dToDay.getSeconds(), 2);
    }    
    FindByText(DDListY, sDateTime.substr(0, 4));        // 所選擇的日期
    //$('select[name*="DDListY"]:eq(0)').val(sDateTime.substr(0, 4));
    //console.log($('select[name*="DDListY"]:eq(0)').val());
    FindByText(DDListM, sDateTime.substr(5, 2));        //
    sDay = sDateTime.substr(8, 2);                      //
    FindByText(DDList1, sDateTime.substr(11, 2));
    FindByText(DDList2, sDateTime.substr(14, 2));
    FindByText(DDList3, sDateTime.substr(17, 2));

    aMonthDays[0] = 31;
    aMonthDays[1] = 28;
    aMonthDays[2] = 31;
    aMonthDays[3] = 30;
    aMonthDays[4] = 31;
    aMonthDays[5] = 30;
    aMonthDays[6] = 31;
    aMonthDays[7] = 31;
    aMonthDays[8] = 30;
    aMonthDays[9] = 31;
    aMonthDays[10] = 30;
    aMonthDays[11] = 31;
    ClearBox();
    SetBox();
}

function SetBox() {
    var nYear = parseInt(DDListY.options[DDListY.selectedIndex].text, 10);
    var nMonth = parseInt(DDListM.options[DDListM.selectedIndex].text, 10) - 1;
    var nDay = parseInt(sDay, 10);
    if (((nYear % 4 == 0) && (nYear % 100 != 0)) || (nYear % 400 == 0)) aMonthDays[1] = 29;
    else aMonthDays[1] = 28;
    var nDays = aMonthDays[nMonth];
    if (nDay > nDays) nDay = nDays;
    var dDate1 = new Date();
    dDate1.setFullYear(nYear, nMonth, 1);
    var nWeek = dDate1.getDay();

    var oTd;
    for (var i = 1; i <= nDays; i++) {
        oTd = $get('Box' + (nWeek + i));
        oTd.innerText = Ntoc(i, 2);
        oTd.style.cursor = "default";
        $addHandlers(oTd, { click: onSelect1 });
        $addHandlers(oTd, { dblclick: onSelect2 });
        if ((nYear == dToDay.getFullYear()) && (nMonth == dToDay.getMonth()) && (i == dToDay.getDate())) oTd.style.background = '#ccccff';
        if (i == nDay) {
            oSelectTd = oTd;
            oTd.style.background = '#249AD1';
        }
    }
    ShowSelectDateTime();
}

function onSelect1(eventElement) {
    oTd = eventElement.target;
    sDay = oTd.innerText;
    if (Ntoc(dToDay.getDate(), 2) == oSelectTd.innerText) oSelectTd.style.background = '#249AD1';
    else oSelectTd.style.background = '#FFFFFF';
    oSelectTd = oTd;
    oSelectTd.style.background = '#999999';
    ShowSelectDateTime();
}

function onSelect2(eventElement) {
    oTd = eventElement.target;
    sDay = oTd.innerText;
    if (Ntoc(dToDay.getDate(), 2) == oSelectTd.innerText) oSelectTd.style.background = '#249AD1';
    else oSelectTd.style.background = '#FFFFFF';
    oSelectTd = oTd;
    oSelectTd.style.background = '#999999';
    ShowSelectDateTime();
    Call_Yes();
}

function ClearBox() {
    var oTd;
    for (var i = 1; i < 43; i++) {
        oTd = $get('Box' + i);
        $clearHandlers(oTd);
        oTd.innerText = ' ';
        oTd.style.background = '#FFFFFF';
    }
}

function ShowSelectDateTime() {
    LaData.innerText = DDListY.options[DDListY.selectedIndex].text + '/' +
                       DDListM.options[DDListM.selectedIndex].text + '/' + oSelectTd.innerText + ' ' +
                       DDList1.options[DDList1.selectedIndex].text + ':' +
                       DDList2.options[DDList2.selectedIndex].text + ':' +
                       DDList3.options[DDList3.selectedIndex].text;
    LaData.style.color = "#000000";
    bClean = false;
}

function DateOnChange() {
    ClearBox();
    SetBox();
}

function Call_ToDay() {
    FindByText(DDListY, Ntoc(dToDay.getFullYear(), 4));
    //$("#DDListY").val(Ntoc(dToDay.getFullYear(), 4));
    FindByText(DDListM, Ntoc(dToDay.getMonth() + 1, 2));
    sDay = Ntoc(dToDay.getDate(), 2);
    ClearBox();
    SetBox();
}

function Call_First() {
    FindByText(DDList1, '00');
    FindByText(DDList2, '00');
    FindByText(DDList3, '00');
    ShowSelectDateTime();
}

function Call_15Min() {
    FindByText(DDList2, '15');
    FindByText(DDList3, '00');
    ShowSelectDateTime();
}

function Call_30Min() {
    FindByText(DDList2, '30');
    FindByText(DDList3, '00');
    ShowSelectDateTime();
}

function Call_45Min() {
    FindByText(DDList2, '45');
    FindByText(DDList3, '00');
    ShowSelectDateTime();
}

function Call_Last() {
    FindByText(DDList1, '23');
    FindByText(DDList2, '59');
    FindByText(DDList3, '59');
    ShowSelectDateTime();
}

function Call_Clean() {
    LaData.style.color = "#FFFFFF";
    bClean = true;
}

function Call_Previous() {
    var y = DDListY.selectedIndex - 1;
    var m = DDListM.selectedIndex - 1;
    if (m >= 0) {
        DDListM.selectedIndex = m;
        ClearBox();
        SetBox();
    } else {
        if (y >= 0) {
            DDListY.selectedIndex = y;
            DDListM.selectedIndex = DDListM.options.length - 1;
            ClearBox();
            SetBox();
        }
    }
}

function Call_Next() {
    var y = DDListY.selectedIndex + 1;
    var m = DDListM.selectedIndex + 1;
    if (m < DDListM.options.length) {
        DDListM.selectedIndex = m;
        ClearBox();
        SetBox();
    } else {
        if (y < DDListY.options.length) {
            DDListY.selectedIndex = y;
            DDListM.selectedIndex = 0;
            ClearBox();
            SetBox();
        }
    }
}

function Call_Cancel()
{ close(); }

function Call_Yes() {
    if (bClean != true) {
        returnValue = LaData.innerText;
    }
    else
        returnValue = "";
    close();
}
