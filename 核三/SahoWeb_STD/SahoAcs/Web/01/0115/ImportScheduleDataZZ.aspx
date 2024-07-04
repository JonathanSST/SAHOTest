<%@ Page  Title="" Theme="UI"  Language="C#" MasterPageFile="~/Site1.Master"  AutoEventWireup="true" CodeBehind="ImportScheduleDataZZ.aspx.cs" Inherits="SahoAcs.Web._01._0115.ImportScheduleDataZZ" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function DoUpload()
        {
            $("#form1").prop('enctype', 'multipart/form-data');
            $("#form1").submit();
        }

        //傳入FileControl，配合在asp.net的page load當中
        function UploadFileAsync() {
            var FileControl = document.getElementById('UploadFile');
            var formData = new FormData();
            //抓取File Control中的檔案            
            var file = FileControl.files[0];
            formData.append("file", file);
            formData.append("PageEvent", "Save");
            //非同步post給自己
            var uploadServerSideScriptPath = window.location.href;
            $("#SpanBusing").css("display", "block");
            $.ajax({
                type: "POST",
                url: uploadServerSideScriptPath,
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (data) {
                    //$("#ContentPlaceHolder1_UpdatePanel1").html($(data).find("#ContentPlaceHolder1_UpdatePanel1").html());                    
                    //$("#ResultArea").html($(data).find("#ResultArea").html());
                    //SetDataScroll();
                    $("#SpanBusing").css("display", "none");                          
                    alert(data);      
                    $("#form1")[0].reset();
                }
            });
        }

        function ClearData() {
            $("#form1")[0].reset();
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideErrorData" runat="server" EnableViewState="False" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Save" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <fieldset id="PsnCard_List" runat="server" style="width:550px">
                    <legend id="PsnCard_Legend" runat="server" >次月排班表匯入</legend>
                    <table>
                        <tr>
                            <td>                                
                                <input type="file" value="選擇檔案" name="UploadFile" id="UploadFile" multiple="multiple" />
                            </td>
                            <td>                                
                                <input type="button" value="匯入次月排班表資料" id="BtnImport" style="font-size:small;color:aliceblue" name="BtnImport" onclick="UploadFileAsync()" class="IconImport" />
                                 <input type="button" value="清空資料表" id="BtnClear" style="font-size: small; color: aliceblue" name="BtnImport" onclick="ClearData()" class="IconDelete" />
                                <span id="SpanBusing" style="display:none;color:white;background-color:red">資料匯入中...</span>
                            </td>                                                        
                        </tr>
                    </table>                    
                </fieldset>
            </td>
        </tr>        
    </table>
    <style>
        fieldset {
            border: solid;
            border-radius: 10px;
            color: aliceblue;
        }
            fieldset legend {
                font-size: 20px;
                color: aliceblue;
            }
            #BtnImport{

            }
    </style>


</asp:Content>
