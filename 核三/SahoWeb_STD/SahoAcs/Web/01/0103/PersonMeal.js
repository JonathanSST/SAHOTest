var spanChk1;
var CodeStatus = 0;

//document.getElementById("BtCardGroup");
// 依照模式設定各按鈕的啟用狀態
function SetMode(sMode) {
    switch (sMode) {
        case 'CardAdd':            
            popInput_CardNo.disabled = false;
            popB_Add.style.display = "inline";
            popB_Edit.style.display = "none";
            popB_Delete.style.display = "none";
            popInput_CardType.disabled = false;
            break;
        case 'CardEdit':
            popInput_CardNo.disabled = true;
            popB_Add.style.display = "none";
            popB_Edit.style.display = "inline";
            popB_Delete.style.display = "inline";
            popInput_CardType.disabled = true;
            break;
        case 'Edit':
            btCardInfo.style.display = "inline";
            btCardAdd.style.display = "inline";            
            btDelete.style.display = "inline";
            if (ShowCardAuthAdj.value === "1") {
                $("#BtCardGroup").css("display", "inline");
            }
            $("#BtSetting").css("display", "inline");
            $("#btnChange").css("display", "inline");
            $("#btnPic").css("display", "inline");
            PsnPic.style.display = "inline";
            lblPsnPicSource.style.display = "inline";
            Input_PsnPicSource.style.display = "inline";
            lblCardInfo.style.display = "inline";
            List_CardInfo.style.display = "inline";
            lblCardNo.style.display = "none";
            Input_CardNo.style.display = "none";
            Input_CardVer.style.display = "none";
            lbl_CardVer.style.display = "none";
            CodeStatus = 0;
            btSave.disabled = false;
            break;
        case '':
            Input_PsnNo.disabled = false;
            Input_PsnNo.value = '';
            DefaultData();
            break;
    }
    SetUserLevel($("#FunAuthSet").val());
    //SetCheckMax();
}

//GridView自動設置位置 (動作在 xObj.js 中)
function GridSelect() {        
    GridGoToRow(0, tablePanel.id, 'MainGridView', 0, SelectNowNo.value);
}


function SetCheckMax() {
    if (parseInt($('input[name*="CurrentPerson"]:eq(0)').val()) >= parseInt($('input[name*="MaxPerson"]:eq(0)').val())) {
        btnPsnAdd.disabled = true;
        btCardAdd.disabled = true;
        if ($('input[name*="SelectValue"]:eq(0)').val() == "") {
            btSave.disabled = true;
        }
    } else {
        btnPsnAdd.disabled = false;
        btCardAdd.disabled = false;
        if ($('input[name*="SelectValue"]:eq(0)').val() == "") {
            btSave.disabled = false;
        }
    }
}


//設定使用者按鈕的權限
function SetUserLevel(str) {
    if ($('[name*="btSave"]').length == 0) {
        return false;
    }
    btSave.disabled = true;
    btDelete.disabled = true;
    btCardInfo.disabled = true;
    btCardAdd.disabled = true;
    //BtSetting.disabled = true;
    //BtCardGroup.disabled = true;
    $("#BtSetting").prop("disabled", true);
    $("#BtCardGroup").prop("disabled", true);
    $("#btnChange").prop("disabled", true);
    $("#btnPic").prop("disabled", true);
    BtnAuthMode.disabled = true;
    btnPsnAdd.disabled = true;
    popB_Add.disabled = true;
    popB_Edit.disabled = true;
    popB_Delete.disabled = true;

    if (str != '') {
        var data = str.split(",");
        for (var i = 0; i < data.length; i++) {
            if (data[i] == 'Add') {
                btSave.disabled = false;
                //btCardInfo.disabled = false;
                btCardAdd.disabled = false;
                $("#BtSetting").prop("disabled", false);
                $("#BtCardGroup").prop("disabled", false);
                BtnAuthMode.disabled = false;
                btnPsnAdd.disabled = false;
                popB_Add.disabled = false;
                SetCheckMax();
            }
            if (data[i] == 'Edit') {
                btSave.disabled = false;
                //btCardInfo.disabled = false;
                btCardAdd.disabled = false;
                $("#BtSetting").prop("disabled", false);
                $("#BtCardGroup").prop("disabled", false);
                $("#btnChange").prop("disabled", false);
                $("#btnPic").prop("disabled", false);
                BtnAuthMode.disabled = false;
                popB_Edit.disabled = false;
                SetCheckMax();
            }
            if (data[i] == 'Del') {
                btDelete.disabled = false;
                popB_Delete.disabled = false;
            }
            btCardInfo.disabled = false;
        }
    }    
}


//匯出資料
function ExportData() {
    window.open("ExportPerson.aspx?PsnName=" + $('input[name*="InputWord"]').val()
        + "&PsnType=" + $('[name*="dropPsnType"]').val()
        + "&Company=" + $('[name*="dropCompany"]').val()
        + "&Department=" + $('[name*="dropDepartment"]').val())
}

//新增人員
function AddPerson() {
    PageMethods.DefaultData(hUserId.value, hPsnId.value, ShowDefaultData, onPageMethodsError);
}

//呼叫編輯資料
function CallEdit(sNo) {    
    if (sNo != '') {
        SelectNowNo.value = sNo;
        //PageMethods.SingleData(SelectValue.value, hUserId.value, ShowPsnData, onPageMethodsError);
        PageMethods.SingleData(sNo, hUserId.value, ShowPsnData, onPageMethodsError);
        SetMode('Edit');
    }
}

//顯示選取的單人資料
function ShowPsnData(ResultArray) {
    var DataArray = ResultArray[0];
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        var str = '';
        var option = null;
        if (SelectValue.value == "") {
            SelectValue.value = DataArray[0];
        }        
        Input_PsnNo.value = DataArray[1];//員工編號
        Input_PsnName.value = DataArray[2];//姓名
        Input_PsnEName.value = DataArray[3];//英文姓名
        Input_PsnType.value = DataArray[4];//選取人員類別
        Input_PsnIDNum.value = DataArray[5];//身份證號
        Input_PsnAccount.value = DataArray[8];//登入帳號
        Input_PsnPW.value = DataArray[9];//登入密碼
        Input_PsnAuthAllow.selectedIndex = DataArray[10];//權限狀態
        Input_PsnSTime.value = DataArray[11];//起始日期
        Input_PsnETime.value = DataArray[12] !== null ? DataArray[12] : '';//結束日期
        Input_PsnPicSource.value = DataArray[13];//圖片來源
        Input_Remark.value = DataArray[14];//備註

        $("#Input_Text1").val(DataArray[16]);//文字一
        $("#Input_Text2").val(DataArray[17]);//文字二
        $("#Input_Text3").val(DataArray[18]);//文字三
        $("#Input_Text4").val(DataArray[19]);//文字四
        $("#Input_Text5").val(DataArray[20]);//文字五

        Input_MealFood.selectedIndex = DataArray[21]; //葷素食

        $('input[name*="CardNo"]:eq(0)').attr("must_keyin_yn", "N");
        //生日
        if (DataArray[6] !== null) {
            for (var i = 0; i < Input_PsnBirthdayYear.children.length; i++) {
                if (Input_PsnBirthdayYear.options[i].value == DataArray[6].substr(0, 4)) {
                    Input_PsnBirthdayYear.options[i].selected = true;
                }
            }

            for (var i = 0; i < Input_PsnBirthdayMonth.children.length; i++) {
                if (Input_PsnBirthdayMonth.options[i].value == DataArray[6].substr(5, 2)) {
                    Input_PsnBirthdayMonth.options[i].selected = true;
                }
            }

            for (var i = 0; i < Input_PsnBirthdayDate.children.length; i++) {
                if (Input_PsnBirthdayDate.options[i].value == DataArray[6].substr(8, 2)) {
                    Input_PsnBirthdayDate.options[i].selected = true;
                }
            }
        }

        //組織列表處理
        var arrItemInfo2 = hItemInfo2.value.split('|');
        var arrOrgIDList = DataArray[DataArray.length - 4].slice(1, DataArray[DataArray.length - 4].length - 1).split('\\');
        var arrOrgNoList = DataArray[DataArray.length - 3].slice(1, DataArray[DataArray.length - 3].length - 1).split('\\');
        var j = 0;        
        for (var i = 0; i < arrItemInfo2.length; i++) {
            document.getElementById('ContentPlaceHolder1_dropOrg_' + arrItemInfo2[i]).value = "0";
            if (typeof(arrOrgNoList[j]) != 'undefined') {
                var ItemInfo2 = arrOrgNoList[j].slice(0, 1);
                if (arrItemInfo2[i] === ItemInfo2) {                    
                    document.getElementById('ContentPlaceHolder1_dropOrg_' + ItemInfo2).value = arrOrgIDList[j];
                    j++;
                }
                else {
                    document.getElementById('ContentPlaceHolder1_dropOrg_' + arrItemInfo2[i]).value = "0";
                }
            }            
        }
        //卡片資訊
        //console.log(List_CardInfo);
        while (List_CardInfo.length > 0) {            
            List_CardInfo.remove(0);
        }        
        if (DataArray[DataArray.length - 2] != '') {
            str = DataArray[DataArray.length - 2].split('|');

            for (var i = 0; i < str.length; i += 3) {
                option = document.createElement("option");
                option.text = '[' + str[i + 1] + ']  ' + str[i];
                option.value = str[i + 2];
                List_CardInfo.options.add(option);
            }
            List_CardInfo.options[0].selected = true;
        }        
        if ($("select[name*='ddlVerifiMode']:eq(0) > option").length > 0) {
            BtnAuthMode.style.display = 'block';
            $("select[name*='ddlVerifiMode']:eq(0)").val(ResultArray[1]);
        }
        //相片顯示
        PicShow(DataArray[DataArray.length - 1]);
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//日期選項
function BirthdayData(year, cnt) {
    var option = null;
    //Year
    for (var i = year; i > year - cnt; i--) {
        option = document.createElement("option");
        option.text = i;
        option.value = i;
        Input_PsnBirthdayYear.add(option);
    }
    //Month
    for (var i = 1; i <= 12; i++) {
        option = document.createElement("option");
        option.text = padLeft(i.toString(), 2);
        option.value = padLeft(i.toString(), 2);
        Input_PsnBirthdayMonth.add(option);
    }
    //Day
    for (var i = 1; i <= 31; i++) {
        option = document.createElement("option");
        option.text = padLeft(i.toString(), 2);
        option.value = padLeft(i.toString(), 2);
        Input_PsnBirthdayDate.add(option);
    }
}

//左邊補0
function padLeft(str, lenght) {
    if (str.length >= lenght) {
        return str;
    }
    else {
        return padLeft("0" + str, lenght);
    }
}

//右邊補0
function padRight(str, lenght) {
    if (str.length >= lenght) {
        return str;
    }
    else {
        return padRight(str + "0", lenght);
    }
}

//各動作完成
function Excutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            alert(sRet.message);
            break;
        default:
            switch (sRet.act) {
                case "Add":
                    var data = sRet.message.split("|");
                    SelectNowNo.value = data[0];
                    SelectValue.value = data[1];
                    alert(data[2]);
                    CallEdit(SelectNowNo.value);
                    break;
                case "Edit":
                    SelectNowNo.value = Input_PsnNo.value.trim();
                    alert(sRet.message);
                    break;
                case "Delete":
                    SelectNowNo.value = '';
                    SelectValue.value = '';
                    SetMode('');
                    break;
            }

            //InputWord.value = "";
            if (SelectNowNo.value != "") {
                __doPostBack(UpdatePanel1.id, 'popPagePost');
            } else {
                __doPostBack(UpdatePanel1.id, 'NewQuery');
            }                        
            break;
    }
    $.unblockUI();
}

//動態輸入相片檔名
function InputPicText() {
    PageMethods.PicChange(Input_PsnPicSource.value, PicShow, onPageMethodsError);
}

//顯示照片
function PicShow(data) {
    PsnPic.src = data;
}

//初始動作
function DefaultData() {
    PageMethods.DefaultData(hUserId.value, hPsnId.value, ShowDefaultData, onPageMethodsError);
}


function CheckKeyUp() {
    var x = event.which || event.keyCode;
    //console.log(x);
    if (x == 13) {
        SelectState();
    }
}



//將初始帶入UI
function ShowDefaultData(DataArray) {
    SelectValue.value = '';
    SelectNowNo.value = '';
    Input_PsnNo.value = '';
    Input_PsnName.value = '';
    Input_PsnEName.value = '';
    Input_PsnIDNum.value = '';
    Input_PsnAccount.value = '';
    Input_PsnPW.value = '';
    Input_CardNo.value = '';
    Input_PsnAuthAllow.selectedIndex = 1;
    Input_PsnSTime.value = DataArray[1];
    Input_PsnETime.value = '';
    Input_PsnPicSource.value = '';
    Input_Remark.value = '';
    Input_PsnBirthdayYear.selectedIndex = 0;
    Input_PsnBirthdayMonth.selectedIndex = 0;
    Input_PsnBirthdayDate.selectedIndex = 0;
    Input_MealFood.selectedIndex = 0;
    BtnAuthMode.style.display = "none";
    PsnPic.src = '/Img/default.png';
    $('input[name*="CardNo"]:eq(0)').attr("must_keyin_yn", "Y");
    var arrItemInfo2 = hItemInfo2.value.split('|');

    for (var i = 0; i < $("select[id^='ContentPlaceHolder1_dropOrg_']").length; i++) {
        $("#ContentPlaceHolder1_dropOrg_" + arrItemInfo2[i])[0].selectedIndex = 0
    }
    if ($('[name*="DefaultOrg"]').val() != "") {
        //$("#ContentPlaceHolder1_dropOrg_C")[0].selectedIndex = 1;
        $("select[id^='ContentPlaceHolder1_dropOrg_C'] option:contains(" + $('[name*="DefaultOrg"]').val() + ")").prop('selected', 'selected');
    }
    while (Input_PsnType.length > 0) {
        Input_PsnType.remove(0);
    }
    
    if (DataArray[0] != '') {
        var str = DataArray[0].split('|');

        for (var i = 0; i < str.length; i += 2) {
            option = document.createElement("option");
            option.text = str[i + 1];
            option.value = str[i];
            Input_PsnType.options.add(option);
        }
    }

    //Input_Text1.value = "";
    //Input_Text2.value = "";
    //Input_Text3.value = "";
    //Input_Text4.value = "";
    //Input_Text5.value = "";
    $('input[name*="Input_Text"]').val("");    
    lblCardNo.style.display = "";
    Input_CardNo.style.display = "";
    lbl_CardVer.style.display = "";
    Input_CardVer.style.display = "";
    PsnPic.style.display = "none";
    lblPsnPicSource.style.display = "none";
    Input_PsnPicSource.style.display = "none";
    lblCardInfo.style.display = "none";
    List_CardInfo.style.display = "none";
    btCardInfo.style.display = "none";
    btCardAdd.style.display = "none";    
    $("#BtCardGroup").css("display", "none");
    $("#BtSetting").css("display", "none");
    $("#btnChange").css("display", "none");
    $("#btnPic").css("display","none");
    btDelete.display = "none";
    btSave.disabled = btnPsnAdd.disabled;
}

//人員資料存檔
function SaveData(UserId) {   
    if ($('input[name*="IDNum"]').attr("MUST_KEYIN_YN") == "Y" && $('input[name*="IDNum"]').prop("maxlength") != $('input[name*="IDNum"]').val().length) {
        alert("身份證號長度必須為" + $('input[name*="IDNum"]').prop("maxlength")+"\n身份證號為必填");
        $('input[name*="IDNum"]').focus();
        return false;
    }
    //生日組合
    var birthday = Input_PsnBirthdayYear.options[Input_PsnBirthdayYear.selectedIndex].value + '/' +
        Input_PsnBirthdayMonth.options[Input_PsnBirthdayMonth.selectedIndex].value + '/' +
        Input_PsnBirthdayDate.options[Input_PsnBirthdayDate.selectedIndex].value;
    //組織列表組合
    var OrgListCount = $("select[id^='ContentPlaceHolder1_dropOrg_']").length;
    var arrItemInfo2 = hItemInfo2.value.split('|');
    var OrgIDList = "", OrgNoList = "";

    /*原來的組織架構設定
    for (var i = 0; i < OrgListCount; i++) {
        var arrOrgNo = $("#ContentPlaceHolder1_dropOrg_" + arrItemInfo2[i] + " :selected").text().split('.');
        if ($("#ContentPlaceHolder1_dropOrg_" + arrItemInfo2[i]).val() !== "0") {
            OrgIDList += "\\" + $("#ContentPlaceHolder1_dropOrg_" + arrItemInfo2[i]).val();
            OrgNoList += arrOrgNo[1] + ".";
        }        
    }
    */
    $('[name*="dropOrg_"]').each(function () {
        var that = $(this);
        if ($(that).val() !== "0") {
            OrgIDList += "\\" + $(that).val();
            var arrOrgNo = $(that).find(":selected").text().split('.');
            OrgNoList += arrOrgNo[1] + "."
        }
    });

    OrgIDList += "\\";
    OrgNoList = OrgNoList.slice(0, OrgNoList.length - 1);
    //檢查該組織架構是否存在
    //if (JsFunBASE_VERTIFY()) {
    //Block();
        PageMethods.CheckOrgStruc(UserId, birthday, OrgIDList, OrgNoList, SaveDataCheck);
    //}
}

//人員資料的新增及更新動作
function SaveDataCheck(CheckData) {
    var UserId = CheckData[0];
    var birthday = CheckData[1];
    var OrgIDList = CheckData[2];
    var OrgNoList = CheckData[3];
    var txt1 = $("#Input_Text1").length > 0 ? $("#Input_Text1").val() : "";
    var txt2 = $("#Input_Text2").length > 0 ? $("#Input_Text2").val() : "";
    var txt3 = $("#Input_Text3").length > 0 ? $("#Input_Text3").val() : "";
    var txt4 = $("#Input_Text4").length > 0 ? $("#Input_Text4").val() : "";
    var txt5 = $("#Input_Text5").length > 0 ? $("#Input_Text5").val() : "";
    if (SelectValue.value != "") {
        if ($('input[name*="Input_PsnETime"]').val() < $('input[name*="Input_PsnSTime"]').val()) {
            alert($("#AlertMsg").val());
            return false;
        }
    }
    if (CheckData[4] === "2") {
        alert("組織架構格式錯誤，無法進行人員資料異動")
        return false;
    }
    if (CheckData[4] === "0") {
        if (confirm("該組織架構不存在，該人員無預設權限，是否繼續處理?")) {
            //自訂欄位傳入參數若畫面沒有則傳入""空字串即可
            //console.log(1234);
            //console.log(Input_CardVer.value);
            //Input_CardNo.value = isValue(Input_CardNo);
            
            if (SelectValue.value == '') {                
                    PageMethods.Insert(Input_PsnNo.value.trim(), Input_PsnName.value.trim(), Input_PsnEName.value.trim(),
                    Input_PsnType.options[Input_PsnType.selectedIndex].value, Input_PsnIDNum.value.trim(), birthday,
                    OrgIDList, OrgNoList, Input_CardNo.value.trim(), Input_PsnAccount.value.trim(), Input_PsnPW.value.trim(),
                    Input_PsnAuthAllow.options[Input_PsnAuthAllow.selectedIndex].value, Input_PsnSTime.value,
                        Input_PsnETime.value, Input_PsnPicSource.value.trim(), Input_Remark.value,
                        txt1, txt2, txt3, txt4, txt5, ddlVerifiMode.value, UserId, Input_CardVer.value.trim(), Input_MealFood.value,
                        Excutecomplete, onPageMethodsError);                
            }
            else {
                PageMethods.Update(SelectValue.value, SelectNowNo.value, Input_PsnNo.value.trim(), Input_PsnName.value.trim(),
                    Input_PsnEName.value.trim(), Input_PsnType.options[Input_PsnType.selectedIndex].value,
                    Input_PsnIDNum.value.trim(), birthday, OrgIDList, OrgNoList, Input_PsnAccount.value.trim(),
                    Input_PsnPW.value.trim(), Input_PsnAuthAllow.options[Input_PsnAuthAllow.selectedIndex].value,
                    Input_PsnSTime.value, Input_PsnETime.value, Input_PsnPicSource.value.trim(), Input_Remark.value,
                    txt1, txt2, txt3, txt4, txt5, ddlVerifiMode.value, UserId,Input_MealFood.value,
                    Excutecomplete, onPageMethodsError);
            }
        } else {
            $.unblockUI();
        }
    }
    else {
        //自訂欄位傳入參數若畫面沒有則傳入""空字串即可
        if (SelectValue.value == '') {
            PageMethods.Insert(Input_PsnNo.value.trim(), Input_PsnName.value.trim(), Input_PsnEName.value.trim(),
                Input_PsnType.options[Input_PsnType.selectedIndex].value, Input_PsnIDNum.value.trim(), birthday,
                OrgIDList, OrgNoList, Input_CardNo.value.trim(), Input_PsnAccount.value.trim(), Input_PsnPW.value.trim(),
                Input_PsnAuthAllow.options[Input_PsnAuthAllow.selectedIndex].value, Input_PsnSTime.value,
                Input_PsnETime.value, Input_PsnPicSource.value.trim(), Input_Remark.value,
                txt1, txt2, txt3, txt4, txt5, ddlVerifiMode.value, UserId, Input_CardVer.value.trim(), Input_MealFood.value,
                Excutecomplete, onPageMethodsError);
        }
        else {
            PageMethods.Update(SelectValue.value, SelectNowNo.value, Input_PsnNo.value.trim(), Input_PsnName.value.trim(),
                Input_PsnEName.value.trim(), Input_PsnType.options[Input_PsnType.selectedIndex].value,
                Input_PsnIDNum.value.trim(), birthday, OrgIDList, OrgNoList, Input_PsnAccount.value.trim(),
                Input_PsnPW.value.trim(), Input_PsnAuthAllow.options[Input_PsnAuthAllow.selectedIndex].value,
                Input_PsnSTime.value, Input_PsnETime.value, Input_PsnPicSource.value.trim(), Input_Remark.value,
                txt1, txt2, txt3, txt4, txt5, ddlVerifiMode.value, UserId, Input_MealFood.value,
                Excutecomplete, onPageMethodsError);
        }
    }
}

//人員資料刪除檢查
function DeleteData(UserId) {
    PageMethods.DeleteConfirm(SelectValue.value, SelectNowNo.value, UserId, DeleteExcute, onPageMethodsError);
}

//人員資料刪除
function DeleteExcute(sRet) {
    switch (sRet.result) {
        case false:
            alert(sRet.message);
            break;
        case true:
            var data = sRet.message.split('|');

            if (confirm(data[0])) {
                PageMethods.Delete(SelectValue.value, SelectNowNo.value, data[1], Excutecomplete, onPageMethodsError);
            }

            while (List_CardInfo.length > 0) {
                List_CardInfo.remove(0);
            }

            break;
    }
}

//呼叫卡片新增
function CallCardAdd(Title) {
    ChangeText(L_popName1, Title);
    SetMode('CardAdd');
    PageMethods.DefaultCardData(ShowCardDefaultData, onPageMethodsError);
}

//顯示卡片新增資訊
function ShowCardDefaultData(DataArray) {
    PopupTrigger1.click();
    SelectCardValue.value = '';
    popInput_CardNo.value = '';
    popInput_CardVer.value = '';
    popInput_CardPW.value = '';
    popInput_CardSerialNo.value = '';
    popInput_CardNum.value = '';
    popInput_CardSTime.value = '';
    popInput_CardETime.value = '';
    popInput_CardDesc.value = '';

    while (popInput_CardType.length > 0) {
        popInput_CardType.remove(0);
    }

    if (DataArray[0] != '') {        
        var str = DataArray[0].split('|');        
        for (var i = 0; i < str.length; i += 2) {
            option = document.createElement("option");
            option.text = str[i + 1];
            option.value = str[i];
            popInput_CardType.options.add(option);
        }
    }

    while (popInput_CardAuthAllow.length > 0) {
        popInput_CardAuthAllow.remove(0);
    }

    if (DataArray[1] != '') {
        var str = DataArray[1].split('|');

        for (var i = 0; i < str.length; i += 2) {
            option = document.createElement("option");
            option.text = str[i + 1];
            option.value = str[i];            
            popInput_CardAuthAllow.options.add(option);
        }
    }
    popInput_CardAuthAllow.options[1].selected = true;
    popInput_CardSTime.value = DataArray[2];

    $("#ContentPlaceHolder1_GridView1").find("tr").each(function () {        
        $(this).hide();
    });
}

//呼叫卡片編輯
function CallCardEdit(Title) {
    ChangeText(L_popName1, Title);
    SetMode('CardEdit');
    var cardid = '';

    if (List_CardInfo.options[List_CardInfo.selectedIndex] !== undefined) {
        cardid = List_CardInfo.options[List_CardInfo.selectedIndex].value;
    }

    PageMethods.SingleCardData(cardid, ShowCardSingleData, onPageMethodsError);
}

//顯示卡片編輯資訊
function ShowCardSingleData(DataArray) {
    console.log(DataArray);
    if (DataArray[0] != 'Saho_SysErrorMassage') {
        PopupTrigger1.click();
        SelectCardValue.value = DataArray[0];
        popInput_CardNo.value = DataArray[1];
        popInput_CardVer.value = DataArray[2];
        popInput_CardPW.value = DataArray[3];
        popInput_CardSerialNo.value = DataArray[4];
        popInput_CardNum.value = DataArray[5];
        popInput_CardSTime.value = DataArray[9];
        popInput_CardETime.value = DataArray[10];
        popInput_CardDesc.value = DataArray[11];

        //卡片類型
        while (popInput_CardType.length > 0) {
            popInput_CardType.remove(0);
        }

        if (DataArray[DataArray.length - 3] != '') {
            var str = DataArray[DataArray.length - 3].split('|');

            for (var i = 0; i < str.length; i += 2) {
                option = document.createElement("option");
                option.text = str[i + 1];
                option.value = str[i];
                popInput_CardType.options.add(option);
            }
        }

        if (popInput_CardType.children.length > 0) {
            for (var i = 0; i < popInput_CardType.children.length; i++) {
                if (popInput_CardType.options[i].value == DataArray[6]) {
                    popInput_CardType.options[i].selected = true;
                    //if (DataArray[6] == "T") {
                    //    popB_Edit.disabled = true;
                    //    popB_Delete.disabled = true;
                    //} else {
                    //    popB_Edit.disabled = false;
                    //    popB_Delete.disabled = false;
                    //}
                }
            }
        }

        //卡片權限
        while (popInput_CardAuthAllow.length > 0) {
            popInput_CardAuthAllow.remove(0);
        }

        if (DataArray[DataArray.length - 2] != '') {
            var str = DataArray[DataArray.length - 2].split('|');

            for (var i = 0; i < str.length; i += 2) {
                option = document.createElement("option");
                option.text = str[i + 1];
                option.value = str[i];
                popInput_CardAuthAllow.options.add(option);
            }
        }

        if (popInput_CardAuthAllow.children.length > 0) {
            for (var i = 0; i < popInput_CardAuthAllow.children.length; i++) {
                if (popInput_CardAuthAllow.options[i].value == DataArray[8]) {
                    popInput_CardAuthAllow.options[i].selected = true;
                }
            }
        }

        //卡片設碼資料        
        var equ_array = []; // 空陣列
        if (DataArray[DataArray.length - 1] != '') {            
            var str = DataArray[DataArray.length - 1].split('|');            
            for (var i = 0; i < str.length; i += 2) {
                //$("input[name='ctl00$ContentPlaceHolder1$GridView1$GV_Row" + str[i] + "$RowCheckState1']").click();
                if (str[i + 1].indexOf("ADM100FP") > 0)
                {                    
                    $("select[name*='GV_Row" + str[i] + "$ddlCardVirife']:eq(0)").attr("disabled", false);                    
                    $("select[name*='GV_Row" + str[i] + "$ddlCardVirife']:eq(0)").val(str[i + 1].split('%')[2]);
                }
                equ_array.push(str[i]);
            }
        }
        //逐步設定認證欄位
        $("#ContentPlaceHolder1_GridView1").find("tr").each(function () {            
            //$(this).find("td:eq(4)").css("text-align", "center");
            if (equ_array.indexOf($(this).find("#EquID").val()) < 0) {
                $(this).hide();
            } else {
                $(this).show();
                //$(this).find("td:eq(4)").css("text-align", "center");
            }
        });        
    }
    else {
        var sRet = new Object;
        sRet.result = false;
        sRet.message = DataArray[1];
        Excutecomplete(sRet);
    }
}

//新增、更新卡片資料
function SaveCardData(UserId) {
    var adm100setting = "";
    $("#ContentPlaceHolder1_GridView1").find("tr").each(function () {
        //adm100setting+=$(this).find("#EquID").val()+"%";        
        if($(this).find("select[name*='ddlCardVirife']").length > 0)
        {
            adm100setting +="|"+ $(this).find("#EquID").val() + "%" + $(this).find("select[name*='ddlCardVirife']").val();
        }
    });
    if ($('input[name*="popInput_CardPW"]').prop("maxlength") != $('input[name*="popInput_CardPW"]').val().length) {
        alert($('input[name*="hCardPwdAlertMsg"]').val());
        $('input[name*="popInput_CardPW"]').focus();
        return false;
    }
    //var cardno = isValue(popInput_CardNo);
    var cardno = popInput_CardNo.value;
    //console.log(cardno);
    if (SelectCardValue.value == '') {        
        PageMethods.CardInsert(SelectValue.value, popInput_CardNo.value, popInput_CardVer.value,
            popInput_CardPW.value, popInput_CardSerialNo.value, popInput_CardNum.value,
            popInput_CardType.options[popInput_CardType.selectedIndex].value,
            popInput_CardAuthAllow.options[popInput_CardAuthAllow.selectedIndex].value, popInput_CardSTime.value,
            popInput_CardETime.value, popInput_CardDesc.value, UserId, CardExcutecomplete, onPageMethodsError);
    }else {
        PageMethods.CardUpdate(SelectValue.value, SelectCardValue.value, cardno,
            popInput_CardVer.value, popInput_CardPW.value, popInput_CardSerialNo.value,
            popInput_CardNum.value, popInput_CardType.options[popInput_CardType.selectedIndex].value,
            popInput_CardAuthAllow.options[popInput_CardAuthAllow.selectedIndex].value, popInput_CardSTime.value,
            popInput_CardETime.value, popInput_CardDesc.value, adm100setting, UserId, CardExcutecomplete, onPageMethodsError);
    }
}

function DeleteCardData(UserId) {
    PageMethods.CardDelete(false, SelectValue.value, SelectCardValue.value, UserId, CardExcutecomplete, onPageMethodsError);
}


var card_id_value = '';
//各動作完成
function CardExcutecomplete(sRet) {
    switch (sRet.result) {
        case false:
            alert(sRet.message);
            break;
        default:
            switch (sRet.act) {
                case "Add":
                    card_id_value = SelectCardValue.value;
                    SelectCardValue.value = '';
                    ReCardList(sRet.message);
                    break;
                case "Edit":
                    card_id_value = SelectCardValue.value;
                    SelectCardValue.value = '';
                    ReCardList(sRet.message);
                    break;
                case "Delete":
                    DeleteCard(sRet.message)
                    break;
            }
            break;
    }
    $('[name*="CurrentPerson"]').val(sRet.CurrentData);
    console.log($('[name*="CurrentPerson"]').val());
    console.log($('[name*="MaxPerson"]').val());
    SetCheckMax();
}

//刪除卡片資料
function DeleteCard(data) {
    if (data != '') {
        var str = data.split('|');

        if (str[1] == 'False') {
            if (confirm(str[0])) {
                PageMethods.CardDelete(true, SelectValue.value, SelectCardValue.value, hUserId.value, CardExcutecomplete, onPageMethodsError);
            }
        }
        else {
            SelectCardValue.value = '';
            CancelTrigger1.click();

            while (List_CardInfo.length > 0) {
                List_CardInfo.remove(0);
            }

            alert(str[0]);

            if (str[2] != '') {
                for (var i = 2; i < str.length; i += 3) {
                    option = document.createElement("option");
                    option.text = '[' + str[i + 1] + ']  ' + str[i];
                    option.value = str[i + 2];
                    List_CardInfo.options.add(option);
                }
            }
        }
    }
}

//重整卡片List
function ReCardList(data) {
    CancelTrigger1.click();

    while (List_CardInfo.length > 0) {
        List_CardInfo.remove(0);
    }

    if (data != '') {
        str = data.split('|');
        alert(str[0]);

        for (var i = 1; i < str.length; i += 3) {
            option = document.createElement("option");
            option.text = '[' + str[i + 1] + ']  ' + str[i];
            option.value = str[i + 2];
            List_CardInfo.options.add(option);
        }
        $('select[name*="List_CardInfo"]').val(card_id_value);
    }
    //List_CardInfo.options[0].selected = true;
}

//取消Listbox選取
function UnSelectItem() {
    SelectCardValue.value = '';

    for (var i = 0; i < List_CardInfo.length; i++) {
        //List_CardInfo.options[i].selected = false;
    }
}

function SelectState() {
    SelectValue.value = '';    
    SelectCompanyNo.value = dropCompany.options[dropCompany.selectedIndex].value;
    SelectDepartmentNo.value = dropDepartment.options[dropDepartment.selectedIndex].value;
    SelectPsnTypeNo.value = dropPsnType.options[dropPsnType.selectedIndex].value;
    __doPostBack(QueryButton.id, 'NewQuery');
}

//設定所有卡片的認證模式
function SetAuthMode() {    
    PageMethods.SetCardVerifiMode(SelectValue.value, $("select[name*='ddlVerifiMode']:eq(0)").val(), hUserId.value, function (sRet) { alert(sRet.message); });    
}


//記錄下拉式選單index
function ddlState() {
    var gridview = document.getElementById("ContentPlaceHolder1_GridView1");
    var no = '';
    var ref = '';

    if (gridview != null) {
        for (var i = 0; i < gridview.rows.length; i++) {
            no += gridview.rows[i].cells[1].textContent + '|';
        }
        elm = document.forms[0];
        var data = '';
        for (var i = 0; i < elm.length; i++) {
            if (elm[i].type == "select-one") {
                var str = elm[i].name.split('$');
                if (str[4] == "ddlCardRule") {
                    data += elm.elements[i].selectedIndex + '|';
                }
            }
        }

        var nodata = no.split('|');
        var seldata = data.split('|');
        for (var i = 0; i < nodata.length - 1; i++) {
            ref += nodata[i] + '|' + seldata[i] + '|';
        }
    }

    hddlState.value = ref;
}

//全選
function SelectAllCheckboxes(spanChk) {
    elm = document.forms[0];    
    switch (spanChk) {
        case spanChk1:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk1.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState1") {
                        if (elm.elements[i].checked != spanChk1.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk2:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk2.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState2") {
                        if (elm.elements[i].checked != spanChk2.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk3:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk3.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState3") {
                        if (elm.elements[i].checked != spanChk3.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        case spanChk4:
            for (var i = 0; i < elm.length; i++) {
                if (elm[i].type == "checkbox" && elm[i].id != spanChk4.id) {
                    var str = elm[i].name.split('$');
                    if (str[4] == "RowCheckState4") {
                        if (elm.elements[i].checked != spanChk4.checked)
                            elm.elements[i].click();
                    }
                }
            }
            break;
        default:
            break;
    }
}

//統計GV1所勾選的資料列
function GV1State() {
    elm = $("input[type='checkbox']");
    hRemoveData.value = '';
    var num = 0;
    var data = '';

    for (var i = 0; i < elm.length; i++) {
        if (elm[i].id != spanChk1.id) {
            var str = elm[i].name.split('$');
            if (str[4] == "RowCheckState1") {
                if (elm[i].checked == true) {
                    data += num + '|';
                    num++;
                }
            }
        }
    }

    hRemoveData.value += num + '|' + data;
}


function SetCardFloor(equ_id,floor_data)
{    
    window.open("../../../web/03/CardFloor2.aspx?EquID=" + equ_id
                            + "&FinalFloorData=" + floor_data
                            + "&CardID=" + SelectCardValue.value
                            , "", "height=400,width=405,status=no,toolbar=no,menubar=no,location=no,top=50,left=100", "");    
}



function isValue(val) {
    var v = val.value;
    if (v == 'NaN' || v == null || v == '') {
        return 0;
    } else {
        //全行轉半型
        result = "";
        //console.log("啟始" + String.fromCharCode(65249) + "End")
        for (i = 0; i <= v.length; i++) {
            if (v.charCodeAt(i) == 12288) {
                result += " ";
            } else {
                if (v.charCodeAt(i) > 65280 && v.charCodeAt(i) < 65375) {
                    result += String.fromCharCode(v.charCodeAt(i) - 65248);
                } else {
                    result += String.fromCharCode(v.charCodeAt(i));
                }
            }
        }
        val.value = result;
        v = result;
        return v;
    }
}