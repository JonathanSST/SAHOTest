function SetQuery() {
    if ($('input[name*="txtPsnEName"]').val() == "") {
        alert("英文姓名欄位未輸入")
        return false;
    }
    Block();
    
    $.post("QueryCompCardLog.aspx",
      {
          "PsnEName": $('input[name*="txtPsnEName"]').val(),
          "CardTime1": $('input[name*="CalendarTextBox"]:eq(0)').val(),
          "CardTime2": $('input[name*="CalendarTextBox"]:eq(1)').val(),
          "PageEvent":"Query"
      }      
      , function (data) {
          var content = $(data).find("#ContentPlaceHolder1_td_showGridView").html();
          $("#ContentPlaceHolder1_td_showGridView").html("");
          $("#ContentPlaceHolder1_td_showGridView").append(content);
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

