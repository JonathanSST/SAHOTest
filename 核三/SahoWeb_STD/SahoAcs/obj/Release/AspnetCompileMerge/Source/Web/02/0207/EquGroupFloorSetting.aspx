<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquGroupFloorSetting.aspx.cs" Inherits="SahoAcs.EquGroupFloorSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>FloorSetting</title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
            <asp:HiddenField ID="hideFloorName" runat="server" />
        </div>
        <div>
            <table border="0">
                <tr>
                    <td style="height: 450px; width:620px; vertical-align: top" id="EquArea">
                        <fieldset id="Fieldset3" runat="server" style="width: 580px; height: 430px">
                            <legend id="Legend3" runat="server">
                                <asp:Label ID="Label4" runat="server"><%=this.GetLocalResourceObject("ttElevCtrl") %>(<%=this.EquName %>)</asp:Label>
                            </legend>                            
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">                                        
                                        <th scope="col" style="width: 70px;"><input type="checkbox" id="CheckAll" onchange="SetCheckAll(this)" /></th>
                                        <th scope="col" style="width:240px"><%=this.GetLocalResourceObject("ttIO") %></th>
                                        <th scope="col"><%=this.GetLocalResourceObject("ttFloor") %></th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="4">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 385px; width: 580px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" 
                                                        id="ContentPlaceHolder1_GridViewFloor" style="border-collapse: collapse;">
                                                        <tbody>                                                            
                                                            <%foreach(var a in this.list_floor){ %>
                                                                <tr>                                                                    
                                                                    <td style="width:74px;text-align:center">                                                                                                                                                
                                                                        <input type="checkbox" name="CheckIO" id="CheckIO" value="<%=a.IoIndex %>" />
                                                                    </td>
                                                                    <td style="width:244px">
                                                                        <%=a.IoIndex %>
                                                                    </td>
                                                                    <td>                                                                           
                                                                        <%=a.FloorName %>
                                                                    </td>                                                                    
                                                                </tr>
                                                            <%} %>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center">      
                        <input type="hidden" value="<%=this.ext_data_bin%>" id="FloorData" />                  
                        <input type="hidden" value="<%=this.EquIndex %>" id="EquIndex" />
                        <input type="button" id="btnUpdate" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconChange" />
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
