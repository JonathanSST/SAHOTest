<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParaSettingBox.aspx.cs" Inherits="SahoAcs.ParaSettingBox"  EnableEventValidation="false" %>

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
                            <legend id="Elevator_Legend" runat="server" style="font-size: 15px">「<%=this.EquName %>」參數列表</legend>
                            <table class="TableS3" border="0">
                                <tbody>
                                    <tr>
                                        <th style="width: 39px;" scope="col">項目</th>
                                        <th style="width: 260px;" scope="col">參數名稱</th>
                                        <th style="width: 140px;" scope="col">參數數值</th>
                                        <th scope="col">重新傳送</th>
                                    </tr>
                                    <tr>
                                        <td style="padding: 0px;" colspan="4">
                                            <div id="Elevator_Panel" style="height: 300px;overflow-y: scroll;width:100%">
                                                <div>
                                                    <table id="ParaGridView" style="width:100%;border-width: 0px; border-collapse: collapse;" rules="all" cellspacing="0">
                                                        <tbody>
                                                            <%foreach(System.Data.DataRow r in this.ProcessTable.Rows){ %>
                                                            <tr style="background-color:#069; color:#fff">
                                                                <td style="width: 41px;text-align:center"><%=r["Seq"].ToString() %>                                                                    
                                                                </td>
                                                                <td style="width: 262px;"><%=r["ParaDesc"].ToString() %></td>
                                                                <td style="width:142px;text-align:center">
                                                                    <%if(r["ParaUI"].ToString().Equals("0")){ %>
                                                                        <input type="text" style="width:125px" value="<%=r["ParaValue"].ToString() %>" name="ParaValue" />
                                                                    <%}
                                                                      else if (r["ParaUI"].ToString().Equals("1"))
                                                                      { %>
                                                                        <input type="text" style="width:125px" value="<%=r["ParaValue"].ToString() %>" name="ParaValue" />
                                                                    <%}
                                                                      else if (r["ParaUI"].ToString().Equals("2"))
                                                                      { %>
                                                                    <select name="ParaValue" style="width: 125px" class="DropDownListStyle">
                                                                        <%foreach (var s in r["ValueOptions"].ToString().Split('/'))
                                                                        { %>
                                                                        <%if (r["ParaValue"].ToString() == s.Split(':')[1])
                                                                            { %>
                                                                        <option selected="selected" value="<%=s.Split(':')[1] %>"><%=s.Split(':')[0] %></option>
                                                                        <%}
                                                                        else
                                                                        { %>
                                                                        <option value="<%=s.Split(':')[1] %>"><%=s.Split(':')[0] %></option>
                                                                        <%} %>
                                                                        <%} %>
                                                                    </select>
                                                                    <%}
                                                                    else
                                                                    { %>
                                                                        <input name="ParaBtn" class="IconSet" id="ParaValue" style="margin: 2px; padding: 2px 10px; width: 135px; font-size: 10pt;"
                                                                            type="button" value="設　　定" onclick="PopURL(this)"  />
                                                                        <input type="hidden" name="ParaValue" value="<%=r["ParaValue"].ToString() %>" />
                                                                    <%} %>
                                                                        <input type="hidden" value="<%=r["EquID"].ToString() %>" name="EquID" />
                                                                        <input type="hidden" value="<%=r["EquParaID"].ToString() %>" name="EquParaID" />
                                                                        <input type="hidden" value="<%=r["EditFormUrl"].ToString() %>" name="FormUrl" />                                                                        
                                                                        <input type="hidden" value="<%=r["ParaUI"] %>" name="ParaUI" id="ParaUI" />
                                                                        <input type="hidden" value="<%=r["ParaDesc"] %>" name="ParaDesc" id="ParaDesc" />
                                                                    <input type="hidden" value="<%=r["MinValue"] %>" name="MinValue" id="MinValue" />
                                                                    <input type="hidden" value="<%=r["MaxValue"] %>" name="MaxValue" id="MaxValue" />
                                                                        <input type="hidden" value="<%=r["ParaValue"] %>" name="M_ParaValue" id="M_ParaValue" />
                                                                </td>
                                                               <td onclick="ChangeCbxStatus(this)">
                                                                   <input type="checkbox" style="width:25px;height:25px" id="CHK_COL_1" name="CHK_COL_1" value="<%=r["EquParaID"].ToString() %>" /></td>
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
                        <input type="button" value="<%=Resources.Resource.btnSave %>" class="IconSave" id="popB_Save" onclick="SaveParaData()" />    
                        <input type="button" value="<%=Resources.Resource.btnRefresh %>" class="IconRefresh" id="popB_Refresh" onclick="DoRefresh()" />                    
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
