<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PersonRes.aspx.cs"
    Inherits="SahoAcs.Web._01._0103.PersonRes" Theme="UI" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../../../DatePicker/jquery.datepick.css" rel="stylesheet" />       
    <script src="../../../DatePicker/jquery.datepick.js"></script>
    <script src="../../../DatePicker/jquery.datepick-zh-TW.js"></script>
    <script src="../../../scripts/Check/JS_AJAX.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UTIL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_BUTTON_PASS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_CHECK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LEVEL.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.DATE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.ETEK.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.HT.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.LIST.js" type="text/javascript"></script>    
    <script src="../../../scripts/Check/JS_UI.REF.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABLE.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TABS.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI.TOOLTIP.js" type="text/javascript"></script>
    <script src="../../../scripts/Check/JS_UI_FILE_UPLOAD.js" type="text/javascript"></script>
    <script src="../../../Scripts/JsTabEnter.js" type="text/javascript"></script>
    <script src="../../../scripts/JsQueryWindow.js" type="text/javascript"></script>
    
    
    <script type="text/javascript">
        var ML_JS_CHECK = {};
        //ML_JS_CHECK
        ML_JS_CHECK["天"] = '天';
        ML_JS_CHECK["日期格式錯誤，應為 YYYY(西元年)/MM/DD"] = '日期格式錯誤，應為 YYYY(西元年)/MM/DD';
        ML_JS_CHECK["該月僅有"] = '該月僅有';
        ML_JS_CHECK["輸入YYYY(西元年)/MM/DD"] = '輸入YYYY(西元年)/MM/DD';
        ML_JS_CHECK["日期需輸入西元年格式"] = '日期需輸入西元年格式';
        ML_JS_CHECK["西元年範圍需大於 1911 年"] = '西元年範圍需大於 1911 年';
        ML_JS_CHECK["注意！ *  欄位不能為空"] = '注意！ *  欄位不能為空';
        ML_JS_CHECK["注意！欄位是數字型態"] = '注意！欄位是數字型態';
        ML_JS_CHECK["注意！日期有誤，終止日期不能小於起始日期"] = '注意！日期有誤，終止日期不能小於起始日期';
        ML_JS_CHECK["注意！此欄位數字太大"] = '注意！此欄位數字太大';
        ML_JS_CHECK["注意！此欄位數字太小"] = '注意！此欄位數字太小';

        $(document).ready(function () {
            QueryData();
            $('[name*="Birthday"]').datepick();
            $('[name*="PsnETime"]').datepick();
            $('[name*="PsnSTime"]').datepick();
            funTabEnter();
        });
        function QueryData() {
            $.post("ResultTable.aspx", { "action": "q", "InputWord": $("#InputWord").val() },
               function (data) {
                   $("#TableDetail").html(data);
               }
           );
        }

        function EditData(id) {
            $.ajax({
                type: "POST",
                url: "Person.ashx",
                dataType: 'json',
                data: { 'action': "q", 'id': id },
                success: function (data) {
                    $("[name*='PsnNo']").val(data.psn_no);
                    $("[name*='PsnID']").val(data.psn_id);
                    $("[name*='PsnIDNum']").val(data.id_num);
                    $("[name*='PsnName']").val(data.psn_name);
                },
                fail: function () {
                    alert("failed");
                }
            });
        }

        function AddNew() {
            $("#TableViewer").find("input").val("");
            $("[name*='Birthday']").val("1940/01/01");
            $("[name*='Company']").val(0);
            $("[name*='Department']").val(0);
            $("[name*='Unit']").val(0);
            $("[name*='Title']").val(0);
        }

        function EditOneData(id) {
            $.ajax({
                type: "POST",
                url: "Person.ashx",
                dataType: 'json',
                data: { 'action': "q", 'id': id },
                success: function (data) {
                    console.log(data);
                    $("[name*='PsnNo']").val(data.psn_no);
                    $("[name*='PsnID']").val(data.psn_id);
                    $("[name*='PsnIDNum']").val(data.id_num);
                    $("[name*='PsnName']").val(data.psn_name);
                    $("[name*='PsnEName']").val(data.psn_e_name);
                    $("[name*='PsnETime']").val(data.psn_e_time);
                    $("[name*='PsnSTime']").val(data.psn_s_time);
                    $("[name*='Birthday']").val(data.birthday);
                    $("[name*='Company']").val(data.org_c);
                    $("[name*='Department']").val(data.org_d);
                    $("[name*='Unit']").val(data.org_u);
                    $("[name*='Title']").val(data.org_t);
                    $("[name*='PsnAccount']").val(data.psn_account);
                    $("[name*='PsnPW']").val(data.psn_pw);
                    $("[name*='Remark']").val(data.remark);
                },
                fail: function () {
                    alert("failed");
                }
            });
        }

        function SaveData() {
            console.log("save...");
            if (JsFunBASE_VERTIFY()) {
                console.log('check in..');
            }
        }
        

    </script>

    <style type="text/css">
        .GVStyle > tbody > tr:nth-of-type(odd) {
            background-color: #f0f0f0;
        }
     
      
        .draggable
        {
                border: 1px solid #4EA000;             
            color: #4EA000;            
            position:absolute;left:150px;width:800px;top:250px;background-color:#E5FFCD;
        }

        .datepicker{z-index:1151 !important;}
        .datepick-popup
        {
            z-index:1151 !important;
        }

    </style>

    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <span><%=GetLocalResourceObject("lblKeyWord") %></span>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <input type="text" id="InputWord" name="InputWord" />
            </td>
            <td>
                <asp:Button ID="QueryButton" runat="server" Text="<%$ Resources:Resource, btnQuery %>" CssClass="IconSearch"
                    OnClientClick="QueryData();return false" />
                <asp:Button ID="btnPsnAdd" runat="server" Text="<%$ Resources:btnPsnAdd %>" CssClass="IconNew"
                    OnClientClick="AddNew();return false" />
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td id="TableDetail" style="vertical-align: top; width: 600px"></td>
            <td id="TableViewer" style="vertical-align: top">
                <fieldset id="Pending_List" runat="server">
                    <legend id="Pending_Legend" runat="server">人員基本資料</legend>
                    <table class="Item">
                        <tr>
                            <td style="vertical-align: top; width: 250px;">
                                <div>
                                    <asp:Label ID="lb_PsnNo" runat="server" Text="<%$ Resources:lblPsnNo %>"></asp:Label>
                                </div>
                                <div>                                    
                                    <input type="text" name="PsnNo" id="PsnNo" style="width:180px" field_name="<%=GetLocalResourceObject("lblPsnNo") %>" 
                                        must_keyin_yn="Y" />
                                </div>
                                <div>
                                    <asp:Label ID="lb_PsnName" runat="server" Text="<%$ Resources:lblPsnName %>"></asp:Label>
                                </div>
                                <div>                                    
                                    <input type="text" name="PsnName" id="PsnName" style="width:180px" field_name="<%=GetLocalResourceObject("lblPsnName") %>" 
                                        must_keyin_yn="Y" />
                                </div>
                                <div>
                                    <asp:Label ID="lb_PsnEName" runat="server" Text="<%$ Resources:lblPsnEName %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnEName" runat="server" Width="180px" must_keyin_yn="Y"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnClass" runat="server" Text="<%$ Resources:lblPsnClass %>"></asp:Label>
                                </div>
                                <div>
                                    <select width="80px" name="PsnType" id="PsnType">
                                        <option value="E">員工</option>
                                        <option value="V">貴賓</option>
                                        <option value="F">廠商</option>
                                    </select>
                                </div>
                                <div>
                                    <asp:Label ID="lblIDNum" runat="server" Text="<%$ Resources:lblIDNum %>"></asp:Label>
                                </div>
                                <div>                                    
                                    <input type="text" id="PsnIDNum" name="PsnIDNum" style="width:180px" must_keyin_yn="Y" maxlength="10" />
                                </div>
                                <div>
                                    <asp:Label ID="lblBirthday" runat="server" Text="<%$ Resources:lblBirthday %>"></asp:Label>
                                </div>
                                <div>
                                    <input type="text" name="Birthday" id="Birthday" value="1940/01/01" />
                                </div>
                                <div>
                                    公司：
                                </div>
                                <div>
                                    <select name="Company">
                                        <option value="0">請選擇</option>
                                        <%foreach(DataRow r in this.DataOrgCompany.Rows){ %>
                                            <option value="<%=r["OrgId"].ToString() %>"><%=String.Format("{0}.{1}",r["OrgName"],r["OrgNo"]) %></option>
                                        <%} %>
                                    </select>
                                </div>
                                <div>
                                    單位：
                                </div>
                                <div>
                                    <select name="Unit">
                                        <option value="0">請選擇</option>
                                        <%foreach(DataRow r in this.DataOrgUnit.Rows){ %>
                                            <option value="<%=r["OrgId"].ToString() %>"><%=String.Format("{0}.{1}",r["OrgName"],r["OrgNo"]) %></option>
                                        <%} %>
                                    </select>
                                </div>
                                <div>
                                    部門：
                                </div>
                                <div>
                                    <select name="Department">
                                        <option value="0">請選擇</option>
                                        <%foreach(DataRow r in this.DataOrgDept.Rows){ %>
                                            <option value="<%=r["OrgId"].ToString() %>"><%=String.Format("{0}.{1}",r["OrgName"],r["OrgNo"]) %></option>
                                        <%} %>
                                    </select>
                                </div>
                                <div>
                                    職稱：
                                </div>
                                <div>
                                    <select name="Title">
                                        <option value="0">請選擇</option>
                                        <%foreach(DataRow r in this.DataOrgTitle.Rows){ %>
                                            <option value="<%=r["OrgId"].ToString() %>"><%=String.Format("{0}.{1}",r["OrgName"],r["OrgNo"]) %></option>
                                        <%} %>
                                    </select>
                                </div>
                            </td>
                            <td style="vertical-align: top; width: 250px;">
                                <div>
                                    <asp:Label ID="lblCardNo" runat="server" Text="<%$ Resources:lblCardNo %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_CardNo" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnAccount" runat="server" Text="<%$ Resources:lblPsnAccount %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnAccount" runat="server" Width="180px"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnPW" runat="server" Text="<%$ Resources:lblPsnPW %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_PsnPW" runat="server" Width="180px" TextMode="Password"></asp:TextBox>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnAuthAllow" runat="server" Text="<%$ Resources:lblPsnAuthAllow %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:DropDownList ID="Input_PsnAuthAllow" runat="server" Width="80px">
                                        <asp:ListItem Value="0">無效</asp:ListItem>
                                        <asp:ListItem Value="1" Selected="True">有效</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnSTime" runat="server" Text="<%$ Resources:lblPsnSTime %>"></asp:Label>
                                </div>
                                <div>
                                    <input type="text" name="PsnSTime" id="PsnSTime" value="<%=string.Format("{0:yyyy-MM-dd}",DateTime.Now) %>" />
                                </div>
                                <div>
                                    <asp:Label ID="lblPsnETime" runat="server" Text="<%$ Resources:lblPsnETime %>"></asp:Label>
                                </div>
                                <div>
                                    <input type="text" name="PsnETime" id="PsnETime" value="" />
                                </div>
                                <div>
                                    <asp:Label ID="lblRemark" runat="server" Text="<%$ Resources:lblRemark %>"></asp:Label>
                                </div>
                                <div>
                                    <asp:TextBox ID="Input_Remark" runat="server" Height="52px" Width="190px" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align: center;" colspan="2">
                                <hr />
                                <input type="hidden" value="" id="PsnID" name="PsnID" />
                                <input type="hidden" value="<%=string.Format("{0:yyyy/MM/dd}",DateTime.Now) %>" id="NowDate" name="NowDate" />
                                <asp:Button ID="btSave" runat="server" Text="<%$ Resources:Resource, btnSave %>" Enabled="true"
                                     CssClass="IconSave" OnClientClick="SaveData();return false" />
                                <asp:Button ID="btDelete" runat="server" Text="<%$ Resources:Resource, btnDelete %>" Enabled="False" CssClass="IconDelete" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
    <div class="Footer">
        <p>
            <span><%=Resources.Resource.lblFooter %>"></span>
        </p>
    </div>
</asp:Content>
