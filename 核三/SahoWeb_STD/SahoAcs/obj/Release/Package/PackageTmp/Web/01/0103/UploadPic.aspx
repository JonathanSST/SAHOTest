<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadPic.aspx.cs" Inherits="SahoAcs.Web.UploadPic" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tbody>                
                <tr>                    
                    <td style="text-align: center">
                        <img style="height:189px;width:148px" src="<%=this.oPicStr %>" /><br />
                         <input id="file" type="file" value="UploadFile" accept="image/jpeg,image/png"/>
                    </td>
                </tr>
                <tr>                    
                    <td style="text-align: center">                              
                        <input type="button" id="btnUpload" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconSave" />
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel"/>
                        <input type="hidden" id="PsnID" value="<%=this.PsnEntity.PsnID %>" name="PsnID"/>
                        <input type="hidden" id="PsnIDNum" value="<%=this.PsnEntity.IDNum %>" name="PsnIDNum"/>
                    </td>
                </tr>                
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
