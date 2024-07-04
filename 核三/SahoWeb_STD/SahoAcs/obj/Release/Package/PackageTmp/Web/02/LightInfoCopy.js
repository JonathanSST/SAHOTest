
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


function SaveParaData()
{
    //console.log($("#MasterDiv").find("input").serialize());
    $.ajax({
        type: "POST",
        url: "/web/02/LightInfoCopy.aspx",
        dataType: 'json',
        data: $("#MasterDiv").find("input,select").serialize(),        
        success: function (data) {
            alert('燈號文字複製作業完成!!');
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
    $(obj).find('input[name*="CHK_COL_1"]').prop("checked", !$(obj).find('input[name*="CHK_COL_1"]').prop("checked"));
}
