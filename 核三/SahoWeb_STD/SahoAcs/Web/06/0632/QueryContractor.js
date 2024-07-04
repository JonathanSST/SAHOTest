//設定使用者按鈕的權限
function SetUserLevel(str) {
    ViewButton.disabled = true;

    if (str != '') {

        var data = str.split(",");

        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'View') {
                ViewButton.disabled = false;
            }
        }
    }
}

//*主要作業畫面：執行「查詢」動作
function SelectState() {
    Block();
    SelectValue.value = '';
    //console.log(QueryButton.id);
    __doPostBack(QueryButton.id, '');
}

//*主要作業畫面：設定報到時間和設備群組
function SetValue() {
    Block();
    SelectValue.value = '';
    __doPostBack(btnSetup.id, '');
}

//*主要作業畫面：執行「匯出」動作
function ExportQuery() {
    SelectValue.value = '';
    __doPostBack(ExportButton.id, '');
}

//*主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}
