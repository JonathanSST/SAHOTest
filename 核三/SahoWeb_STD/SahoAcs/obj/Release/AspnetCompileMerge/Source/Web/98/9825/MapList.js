var area_list = new Array();
var select_id = '';
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
    LoadEquData();
    LoadEquMap();
});

function LoadEquMap() {
    area_list = new Array();
    d3.selectAll('text').remove();
    $.ajax({
        type: "POST",
        url: "EquMapList.aspx",
        data: { "PageEvent": "LoadMap" },
        dataType: "json",
        success: function (data) {
            console.log(data);
            var arr2 = Array.prototype.slice.call(data.map_obj);
            arr2.forEach(function (m_obj) {
                var alive_color = m_obj.AliveState == 1 ? 'blue' : 'red';
                if (m_obj.IsOpen == 1) {
                    point = svg.append('text').attr({
                        x: m_obj.PointX,
                        y: m_obj.PointY,
                        id: m_obj.MapObjID,
                        equ_id: m_obj.EquNo,
                        alive_time: m_obj.VarStateTime,
                        equ_name: m_obj.EquName,
                        is_open: m_obj.IsOpen
                    }).style({
                        fill: alive_color,
                        'font-size': '11pt',
                        'width': '128px', 'height': '128px', 'color': '#88AA00'
                    }).text(m_obj.EquNo + " ■").on('contextmenu', function (d, i) {
                        d3.event.preventDefault();
                        $('#popOverlay2').css('display', 'block');
                        $('#ParaExtDiv2').css('display', 'block');
                        $("#ParaExtDiv2").css("left", ($(document).width() - $("#ParaExtDiv2").width()) / 2);
                        $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);
                        select_id = d3.select(this).attr('id');
                        $("#EquList").val(m_obj.EquNo);
                    }).on("click", SetClick);
                } else {
                    point = svg.append('text').attr({
                        x: m_obj.PointX,
                        y: m_obj.PointY,
                        id: m_obj.MapObjID,
                        equ_id: m_obj.EquNo,
                        alive_time: m_obj.VarStateTime,
                        equ_name: m_obj.EquName,
                        is_open: m_obj.IsOpen
                    }).style({

                    })
                }
                area_list.push(point);
            });
        },
        fail: function () {
            alert("資料載入失敗");
        }
    });

   
}


function LoadEquData() {
    $.ajax({
        type: "POST",
        url: "EquMapList.aspx",
        data: { "PageEvent": "Load" },
        dataType: "json",
        success: function (data) {
            console.log(data);
            var arr = Array.prototype.slice.call(data.equ_data);
            arr.forEach(function (ele) {
                var o = new Option(ele.EquNo, ele.EquNo);
                $("#EquList").append(o);
            });           
        },
        fail: function () {
            alert("資料載入失敗");
        }
    });
}

function SetClick(d) {
    //console.log(area_list);
    alert(d3.select(this).attr('equ_id') + ' '+d3.select(this).attr('equ_name') + ' 斷連線時間：' + d3.select(this).attr('alive_time').toString());
}


function AddCtrl() {
    svg.on('mousedown', mousedown);
    svg.on('mouseup', mouseup);
    $('#BtnAddCtrl').hide();
}



function mousedown() {
    var m = d3.mouse(this);  //取得滑鼠的位置
    point = svg.append('text').attr({
        x: m[0],
        y: m[1],
        id: "myText" + area_list.length,
        equ_id: 0,
        is_open: 1,
        alive_time: ''
        }).style({
            fill: 'brown',
            'font-size': '11pt',
            'width': '128px', 'height': '128px', 'color': '#88AA00'
        }).text('無連接 ■')
        .on('contextmenu', function (d, i) {
            d3.event.preventDefault();
            //d3.select(this).remove();
            //alert(d3.select(this));
            $('#popOverlay2').css('display', 'block');
            $('#ParaExtDiv2').css('display', 'block');
            $("#ParaExtDiv2").css("left", ($(document).width() - $("#ParaExtDiv2").width()) / 2);
            $("#ParaExtDiv2").css("top", $(document).scrollTop() + 50);
            select_id = d3.select(this).attr('id');
        });
}


function mouseup() {
    area_list.push(point);
    svg.on("mouseup", null);	//註銷移除事件
    svg.on("mousedown", null);
    $('#BtnAddCtrl').show();
}

function SaveEquMap() {
    var result_arr = new Array();
    console.log(area_list);
    d3.selectAll('text').each(function () {
        //result_arr.push(d3.select(this).text());
        result_arr.push({ 'obj_id': d3.select(this).attr('id'), 'equ_no': d3.select(this).attr('equ_id'), 'x': d3.select(this).attr('x'), 'y': d3.select(this).attr('y'), 'is_open':d3.select(this).attr('is_open') });
    });
    var post_data = {
        "PageEvent": "Save", "EquMapList": result_arr
    };
    console.log(JSON.stringify(post_data));
    
    $.ajax({
        type: "POST",
        url: "EquMapList.aspx",
        data: JSON.stringify(post_data),
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            alert('修改完成');
            LoadEquMap();
        },
        fail: function () {
            alert("更新失敗");
        }
    });
    
}

function SetEnter() {
    d3.select('#' + select_id).text($('#EquList').find('option:selected').text() + " ■");
    d3.select('#' + select_id).attr('equ_id', $('#EquList').find('option:selected').text());
    d3.select('#' + select_id).style('fill', 'blue');
    $('#popOverlay2').css('display', 'none');
    $('#ParaExtDiv2').css('display', 'none');
}