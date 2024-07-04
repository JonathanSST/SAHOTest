//*主要作業畫面：執行「查詢」動作
function SelectState() {
    if (pdSDate.value === '' || pdEDate.value === '') {
        alert("起訖時間不得為空！");
    }
    else {
        //var SDate = new Date(pdSDate.value);
        //var EDate = new Date(pdEDate.value);
        //var DiffDate = (EDate - SDate) / 86400000;

        //if(DiffDate > 6){
        //    alert("起訖時間不得超過七天！");
        //}
        //else{
        Block();
        SelectValue.value = '';
        __doPostBack(QueryButton.id, '');
        //}
    }
}