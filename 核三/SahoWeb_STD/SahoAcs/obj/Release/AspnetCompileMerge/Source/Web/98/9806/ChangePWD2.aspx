<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="ChangePWD2.aspx.cs" Inherits="SahoAcs.ChangePWD2" Debug="true" EnableEventValidation="false" Theme="UI" %>




<asp:Content ID="ContentHeader" ContentPlaceHolderID="head" runat="server">
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
    <script src="../../../scripts/JsTabEnter.js" type="text/javascript"></script>
    <script src="../../../scripts/JsQueryWindow.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="ValueKeep">        
        <input type="hidden" id="hideUserID" name="hideUserID" value="<%=Sa.Web.Fun.GetSessionStr(this,"UserID") %>" />
        <input type="hidden" id="hidePsnID" name="hidePsnID" value="<%=Sa.Web.Fun.GetSessionStr(this,"PsnID") %>" />        
        <input type="hidden" id="msgSuccess" value="異動完成" />
        <input type="hidden" id="msgRegRule" value="密碼原則：英文大寫、英文小寫、特殊符號、數字，四條件取三種，長度至少12碼" />
        <input type="hidden" id="msgConfirm" value="密碼輸入二次前後不一致" />
    </div>
    <table class="Item">
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_OldPWD" runat="server" Text="舊密碼"></asp:Label>
            </th>
            <td>                
                <input type="password" id="OldPwd" name="OldPwd" MUST_KEYIN_YN="Y" field_name="舊密碼" class="TextBoxRequired"  />
            </td>
        </tr>
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_NewPWD" runat="server" Text="新密碼"></asp:Label>
            </th>
            <td>                
                <input type="password" id="NewPwd" name="NewPwd" MUST_KEYIN_YN="Y" field_name="新密碼" class="TextBoxRequired" />
            </td>
        </tr>
        <tr>
            <th style="text-align:left">
                <span class="Arrow01"></span>
                <asp:Label ID="Label_CheckPWD" runat="server" Text="確認新密碼"></asp:Label>
            </th>
            <td>
                <input type="password" id="CheckPwd" name="CheckPwd" MUST_KEYIN_YN="Y" field_name="確認新密碼" class="TextBoxRequired" />
            </td>
        </tr>
        <tr>
            <th colspan="2">
                <asp:Label ID="Label_Remind" runat="server" Text="密碼原則：必須為數字+大小寫字母組合，且12碼以上"></asp:Label>
            </th>
        </tr>
    </table>    
    <input type="button" id="BtnSave" value="儲    存" class="IconSave" onclick="SaveExcute()" />
</asp:Content>
