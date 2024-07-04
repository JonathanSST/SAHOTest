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
    LoadEquMap();
});

function LoadEquMap() {
    area_list = new Array();
    var e = document.getElementById('ContentPlaceHolder1_ddlMapList'); // select element    
    var PicID = e.options[e.selectedIndex].value;
    d3.selectAll('text').remove();
    $.ajax({
        type: "POST",
        url: location.href,
        data: {
            "PageEvent": "LoadMap",
            "PicID": PicID
        },
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
                    }).text(m_obj.EquNo + " ■").on("click", SetClick);
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


function SetClick(d) {
    //console.log(area_list);
    alert(d3.select(this).attr('equ_id') + ' '+d3.select(this).attr('equ_name') + ' 斷連線時間：' + d3.select(this).attr('alive_time').toString());
}
