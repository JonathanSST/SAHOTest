<%@ Page  Title="" Theme="UI"  Language="C#" MasterPageFile="~/Site1.Master"  AutoEventWireup="true" CodeBehind="ImportHoliday.aspx.cs" Inherits="SahoAcs.Web._01._0117.ImportHoliday" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

       
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
                <fieldset id="PsnCard_List" runat="server">
                    <legend id="PsnCard_Legend" runat="server" >請假憑證匯入</legend>
                    <table class="Item">
                        <tr>
                            <td>                                
                                <input type="file" value="選擇檔案" name="UploadFile" id="UploadFile" multiple="multiple" />
                            </td>
                            <td>                                
                                <input type="button" value="匯入請假資料" id="BtnImport" style="font-size:small;color:aliceblue" name="BtnImport" onclick="UploadFileAsync()" class="IconImport" />
                                 <%--<input type="button" value="清空資料表" id="BtnClear" style="font-size: small; color: aliceblue" name="BtnImport" onclick="ClearData()" class="IconDelete" />--%>
                                <span id="SpanBusing" style="display:none;color:white;background-color:red">資料匯入中...</span>
                            </td>                                                        
                        </tr>
                        <tr>
                            <td colspan="2">
                                <span>訊息</span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <textarea id="ProcMessage" name="ProcMessage" style="width:100%;height:320px;" rows="10" cols="20" readonly="readonly">
                                </textarea>                                
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
