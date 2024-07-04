<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LightInfoCopy.aspx.cs" Inherits="SahoAcs.LightInfoCopy"  EnableEventValidation="false" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <base target="_self" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>ParaSetting</title>
    <link href="/Css/Reset.css" rel="stylesheet" type="text/css" />
    <link href="/Css/StyleWindow.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">               
        <div id="MasterDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid">
            <div id="ValueKeep">
            <asp:HiddenField ID="hideUserID" runat="server" />
            <asp:HiddenField ID="hideEquID" runat="server" /> 
            <asp:HiddenField ID="hideCtrlID" runat="server" /> 
            <input type="hidden" value="Save" id="PageEvent" name="PageEvent" />
            <input type="hidden" value="<%=Request["ParaType"] %>" id="ParaType" name="ParaType" />
        </div> 
            <table class="Item" style="background-color: #069">
                <tr>
                    <td>
                        <fieldset id="Elevator_List" name="Elevator_List" runat="server" style="width: 600px">
                            <legend id="Elevator_Legend" runat="server" style="font-size: 15px">來源設備『<%=Request["EquNo"] %>』燈號資訊複製設備列表</legend>
                            <table class="TableS3" border="0">
                                <tbody>
                                    <tr>
                                        <th style="width: 39px;" scope="col">項目</th>
                                        <th style="width: 160px;" scope="col">設備編號</th>
                                        <th style="" scope="col">設備名稱</th>                                        
                                    </tr>
                                    <tr>
                                        <td style="padding: 0px;" colspan="4">
                                            <div id="Elevator_Panel" style="height: 300px;overflow-y: scroll;width:100%">
                                                <div>
                                                    <table id="ParaGridView" style="width:100%;border-width: 0px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                        <tbody>
                                                            <%foreach(var r in this.ProcessTable){ %>
                                                            <tr style="background-color:#069; color:#fff">
                                                                <td style="width: 41px;text-align:center">
                                                                    <input type="checkbox" style="width:25px;height:25px" id="CHK_COL_1" name="CHK_COL_1" value="<%=r.EquID %>" checked="checked" />
                                                                </td>
                                                                <td style="width: 162px;"><%=r.EquNo %></td>
                                                                <td style="text-align:left">
                                                                    <%=r.EquName %>
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
                        <input type="button" value="複製" class="IconSave" id="popB_Save" onclick="SaveParaData()" />                            
                        <input type="button" value="<%=Resources.Resource.btnCancel %>" class="IconCancel" id="popB_Cancel" onclick="DoCancel()" />                        
                        <input type="hidden" value="<%=Request["EquNo"] %>" id="EquNo" name="EquNo" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="ParaExtDiv" style="position:absolute;left:20px;top:30px;z-index:1000000000;background-color:#1275BC; 
                border-style:solid; border-width:2px; border-color:#069" ></div>
    </form>
</body>
</html>
