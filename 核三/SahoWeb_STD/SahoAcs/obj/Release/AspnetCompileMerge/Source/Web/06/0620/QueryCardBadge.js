$(document).ready(function () {
    $("#QueryButton").click(function () {
        /*
        $.colorbox({
            href: 'QueryPerson.aspx?PsnNo=' + $('input[name*="TextBox_CardNo_PsnName"]').val(),
            open: true,
            width: '950px',
            height: '465px',
            //scrolling: true,
            title: '人員識別證資料查詢',
            onComplete: function () {
                $(".ResultRow").click(function () {
                    $.colorbox.close();
                    __doPostBack('NewQuery',$(this).find("td:eq(1)").html());
                });
            }
        });
        */
        $.post("QueryPerson.aspx",
            { PsnNo: $('input[name*="TextBox_CardNo_PsnName"]').val(), PsnName: $('input[name*="TextBox_CardNo_PsnName"]').val() },
              function (data) {
                  $.colorbox({
                      width: '950px',
                      height: '465px',
                      open: true,
                      preloading:false,
                      html:data,
                      title: '人員識別證資料查詢',
                      onComplete: function () {
                          $(".ResultRow").click(function () {
                              $.colorbox.close();
                              __doPostBack('NewQuery', $(this).find("td:eq(1)").html());
                          });
                      }
                  });
              });
    });
});