<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Video2.aspx.cs" Inherits="SahoAcs.Web._06._0601.Video2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="placeholder">
                <img style="width:800px;height:600px" id="video" src="" />
         </div>
        <script type="text/javascript">
            "use strict";
			var imgbuffer = new Image();
			var done=true;
			var timestamps = 1475468265974;
			var ts = new Date();
            var initialTimeout = 1000,              // Time before 1st frame in ms
                fetchTimeout = 100,                 // Time gap between video requests in ms
                placeholderId = 'placeholder',      // DIV element id
                imageId = 'video',                  // IMG element id
                videoLink = [                       // URI components
                    'http://192.168.0.151:8080',
                    '/archive-video-frame.dyn',
                    '?device={localhost:60554-Media%20Source%5c001}',
					'&ts=' + ts.getTime(),
					'&time='+timestamps,
                    '&u=admin',                    
					'&p=d41d8cd98f00b204e9800998ecf8427e',
                    '&res=4CIF',
                    '&still=yes'
                ],
                videoLinkStr = videoLink.join(''),  // URI string representation				
                UpdateImage = function()            // Time event callback
                {
					if(done){
						var placeholder = UpdateImage.placeholder =
								(UpdateImage.placeholder != undefined ?
									UpdateImage.placeholder :                           // Cached element
									document.getElementById(placeholderId)),
							oldImage = document.getElementById(imageId),                // Live element
							newImage = new Image();   							// New img object					
							
						newImage.onload = function(ev){  // Success callback								
									placeholder.replaceChild(this, oldImage);								
									tmId = setTimeout(UpdateImage, fetchTimeout);
									//var c = document.getElementById("myCanvas");
									//var ctx = c.getContext("2d");								
									//ctx.drawImage(oldImage, 10, 10);								
								};
						newImage.onerror = function(ev){                                // Error callback
									var loadingError = document.createElement("h3");
									loadingError.innerHTML = new Date() + ' Error: ' + 'Can\'t load image.' + videoLink[2];
									placeholder.appendChild(loadingError);
									tmId = setTimeout(UpdateImage, initialTimeout);
								};					    
						newImage.setAttribute("id", imageId);                           // IMG element id						
						newImage.setAttribute('src', videoLinkStr);        // Initiate receiving video frame						
						timestamps += -1;
						videoLink = [                       // URI components
							'http://192.168.0.151:8080',
							'/archive-video-frame.dyn',
							'?device={localhost:60554-Media%20Source%5c001}',							
							'&time=' + timestamps,                            
							'&u=admin',                    
							'&p=d41d8cd98f00b204e9800998ecf8427e',
							'&res=4CIF'                            
						];
						videoLinkStr = videoLink.join('');
					}
                },
                tmId = setTimeout(UpdateImage, initialTimeout);                     // Start main refresh loop																
        </script>	
    </form>
</body>
</html>
