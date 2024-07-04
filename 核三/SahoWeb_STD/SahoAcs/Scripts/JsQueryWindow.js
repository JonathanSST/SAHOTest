//查詢系統功能目錄
function SelectMenu(MenuName) {        
    var returnValue = window.showModalDialog("../Tool/MenuList.aspx?key=" + encodeURIComponent(userObj.value), window, "dialogHeight:500px;dialogWidth:500px;");
    return returnValue;
}


//查詢廠商資料
function SelectFactory() {
    var returnValue = window.showModalDialog("../Tool/FactoryQuery.aspx", window, "dialogHeight:500px;dialogWidth:500px;");
    return returnValue;
}


