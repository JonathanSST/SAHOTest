
function OnLoad(){    
    $("#ContentPlaceHolder1_MainGridView").find("tr").each(function () {
        $(this).find('select[name*="MenuIsUse"]').val($(this).find('input[name*="hMenuUse"]').val());        
    });

    var focusables = $(':input[type=text],select').not($("#NewRow").find(':input[type=text],select'));
    //alert(focusables.length);
    focusables.keydown(function (e) {
        if (e.keyCode == 13) {
            var current = focusables.index(this),
                                                 next = focusables.eq(current + 1).length ? focusables.eq(current + 1) : focusables.eq(0);
            //alert(focusables.eq(current + 1).length);
            next.focus();
            closeAllCompont();
        }
    });

}
    

function DoSave() {
    //資料儲存
    if (JsFunBASE_VERTIFY()) {
        //console.log('success!!');
        //送出資料進行更新
        var form_data = $("#ContentPlaceHolder1_MainGridView").find(":input[type=text],select").serialize();        
        $.ajax({
            type: "POST",
            url: "SysMenuManage.aspx",
            dataType: "json",
            data: form_data + "&PageEvent=Save",
            success: function (data) {
                alert('修改完成!!')
                //window.location = window.location;
            },
            fail: function () {

            }
        });
    }
}