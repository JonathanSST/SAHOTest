

function funTabEnter() {
    //if (event.keyCode == 13) { // 13 Enter
    //    //closeAllCompont();
    //    event.keyCode = 9; // 9 Tab        
    //}    

    var focusables = $(':input[type=text],select,textarea').not($("#NewRow").find(':input[type=text],select,textarea'));
    //alert(focusables.length);
    focusables.keydown(function (e) {        
        if (e.keyCode == 13) {            
            var current = focusables.index(this),
                                                 next = focusables.eq(current + 1).length ? focusables.eq(current + 1) : focusables.eq(0);
            //alert(focusables.eq(current + 1).length);
            next.focus();
            //closeAllCompont();
        }        
    });
}

/***
jQuery.fn.enter2tab = function(){    
    this.onkeydown(function(e)    
    {        
        // get key pressed (charCode from Mozilla/Firefox and Opera / keyCode in IE)        
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;         
        var tmp = null;        
        var maxTabIndex = 9999;         
        // get tabindex from which element keypressed        
        var nTabIndex = this.tabIndex + 1;         
        // get element type (text or select)        
        var myNode = this.nodeName.toLowerCase();         
        // allow enter/return key (only when in an input box or select)        
        if (nTabIndex > 0&& key == 13&& nTabIndex <= maxTabIndex&& ((myNode == "textarea") || (myNode == "input") || (myNode == "select") || (myNode == "a")))
        {
            for (var x = 0; x < 3; x++)            
            {
                tmp = $("a[tabIndex='" + nTabIndex + "'],textarea[tabIndex='" + nTabIndex + "'],select[tabIndex='" + nTabIndex + "'],input[tabIndex='" + nTabIndex + "']").get(0);                
                if (typeof tmp != "undefined" && !$(tmp).attr("disabled"))                
                {
                    alert(123);
                    $(tmp).focus();
                    return false;                    
                    //break;                
                }
                else
                {
                    //如果要循環的話，就解開以下的封印                    
                    //var first = $("a[tabIndex='1'],textarea[tabIndex='1'],select[tabIndex='1'],input[tabIndex='1']").get(0);
                    //$(first).focus();                                        
                    return false;                
                }
            }
            return false;
        }
        else if (key == 13)
        {
            return false;        
        }
    })
    return this;}

    ***/