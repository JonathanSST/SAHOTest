
var target_para_obj;

function PopURL(obj) {
    //var paraqueue = vArguments.split('&');
    var oTD = JsFunFindParent(obj, "TR");
    //console.log($("#ParaGridView").find("tr").index(oTD));    
    target_para_obj = $(oTD).find('input[name="ParaValue"]');
    $.ajax({
        type: "POST",
        url: "/web/02/"+$(oTD).find('input[name*="FormUrl"]').val().replace(".aspx","Pop.aspx").replace("ParaAP","ParaPop"),
        data: {
            'EquID': $('input[name*="hideEquID"]').val(),
            'EquParaID': $(oTD).find('input[name*="EquParaID"]').val(),
            'ParaValue': $(oTD).find('input[name*="ParaValue"]').val()           
        },
        async: true,
        success: function (data) {            
            $('<div id="ParaPopContent" style="position:absolute;left:20px;top:30px;z-index:1000000000'
             + ';background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;" ></div>').appendTo($("#ParaExtDiv"));
            $("#ParaPopContent").html(data);
        }
    });
}

function DoRefresh() {
    $("#PageEvent").val("Refresh");
    $.ajax({
        type: "POST",
        url: "/web/02/ParaSettingBox.aspx",
        dataType: 'json',
        data: $("#MasterDiv").find("input,select").serialize(),
        success: function (data) {
            alert("參數重新傳送，等待設碼中");
            DoCancel();
        }
    });
}

function SaveParaData()
{
    //console.log($("#MasterDiv").find("input").serialize());
    $.ajax({
        type: "POST",
        url: "/web/02/ParaSettingBox.aspx",
        dataType: 'json',
        data: $("#MasterDiv").find("input,select").serialize(),        
        success: function (data) {
            DoCancel();
        }
    });
}

function ChangeCbxStatus(obj)
{
    /*
    var tbl = document.getElementById('ParaGridView');

    if (document.getElementById('cbxHeader').checked == true) {
        for (i = 0; i < tbl.rows.length; i++) {
            tbl.rows[i].cells[3].childNodes[1].checked = true;
        }
    }
    else
    {
        for (i = 0; i < tbl.rows.length; i++) {
            tbl.rows[i].cells[3].childNodes[1].checked = false;
        }
    }
    */
    //$(obj).find('input[name*="CHK_COL_1"]').prop("checked", !$(obj).find('input[name*="CHK_COL_1"]').prop("checked"));
}


/*
function ShowDialog(strType, strTitle, strMsg) {
    var vIcon;

    if (strType == 'message') {
        vIcon = "<img src='/Img/info.png' width='30px' />";
    }
    else if (strType == 'alert') {
        vIcon = "<img src='/Img/Close.png' width='30px' />";
    }

    var vHtml = '';
    vHtml += "<div align='center' valign='center'>";
    vHtml += "<table style='width: 90%'>";
    vHtml += "<tr>";
    vHtml += "<td style='text-align: middle; vertical-align: middle; width: 50px'>" + vIcon + "</td>";
    vHtml += "<td style='text-align: left; vertical-align: middle'><p style='color: #FFFFFF;'>" + strMsg + "</p></td>";
    vHtml += "</tr>";
    vHtml += "</table>";
    vHtml += "</div>";

    $(function () {
        $("#dialog").html(vHtml);
        $("#dialog").dialog({
            autoOpen: true,
            draggable: true,
            resizable: false,
            title: strTitle,
            closeOnEscape: true,
            buttons: {
                "確認": function () {
                    $(this).dialog('close');
                    window.close();
                }
            },
            modal: true
            //hide: { effect: "blind", duration: 1000 }
        });
    });
}
*/