//************************************************** 網頁畫面設計(一)相關的JavaScript方法 **************************************************

//主要作業畫面：執行「查詢」動作
function SelectState() {
    ContentPlaceHolder1_sCtrlModel.value = ContentPlaceHolder1_ddlCtrlModel.value;
    ContentPlaceHolder1_sLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    ContentPlaceHolder1_tmpLocArea.value = ContentPlaceHolder1_ddlLocArea.value;
    ContentPlaceHolder1_sLocBuilding.value = ContentPlaceHolder1_ddlLocBuilding.value;
    ContentPlaceHolder1_tmpLocBuilding.value = ContentPlaceHolder1_ddlLocBuilding.value;
    ContentPlaceHolder1_sLocFloor.value = ContentPlaceHolder1_ddlLocFloor.value;

    __doPostBack(QueryButton.id, '');
}

//主要作業畫面：自動設置「GridView」控制項資料的選項位置(動作在xObj.js中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, hSelectValue.value);
}

function SelectAll(HeaderCheckState) {
    var itemChk = $("#ContentPlaceHolder1_MainGridView [id*=RowCheckState]");
    itemChk.each(function () {
        this.checked = HeaderCheckState.checked;
    });  
}

function SetStatus() {
    ContentPlaceHolder1_sSenStatus.value = "1";
    if (ContentPlaceHolder1_SenStatus_0.checked) {
        ContentPlaceHolder1_sSenStatus.value = "0";
    } else if (ContentPlaceHolder1_SenStatus_2.checked) {
        ContentPlaceHolder1_sSenStatus.value = "2";
    }
}
