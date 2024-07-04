<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" Theme="UI"
    CodeBehind="QueryCtrlEquRealtime.aspx.cs" Inherits="SahoAcs.Web._04._0404.QueryCtrlEquRealtime" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1">
            <tbody>
                <tr class="GVStyle">
                    <%--<th scope="col" style="width: 40px;">選取</th>--%>
                    <th scope="col" style="width: 50px;">識別碼</th>
                    <th scope="col" style="width: 150px;">設備編號</th>
                    <th scope="col" style="width: 150px;">設備名稱</th>
                    <th scope="col" style="width: 150px;">設備英文名稱</th>
                    <th scope="col" style="width: 40px;">兼作卡鐘</th>
                    <th scope="col" style="width: 90px;">設備型號</th>
                    <th scope="col" style="width: 90px;">設備類型</th>
                    <th scope="col" style="width: 150px;">建築物名稱</th>
                    <th scope="col" style="width: 40px;">
                        <span id="ContentPlaceHolder1_MainGridView_HeaderSubmitState" style="font-weight: bold;">傳送狀態</span>
                    </th>
                    <th scope="col">
                        <span id="ContentPlaceHolder1_MainGridView_HeaderResponseState" style="font-weight: bold;">設備連線狀態</span>
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="12">
                        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1650px;overflow-y: scroll;">
                            <div v-show="items.count>0">
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" 
                                    style="width: 100%; border-collapse: collapse;">
                                    <tbody id="list_each">                                        
                                        <tr id="GVRow_57" onmouseover="onMouseMoveIn(0, this, '', '');" v-for="item in items" onclick="CheckOpen(this)"
                                            onmouseout="onMouseMoveOut(0, this);" style="color: rgb(0, 0, 0);">  
                                            <td title="47" align="center" style="width: 44px;"><input type="checkbox" id="ChkGetTime" name="ChkGetTime" value="{{item.EquID}}" class="ChkClass" checked="checked"/></td>                                          
                                            <td title="57" align="center" style="width: 54px;">{{item.EquID}}</td>
                                            <td title="{{item.EquNo}}" style="width: 154px;">{{item.EquNo}}</td>
                                            <td title="{{item.EquName}}" style="width: 154px;">{{item.EquName}}</td>
                                            <td title="{{item.EquEName}}" style="width: 154px;">{{item.EquEName}}</td>
                                            <td title="N" align="center" style="width: 44px;" v-show="item.IsAndTrt==0">N</td>
                                            <td title="Y" align="center" style="width: 44px;" v-show="item.IsAndTrt==1">Y</td>
                                            <td title="{{item.EquModel}}" align="center" style="width: 94px;">{{item.EquModel}}</td>
                                            <td title="{{item.EquClass}}" align="center" style="width: 94px;">{{item.EquClass}}</td>                                            
                                            <td title="{{item.Building}}" style="width: 154px;">{{item.Building}}</td>
                                            <td align="center" style="width: 44px;">
                                                <span id="RowSubmitState" style="font-weight: bold;">{{item.EquDesc}}</span>
                                            </td>
                                            <td>
                                                <span id="RowResponseState" style="font-weight: bold;">{{item.EquResult}}</span>
                                            </td>
                                        </tr>                                                                        
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <table cellspacing="5" style="padding-top: 10px">
        <tr>
            <td>
                <%--<asp:Button ID="SubmitButton" runat="server" Text="<%$Resources:ResourceCtrls,btnSendCmd %>" OnClick="SubmitButton_Click" CssClass="IconTransit" />--%>
                <input type="button" id="btnSend" value="傳送" class="IconTransit" onclick="SendData()" />                
            </td>
            <td>
                <div id="Waiting" style="color:white;background-color:red;display:none">查詢中...</div>
            </td>
        </tr>
    </table>
</asp:Content>
