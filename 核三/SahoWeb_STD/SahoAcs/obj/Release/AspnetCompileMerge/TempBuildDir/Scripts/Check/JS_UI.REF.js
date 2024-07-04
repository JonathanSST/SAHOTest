/*
=========================================================================================
//頁面初始化
=========================================================================================*/
function JsFunFIELD_REF_BIND(I_OBJ) {
    var PARAM_FIELD_REF = JsFunGET_PARAM('FIELD_REF');
    var strREF_ID = $(I_OBJ).attr("id");
    var strVAL = $(I_OBJ).val();
    var ARR_SRC = PARAM_FIELD_REF[strREF_ID]["SRC"];
    var ARR_DSC = PARAM_FIELD_REF[strREF_ID]["DSC"];

    for (var i = 0; i < ARR_SRC.length; i++) {
        var strSRC_VAL = $(ARR_SRC[i]).val();

        if (strREF_ID == $(ARR_SRC[i]).attr('id'))
            strSRC_VAL = strVAL;

        $(ARR_DSC[i]).val(strSRC_VAL);
    }
}

//======================================================================
function JsFunREF_INIT(I_OBJ) {
    var oFORM0 = JsFunGET_PARAM("FORM")[0];
    var PARAM_FIELD_REF = JsFunGET_PARAM('FIELD_REF');
    var oTR = null;
    var oINPUT = null;
    var oFIELD_REF = null;

    /*
    =========================================================================================
    //設定參考物件(當參考物件失去焦點時, 被參考物件會更新為參考物件的值)
    =========================================================================================*/
    $(I_OBJ).find(":input[FIELD_REF]").each(function (index) {
        var strFIELD_REF = $(this).attr("FIELD_REF");
        oINPUT = this;
        oFIELD_REF = null;

        //先搜尋所在列
        oTR = JsFunFindParent(oINPUT, 'TR');
        oFIELD_REF = $(oTR).find(':input[name="' + strFIELD_REF + '"]:eq(0)');
        if ($(oFIELD_REF).length > 0) {
            oFIELD_REF = oFIELD_REF[0];
        } else {
            //往上階層尋找參考
            oFIELD_REF = null;
            for (var i = 0; i < oFORM0.length; i++) {
                //先找到自身階層     
                if ($(oFORM0[i]).attr('id') == $(oINPUT).attr('id')) {
                    for (var j = i; j > -1; j--) {
                        //從自身階層往上找參考
                        if ($(oFORM0[j]).attr('name') == strFIELD_REF) {
                            oFIELD_REF = oFORM0[j];
                            break;
                        }
                    }
                }

                if (oFIELD_REF != null)
                    break;
            }
        }

        //設定參考值
        if (oFIELD_REF != null) {
            var strREF_ID = $(oFIELD_REF).attr('id');

            //假設被參考不存在字典裡, 建立字典參考
            if (PARAM_FIELD_REF[strREF_ID] == undefined) {
                PARAM_FIELD_REF[strREF_ID] = { SRC: new Array(), DSC: new Array() };

                //設定參考物件事件, 當離開焦點時更新子控制項
                var oOBJ = JsFunATTR_BIND(oFIELD_REF, 'onblur', "JsFunFIELD_REF_BIND(this);", false);
                //var oOBJ = JsFunATTR_BIND(oFIELD_REF, 'onchange', "JsFunFIELD_REF_BIND(this);", false);
                oFIELD_REF.outerHTML = oOBJ.outerHTML;
            }

            //設定參考快取
            PARAM_FIELD_REF[strREF_ID]["SRC"].push(oFIELD_REF);
            PARAM_FIELD_REF[strREF_ID]["DSC"].push(oINPUT);

            //更新為參考值
            $(oINPUT).val($(oFIELD_REF).val());
        }
    });
}

