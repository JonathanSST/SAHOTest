<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapList.aspx.cs" Inherits="SahoAcs.Web._98._9825.EquMapList" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="MapDiv">

    </div>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <input value="加入新控制器" type="button" onclick="AddCtrl()" id="BtnAddCtrl" class="IconNew" />
                <input type="button" value="儲存所有控制器配置" onclick="SaveEquMap()" class="IconSave" id="BtnSave"/>
            </td>
        </tr>
    </table>
    <div id="popOverlay2" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
</div>
<div id="ParaExtDiv2" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
	<select id="EquList" name="EquList">
	</select>
	<input value="確認設定" type="button" onclick="SetEnter()" id="BtnEnter" class="IconOk" />
</div>
    <input type="hidden" name="PageEvent" value="Save" />
    <input type="hidden" name="PageMapSrc" id="PageMapSrc" value="<%=this.MapSrc %>" />
</asp:Content>
