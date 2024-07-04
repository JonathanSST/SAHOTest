/**
 * 
 */
 /**
  * 查詢元件的index
  * **/
 var query_index = 0;
 
 function adjustAutoLocation(divname, prodBoxName, top_distance, left_distance) {

     var top = 0;
     var left = 0;
     var objTop = cumulativeOffset(prodBoxName)[1];
     var objLeft = cumulativeOffset(prodBoxName)[0];
     var docheight = $(document).height();
     var windowWidth = $(window).width();
     //top_distance
     if (top_distance == undefined) {
         divname.style.top = objTop - realOffset(prodBoxName)[1] + document.body.scrollTop + 22 + 'px';
     } else {
         //top = top_distance;
         //divname.style.top = objTop - realOffset(prodBoxName)[1] + document.body.scrollTop + top + 'px';
         top = (objTop + $(divname).height() - docheight);       //取得差異高度
         if (top < 0) {        	 
             top = objTop + 22;
         } else {
        	 //console.log(objTop);
             //console.log($(divname).height());
        	 if(objTop < $(divname).height()){
        		 top = $(divname).height()-objTop;	 
        	 }else{
        		 top = objTop - $(divname).height();
        	 }
             
         }         
         divname.style.top = top + 'px';
     }

     //left_distance
     if (left_distance == undefined) {
         divname.style.left = objLeft - document.body.scrollLeft + 22 + 'px';
     } else {
         //left = left_distance;
         //divname.style.left = objLeft + left + 'px'; //+document.body.scrollLeft
         left = (objLeft + $(divname).width() - windowWidth);
         if (left < 0) {
             left = objLeft;
         } else {
             left = objLeft - left;
         }
         divname.style.left = left + 'px';
     }
 }



    //當進行快顯工作前，須關閉作用中的選單
    function closeAllCompont() {
        $("#dvContent").html("");
    }

    document.addEventListener("onmousedown", hiddenCompont);

    //隱藏開啟中的元件
    function hiddenCompont(event) {
        //alert(1);
        if (mainDiv != null) {
            var offsetX = 0;
            var offsetY = 0;
            var scrollX = 0;
            var scrollY = 0;
            var cX = event.clientX;
            var cY = event.clientY;

            offsetX = cumulativeOffset(mainDiv)[0];
            offsetY = cumulativeOffset(mainDiv)[1];
            scrollX = realOffset(mainDiv)[0];
            scrollY = realOffset(mainDiv)[1];
            
            if (cX >= (offsetX - document.body.scrollLeft) && cX <= ((offsetX + mainDiv.offsetWidth) - document.body.scrollLeft) && cY >= (offsetY - document.body.scrollTop - scrollY)) {

            } else {
                closeAllCompont();
            }
        }
    }

    function realOffset(element) {
        var valueT = 0, valueL = 0;
        do {
            valueT += element.scrollTop || 0;
            valueL += element.scrollLeft || 0;
            element = element.parentNode;
        } while (element);
        return [valueL, valueT];
    }

    function cumulativeOffset(element) {
        var valueT = 0, valueL = 0;
        do {
            valueT += element.offsetTop || 0;
            valueL += element.offsetLeft || 0;
            element = element.offsetParent;
        } while (element);
        return [valueL, valueT];
    }