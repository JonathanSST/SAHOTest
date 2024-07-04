// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'Add':
            //popInput_No_Title.disabled = false;
            //popInput_No_Title.style.display = "none";
            //popInput_No.setAttribute("style", "width:370px");
            //popInput_No.disabled = false;
            //popInput_Name.disabled = false;
            //popInput_Class.disabled = false;
            //popInput_No_Title.value = '';
            //popInput_No.value = '';
            //popInput_Name.value = '';
            //popB_Add.style.display = "inline";
            //popB_Edit.style.display = "none";
            //ChangeText(DeleteLableText, '');
            break;
        case 'Edit':
            //popInput_No_Title.disabled = true;
            //popInput_No_Title.style.display = "inline";
            //popInput_No_Title.setAttribute("style", "width:20px;text-align: center");
            //popInput_No.setAttribute("style", "width:345px");
            //popInput_No.disabled = false;
            //popInput_Name.disabled = false;
            //popInput_Class.disabled = false;
            //popB_Add.style.display = "none";
            //popB_Edit.style.display = "inline";
            //ChangeText(DeleteLableText, '');
            break;
        case 'Delete':
            //popInput_No2.disabled = true;
            //popInput_Name2.disabled = true;
            //popInput_Class2.disabled = true;
            //popInput_Class3.disabled = false;
            //popB_Delete.style.display = "inline";
            break;
        case '':
            //popInput_No.disabled = false;
            //popInput_Name.disabled = false;
            //popInput_Class.disabled = false;
            //popInput_No.value = '';
            //popInput_Name.value = '';
            //popB_Add.style.display = "none";
            //popB_Edit.style.display = "none";
            //popB_Delete.style.display = "none";
            break;
    }
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectPsnNo.value);
}