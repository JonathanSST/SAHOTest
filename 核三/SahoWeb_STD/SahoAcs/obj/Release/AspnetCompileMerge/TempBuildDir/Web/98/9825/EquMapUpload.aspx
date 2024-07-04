<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquMapUpload.aspx.cs" Inherits="SahoAcs.Web._98._9825.EquMapUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
   <form id="MapForm" runat="server">
        <div>
            <table>
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" style="width: 300px">圖幅名稱</th>
                    <th scope="col" style="width: 300px">上傳檔案</th>
                </tr>
                <tr class="GVStyle">
                    <th scope="col" style="width: 300px">
                        <input type="text" style="width:98%" name="PicDesc" id="PicDesc" value="<%=this.mapobj.PicDesc %>" />
                    </th>
                    <th scope="col" style="width: 300px">
                        <input type="file" name="Uploadfile" id="Uploadfile" multiple="multiple" accept="image/jpeg,image/png" />
                    </th>                    
                </tr>
                <tr class="GVStyle" >
                    <th scope="col" colspan="2" style="text-align: left">
                        是否顯示
                        <asp:RadioButtonList ID="rblIsOpen" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" RepeatLayout="Flow" Width="250px" Font-Size="Small" ForeColor="Black">
                                <asp:ListItem Selected="True" Value="1">是</asp:ListItem>
                                <asp:ListItem Value="0">否</asp:ListItem>
                            </asp:RadioButtonList>
                    </th>
                </tr>
                <tr>
                    <td style="text-align: center" colspan="2">
                        <progress></progress>
                        <div style="color: black" data-id="fileContainer">
                            <ul style="list-style: none; padding: 0px;"></ul>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center" colspan="2">                              
                        <input type="button" id="btnFileUpload" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave" />
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel"/>
                        <input type="hidden" value="<%=this.mapobj.PicID %>" name="PicID" id="PicID" />
                        <input type="hidden" value="Add" id="PageEvent" name="PageEvent" />
                    </td>
                </tr>                
            </tbody>
        </table>
        </div>
    </form>
</body>
</html>
