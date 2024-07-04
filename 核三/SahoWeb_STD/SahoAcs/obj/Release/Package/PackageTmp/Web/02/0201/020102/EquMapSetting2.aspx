<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapSetting2.aspx.cs" Inherits="SahoAcs.EquMapSetting2" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <link href="../../../../Css/colorbox.css" rel="stylesheet" />
    <script src="../../../../Scripts/jquery.colorbox-min.js"></script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" />
    </div>
    <div runat="server" id="RightClickmenu" class="RightClickmenuskin" onmouseover="highlightMenu()" onmouseout="lowlightMenu()">
        <div id="new_childItem" class="menuitems" onclick="clickNewMenu();">
        </div>
        <div id="del_childItem" class="menuitems" onclick="clickdelMenu();">
        </div>
    </div>
    <table>
        <tr>
            <td style="vertical-align: top; width: 400px">
                <asp:UpdatePanel ID="TreeView_UpdatePanel" runat="server" UpdateMode="Conditional" style="height: 570px">
                    <ContentTemplate>
                        <asp:Panel ID="TreeView_Panel" runat="server" ScrollBars="Vertical" Height="100%" class="TableS1">
                            <asp:TreeView ID="EquOrg_TreeView" runat="server" Width="100%"></asp:TreeView>
                            <asp:HiddenField ID="txt_NodeTypeList" runat="server" />
                            <asp:HiddenField ID="txt_NodeIDList" runat="server" />
                            <asp:HiddenField ID="NodeAct" runat="server" />
                            <asp:HiddenField ID="RightClickItem" runat="server" />
                            <asp:HiddenField ID="ClickItem" runat="server" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="vertical-align: top">
                <table style="margin-left: 15px; width: 98%">
                    <%--連線裝置介面--%>
                    <tr>
                        <td id="MasterUI_Title" style="text-align: center; width: 100%" class="title01">
                            <asp:Label ID="Master_Label_Title" runat="server" Text="連線裝置介面" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                            <asp:HiddenField ID="DciIDValue" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px 0 15px 0; text-align: center" class="Bg01">
                            <div runat="server" id="MasterUI">
                                <table class="TableWidth">
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_No" runat="server" Text="連線裝置編號"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_Desc" runat="server" Text="連線裝置說明"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Master_Input_No" runat="server" CssClass="TextBoxRequired" Width="120px"></asp:TextBox>
                                                    </th>
                                                    <th>
                                                        <asp:TextBox ID="Master_Input_Desc" runat="server" Width="220px"></asp:TextBox>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_Dci" runat="server" Text="使用連線"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_Type" runat="server" Text="連線類型"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:Label ID="Master_Input_Dci" runat="server" Width="150px"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <asp:RadioButton ID="Master_Input_Type_TCPIP" runat="server" Text="TCPIP" Value="T" GroupName="MasterType" />
                                                        <asp:RadioButton ID="Master_Input_Type_COMPort" runat="server" Text="COM Port" Value="C" GroupName="MasterType" />
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div runat="server" id="IPParam">
                                                <table style="width: 400px" class="Item">
                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_IPParam_IP" runat="server" Text="IP "></asp:Label>
                                                        </th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_IPParam_IP" runat="server" CssClass="TextBoxRequired"></asp:TextBox>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_IPParam_Port" runat="server" Text="Port "></asp:Label>
                                                        </th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_IPParam_Port" runat="server" CssClass="TextBoxRequired" Width="60px"></asp:TextBox>
                                                        </th>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div runat="server" id="ComPortParam">
                                                <table class="Item">
                                                    <tr>
                                                        <th style="text-align: right">
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_ComPortParam_ComPort" runat="server" Text="ComPort"></asp:Label>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th>
                                                            <asp:DropDownList ID="Master_Input_ComPortParam_ComPort" runat="server" BackColor="#FFE5E5">
                                                                <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="COM1" Value="COM1"></asp:ListItem>
                                                                <asp:ListItem Text="COM2" Value="COM2"></asp:ListItem>
                                                                <asp:ListItem Text="COM3" Value="COM3"></asp:ListItem>
                                                                <asp:ListItem Text="COM4" Value="COM4"></asp:ListItem>
                                                                <asp:ListItem Text="COM5" Value="COM5"></asp:ListItem>
                                                                <asp:ListItem Text="COM6" Value="COM6"></asp:ListItem>
                                                                <asp:ListItem Text="COM7" Value="COM7"></asp:ListItem>
                                                                <asp:ListItem Text="COM8" Value="COM8"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th style="padding-left: 20px; text-align: right">
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_ComPortParam_BaudRate" runat="server" Text="BaudRate"></asp:Label>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th>
                                                            <asp:DropDownList ID="Master_Input_ComPortParam_BaudRate" runat="server" BackColor="#FFE5E5">
                                                                <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="2400" Value="2400"></asp:ListItem>
                                                                <asp:ListItem Text="4800" Value="4800"></asp:ListItem>
                                                                <asp:ListItem Text="9600" Value="9600"></asp:ListItem>
                                                                <asp:ListItem Text="19200" Value="19200"></asp:ListItem>
                                                                <asp:ListItem Text="38400" Value="38400"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th style="text-align: right">
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_ComPortParam_Parity" runat="server" Text="Parity"></asp:Label>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th>
                                                            <asp:DropDownList ID="Master_Input_ComPortParam_Parity" runat="server" BackColor="#FFE5E5">
                                                                <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="None" Value="N"></asp:ListItem>
                                                                <asp:ListItem Text="Odd" Value="O"></asp:ListItem>
                                                                <asp:ListItem Text="Even" Value="E"></asp:ListItem>
                                                                <asp:ListItem Text="Mark" Value="M"></asp:ListItem>
                                                                <asp:ListItem Text="Space" Value="S"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th style="padding-left: 20px; text-align: right">
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_ComPortParam_DataBits" runat="server" Text="DataBits"></asp:Label>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th>
                                                            <asp:DropDownList ID="Master_Input_ComPortParam_DataBits" runat="server" BackColor="#FFE5E5">
                                                                <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th style="text-align: right">
                                                            <span class="Arrow01"></span>
                                                            <asp:Label ID="Master_Label_ComPortParam_StopBits" runat="server" Text="StopBits"></asp:Label>
                                                        </th>
                                                        <th style="width: 8px"></th>
                                                        <th colspan="5">
                                                            <asp:DropDownList ID="Master_Input_ComPortParam_StopBits" runat="server" BackColor="#FFE5E5">
                                                                <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="One" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="Two" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="OnePointFive" Value="1.5"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </th>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_Model" runat="server" Text="連線裝置機型"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_Status" runat="server" Text="狀態"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_CtrlModel" runat="server" Text="控制器機型"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Master_Input_Model" runat="server" Width="120px"></asp:TextBox>
                                                    </th>
                                                    <th>
                                                        <asp:DropDownList ID="Master_Input_Status" runat="server" Width="100px" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                            <asp:ListItem Text="啟用" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="停用" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="被限制" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </th>
                                                    <th>
                                                        <asp:DropDownList ID="Master_Input_CtrlModel" runat="server" BackColor="#FFE5E5" Width="115px"></asp:DropDownList>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_LinkMode" runat="server" Text="連線模式"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_AutoRerun" runat="server" Text="自動回傳"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:RadioButton ID="Master_Input_LinkMode_OneShot" runat="server" Text="One Shot" value="0" GroupName="MasterLinkMode" Width="100px" />
                                                        <asp:RadioButton ID="Master_Input_LinkMode_Always" runat="server" Text="Always" value="1" GroupName="MasterLinkMode" Width="80px" />
                                                    </th>
                                                    <th>
                                                        <asp:RadioButton ID="Master_Input_AutoRerun_Y" runat="server" Text="是" value="1" GroupName="MasterAutoRerun" Width="60px" />
                                                        <asp:RadioButton ID="Master_Input_AutoRerun_N" runat="server" Text="否" value="0" GroupName="MasterAutoRerun" Width="60px" />
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_FwVer" runat="server" Text="韌體版本"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:Label ID="Master_Input_FwVer" runat="server"></asp:Label>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 10px">
                                            <asp:Button ID="Master_B_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Master_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Master_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                            <asp:Button ID="Master_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <%--控制器介面--%>
                    <tr>
                        <%--<td id="ControllerUI_Title" style="text-align:center" style="background: #3B5998; width: 500px">--%>
                        <td id="ControllerUI_Title" style="text-align: center" class="title01">
                            <asp:Label ID="Controller_Label_Title" runat="server" Text="控制器介面" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                            <asp:HiddenField ID="MstIDValue" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding: 10px 0 15px 0" class="Bg01">
                            <div runat="server" id="ControllerUI">
                                <table class="TableWidth">
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_No" runat="server" Text="控制器編號"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Name" runat="server" Text="控制器名稱"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Controller_Input_No" runat="server" CssClass="TextBoxRequired" Width="120px"></asp:TextBox>
                                                    </th>
                                                    <th>
                                                        <asp:TextBox ID="Controller_Input_Name" runat="server" Width="220px"></asp:TextBox>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Desc" runat="server" Text="控制器說明"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Controller_Input_Desc" TextMode="MultiLine" Rows="3" runat="server" Width="350px"></asp:TextBox>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Mst" runat="server" Text="連線裝罝"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Model" runat="server" Text="機型"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Status" runat="server" Text="控制器狀態"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:Label ID="Controller_Input_Mst" runat="server" Text="" Width="150px"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <asp:Label ID="Controller_Input_Model" runat="server" Text="" Width="80px"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <asp:DropDownList ID="Controller_Input_Status" runat="server" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                            <asp:ListItem Text="啟用" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="停用" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="被限制" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_Addr" runat="server" Text="機號"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_FwVer" runat="server" Text="韌體版本"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Controller_Label_AddReader" runat="server" Text="新增讀卡機"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Controller_Input_Addr" runat="server" Width="50px" CssClass="TextBoxRequired"></asp:TextBox>
                                                    </th>
                                                    <th>
                                                        <asp:Label ID="Controller_Input_FwVer" runat="server" Width="180px"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <asp:DropDownList ID="Controller_Input_AddReader" runat="server" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="- 請選擇 -" Value=""></asp:ListItem>
                                                            <asp:ListItem Text="0台" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="1台" Value="1" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Text="2台" Value="2"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 10px">
                                            <asp:Button ID="Controller_B_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Controller_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Controller_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                            <asp:Button ID="Controller_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <%--讀卡機介面--%>
                    <tr>
                        <%--<td id="ReaderUI_Title" style="text-align:center" style="background: #3B5998">--%>
                        <td id="ReaderUI_Title" style="text-align: center" class="title01">
                            <asp:Label ID="Reader_Label_Title" runat="server" Text="讀卡機介面" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                            <asp:HiddenField ID="CtrlIDValue" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px 0 15px 0; text-align: center" class="Bg01">
                            <div runat="server" id="ReaderUI">
                                <table class="TableWidth">
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_No" runat="server" Text="讀卡機編號"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_Name" runat="server" Text="讀卡機名稱"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Reader_Input_No" runat="server" Width="80px" CssClass="TextBoxRequired"></asp:TextBox>
                                                    </th>
                                                    <th>
                                                        <asp:TextBox ID="Reader_Input_Name" runat="server" Width="230px"></asp:TextBox>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_Desc" runat="server" Text="讀卡機說明"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:TextBox ID="Reader_Input_Desc" TextMode="MultiLine" Rows="3" runat="server" Width="320px"></asp:TextBox>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_EquNo" runat="server" Text="<%$Resources:Resource,MapEqu %>"></asp:Label>                                                        
                                                    </th>                                                    
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <asp:DropDownList ID="Reader_Input_EquNo" runat="server" Width="260px" BackColor="#FFE5E5"></asp:DropDownList>
                                                        <asp:Button ID="btnAddEquData" runat="server" Text="新增設備" EnableViewState="False" CssClass="IconNew" />
                                                        <asp:Button ID="btnFilter" runat="server" Text="篩選" EnableViewState="False" CssClass="IconSearch" />                                                        
                                                    </th>                                                   
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_Dir" runat="server" Text="方向控制"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_CtrlName" runat="server" Text="控制器名稱"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Reader_Label_CtrlModel" runat="server" Text="機型"></asp:Label>
                                                    </th>
                                                </tr>
                                                <tr>
                                                     <th>
                                                        <asp:DropDownList ID="Reader_Input_Dir" runat="server" Width="100px" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="進" Value="進"></asp:ListItem>
                                                            <asp:ListItem Text="出" Value="出"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </th>
                                                    <th>
                                                        <asp:Label ID="Reader_Input_CtrlName" runat="server" Width="200px"></asp:Label>
                                                    </th>
                                                    <th>
                                                        <asp:Label ID="Reader_Input_CtrlModel" runat="server" Width="120px"></asp:Label>
                                                    </th>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-top: 10px">
                                            <asp:Button ID="Reader_B_Add" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Reader_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                            <asp:Button ID="Reader_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                            <asp:Button ID="Reader_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="260px"
        EnableViewState="False" CssClass="PopBg">
        <asp:Panel ID="PanelDrag1" runat="server" SkinID="PanelDrag" EnableViewState="False">
            <table style="width: 100%; padding: 0px;">
                <tr>
                    <td>
                        <asp:Label ID="L_popName1" runat="server" Font-Bold="True" ForeColor="White"
                            EnableViewState="False" Style="vertical-align: middle"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:ImageButton ID="ImgCloseButton1" runat="server" Height="25px" ImageUrl="/Img/close_button.png"
                            EnableViewState="False" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <table class="popItem">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblKeyWord" runat="server" Text="關鍵字："></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtKeyWord" runat="server"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="搜尋" EnableViewState="False" CssClass="IconSearch" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>
    <%--<asp:TextBox ID="txt_NodeTypeList" runat="server"></asp:TextBox>
    <asp:TextBox ID="txt_NodeIDList" runat="server"></asp:TextBox>
    <asp:TextBox ID="NodeAct" runat="server"></asp:TextBox>
    <asp:TextBox ID="CreateSource" runat="server"></asp:TextBox>
    <asp:TextBox ID="ClickItem" runat="server"></asp:TextBox>--%>
    <div id="dvContent" style="position:absolute;left:20px;top:30px;z-index:1000000000" ></div>
</asp:Content>
