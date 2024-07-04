$(document).ready(function () {
   document.getElementById("BtnKeyIn").style.display = "none";
});

function KeyIn() {
    var PsnNo = $("#ContentPlaceHolder1_Input_PsnNo").val();
    var CardNo = $("#ContentPlaceHolder1_Input_CardNo").val();
    var PsnName = $("#ContentPlaceHolder1_Input_PsnName").val();
    var SDate = $("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val();
    var MealNo_0 = $("#ContentPlaceHolder1_rdb_MealNo_0").is(":checked");
    var MealNo_1 = $("#ContentPlaceHolder1_rdb_MealNo_1").is(":checked");
    var MealNo;
    if (MealNo_0) {
        MealNo = "1";
    }
    else {
        MealNo = "2";
    }

    if (CardNo == "" && PsnNo == "" && PsnName == "") {
        alert("人員編號或姓名或卡號為必填!!");
        return;
    };
    if (SDate == "") {
        alert("用餐日為必填!!");
        return;
    }

    var myCurrentDate = new Date();
    var myPastDate = new Date(myCurrentDate);
    myPastDate.setDate(myPastDate.getDate() - 1);
    if (SDate > myPastDate.format('yyyy/MM/dd')) {
        alert("用餐日，只可選擇在今日以前，請確認！");
        return;
    }

    var answer = window.confirm("請務必 確定 資料正確？ (包括:人員資料、用餐日、餐別)");
    if (answer) {
       
        Block();
        $.ajax({
            type: "POST",
            url: window.location.href,
            dataType: "json",
            data: {
                "PageEvent": "KeyIn",
                "SDate": SDate,
                "CardNo": CardNo,
                "PsnNo": PsnNo,
                "PsnName": PsnName,
                "MealNo": MealNo
            },
            success: function (data) {
                if (data.result) {
                    $.unblockUI();
                    alert(data.message);
                }
            }
        });
    }
    else {
        return;
    }
}

function QuerySearch() {
    
    var PsnNo = $("#ContentPlaceHolder1_Input_PsnNo").val();
    var CardNo = $("#ContentPlaceHolder1_Input_CardNo").val();
    var PsnName = $("#ContentPlaceHolder1_Input_PsnName").val();

    if (CardNo == "" && PsnNo == "" && PsnName == "") {
        alert("人員編號或姓名或卡號需輸入\n方可查詢資料!!");
        return;
    };
    Block();
    $.ajax({
        type: "POST",
        url: location.href,
        data: {
            "PageEvent": "QueryPerson",
            "CardNo": CardNo,
            "PsnNo": PsnNo,
            "PsnName": PsnName
        },
        dataType: "json",
        success: function (data) {
            if (data.success) {
                $.unblockUI();
                //console.log(data.mga_list[0]);
                //設定資料到UI欄位
                $("#ContentPlaceHolder1_Input_PsnNo").val(data.mga_list[0].PsnNo);
                $("#ContentPlaceHolder1_Input_CardNo").val(data.mga_list[0].CardNo);
                $("#ContentPlaceHolder1_Input_PsnName").val(data.mga_list[0].PsnName);
                document.getElementById("BtnKeyIn").style.display = "inline";
                return;
            } else {
                $.unblockUI();
                alert(data.message);
                return;
            }
        }
    });
}

function Clear() {
    var day = new Date(); 
    day.setDate(day.getDate() - 1);
    var date = day.format("yyyy/MM/dd"); 
    $("#ContentPlaceHolder1_Calendar_CardTimeSDate_CalendarTextBox").val(date);
    $("#ContentPlaceHolder1_Input_PsnNo").val('');
    $("#ContentPlaceHolder1_Input_CardNo").val('');
    $("#ContentPlaceHolder1_Input_PsnName").val('');
    $("#ContentPlaceHolder1_rdb_MealNo_0").prop('checked', true);
    $("#ContentPlaceHolder1_rdb_MealNo_1").prop('checked', false);
    document.getElementById("BtnKeyIn").style.display = "none";
}