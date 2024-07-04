function HexToInt(sHex) {
    return parseInt(sHex, 16);
}

function IntToHex(nNum, nLength) {
    var sHexStr = '0123456789ABCDEF';
    var sRet = '';
    var nTmp;
    while (nNum >= 15) {
        nTmp = nNum % 16;
        nNum = (nNum - nTmp) / 16;
        sRet = sHexStr.charAt(nTmp) + sRet;
    }
    if (nNum > 0) sRet = sHexStr.charAt(nNum) + sRet;
    var nLen = sRet.length;
    if (nLen < nLength) sRet = Repeat('0', nLength - nLen) + sRet;
    return sRet;
}

function BinToInt(sBin, IsReverse) {
    if (IsReverse == true) sBin = StrReverse(sBin);
    return parseInt(sBin, 2);
}

function IntToBin(nNum, nLength, IsReverse) {
    var sRet = '';
    var nTmp;
    while (nNum >= 2) {
        nTmp = nNum % 2;
        nNum = (nNum - nTmp) / 2;
        sRet = nTmp + sRet;
    }
    if (nNum > 0) sRet = nNum + sRet;
    var nLen = sRet.length;
    if (nLen < nLength) sRet = Repeat('0', nLength - nLen) + sRet;
    if (IsReverse == true) sRet = StrReverse(sRet);
    return sRet;
}

function HexToBin(sHex, nLength, IsReverse) {
    var nTmp = parseInt(sHex, 16);
    return IntToBin(nTmp, nLength, IsReverse);
}

function BinToHex(sBin, nLength, IsReverse) {
    var nTmp = BinToInt(sBin, IsReverse);
    return IntToHex(nTmp, nLength);
}

function StrReverse(sStr) {
    var aTmp = sStr.split('');
    aTmp = aTmp.reverse();
    return aTmp.join('');
}

// 檢查字串是否為空值或空白
function IsEmpty(sStr) { return ((sStr == undefined) || (sStr == null) || (sStr == '')); }

// 將字串重覆複製指定的次數
function Repeat(sBaseStr, nFrequency) {
    var sbTmp = new Sys.StringBuilder('');
    for (var i = 0; i < nFrequency; i++) sbTmp.append(sBaseStr);
    return sbTmp.toString();
}

// 將 sSource 的字串從 nStart 位置開始取出 nLen 個字元，並在當中填入 sReplStr 字串
function Stuff(sSource, nStart, nLen, sReplStr) {
    var sRet = '';
    if ((nStart < 0) || (nLen < 0) || IsEmpty(sSource)) return sRet;
    var nStrLen = sSource.length;   // 取得原字串的長度
    if (nStrLen > nStart) sRet += sSource.substr(0, nStart) + sReplStr;
    else {
        var sBase = '';
        for (var i = 0; i < nLen; i++) sBase += ' ';
        sSource += sBase;
        return sSource.substr(0, nStart) + sReplStr;
    }
    if (nStrLen > (nStart + nLen)) sRet += sSource.substr(nStart + nLen, nStrLen - (nStart + nLen));
    return sRet;
}

// 傳回前面補零的數值字串(指定長度)可傳送字串或數字
function Ntoc(vSource, nNum) {
    var sRet = '' + vSource;
    if (sRet.length < nNum) {
        var sBase = '';
        for (var i = 0; i < nNum; i++) sBase += '0';
        var sTmp = sBase + vSource;
        sRet = sTmp.substr(sTmp.length - nNum, nNum);
    }
    return sRet;
}

// 檢查是否符合 IP 字串格式(非 IP 字串則傳回空白字串否則傳回經過整理過的正確 IP 字串
function CheckIP(sTcpip) {
    var sRet = '';
    var sIp = '';
    if (!IsEmpty(sTcpip)) {
        sTcpip = Trim(sTcpip);
        if (sTcpip != '') {
            var j = 0;
            var nLen = sTcpip.length;
            for (var i = 0; i < nLen; i++) { if (sTcpip.charAt(i) == '.') j++; }
            if (j != 3) sRet = '0';
            else {
                var aIp = sTcpip.split('.');
                var nIp = 0;
                for (var i = 0; i < 4; i++) {
                    if (isNaN(aIp[i])) cRet = '0';
                    else {
                        nIp = parseInt(aIp[i], 10);
                        if ((nIp < 0) || (nIp > 255)) sRet = '0';
                        aIp[i] = '' + nIp;
                    }
                }
                if (sRet == '') sRet = aIp[0] + '.' + aIp[1] + '.' + aIp[2] + '.' + aIp[3];
            }
        }
    }
    return sRet;
}

// 將字串前後的空白字元除掉
function Trim(sText) { return LTrim(RTrim(sText)) }

// 將字串前面的空白字元除掉
function LTrim(sText) {
    var sRet = '';
    if (!IsEmpty(sText)) {
        var nLen = sText.length;
        if (nLen > 0) {
            var j = 0;
            for (var i = 0; i < nLen; i++) { if (sText.charAt(i) == ' ') j++; else break; }
            if (j == 0) sRet = sText;
            else { if (j < nLen) sRet = sText.slice(j - 1); }
        }
    }
    return sRet;
}

// 將字串後面的空白字元除掉
function RTrim(sText) {
    var sRet = '';
    if (!IsEmpty(sText)) {
        var nLen = sText.length;
        if (nLen > 0) {
            var j = 0;
            for (var i = nLen - 1; i >= 0; i--) { if (sText.charAt(i) == ' ') j++; else break; }
            if (j == 0) sRet = sText;
            else { if (j < nLen) sRet = sText.substr(0, nLen - j); }
        }
    }
    return sRet;
}

// 檢查輸入的字串是否符合日期格式
function CheckDate(sDate, bPassNoData) {
    var nLen, nYear, nMon, nDay, nKDay, n2Day;
    var aRet = new Array();
    var sTemplet = '1234567890/';

    aRet[0] = true;
    aRet[1] = '';
    sDate = Trim(sDate);
    nLen = sDate.length;
    if (nLen == 4) sDate = sDate.substr(0, 2) + '/' + sDate.substr(2, 2);
    if (nLen == 6) sDate = sDate.substr(0, 4) + '/' + sDate.substr(4, 2);
    if (nLen == 8) sDate = sDate.substr(0, 4) + '/' + sDate.substr(4, 2) + '/' + sDate.substr(6, 2);
    aRet[1] = sDate;
    if (((sDate == '----/--') || sDate == '--/--') || (sDate == '----/--/--') || (sDate == '')) aRet[0] = bPassNoData;
    else {
        nLen = sDate.length;
        for (var i = 0; i < nLen; i++) {
            if (sTemplet.indexOf(sDate.charAt(i), 0) == -1) aRet[0] = false;
        }
        if (nLen == 5) { if (sDate.charAt(2) != '/') aRet[0] = false; }
        if (nLen >= 7) { if (sDate.charAt(4) != '/') aRet[0] = false; }
        if (nLen == 10) { if (sDate.charAt(7) != '/') aRet[0] = false; }
    }

    if (aRet[0] = true) {
        if (nLen == 5) {
            nMon = parseInt(sDate.substr(0, 2), 10);
            nDay = parseInt(sDate.substr(3, 2), 10);
            if ((nMon < 1) || (nMon > 12)) aRet[0] = false;
            if (aRet[0] == true) {
                switch (nMon) {
                    case 2: nKDay = 29;
                    case 4: nKDay = 30;
                    case 6: nKDay = 30;
                    case 9: nKDay = 30;
                    case 11: nKDay = 30;
                    default: nKDay = 31;
                }
                if ((nDay < 1) || (nDay > nKDay)) aRet[0] = false;
            }
        } else {
            if (nLen >= 7) {
                nYear = parseInt(sDate.substr(0, 4), 10);
                nMon = parseInt(sDate.substr(5, 2), 10);
                nDay = 1;
            }
            if (nLen == 10) {
                nDay = parseInt(sDate.substr(8, 2), 10);
            }

            if ((nYear % 4) == 0) {
                if ((nYear % 100) != 0) n2Day = 29;
                else { n2Day = ((nYear % 400) == 0) ? 29 : 28; }
            } else n2Day = 28;
            if ((nMon < 1) || (nMon > 12)) aRet[0] = false;
            if (aRet[0] == true) {
                switch (nMon) {
                    case 2: nKDay = n2Day;
                    case 4: nKDay = 30;
                    case 6: nKDay = 30;
                    case 9: nKDay = 30;
                    case 11: nKDay = 30;
                    default: nKDay = 31;
                }
                if ((nDay < 1) || (nDay > nKDay)) aRet[0] = false;
            }
        }
    }
    return aRet;
}

// 檢查輸入的字串是否符合時間格式
function CheckTime(sTime, bPassNoData) {
    var nH, nM, nS, nLen;
    var aRet = new Array();
    var sTemplet = '1234567890:';

    aRet[0] = true;
    aRet[1] = '';
    sTime = Trim(sTime);
    nLen = sTime.length;
    if (nLen == 4) sTime = sTime.substr(0, 2) + ':' + sTime.substr(2, 2);
    if (nLen == 6) sTime = sTime.substr(0, 2) + ':' + sTime.substr(2, 2) + ':' + sTime.substr(4, 2);
    if ((sTime == '--:--') || (sTime == '--:--:--') || (sTime == '')) aRet[0] = bPassNoData;
    else {
        nLen = sTime.length;
        for (var i = 0; i < nLen; i++) {
            if (sTemplet.indexOf(sTime.charAt(i), 0) == -1) aRet[0] = false;
        }
        if (sTime.charAt(2) != ':') aRet[0] = false;
        if (nLen == 8) { if (sTime.charAt(5) != ':') aRet[0] = false; }
    }
    if (aRet[0] == true) {
        nH = parseInt(sTime.substr(0, 2), 10);
        nM = parseInt(sTime.substr(3, 2), 10);
        if ((nH < 0) || (nH > 23)) aRet[0] = false;
        if ((nM < 0) || (nM > 59)) aRet[0] = false;
        if (nLen == 8) {
            nS = parseInt(sTime.substr(6, 2), 10);
            if ((nS < 0) || (nS > 59)) aRet[0] = false;
        }
    }
    aRet[1] = sTime;
    return aRet;
}

//把左邊的空格刪除  
function Ltrim(s) {
    var flg = 0;
    var lstr = '';
    while (s.charCodeAt(flg) == 32) flg++;
    alert(flg);
    for (var index = 0; index < s.length - flg; index++)
        lstr += s.charAt(flg + index);
    return lstr;
}

//把右邊的空格刪除  
function Rtrim(s) {
    var flg = 0;
    var rstr = '';
    var strLength = s.length;
    while (s.charAt(strLength - 1 - flg) == ' ') flg++;
    for (var index = 0; index < s.length - flg; index++)
        rstr += s.charAt(index);
    return rstr;
}

//利用正規表達式刪除空格 
function ReplaceSpace(str) {
    return str.replace(/\s/g, '');
}

//刪除所有空格  
function RemoveAllSpace(str) {
    var localString = '';
    for (var index = 0; index < str.length; index++)
        if (str.charCodeAt(index) != 32) {
            localString += str.charAt(index);
        };
    return localString;
}

function ShowErrorMsg(msg) {
    alert(msg);
}

//日期轉換成字串(yyyy/MM/dd HH:mm:ss)
function ConvertDateToStr(objData) {
    var year = objData.getFullYear();
    var month = ((objData.getMonth() + 1) < 10 ? "0" + objData.getMonth() : objData.getMonth() + 1);
    var date = (objData.getDate() < 10 ? "0" + objData.getDate() : objData.getDate());
    var hour = objData.getHours();
    var minute = objData.getMinutes();
    var second = objData.getSeconds();
    return year + "/" + month + "/" + date + " " + hour + ":" + minute + ":" + second;
}


function CheckGlobalSessionStatus() {
    var defer = $.Deferred();
    $.ajax({
        type: "POST",
        url: "../../../ReCheck.ashx",    //執行驗證的頁面
        data: { "StatusName": "UserID", "PageEvent": "CheckStatus" },
        dataType: "json",
        success: function (data) {
            if (data.isSuccess) {
                defer.resolve(true);
            } else {
                alert(data.message);
                //產生登出視窗
                defer.resolve(false);
            }
            return defer.promise();
        }
    });
    return defer.promise();
}