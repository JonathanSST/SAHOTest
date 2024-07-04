
var IsEnabled = false;              // 頁面載入時，預防滑鼠一開始就停在 Item 上面，所造成的影響，而設的變數
var IsBegin = false;                // 同上
var SaveColor = new Array();
var SaveBGColor = new Array();

function SetHighLifthEnabled() { IsEnabled = true; }        // 本函數必須以計時器於頁面載入一秒後執行

// 進入 Item 時，記錄該 Item 的背景及文字的顏色
function onMouseMoveIn(nGroup, Source, FontColor, BackRoundColor) {

    if (!IsEnabled) return;
    IsBegin = true;
    if (FontColor == '') FontColor = '#ffffff';                                 //沒傳值即預設為白色文字
    if (BackRoundColor == '') BackRoundColor = '#6bc5dd';                       //沒傳值即預設為淡藍色底色
    SaveColor[nGroup] = Source.style.Color;
    if (SaveColor[nGroup] == undefined) SaveColor[nGroup] = '#000000';          //文字儲存若無值則預設為黑色
    SaveBGColor[nGroup] = Source.style.backgroundColor;
    if (SaveBGColor[nGroup] == undefined) SaveBGColor[nGroup] = '#FFFFFF';      //底色儲存若無值則預設為白色
    Source.style.color = FontColor;
    Source.style.backgroundColor = BackRoundColor;
    //alert(SaveBGColor[nGroup]);
}

// 離開 Item 時，回復該 Item 原來的背景及文字的顏色
function onMouseMoveOut(nGroup, Source) {
    if ((!IsEnabled) || (!IsBegin)) return;
    Source.style.color = SaveColor[nGroup];
    Source.style.backgroundColor = SaveBGColor[nGroup];
}

var SaveSingleSelectRow = new Array();
var SaveSingleSelectBeforeColor = new Array();
var SaveSingleSelectBeforeBGColor = new Array();
var SaveSingleSelectBeforeFontWeight = new Array();

// Item 單選時處理，且搭配滑鼠移入移出改變 Item 顏色
function SingleRowSelect(nGroup, Source, oSelect, value, FontColor, BackRoundColor, CallBackFunction) {    
    if ((!IsEnabled) || (!IsBegin)) return;
    if (SaveSingleSelectRow[nGroup] != Source) {
        // 如果記錄的變數不是空值
        if (SaveSingleSelectRow[nGroup] != undefined) {
            SaveSingleSelectRow[nGroup].ID = "";
            SaveSingleSelectRow[nGroup].style.fontWeight = SaveSingleSelectBeforeFontWeight[nGroup];        // 回復原來的字體粗細
            SaveSingleSelectRow[nGroup].style.color = SaveSingleSelectBeforeColor[nGroup];                  // 回復正常的字體顏色
            SaveSingleSelectRow[nGroup].style.backgroundColor = SaveSingleSelectBeforeBGColor[nGroup];      // 回復正常的背景顏色
        }
        SaveSingleSelectRow[nGroup] = Source;                                                               // 記錄被選到的 Item

        // 由於滑鼠移入時顏色已被改變了，所以要記錄的是滑鼠移入前的顏色
        SaveSingleSelectBeforeFontWeight[nGroup] = SaveSingleSelectRow[nGroup].style.fontWeight;            // 保存原來的字體粗細
        SaveSingleSelectBeforeColor[nGroup] = SaveColor[nGroup];                                            // 保存正常的字體顏色
        SaveSingleSelectBeforeBGColor[nGroup] = SaveBGColor[nGroup];                                        // 保存正常的背景顏色

        // 改成被選到到的顏色
        if (FontColor == '') FontColor = '#000000';
        if (BackRoundColor == '') BackRoundColor = '#6bc5dd';
        //Source.ID = "MyItemSelected";
        //        Source.style.fontWeight = "bold";                                     // 設定被選到的 Item 字體變粗
        Source.style.color = FontColor;                                         // 設定被選到的 Item 字體顏色
        Source.style.backgroundColor = BackRoundColor;                          // 設定被選到的 Item 背景顏色

        // 改變先前滑鼠移入時所記錄的顏色，以供滑鼠移出使用
        SaveColor[nGroup] = Source.style.Color;
        if (SaveColor[nGroup] == undefined) SaveColor[nGroup] = '#000000';
        SaveBGColor[nGroup] = Source.style.backgroundColor;

        // 將選取到欄位的值存放至頁面控制項中
        if (oSelect != '') oSelect.value = value;

        // 如果有設定要執行函式就執行
        if (CallBackFunction != undefined) CallBackFunction(Source);
    }
}


