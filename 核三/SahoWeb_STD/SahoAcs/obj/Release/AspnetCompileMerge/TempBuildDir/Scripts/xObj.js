function GetObj(sObjId) {
    return document.getElementById(sObjId);
}

function GetText(oObj) {
    var sRet = '';
    if (oObj.type == 'checkbox') sRet = oObj.parentNode.lastChild.innerText;
    return sRet;
}

function SetText(oObj, sText) {
    if (oObj.type == 'checkbox') oObj.parentNode.lastChild.innerText = sText;
}

// 在 GridView 尋找資料，並將資料呈現在捲軸面板內
function GridGoToRow(nGroup, sScroll_DivID, sGridViewID, nCheckRow, sCheckString) {
    var IsOK = false;
    if (IsEmpty(sCheckString)) return IsOK;                         // 如果目標字串為空字串時什麼都不用做
    var aGrid, oGrid, aTr, oTr, aTd;
    var oPanel = document.getElementById(sScroll_DivID);            // 尋找捲軸面板
    if (oPanel == null) return IsOK;                                // 找不到就結束
    aGrid = oPanel.getElementsByTagName('table');                   // 尋找目標 GridView
    for (var i = 0; i < aGrid.length; i++) {
        if (aGrid[i].id.indexOf(sGridViewID) >= 0) {
            oGrid = aGrid[i]; break;
        }
    }
    if (oGrid == null) return IsOK;                                 // 找不到就結束
    aTr = oGrid.getElementsByTagName('tr');                         // 取得資料集合
    if (aTr.length == 0) return IsOK;                               // 資料筆數為 0 就結束
    aTd = aTr[0].getElementsByTagName('td');                        // 檢查資料欄位數量是否足夠
    if (aTd.length <= nCheckRow) return IsOK;                       // 如果不足就結束
    for (var i = 0; i < aTr.length; i++) {
        aTd = aTr[i].getElementsByTagName('td');
        if (aTd[nCheckRow].innerText == sCheckString) {             // 尋找最終目標
            oTr = aTr[i];
            oPanel.scrollTop = oTr.offsetTop;
            onMouseMoveIn(nGroup, oTr, '', '');
            oTr.click();
            IsOK = true;
            break;
        }
    }
    return IsOK;
}

// 讓 DropDownList 或 ListBox 依指定資料進行項目選擇設定
function FindByText(oList, sText) {    
    var nCount = oList.length;
    if (nCount > 0) {
        var IsSelected = false;
        for (var i = 0; i < nCount; i++) {
            if (oList.options[i].text == sText) {
                oList.options[i].selected = true;
                IsSelected = true;
                break;
            }
        }
        if (IsSelected == false) {
            if ((oList.options[0].value == '') || (oList.options[0].text == '-- 請選擇 --')) oList.options[0].selected = true;
        }
    }
}

// 讓 DropDownList 或 ListBox 依指定資料進行項目選擇設定
function FindByValue(oList, sValue) {
    var nCount = oList.length;
    if (nCount > 0) {
        var IsSelected = false;
        for (var i = 0; i < nCount; i++) {
            if (oList.options[i].value == sValue) {
                oList.options[i].selected = true;
                IsSelected = true;
                break;
            }
        }
        if (IsSelected == false) {
            if ((oList.options[0].value == '') || (oList.options[0].text == '-- 請選擇 --')) oList.options[0].selected = true;
        }
    }
}

// 讓 DropDownList 或 ListBox 從未端加入一個項目
function OptionAppend(oList, sValue, sText) {
    var oOption = new Option(sText, sValue);
    oList.options.add(oOption);
}

// 呼叫一個對話型視窗
function CallDialog(sPageName, nHeight, nWidth, Argument) {

    //    var H = nHeight + 32;
    //    var sPara = "dialogHeight: " + H + "px; dialogWidth: " + W + "px; center: Yes; help: No; resizable: No; status: No; scroll: No ";
    var H = nHeight + 52;
    var W = nWidth + 6;
    var sPara = "dialogHeight: " + H + "px; dialogWidth: " + W + "px; center: Yes; help: No; resizable: No; scroll: No ";
    if (Argument != undefined)
        RetValue = window.showModalDialog(sPageName, Argument, sPara);
    else
        RetValue = window.showModalDialog(sPageName, window, sPara);
    return RetValue;
}

function ShowReport(sPageName) {
    var H = 700 + 52;
    var W = 1024 + 6;
    var sPara = "dialogHeight: " + H + "px; dialogWidth: " + W + "px; center: Yes; help: No; resizable: Yes; scroll: Yes ";
    window.showModalDialog(sPageName, window, sPara);
}

// 開啟/關閉 物件display屬性
function ReverseDisplay(id) {
    if (document.getElementById(id).style.display == "none") { document.getElementById(id).style.display = "block"; }
    else { document.getElementById(id).style.display = "none"; }
}

//因Firefox不支援innerText，故使用此方法切換
function ChangeText(L_popName, Str) {
    if (L_popName.textContent != undefined)
        L_popName.textContent = Str;
    else
        L_popName.innerText = Str;
}

function Block() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}
