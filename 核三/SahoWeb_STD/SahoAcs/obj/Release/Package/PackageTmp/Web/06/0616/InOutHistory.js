//執行「查詢」動作
function SelectState() {
    Block();
    __doPostBack(QueryButton.id, '');
}

//控制器跟設備群組條件互斥
function CheckDrop(sMode) {
    if (sMode === "Ctrl" && dropEquGroup.selectedIndex !== 0) {
        dropEquGroup.selectedIndex = 0;
    }
    else if (sMode === "EquGroup" && dropCtrl.selectedIndex !== 0) {
        dropCtrl.selectedIndex = 0;
    }
}