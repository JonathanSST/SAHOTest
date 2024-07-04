function SelectState() {
    window.alert = null;
    delete window.alert;
    elm = window.form1;
    var data = '';
    for (var i = 0; i < elm.length; i++) {
        var str = elm[i].name.split('$');
        var strlen = str.length - 1;
        if (str[strlen] == "RowCheckState1") {
            if (elm.elements[i].checked == true)
                data = '1' + data;
            else
                data = '0' + data;
        }
    }
    hFloor.value = data;
}

function CheckBoxEnable() {
    var cb = document.getElementById('GridView1_CheckBox1');
    cb.disabled = true;
    BtSave.disabled = true;
}

function CheckBoxData(data) {
    console.log(data);
    if (data.length > 0) {
        var num = 0;
        var allstate = 0;
        window.alert = null;
        delete window.alert;
        elm = window.form1;
        for (var i = 0; i < elm.length; i++) {
            var str = elm[i].name.split('$');
            var strlen = str.length - 1;
            if (str[strlen] == "RowCheckState1") {
                if (data[num] == '1')
                    elm.elements[i].click();
                else
                    allstate++;
                num++;
            }
        }
        if (allstate == 0) {
            var cb = document.getElementById('GridView1_CheckBox1');
            cb.click();
        }
    }
}