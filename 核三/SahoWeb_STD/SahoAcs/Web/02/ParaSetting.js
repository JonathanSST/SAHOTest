// 呼叫URL連結
function CallURL(sURL, vArguments, sFeatures, TriggerID) {
    // 取得該頁面重新傳送功能打勾的狀態 ex. 000000、111111
    var varCheck = '';
    var varcbxHeader = document.getElementById('cbxHeader');
    if (varcbxHeader.checked) {
        varCheck += '1,';
    }
    else {
        varCheck += '0,';
    }

    var tbl = document.getElementById('ParaGridView');
    for (i = 0; i < tbl.rows.length; i++) {
        if (tbl.rows[i].cells[3].childNodes[1].checked) {
            varCheck += '1,';
        }
        else {
            varCheck += '0,';
        }
    }
    varCheck = varCheck.substring(0, varCheck.length-1);

    //document.getElementById('cbxHeader').checked = true;

    // 
    var paraqueue = vArguments.split('&');
    vArguments = paraqueue[0] //EquID
               + "&" + paraqueue[1] //EquParaID
               + "&" + paraqueue[2]; //ParaValue
    var vReturnValue = window.showModalDialog(sURL, vArguments, sFeatures + ";center:yes;");
    if (typeof vReturnValue != "undefined") {

        if (vReturnValue.indexOf("/SahoFromURL") > -1) {
            var Actstr = "";
            var ExtDatastr = "";
            Actstr = vReturnValue.slice(0, vReturnValue.indexOf("/SahoFromURL"));
            ExtDatastr = vReturnValue.slice(vReturnValue.indexOf("/SahoFromURL")).replace("/SahoFromURL", "")

            if (Actstr == "FloorNameList") {
                __doPostBack(TriggerID, Actstr + "/" + paraqueue[1] + "&" + encodeURIComponent(ExtDatastr) + "/" + varCheck);
            }
            if (Actstr == "FloorSetting") {
                __doPostBack(TriggerID, paraqueue[1] + "&" + ExtDatastr + "&" + varCheck);
            }
        }
        else
            __doPostBack(TriggerID, paraqueue[1] + "&" + vReturnValue + "&" + varCheck);
    }
}

// 呼叫URL連結帶多重參數
function CallURLExtData(sURL, vArguments, ExtData, sFeatures, TriggerID) {
    // 取得該頁面重新傳送功能打勾的狀態 ex. 000000、111111
    var varCheck = '';
    var varcbxHeader = document.getElementById('cbxHeader');

    if (varcbxHeader.checked) {
        varCheck += '1,';
    }
    else {
        varCheck += '0,';
    }

    var tbl = document.getElementById('ParaGridView');
    for (i = 0; i < tbl.rows.length; i++) {
        if (tbl.rows[i].cells[3].childNodes[1].checked) {
            varCheck += '1,';
        }
        else {
            varCheck += '0,';
        }
    }
    varCheck = varCheck.substring(0, varCheck.length-1);

    //document.getElementById('cbxHeader').checked = true;

    ExtData = encodeURIComponent(ExtData);
    var paraqueue = vArguments.split('&');
    vArguments = paraqueue[0] //EquID
               + "&" + paraqueue[1] //EquParaID
               + "&" + paraqueue[2] //ParaValue
               + "&" + ExtData; //ExtData

    var vReturnValue = window.showModalDialog(sURL, vArguments, sFeatures + ";center:yes;");

    if (vReturnValue != '' && typeof vReturnValue != "undefined") {
        if (vReturnValue.indexOf("/SahoFromURL") > -1) {
            var Actstr = "";
            var ExtDatastr = "";
            Actstr = vReturnValue.slice(0, vReturnValue.indexOf("/SahoFromURL"));
            ExtDatastr = vReturnValue.slice(vReturnValue.indexOf("/SahoFromURL")).replace("/SahoFromURL", "")

            if (Actstr == "FloorNameList") {
                __doPostBack(TriggerID, Actstr + "/" + paraqueue[1] + "&" + encodeURIComponent(ExtDatastr) + "/" + varCheck);
            }
            if (Actstr == "FloorSetting") {
                __doPostBack(TriggerID, paraqueue[1] + "&" + encodeURIComponent(ExtDatastr) + "&" + varCheck);
            }
        }
    }
}

function Post(sURL, vArguments, sFeatures, TriggerID) {
    var keys = [];
    var values = [];
    var paraqueue = vArguments.split('&');

    keys[0] = "EquID";
    keys[1] = "EquParaID";
    keys[2] = "ParaValue";
    values[0] = paraqueue[0];
    values[1] = paraqueue[1];
    values[2] = paraqueue[2];

    openWindowWithPost(sURL, "Test", keys, values);
}

function openWindowWithPost(url, name, keys, values) {
    var newWindow = window.open(url, name, 'location=no, resizable=no');
    if (!newWindow) return false;
    var html = "";
    var keysArray = keys.split('&');
    var valuesArray = values.split('&');
    html += "<html><head><base target='_self' />    <title>Transfer</title> </head><body><form id='formid' method='post' action='" + url + "'>";
    if (keysArray.length == valuesArray.length)
        for (var i = 0; i < keysArray.length; i++)
            html += "<input type='hidden' name='" + keysArray[i] + "' value='" + valuesArray[i] + "'/>";
    html += "</form><script type='text/javascript'>document.getElementById(\"formid\").submit()<\/script></body></html>";
    newWindow.document.write(html);






    //var vReturnValue = window.showModalDialog(sURL + vArguments, '', sFeatures + ";center:yes;");
    //if (vReturnValue != '' && typeof vReturnValue != "undefined") {

    //    if (vReturnValue.indexOf("/SahoFromURL") > -1) {
    //        var Actstr = "";
    //        var ExtDatastr = "";
    //        Actstr = vReturnValue.slice(0, vReturnValue.indexOf("/SahoFromURL"));
    //        ExtDatastr = vReturnValue.slice(vReturnValue.indexOf("/SahoFromURL")).replace("/SahoFromURL", "")

    //        if (Actstr == "FloorNameList") {
    //            __doPostBack(TriggerID, Actstr + "/" + paraqueue[1] + "&" + encodeURIComponent(ExtDatastr));
    //        }
    //        if (Actstr == "FloorSetting") {
    //            __doPostBack(TriggerID, paraqueue[1] + "&" + ExtDatastr);
    //        }
    //    }
    //    else
    //        __doPostBack(TriggerID, paraqueue[1] + "&" + vReturnValue);
    //}





    return newWindow;
}

var target_para_obj;

function PopURL(obj) {
    //var paraqueue = vArguments.split('&');
    var oTD = JsFunFindParent(obj, "TR");
    //console.log($("#ParaGridView").find("tr").index(oTD));    
    target_para_obj = $(oTD).find('input[name*="ParaValue"]');
    $.ajax({
        type: "POST",
        url: "../"+$(oTD).find('input[name*="FormUrl"]').val().replace(".aspx","Pop.aspx"),
        data: {
            'EquID': $('input[name*="hideEquID"]').val(),
            'EquParaID': $(oTD).find('input[name*="EquParaID"]').val(),
            'ParaValue': $(oTD).find('input[name*="ParaValue"]').val()
        },
        async: true,
        success: function (data) {            
            $("#ParaExtDiv").html(data);            
        }
    });
}

function SaveParaData()
{
    console.log($("#MasterDiv").find("input").serialize());
    $.ajax({
        type: "POST",
        url: "/web/02/ParaSettingBox.aspx",
        dataType: 'json',
        data: $("#MasterDiv").find("input,select").serialize(),        
        success: function (data) {
            console.log(data);
            $("#ParaDiv").remove();
            $("#popOverlay").remove();
        }
    });
}

function ChangeCbxStatus()
{
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
}

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