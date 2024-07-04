<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormDataInput.aspx.cs" Inherits="SahoAcs.Unittest.FormDataInput" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="<%=".."+Pub.JqueyNowVer %>"></script>
    <script type="text/javascript">
        function FileSubmit() {
            $("#__VIEWSTATE").val('');
            $("#PageEvent").val("Save");
            var oFORM1 = document.getElementById("form1");
            var formData = new FormData(oFORM1);
            console.log(formData);
            $.ajax({
                url: "FormDataInput.aspx",  //Server script to process data
                type: "post",
                //Ajax events                 
                success: function (result) {

                },
                // Form data
                data: formData,
                //Options to tell jQuery not to process data or worry about content-type.
                cache: false,
                contentType: false,
                processData: false
            });

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">    
    <div>
        <input type="hidden" id="PageEvent" name="PageEvent" value="" />
        <input type="text" name="data001" value="1234" /><br />
        <input type="text" name="data002" value="5678" />
        <input type="button" value="儲存" onclick="FileSubmit()" />            
    </div>
    </form>
</body>
</html>
