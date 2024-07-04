<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FloorSettingPop.aspx.cs" Inherits="SahoAcs.FloorSettingPop" %>
<%@ Import Namespace="SahoAcs.DBModel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link href="../../../Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="../../../Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body onload="SetLoadData()">
    <form id="form1" runat="server">
<div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" />
            <asp:HiddenField ID="hideEquParaID" runat="server" />
            <asp:HiddenField ID="hideParaValue" runat="server" />
            <asp:HiddenField ID="hideParaName" runat="server" />
            <asp:HiddenField ID="hideFloorData" runat="server" />
        </div>
        <div>
            <table border="0">
                <tr>
                    <td style="height: 450px; width:620px; vertical-align: top" id="EquArea">
                        <fieldset id="Fieldset3" runat="server" style="width: 580px; height: 430px">
                            <legend id="Legend3" runat="server">
                                <%=this.ParaDesc %>
                            </legend>                            
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">                                                                               
                                        <th scope="col" style="width:240px">接點編號</th>
                                        <th scope="col" style="width:240px">樓層名稱</th>
                                         <th scope="col"><input type="checkbox" style="width:25px;height:25px" id="ChkAll" name="ChkAll" onclick="SetAllData(this)"/></th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="4">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 385px; width: 580px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" 
                                                        id="ContentPlaceHolder1_GridViewFloor" style="border-collapse: collapse;">
                                                        <tbody>                                                            
                                                            <%foreach(var a in this.FloorList){ %>
                                                                <tr>                                                                                                                                        
                                                                    <td style="width:244px">
                                                                        <%=a.IoIndex %>
                                                                    </td>
                                                                    <td style="width:244px">                                                                           
                                                                        <%=a.FloorName %>
                                                                    </td>
                                                                    <td style="text-align:center">                                                                                                                                                
                                                                        <input type="checkbox" style="width:25px;height:25px" name="CheckIO" id="CheckIO" value="<%=a.IoIndex %>" />
                                                                         <input type="hidden" value="<%=this.ParaName %>" id="ParaName" />
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
                        <input type="hidden" value="<%=this.FloorBinValue %>" id="FloorData" />              
                        <input type="button" id="btnUpdate" value="<%=this.GetGlobalResourceObject("Resource","btnOK") %>" class="IconChange" onclick="DoSave()"/>
                        <input type="button" id="btnCancel" value="<%=this.GetGlobalResourceObject("Resource","btnCancel") %>" class="IconCancel" onclick="DoClose()"/>
                    </td>
                </tr>
            </table>            
        </div>
    </form>
</body>
</html>
