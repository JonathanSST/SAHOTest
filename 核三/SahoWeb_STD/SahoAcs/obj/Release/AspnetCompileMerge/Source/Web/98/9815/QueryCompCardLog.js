function SetQuery() {   
    Block();    
    $.post("ClearResult.aspx",
      {
          "PsnEName": $('input[name*="txtPsnEName"]').val(),
          "CardTime1": $('input[name*="CalendarTextBox"]:eq(0)').val(),
          "CardTime2": $('input[name*="CalendarTextBox"]:eq(1)').val(),
          "PageEvent":"Query"
      }      
      , function (data) {
          var content = $(data).find("#MainDiv").html();
          //console.log(content);
          $("#ContentPlaceHolder1_tablePanel").html("");
          $("#ContentPlaceHolder1_tablePanel").append(content);
          $.unblockUI();
      });      
    /*
    $.ajax({
        type: "POST",
        url: "CardLogResult.aspx",
        data: {
            "PsnEName": $('input[name*="txtPsnEName"]').val(),
            "CardTime1": $('input[name*="CalendarTextBox"]:eq(0)').val(),
            "CardTime2": $('input[name*="CalendarTextBox"]:eq(1)').val(),
            "PageEvent": "Query"
        },        
        dataType: "json"
    }).success(function (data) {        
        if (data.message != "OK") {
            
        } else {
            var content_html=
                            '<div><table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;"><tbody>';
            $(data.data).each(function (index, prop) {
                content_html+='<tr id="GV_Row1" onmouseover="onMouseMoveIn(0,this,"","")" onmouseout="onMouseMoveOut(0,this)">';
                content_html+='<td style="width: 103px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 94px;">' + prop.PsnName + '</td>';
                content_html+='<td style="width: 94px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 94px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 124px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 104px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 84px;">' + prop.PsnEName + '</td>';
                content_html+='<td style="width: 124px;">' + prop.PsnEName + '</td>';
                content_html+='<td>' + prop.PsnEName + '</td>';
                content_html+='</tr>';                
            });            
            content_html+='</tbody></table></div>';
            $("#ContentPlaceHolder1_tablePanel").append(content_html);
        }
    }).fail(function () {
        
    });
    */
}



function SetDelete() {
    var valuelist = "";
    $('input[type=checkbox][name=ChkOne]').each(function ()
    {        
        if (this.checked==true) 
        {
            valuelist += $(this).val() + ",";
        }
    });
    if (valuelist == "") {
        alert("未勾選任何未授權刷卡記錄!!");
    } else {
        if (confirm("確定刪除未授權刷卡記錄?")) {
            SetProcDelete(valuelist);
        }
    }
}

function SetProcDelete(delete_list) {
    $.ajax({
        type: "POST",
        url: "CardLogClear.aspx",
        data: {            
            "DeleteList": delete_list,
            "PageEvent": "Delete"
        },
        dataType: "json",
        success: function (data) {
            //console.log(data.message);
            SetQuery();
        }
    });
}


function CheckAll() {
    if ($("#ChkAll").prop("checked") == true) {
        $('input[type=checkbox][name=ChkOne]').prop("checked", true);
    } else {
        $('input[type=checkbox][name=ChkOne]').prop("checked", false);
    }
}