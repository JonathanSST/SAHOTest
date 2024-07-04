
function SaveEquEvo() {
    var equ_array = new Array();
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {        
        equ_array.push({ "EquNo": $(this).find('input[name*="EquNo"]').val(), "EvoName": $(this).find('input[name*="EvoName"]').val() });
    });
    //console.log(JSON.stringify(equ_array));    
    var post_data = {
        "PageEvent": "Save", "EvoHost": $('input[name*="EvoHost"]').val(), "EvoUid": $('input[name*="EvoUid"]').val(),
        "EvoPwd": $('input[name*="EvoPwd"]').val(), "EquEvoList": equ_array
    };
    console.log(post_data)
    //return false;
    $.ajax({
        type: "POST",
        url: "EvoList.aspx",
        data: JSON.stringify(post_data),
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            if (data.success === true) {
                alert("更新完成");
            }
        },
        fail: function () {
            alert("更新失敗");
        }
    });
    
}

function ValidateNumber(e, pnumber) {
    var rt3r = /^\d+/;
    var rt3 = /^\d+$/;
    if (!rt3.test(pnumber)) {
        e.value = rt3r.exec(e.value);
    }
    return false;
}


function AddEvoDevice(btnobj)
{
    
    that = JsFunFindParent(btnobj, "TD");
   
    $.ajax({
        type: "POST",
        url: "DeviceList.aspx",
        data:{'EvoNo':$(that).find('#EvoNo').val(),'EvoName':$(that).find('#EvoName').val()},
        success: function (data)
        {
            $(document.body).append('<div id="popOverlay" style="position:absolute; top:0; left:0; z-index:29999; overflow:hidden;'
             + ' -webkit-transform: translate3d(0,0,0);"></div>');
            $("#popOverlay").css("background", "#000");
            $("#popOverlay").css("opacity", "0.5");
            $("#popOverlay").width($(document).width());
            $("#popOverlay").height($(document).height());
            $(document.body).append('<div id="ParaExtDiv" style="position:absolute;z-index:30000;background-color:#1275BC;'
                  + 'border-style:solid; border-width:2px; border-color:#069" ></div>');
            $("#ParaExtDiv").html(data);
            $("#ParaExtDiv").css("left", ($(document).width() - $("#ParaExtDiv").width()) / 2);
            $("#ParaExtDiv").css("top", $(document).scrollTop() + 50);
            //$("#BtnAlive").focus();
            $("#BtnAlive").click(function () {
                $("#popOverlay").remove();
                $("#ParaExtDiv").remove();                
            });
        }
    });
}


function DoSave() {
    //這裡要將結果帶回去選單
    //console.log($('[name*="DeviceNo"]').val());
    var result = $('[name*="DeviceNo"]').map(function () {
        return $(this).val();
    }).get().join(',');
    console.log(result);
    console.log($(that).find('#EvoName').val());
    $(that).find('#EvoName').val(result);
    $(that).find("#EvoInfo").text(result);
    $("#popOverlay").remove();
    $("#ParaExtDiv").remove();
}


var that;

function DoAdd() {    
    var result = $('[name*="DeviceNo"]').map(function () {
        return $(this).val();
    }).get().join(',');
    if (result.indexOf($('select[name*="SelectEvo"]').val()) > -1) {
        alert('重複設定');
        return false;
    }
    var rowcount = $('#DeviceGridView').find('tr').length + 1;
    $("#DeviceGridView").append('<tr style="color: #fff">'
                                                           + '<td align="center" style="width: 41px;">'
                                                           + rowcount
                                                           + '</td>'
                                                           + '<td style="width: 232px;">'
                                                           + $('select[name*="SelectEvo"] option:selected').text()
                                                           + '<input type="hidden" value="' + $('select[name*="SelectEvo"]').val() + '" name="DeviceNo" />'                                                           
                                                           + '</td>'
                                                           + '<td align="center">'
                                                           + '<input type="button" value="刪除" class="IconDelete" onclick="Remove(this)" />'
                                                            + '<input type="hidden" value="-1" id="RemoveID" name="RemoveID" />'
                                                           + '</td>'
                                                       + '</tr>');
    //$('input[name*="popInput_CardRuleIndex"]').val($("#CardRuleGridView").find("TR").length);
}


function Remove(btnobj) {
    var remove_tr = JsFunFindParent(btnobj, "TR");
    $(remove_tr).remove();
}


function DoRefresh()
{
    $.ajax({
        type: "POST",
        url: "DeviceList.aspx",
        data: { 'PageEvent': 'Refresh', "admin": $('[name*="EvoUid"]').val(), "password": $('[name*="EvoPwd"]').val(), "host": $('[name*="EvoHost"]').val() },
        dataType: "json",
        success: function (data) {
            $("#SelectEvo option").remove();
            var arr = Array.prototype.slice.call(data.resource);
            arr.forEach(function (ele) {
                console.log(ele);
                var o = new Option(ele, ele);
                $("#SelectEvo").append(o);
            });
            if (data.err_message == "") {
                alert('更新完成!!');
            } else {
                alert(data.err_message);
            }
        }
    });
}