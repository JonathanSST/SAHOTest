<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquCodeSetting.aspx.cs" Inherits="SahoAcs.Web._01._0103.EquCodeSetting" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="SahoAcs.DBClass" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form_adj" runat="server">
<div id="MainDiv" style="background-color: #2D89EF; border-color: #818181; border-width: 1px; border-style: solid;height:480px">            
            <div>                                
                <table class="popItem">
                <tr>
                    <th colspan="3">
                        <span class="Arrow01"></span>
                        <asp:Label ID="Label2" runat="server" Text="<%$Resources:lblCardSetCode %>" Font-Bold="True"></asp:Label>
                    </th>
                </tr>
                </table>
                <table>
                <tr>
                    <td style="height: 335px; width:720px; vertical-align: top">
                        <fieldset id="Fieldset3" runat="server" style="width: 660px; height: 355px">
                            <legend id="Legend3" runat="server">
                                <asp:Label ID="Label4" runat="server" Text="<%$Resources:ttEquList %>"></asp:Label>
                            </legend>                            
                            <table class="TableS1">
                                <tbody>
                                    <tr class="GVStyle">                                        
                                        <th scope="col" style="width: 100px;"><%=this.GetLocalResourceObject("ttEquNo") %></th>
                                        <th scope="col" style="width:220px"><%=this.GetLocalResourceObject("ttEquName") %></th>
                                        <th scope="col" style="width:80px"><%=this.GetLocalResourceObject("ttCtrlNo") %></th>
                                        <th scope="col"><%=this.GetLocalResourceObject("ttSetCode") %></th>
                                    </tr>
                                    <tr>
                                        <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                            <div id="ContentPlaceHolder1_tablePanel2" style="height: 290px; width: 660px; overflow-y: scroll;">
                                                <div>
                                                    <table class="GVStyle" cellspacing="0" rules="all" border="1" 
                                                        id="TableCode" style="border-collapse: collapse;">
                                                        <tbody>                                                            
                                                            <%for (int i = 0; i < this.DataResult.Rows.Count;i++ )
                                                              { %>
                                                                <tr>                                                                    
                                                                    <td style="width:104px">
                                                                        <%=this.DataResult.Rows[i]["EquNo"].ToString() %>
                                                                    </td>
                                                                    <td style="width:224px">
                                                                        <%=this.DataResult.Rows[i]["EquName"].ToString() %>
                                                                    </td>
                                                                    <td style="width:84px">
                                                                        <%=this.DataResult.Rows[i]["CtrlNo"].ToString() %>
                                                                    </td>            
                                                                    <td>
                                                                        <%--<input style="text-align:center;width:90%" type="button" value="<%=this.GetLocalResourceObject("btnSetByEqu") %>" class="IconPassword"/>--%>
                                                                        <input type="hidden" value="<%=this.DataResult.Rows[i]["EquID"].ToString() %>" name="EquID"/>
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
                    <td style="text-align:center;width:720px">
                        <input id="BtnCodeSetting" type="button" value="<%=this.GetLocalResourceObject("btnSetByCard") %>" class="IconSave" onclick="DoSetting()"/>
                        <input type="button" value="<%=this.GetGlobalResourceObject("Resource","btnExit") %>" class="IconCancel" onclick="DoCancel()"/>
                    </td>
                </tr>
            </table>
            <input type="hidden" name="CardId" id="CardId" value="<%=this.GetFormEqlValue("card_id") %>" />    
            <input type="hidden" name="DoAction" value="Save" />
            <input type="hidden" name="ResultMsg" id="ResultMsg" value="<%=this.GetLocalResourceObject("ResultMsg") %>" />
            </div>              
        </div>
       <%-- <script type="text/javascript">
            $("#ContentPlaceHolder1_GridView2").find("tr").each(function () {
                var hid = $(this).find('input[name="OpMode_hid"]:eq(0)').val();
                //console.log(hid);
                if (hid != "") {
                    $(this).find('input[name="ChkEquID"]:eq(0)').prop("checked", true);
                }
            });
        </script>--%>
    </form>
</body>
</html>
