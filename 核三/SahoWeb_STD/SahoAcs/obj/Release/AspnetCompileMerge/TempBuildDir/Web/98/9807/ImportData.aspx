<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="ImportData.aspx.cs" Inherits="SahoAcs.ImportData" Debug="true" EnableEventValidation="false" Theme="UI" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--  cellspacing="0"   border="1" style="border-collapse:collapse;" 為GridView呈現<table>的預設屬性，因為Code-Behind目前找不到抓取方法，所以寫死--%>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>

    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hideErrorData" runat="server" EnableViewState="False" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <fieldset id="ReaderPara_List" runat="server" style="width: 880px" visible="false">
                    <legend id="ReaderPara_Legend" runat="server">1.設備資料匯入</legend>
                    <table cellspacing="5">
                        <tr>
                            <td style="text-align:right">連線基本資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_Dci" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_Dci" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_Dci_Click" ImageUrl="/Img/close_button.png" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_Dci" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_Dci_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="DciState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_DciState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right">連線裝置資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_Mst" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_Mst" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_Mst_Click" CssClass="IconImport" /></td>
                            <td>
                                <asp:Button ID="CleanTable_Mst" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_Mst_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="MstState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_MstState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right">控制器資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_Ctrl" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_Ctrl" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_Ctrl_Click" CssClass="IconImport" /></td>
                            <td>
                                <asp:Button ID="CleanTable_Ctrl" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_Ctrl_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="CtrlState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_CtrlState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right">讀卡機資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_Reader" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_Reader" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_Reader_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_Reader" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_Reader_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="ReaderState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_ReaderState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align: right" colspan="5">
                                <asp:Button ID="Equ_ImportButton" runat="server" Text="確定匯入" OnClick="Equ_ImportButton_Click" CssClass="IconImport" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset id="EquGrop_List" runat="server" style="width: 880px" visible="false">
                    <legend id="EquGrop_Legend" runat="server">2.設備群組匯入</legend>
                    <table>
                        <tr>
                            <td style="text-align: right">設備群組資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_EquGroup" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_EquGroup" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_EquGroup_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_EquGroup" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_EquGroup_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="EquGroupState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_EquGroupState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align: right" colspan="5">
                                <asp:Button ID="EquGroup_ImportButton" runat="server" Text="確定匯入" OnClick="EquGroup_ImportButton_Click" CssClass="IconImport" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset id="Org_List" runat="server" style="width: 880px">
                    <legend id="Org_Legend" runat="server">3.組織資料匯入</legend>
                    <table>
                        <tr>
                            <td style="text-align: right">組織單位資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_Org" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_Org" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_Org_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_Org" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_Org_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="OrgState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_OrgState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>                        
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align: right" colspan="5">
                                <asp:Button ID="Org_ImportButton" runat="server" Text="確定匯入" OnClick="Org_ImportButton_Click" CssClass="IconImport" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset id="OrgStruc_List" runat="server" style="width:880px">
                    <legend id="OrgStruc_Legend" runat="server">4.組織架構匯入</legend>
                    <table>
                        <tr>
                            <td style="text-align: left">組織架構資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_OrgStruc" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_OrgStruc" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_OrgStruc_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_OrgStruc" runat="server" Text="清空資料表" Font-Size="Small" OnClick="CleanTable_OrgStruc_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="OrgStrucState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_OrgStrucState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align: right" colspan="5">
                                <asp:Button ID="OrgStruc_ImportButton" runat="server" Text="確定匯入" OnClick="OrgStruc_ImportButton_Click" CssClass="IconImport" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset id="PsnCard_List" runat="server" style="width: 880px">
                    <legend id="PsnCard_Legend" runat="server">5.人員卡片資料匯入</legend>
                    <table>
                        <tr>
                            <td style="text-align: right">人員卡片資料匯入：</td>
                            <td>
                                <asp:FileUpload ID="FileUpload_PsnCard" runat="server" Width="300px" />
                            </td>
                            <td>
                                <asp:Button ID="UpLoadButton_PsnCard" runat="server" Font-Size="Small" Text="匯入測試資料表" OnClick="UpLoadButton_PsnCard_Click" CssClass="IconImport" />
                            </td>
                            <td>
                                <asp:Button ID="CleanTable_PsnCard" runat="server" Text="清空資料表" Font-Size="Small" OnClientClick="Block()" OnClick="CleanTable_PsnCard_Click" CssClass="IconDelete" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="PsnCardState_UpdatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="Label_PsnCardState" runat="server" Text=""></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                    <table class="TableWidth">
                        <tr>
                            <td style="text-align: right" colspan="5">
                                <asp:Button ID="PsnCard_ImportButton" runat="server" Text="確定匯入" OnClientClick="Block()" OnClick="PsnCard_ImportButton_Click" CssClass="IconImport" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <th>
                <hr />
                <asp:Label ID="Label_ShowMsg" runat="server" Text="訊息查看：" Font-Bold="true" CssClass=""></asp:Label>
            </th>
        </tr>
        <tr>
            <td style="text-align: left; vertical-align: middle">
                <asp:UpdatePanel ID="EquMsg_UpdatePanel" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="TextBox_EquMsg" runat="server" Width="100%" Height="120px" TextMode="MultiLine"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>


