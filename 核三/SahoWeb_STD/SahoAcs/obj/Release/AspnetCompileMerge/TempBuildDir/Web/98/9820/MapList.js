var svg = null;
$(document).ready(function () {
    svg = d3.select('#MapDiv').append('svg').attr({
        width: 1280,
        height: 960,
        border: '1px solid #ccc'
    });

    svg.append('svg:image')
        .attr({
            'xlink:href': $('#PageMapSrc').val(),  // can also add svg file here
            x: 0,
            y: 0,
            width: 1280,
            height: 960
        });
});

function AddCtrl(){
	svg.on('mousedown',mousedown);
	svg.on('mouseup',mouseup);
	$('#BtnAddCtrl').hide();
}
var area_list = new Array();
var select_id='';
function mousedown() {
    var m = d3.mouse(this);  //取得滑鼠的位置
    
			
	point = svg.append('text').attr({
		x: m[0], 
		y: m[1],
		id: "myText"+area_list.length,
		equ_id:0
	}).style({
		fill: 'brown',
		'font-size': '11pt',
		'width':'128px','height':'128px','color':'#88AA00'
	})
	// .append('textPath').attr({
		// 'xlink:href': 'http://localhost:8087'
	// })
	.text('無連接 ■')
	.on('contextmenu',function(d, i){
		d3.event.preventDefault();
		//d3.select(this).remove();
		//alert(d3.select(this));
		$('#popOverlay2').css('display','block');
		$('#ParaExtDiv2').css('display','block');
		$("#ParaExtDiv2").css("left", ($(document).width() - $("#ParaExtDiv2").width()) / 2);
        $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);
		select_id=d3.select(this).attr('id');
	});
}




function mouseup() {
	area_list.push(point);
    svg.on("mouseup", null);	//註銷移除事件
	svg.on("mousedown", null);
	$('#BtnAddCtrl').show();
}

function SaveEquMap() {
    var equ_array = new Array();
    $("#ContentPlaceHolder1_GridView2").find("tr").each(function (index) {        
        equ_array.push({ "EquNo": $(this).find('input[name*="EquNo"]').val(), "EvoName": $(this).find('input[name*="EvoName"]').val() });
    });
    //console.log(JSON.stringify(equ_array));    
    var post_data = {
        "PageEvent": "Save", "EvoHost": $('input[name*="EvoHost"]').val(), "EvoUid": $('input[name*="EvoUid"]').val(),
        "EvoPwd": $('input[name*="EvoPwd"]').val(), "EquEvoList": equ_array
    };
    //return false;
    $.ajax({
        type: "POST",
        url: "EquMapList.aspx",
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
