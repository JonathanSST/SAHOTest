var vmshost = $("#vmshost").val();
var device = $("#device").val();

function DelSec() {
    done = false;
    $("#BtnStop").val("Start");
    start_date = new Date();
    video_url = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
    tmid = setTimeout(draw, 45);
    ts = ts - 5000;
    img = new Image();
    img.src = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=' + ts.toString()
        + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
    tmid = setTimeout(draw, 1000);
    done = true;
    $("#BtnStop").val("Stop");
}


function AddSec() {
    done = false;
    $("#BtnStop").val("Start");
    start_date = new Date();
    video_url = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
    tmid = setTimeout(draw, 45);
    ts = ts + 5000;
    img = new Image();
    img.src = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=' + ts.toString()
        + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
    tmid = setTimeout(draw, 1000);
    done = true;
    $("#BtnStop").val("Stop");
}


function SetArchive(times) {
    start_date = new Date();
    ts = ts + times;
    if (img) {

    } else {
        img = new Image();
        img.src = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=' + ts.toString()
            + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
    }
    done = true;
    tmid = setTimeout(draw, 1000);
    $("#BtnStop").val("Stop");
}


function Stop() {
    if ($("#BtnStop").val() == "Stop") {
        done = false;
        $("#BtnStop").val("Start");
        start_date = new Date();
        video_url = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=-1&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&still=yes&res=4CIF&ts=' + start_date.getTime().toString();
        tmid = setTimeout(draw, 100);
    } else {
        start_date = new Date();
        if (img) {

        } else {
            img = new Image();
            img.src = vmshost + '/archive-video-frame.dyn?device=' + device + '&time=' + ts
                + '&u=admin&p=d41d8cd98f00b204e9800998ecf8427e&res=4CIF&still=yes&ts=' + start_date.getTime();
        }
        tmid = setTimeout(draw, 1000);
        done = true;
        $("#BtnStop").val("Stop");
    }
}




//var tmid;

