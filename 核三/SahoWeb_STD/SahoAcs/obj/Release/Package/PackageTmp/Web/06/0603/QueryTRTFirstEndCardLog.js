//************************************************** 網頁畫面設計(一)相關的JavaScript方法 **************************************************

//*主要作業畫面：執行「查詢」動作
function SelectState() {
    Block();
    hComplexQueryWheresql.value = '';
    __doPostBack(QueryButton.id, '');
}

//*主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}


//主要作業畫面：完成動作設定網頁相關的參數內容
function ExcuteComplete(objRet) {
    switch (objRet.result) {
        case true:
            switch (objRet.act) {
                case 'ComplexQuery':
                    hComplexQueryWheresql.value = objRet.message;
                    __doPostBack(ComplexQueryButton.id, '');
                    break;
            }

            CancelTrigger0.click();
            break;
        case false:
            alert(objRet.message);
            break;
        default:
            break;
    }
}
