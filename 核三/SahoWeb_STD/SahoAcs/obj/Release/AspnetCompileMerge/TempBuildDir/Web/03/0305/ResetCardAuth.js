//************************************************** 網頁畫面設計(一)相關的JavaScript方法 **************************************************
var BatchCount = 10;
//主要作業畫面：清除在ListBox裡所有的Item
function ClearListBoxItem(ListBoxID) {
    var MyListBox = document.getElementById(ListBoxID);

    while (MyListBox.options.length > 0) {
        MyListBox.remove(0);
    }
}

//主要作業畫面：移除在ListBox被選擇的Item
function RemoveListBoxSelected(ListBoxID) {
    var MyListBox = document.getElementById(ListBoxID);

    for (i = 0; i < MyListBox.options.length; i++) {
        if (MyListBox.options[i].selected) {
            MyListBox.remove(i);
            i--;
        }
    }
}

//主要作業畫面：新增在ListBox的Item
function InsertListBoxItem(ListBoxID, ItemText, ItemValue) {
    var MyListBox = document.getElementById(ListBoxID);

    var Option = null;

    Option = document.createElement('option');
    Option.text  = ItemText;
    Option.value = ItemValue;

    MyListBox.options.add(Option);
}

//主要作業畫面：排序在ListBox裡所有的Item
function SortListBoxItem(ListBoxID) {
    var MyListBox = document.getElementById(ListBoxID);

    var ItemCount = MyListBox.options.length;

    if (ItemCount <= 1)
        return;

    //
    var ItemArray = new Array(ItemCount);

    for (var i = 0; i < ItemCount; i++) {
        ItemArray[i] = MyListBox.options[i].text;
    }

    ItemArray.sort();

    //
    ClearListBoxItem(ListBoxID);

    for (var i = 0; i < ItemCount; i++) {
        InsertListBoxItem(ListBoxID, ItemArray[i], ItemArray[i]);
    }
}

//主要作業畫面：完成動作設定網頁相關的參數內容
function ExcuteComplete(objRet) {
    switch (objRet.result) {
        case true:
            if (objRet.act == 'ExcuteCardAuthReset') {
                var ErrorCardNoList = objRet.message;

                RefreshAllListBoxItem(ErrorCardNoList);

                if (IsEmpty(ErrorCardNoList))
                    alert('完成卡片權限複製！');
                else
                    alert('未完成卡片權限複製的卡號分別有' + ErrorCardNoList + '！');
            }
            break;
        case false:
            alert(objRet.message);
            break;
        default:
            break;
    }
}

//更新等待重整卡號清單與完成重整卡號清單相關的ListBox裡Item
function RefreshAllListBoxItem(ErrorCardNoList) {
    var WaitResetListBox    = document.getElementById('ContentPlaceHolder1_popInput0_WaitReset');
    var ResetSuccessListBox = document.getElementById('ContentPlaceHolder1_popInput0_ResetSuccess');

    if (IsEmpty(ErrorCardNoList)) {
        ClearListBoxItem('ContentPlaceHolder1_popInput0_ResetSuccess');

        for (var i = 0; i < WaitResetListBox.options.length; i++) {
            var ItemText  = WaitResetListBox.options[i].text;
            var ItemValue = WaitResetListBox.options[i].value;

            InsertListBoxItem('ContentPlaceHolder1_popInput0_ResetSuccess', ItemText, ItemValue);
        }

        ClearListBoxItem('ContentPlaceHolder1_popInput0_WaitReset');
    }
    else {
        var ErrorCardNoArray = ErrorCardNoList.split(',');

        ClearListBoxItem('ContentPlaceHolder1_popInput0_ResetSuccess');

        for (var i = 0; i < WaitResetListBox.options.length; i++) {
            var ItemCheck = true;
            var ItemText  = WaitResetListBox.options[i].text;
            var ItemValue = WaitResetListBox.options[i].value;

            for (var j = 0; j < ErrorCardNoArray.length; j++) {
                if (ItemText == ErrorCardNoArray[j]) {
                    ItemCheck = false;
                    break;
                }
            }

            if (ItemCheck) {
                InsertListBoxItem('ContentPlaceHolder1_popInput0_ResetSuccess', ItemText, ItemValue);
            }
        }

        //
        ClearListBoxItem('ContentPlaceHolder1_popInput0_WaitReset');

        for (var i = 0; i < ErrorCardNoArray.length; i++) {
            InsertListBoxItem('ContentPlaceHolder1_popInput0_WaitReset', ErrorCardNoArray[i], ErrorCardNoArray[i]);
        }
    }

    //
    ChangeText(document.getElementById('ContentPlaceHolder1_popLabel0_WaitReset'), '等待重整卡號清單-共 ' + WaitResetListBox.options.length + ' 個');
    ChangeText(document.getElementById('ContentPlaceHolder1_popLabel0_ResetSuccess'), '完成重整卡號清單-共 ' + ResetSuccessListBox.options.length + ' 個');
}

//主要作業畫面：選取按鈕 - 找出相關等待重整的卡片清單
function ExcuteQuery() {
    var SelectDDList_ResetCardType = document.getElementById("SelectDDList_ResetCardType");

    if (SelectDDList_ResetCardType.selectedIndex > 0) {
        __doPostBack(SelectButton.id, SelectDDList_ResetCardType.options[SelectDDList_ResetCardType.selectedIndex].value);
    }
    else {
        alert('尚未選擇卡片類型！');
    }
}

//主要作業畫面：權限重整按鈕 - 將等待重整卡號清單裡所有的卡號做卡片權限重整
function ExcuteCardAuthReset() {
    var WaitResetListBox = document.getElementById('ContentPlaceHolder1_popInput0_WaitReset');

    var ItemCount = WaitResetListBox.options.length;

    if (ItemCount <= 0) {
        alert("尚無等待權限重整卡號！");
        return;
    }

    //
    var UserID = document.getElementById('ContentPlaceHolder1_hUserID').value;
    var ResetCardNoArray = new Array(ItemCount);

    for (var i = 0; i < ResetCardNoArray.length; i++) {
        ResetCardNoArray[i] = WaitResetListBox.options[i].text;
    }

    PageMethods.ExcuteCardAuthReset(UserID, ResetCardNoArray, ExcuteComplete, onPageMethodsError);
}

function ShowPersonListLabel(Count) {
    lblPersonList.innerHTML = "卡片權限重整清單(共" + Count + "筆)";
}


var count = 0;
var error_list = "";
function SetCardAuthProc() {
    var all_count = $("input[name='card']").length;    
    if (count < all_count) {
        $.ajax({
            type: "POST",
            url: "CardAuthProc.aspx",
            dataType: "json",
            data: { card_no: $("input[name='card']:eq(" + count + ")").val(), action: "Save" },
            success: function (data) {
                if (data.message != "OK") {
                    //console.log(data.card_no + ".....error....");
                    error_list += data.card_no + ",";
                    count++;
                    SetCardAuthProc();
                } else {
                    //console.log($("input[name='card']:eq(" + count + ")").val() + ".....Success");
                    $("textarea[name*='MsgResultTextBox']:eq(0)").val($("input[name='card']:eq(" + count + ")").val() + ".....Success");
                    count++;
                    SetCardAuthProc();
                }
            },
            fail: function () {
                //console.log($("input[name='card']:eq(" + count + ")").val() + ".....Fails");
                count++;
                SetCardAuthProc();
            }
        });
		console.log("5678");
    } else {
        $.unblockUI();
		console.log("1234");
        $("textarea[name*='MsgResultTextBox']:eq(0)").val("重整完成");
        if (error_list.split(',').length > 1) {
            $("textarea[name*='MsgResultTextBox']:eq(0)").append("\n重整失敗卡號：\n" + error_list.slice(0, -1));
        }        
    }    
}


function SetCardAuthProcBach() {
    var all_count = $("input[name='card']").length;
    var cards = new Array();
    if (count == 0) {
        error_list = "";
    }
    if (count < all_count) {
        $("input[name='card']").slice(count, count + BatchCount).each(function (index) {            
            cards.push($(this).val());            
        });        
        //console.log(cards);
        $.ajax({
            type: "POST",
            url: "CardAuthProc.aspx",
            data: JSON.stringify(cards),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                if (data.message != "OK") {
                    //console.log(data.card_no + ".....error....");
                    //error_list += data.card_no + ",";
                    count += BatchCount;
                    SetCardAuthProcBach();
                } else {
                    $("textarea[name*='MsgResultTextBox']:eq(0)").val(data.cards_ok + ".....Success \n 已處理:" + count + "筆資料...");
                    error_list += data.cards_error;
                    count += BatchCount;
                    SetCardAuthProcBach();
                }
            },
            fail: function () {
                //console.log($("input[name='card']:eq(" + count + ")").val() + ".....Fails");
                count += BatchCount;
                SetCardAuthProcBach();
            }
        });
    } else {
        $.unblockUI();
        count = 0;
		
        if (error_list.split(',').length > 1) {
            $("textarea[name*='MsgResultTextBox']:eq(0)").val($("#lblError").val() + "：\n" + error_list.slice(0, -1));
        } else {
            $("textarea[name*='MsgResultTextBox']:eq(0)").val("重整完成");
        }
    }
    
}