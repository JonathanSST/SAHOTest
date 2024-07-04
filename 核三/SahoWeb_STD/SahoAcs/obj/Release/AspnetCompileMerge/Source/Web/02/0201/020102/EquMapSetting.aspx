<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapSetting.aspx.cs" Inherits="SahoAcs.EquMapSetting" Theme="UI" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="/Css/colorbox.css" rel="stylesheet" />
    <link href="/Css/jquery-ui.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery.colorbox-min.js"></script>
    <script src="/Scripts/jquery.blockUI.js"></script>

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

    <div id="dialog" title=""></div>
    <div id="dialog1" title=""></div>

    <table>
        <tr>
            <td style="vertical-align: top; width: 400px">
                <asp:UpdatePanel ID="TreeView_UpdatePanel" runat="server" UpdateMode="Conditional" style="height: 430px">
                    <ContentTemplate>
                        <asp:Panel ID="TreeView_Panel" runat="server" ScrollBars="Vertical" Height="100%" class="TableS1">
                            <asp:TreeView ID="EquOrg_TreeView" runat="server" Width="100%"></asp:TreeView>
                            <asp:HiddenField ID="txt_NodeTypeList" runat="server" />
                            <asp:HiddenField ID="txt_NodeIDList" runat="server" />
                            <asp:HiddenField ID="NodeAct" runat="server" />
                            <asp:HiddenField ID="RightClickItem" runat="server" />
                            <asp:HiddenField ID="ClickItem" runat="server" />
                            <asp:HiddenField ID="tmpEquClass" runat="server" />
                            <asp:HiddenField ID="DciIDValue" runat="server" />
                            <asp:HiddenField ID="MstIDValue" runat="server" />
                            <asp:HiddenField ID="hfEquID" runat="server" />
                            <asp:HiddenField ID="CtrlIDValue" runat="server" />
                            <asp:HiddenField ID="hfReaderID" runat="server" />
                            <asp:HiddenField ID="event_srcElement_id" runat="server" />
                            <asp:HiddenField ID="TreeView_Panel_scrollTop" runat="server" />
                            <asp:HiddenField ID="ControlCount" runat="server" />
                            <asp:HiddenField ID="hfDciID" runat="server" />
                            <asp:HiddenField ID="sLocArea" runat="server" />
                            <asp:HiddenField ID="sLocBuilding" runat="server" />
                            <asp:HiddenField ID="sLocFloor" runat="server" />
                            <asp:HiddenField ID="tmpLocArea" runat="server" />
                            <asp:HiddenField ID="tmpLocBuilding" runat="server" />
                            <asp:HiddenField ID="txtAreaList" runat="server" />
                            <asp:HiddenField ID="txtBuildingList" runat="server" />
                            <asp:HiddenField ID="txtFloorList" runat="server" />
                            <input type="hidden" value="<%=MaxCtrls %>" id="MaxCtrls" name="MaxCtrls" />
                            <input type="hidden" value="<%=CurrentCtrls %>" id="CurrentCtrls" name="CurrentCtrls" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>

            <td style="vertical-align: top">
                <div runat="server" id="Div_Dci">
                    <table style="margin-left: 15px; width: 98%">
                        <%--連線介面--%>
                        <tr>
                            <td id="DciUI_Title" style="text-align: center; width: 100%" class="title01">
                                <div runat="server" id="Div_DciUI_Title">
                                    <asp:Label ID="Dci_Label_Title" runat="server" Text="<%$ Resources: DciSet %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 10px 0 15px 0; text-align: center" class="Bg01">
                                <div runat="server" id="DciUI">
                                    <table class="TableWidth">
                                        <tr>
                                            <td>
                                                <table class="Item">
                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_No" runat="server" Text="<%$Resources:ttConnNo %>" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_Name" runat="server" Text="<%$Resources:ttConnName %>" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_PassWD" runat="server" Text="<%$Resources:ttConnPW %>" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <th>
                                                            <asp:TextBox ID="Dci_No" runat="server" Width="95%" CssClass="TextBoxRequired" MaxLength="20" EnableViewState="False"></asp:TextBox>
                                                        </th>
                                                        <td></td>

                                                        <td></td>
                                                        <th>
                                                            <asp:TextBox ID="Dci_Name" runat="server" Width="95%" CssClass="TextBoxRequired" MaxLength="60" EnableViewState="False"></asp:TextBox>
                                                        </th>
                                                        <td></td>

                                                        <td></td>
                                                        <th>
                                                            <asp:TextBox ID="Dci_PassWD" runat="server" Width="95%" CssClass="TextBoxRequired" MaxLength="20" EnableViewState="False"></asp:TextBox>
                                                        </th>
                                                        <td></td>
                                                    </tr>

                                                    <tr>
                                                        <td style="height: 10px" colspan="9"></td>
                                                    </tr>

                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_Ip" runat="server" Text="<%$Resources:ttIP %>" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_Port" runat="server" Text="<%$Resources:ttPort %>" Font-Bold="True"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="popLabel_IsAssign" runat="server" Text="<%$Resources:ttLockedIP %>" Font-Bold="true"></asp:Label>
                                                        </th>
                                                        <th style="width: 10px"></th>

                                                    </tr>

                                                    <tr>
                                                        <td></td>
                                                        <th>
                                                            <asp:TextBox ID="Dci_Ip" runat="server" Width="95%" MaxLength="50" EnableViewState="False"></asp:TextBox>
                                                        </th>
                                                        <td></td>

                                                        <td></td>
                                                        <th>
                                                            <asp:TextBox ID="Dci_Port" runat="server" Width="95%" MaxLength="5" EnableTheming="False"></asp:TextBox>
                                                        </th>
                                                        <td></td>

                                                        <td></td>
                                                        <td>
                                                            <asp:DropDownList ID="Dci_IsAssign" runat="server" Width="100%" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$Resources:ddlEnabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:ddlDisabled %>" Value="0" Enabled="true"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                        </tr>

                                        <tr>
                                            <td style="text-align: center">
                                                <table style="width: 100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="Dci_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                            <asp:Button ID="Dci_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                            <asp:Button ID="Dci_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                            <asp:Button ID="Dci_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>

                <div runat="server" id="Div_Master">
                    <table style="margin-left: 15px; width: 98%">
                        <%--連線裝置介面--%>
                        <tr>
                            <td id="MasterUI_Title" style="text-align: center; width: 100%" class="title01">
                                <div runat="server" id="Div_MasterUI_Title">
                                    <asp:Label ID="Master_Label_Title" runat="server" Text="<%$Resources:Resource,DeviceConnSet %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
                                </div>
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
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_No" runat="server" Text="<%$Resources:Resource,DeviceNo %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_Desc" runat="server" Text="<%$Resources:Resource,DeviceInfo %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_Dci" runat="server" Text="<%$Resources:Resource,MasterConn %>"></asp:Label>
                                                        </th>

                                                        <%--
                                                <th></th>

                                                <th>
                                                    <span class="Arrow01"></span>
                                                </th>
                                                <th>
                                                    <asp:Label ID="Master_Label_Type" runat="server" Text="<%$Resources:Resource,ConnType %>"></asp:Label>
                                                </th>
                                                        --%>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_No" runat="server" CssClass="TextBoxRequired" Width="120px"></asp:TextBox>
                                                        </th>

                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_Desc" runat="server" Width="220px" MaxLength="20"></asp:TextBox>
                                                        </th>

                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:Label ID="Master_Input_Dci" runat="server" Width="100px"></asp:Label>
                                                        </th>

                                                        <!--
                                                <th></th>

                                                <th></th>
                                                <th>
                                                    <asp:RadioButton ID="Master_Input_Type_TCPIP" runat="server" Text="TCPIP" Value="T" GroupName="MasterType" />
                                                    <%--<asp:RadioButton ID="Master_Input_Type_COMPort" runat="server" Text="COM Port" Value="C" GroupName="MasterType" />--%>
                                                </th>
                                                -->
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <div runat="server" id="IPParam">
                                                    <table class="Item">
                                                        <tr>
                                                            <th>
                                                                <span class="Arrow01"></span>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Master_Label_IPParam_IP" runat="server" Text="IP"></asp:Label>
                                                            </th>

                                                            <th></th>

                                                            <th>
                                                                <span class="Arrow01"></span>
                                                            </th>
                                                            <th style="width: 100px">
                                                                <asp:Label ID="Master_Label_IPParam_Port" runat="server" Text="Port "></asp:Label>
                                                            </th>

                                                            <%--<th></th>

                                                    <th>
                                                        <span class="Arrow01"></span>
                                                    </th>
                                                    <th>
                                                        <asp:Label ID="Master_Label_LinkMode" runat="server" Text="<%$Resources:Resource,ConnMode %>"></asp:Label>
                                                    </th>--%>

                                                            <th></th>

                                                            <th>
                                                                <span class="Arrow01"></span>
                                                            </th>
                                                            <th>
                                                                <asp:Label ID="Master_Label_AutoRerun" runat="server" Text="<%$Resources:Resource,AutoReturn %>"></asp:Label>
                                                            </th>
                                                        </tr>

                                                        <tr>
                                                            <th></th>
                                                            <th>
                                                                <asp:TextBox ID="Master_Input_IPParam_IP" runat="server" Style="width: 120px" CssClass="TextBoxRequired"></asp:TextBox>
                                                            </th>

                                                            <th></th>

                                                            <th></th>
                                                            <th style="width: 100px; text-align: left">
                                                                <asp:TextBox ID="Master_Input_IPParam_Port" runat="server" CssClass="TextBoxRequired" Width="80%"></asp:TextBox>
                                                            </th>
                                                            <th></th>

                                                            <%--
                                                    <th></th>
                                                    <th>
                                                        
				                                        <asp:RadioButton ID="Master_Input_LinkMode_Always" runat="server" Text="Always" value="1" GroupName="MasterLinkMode" Width="80px" Checked="True" />
                                                    </th>

                                                    <th></th>
                                                            --%>

                                                            <th></th>
                                                            <th>
                                                                <asp:RadioButton ID="Master_Input_AutoRerun_Y" runat="server" Text="<%$ Resources:Resource, Yes %>" value="0" GroupName="MasterAutoRerun" Width="60px" />
                                                                <asp:RadioButton ID="Master_Input_AutoRerun_N" runat="server" Text="<%$ Resources:Resource, No %>" value="1" GroupName="MasterAutoRerun" Width="60px" Checked="True" />
                                                            </th>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <%--<tr>
                                    <td>
                                        <div runat="server" id="ComPortParam">
                                            <table class="Item">
                                                <tr>
                                                    <th>
                                                        <span class="Arrow01"></span>
                                                        <asp:Label ID="Master_Label_ComPortParam_ComPort" runat="server" Text="ComPort"></asp:Label>
                                                    </th>
                                                    <th></th>
                                                    <th>
                                                        <asp:DropDownList ID="Master_Input_ComPortParam_ComPort" runat="server" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="<%$ Resources:Resource, ddlSelectDefault %>" Value=""></asp:ListItem>
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
                                                            <asp:ListItem Text="<%$ Resources:Resource, ddlSelectDefault %>" Value=""></asp:ListItem>
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
                                                    <th></th>
                                                    <th>
                                                        <asp:DropDownList ID="Master_Input_ComPortParam_Parity" runat="server" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="<%$ Resources:Resource, ddlSelectDefault %>" Value=""></asp:ListItem>
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
                                                            <asp:ListItem Text="<%$ Resources:Resource, ddlSelectDefault %>" Value=""></asp:ListItem>
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
                                                    <th></th>
                                                    <td colspan="5">
                                                        <asp:DropDownList ID="Master_Input_ComPortParam_StopBits" runat="server" BackColor="#FFE5E5">
                                                            <asp:ListItem Text="<%$ Resources:Resource, ddlSelectDefault %>" Value=""></asp:ListItem>
                                                            <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                                            <asp:ListItem Text="One" Value="1"></asp:ListItem>
                                                            <asp:ListItem Text="Two" Value="2"></asp:ListItem>
                                                            <asp:ListItem Text="OnePointFive" Value="1.5"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>--%>
                                        <tr>
                                            <td>
                                                <table class="Item">
                                                    <tr>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>

                                                            <asp:Label ID="Master_Label_Model" runat="server" Text="<%$Resources:Resource,DeviceModel %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_Status" runat="server" Text="<%$Resources:Resource,Status %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_CtrlModel" runat="server" Text="<%$Resources:Resource,CtrlModel %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_FwVer" runat="server" Text="<%$Resources:Resource,FirmVer %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_Model" runat="server" Width="120px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 100px; text-align: left;">
                                                            <asp:DropDownList ID="Master_Input_Status" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Enabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Disabled %>" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Restrict %>" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th></th>
                                                        <td>
                                                            <asp:DropDownList ID="Master_Input_CtrlModel" runat="server" BackColor="#FFE5E5" Font-Bold="True"></asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Master_Input_FwVer" runat="server" Width="180px" MaxLength="20"></asp:TextBox>
                                                        </th>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="Master_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Master_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Master_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="Master_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>

                <div runat="server" id="Div_Controller">
                    <table style="margin-left: 15px; width: 98%">
                        <%--控制器介面--%>
                        <tr>
                            <%--<td id="ControllerUI_Title" style="text-align:center" style="background: #3B5998; width: 500px">--%>
                            <td id="ControllerUI_Title" style="text-align: center" class="title01">
                                <asp:Label ID="Controller_Label_Title" runat="server" Text="<%$Resources:Resource,CtrlSet %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
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
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_No" runat="server" Text="<%$Resources:Resource,CtrlNo %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Name" runat="server" Text="<%$Resources:Resource,CtrlName %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Desc" runat="server" Text="<%$Resources:Resource,CtrlInfo %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Controller_Input_No" runat="server" CssClass="TextBoxRequired" Width="100px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Controller_Input_Name" runat="server" Width="200px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Controller_Input_Desc" TextMode="MultiLine" Rows="2" runat="server" Width="300px"></asp:TextBox>
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
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Addr" runat="server" Text="<%$Resources:Resource,MacNo %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_EquClass" runat="server" Text="<%$ Resources:labEquClass %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Status" runat="server" Text="<%$Resources:Resource,CtrlStatus %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th id="th_Controller_AddReader_1">
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th id="th_Controller_AddReader_2">
                                                            <asp:Label ID="Controller_Label_AddReader" runat="server" Text="<%$Resources:Resource,AddReader %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th id="th_Controller_FwVer_1">
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th id="th_Controller_FwVer_2">
                                                            <asp:Label ID="Controller_Label_FwVer" runat="server" Text="<%$ Resources:Resource,FirmVer %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Mst" runat="server" Text="<%$Resources:Resource,DeviceIP %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_Label_Model" runat="server" Text="<%$Resources:Resource,CtrlModel %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Controller_Input_Addr" runat="server" Width="50px" CssClass="TextBoxRequired" MaxLength="4"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <td>
                                                            <asp:DropDownList ID="Controller_Input_EquClass" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th></th>
                                                        <td>
                                                            <asp:DropDownList ID="Controller_Input_Status" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Enabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Disabled %>" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Restrict %>" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th id="th_Controller_AddReader_3"></th>
                                                        <td id="th_Controller_AddReader_4">
                                                            <asp:DropDownList ID="Controller_Input_AddReader" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <%--<asp:ListItem Text="0 Set" Value="0"></asp:ListItem>--%>
                                                                <asp:ListItem Text="1 Set" Value="1" Selected="True"></asp:ListItem>
                                                                <asp:ListItem Text="2 Set" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th id="th_Controller_FwVer_3"></th>
                                                        <th id="th_Controller_FwVer_4">
                                                            <asp:Label ID="Controller_Input_FwVer" runat="server" Width="180px"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:Label ID="Controller_Input_Mst" runat="server" Text="" Width="110px"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:Label ID="Controller_Input_Model" runat="server" Text="" Width="100px"></asp:Label>
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
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_labArea" runat="server" Text="<%$Resources: labArea %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Controller_labBuilding" runat="server" Text="<%$Resources: labBuilding %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Controller_labFloor" runat="server" Text="<%$ Resources:Resource, Floor %>"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="ControllerDDLArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectCtrlArea()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 180px">
                                                            <asp:DropDownList ID="ControllerDDLBuilding" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectCtrlBuilding()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="ControllerDDLFloor" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="Controller_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Controller_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Controller_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="Controller_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                                <asp:Button ID="CtrlParaButton" runat="server" Text="<%$ Resources: CtrlParaSetting %>" EnableViewState="False" CssClass="IconSet" />
                                                <input type="button" id="BtnCopyLight" value="燈號文字複製" class="IconCopyPermissions" />
                                                &nbsp;&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="SetButton" runat="server" Text="<%$ Resources:Resource, CodeSetting %>" EnableViewState="False" CssClass="IconPassword" OnClick="SetButton_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>

                <div runat="server" id="Div_Reader">
                    <table style="margin-left: 15px; width: 98%">
                        <%--讀卡機介面--%>
                        <tr>
                            <%--<td id="ReaderUI_Title" style="text-align:center" style="background: #3B5998">--%>
                            <td id="ReaderUI_Title" style="text-align: center" class="title01">
                                <asp:Label ID="Reader_Label_Title" runat="server" Text="<%$ Resources:Reader_Label_Title %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
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
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>

                                                                        <asp:Label ID="Reader_Label_No" runat="server" Text="<%$ Resources:Resource, ReaderNo %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Reader_Label_Name" runat="server" Text="<%$ Resources:Resource, ReaderName %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Reader_Label_Desc" runat="server" Text="<%$ Resources:Resource, ReaderInfo %>"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 100px">
                                                                        <asp:TextBox ID="Reader_Input_No" runat="server" Width="90%" CssClass=""></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 250px">
                                                                        <asp:TextBox ID="Reader_Input_Name" runat="server" Width="95%"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 300px">
                                                                        <asp:TextBox ID="Reader_Input_Desc" TextMode="MultiLine" Rows="2" runat="server" Width="95%"></asp:TextBox>
                                                                    </th>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Reader_Label_Dir" runat="server" Text="<%$ Resources:Resource, InOut %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Reader_Label_CtrlName" runat="server" Text="<%$ Resources:Resource, CtrlName %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Reader_Label_CtrlModel" runat="server" Text="<%$ Resources:Resource, CtrlModel %>"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 100px">
                                                                        <asp:DropDownList ID="Reader_Input_Dir" runat="server" BackColor="#FFE5E5" Width="90%">
                                                                            <asp:ListItem Text="<%$ Resources:Resource, In %>" Value="進"></asp:ListItem>
                                                                            <asp:ListItem Text="<%$ Resources:Resource, Out %>" Value="出"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 250px">
                                                                        <asp:Label ID="Reader_Input_CtrlName" runat="server" Width="90%"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 300px">
                                                                        <asp:Label ID="Reader_Input_CtrlModel" runat="server" Width="90%"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table class="Item">
                                                    <tr>
                                                        <td>
                                                            <table>

                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labEquClass" runat="server" Text="<%$ Resources:labEquClass %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th style="width: 100px">
                                                                        <asp:Label ID="labEquNo" runat="server" Width="90%" Text="<%$ Resources:Resource, EquNo %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labEquModel" runat="server" Text="<%$ Resources:labEquModel %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th style="width: 100px">
                                                                        <asp:Label ID="labCardNoLen" Width="90%" runat="server" Text="<%$ Resources:labCardNoLen %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span id="sIsShowName" class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labIsShowName" runat="server" Text="<%$ Resources:labIsShowName %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <td>
                                                                        <%--<asp:TextBox ID="txtEquClass" runat="server" Width="80px" CssClass="" Text="" ReadOnly="True" MaxLength="20"></asp:TextBox>--%>
                                                                        <asp:DropDownList ID="ddlEquClass" runat="server" CssClass="DropDownListStyle" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="Door Access" Text="<%$ Resources:ddlEquClass_DoorAccess %>"></asp:ListItem>
                                                                            <asp:ListItem Value="Elevator" Text="<%$ Resources:ddlEquClass_Elevator %>"></asp:ListItem>
                                                                            <asp:ListItem Value="TRT" Text="<%$ Resources:ddlEquClass_TRT %>"></asp:ListItem>
                                                                        </asp:DropDownList>

                                                                    </td>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="width: 130px">
                                                                        <asp:TextBox ID="txtEquNo" runat="server" Width="90%" CssClass="TextBoxRequired" Text="" MaxLength="20"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="width: 150px">
                                                                        <asp:TextBox ID="txtEquModel" runat="server" Width="90%" CssClass="TextBoxRequired" Text="" ReadOnly="True" MaxLength="20"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="width: 70px">
                                                                        <asp:TextBox ID="txtCardNoLen" runat="server" Width="90%" CssClass="TextBoxRequired" EnableViewState="False" MaxLength="3"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlIsShowName" runat="server" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="0" Text="<%$ Resources:Resource, No %>"></asp:ListItem>
                                                                            <asp:ListItem Value="1" Text="<%$ Resources:Resource, Yes %>"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>

                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labEquName" runat="server" Text="<%$ Resources:Resource, EquName %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labEquEName" runat="server" Text="<%$ Resources:labEquEName %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <th style="width: 250px">
                                                                        <asp:TextBox ID="txtEquName" runat="server" CssClass="TextBoxRequired" Width="95%" EnableViewState="False" MaxLength="50"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="width: 150px">
                                                                        <asp:TextBox ID="txtEquEName" runat="server" CssClass="TextBoxRequired" Width="90%" EnableViewState="False" MaxLength="50"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labArea" runat="server" Text="<%$ Resources:labArea %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labBuilding" runat="server" Text="<%$ Resources:labBuilding %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span id="sFloor" class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labFloor" runat="server" Text="<%$ Resources:Resource, Floor %>" Font-Bold="true"></asp:Label>
                                                                    </th>


                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <td style="width: 180px">
                                                                        <asp:DropDownList ID="ddlLocationArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                            onchange="SelectArea()">
                                                                            <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <td style="width: 180px">
                                                                        <asp:DropDownList ID="ddlLocationBuilding" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                            onChange="SelectBuilding()">
                                                                            <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <td style="width: 180px">
                                                                        <asp:DropDownList ID="ddlLocationFloor" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>

                                                    <tr id="tr_IOA">
                                                        <td style="text-align: left">
                                                            <table id="tb_IOA">
                                                                <tr>
                                                                    <th class="AreaIo">
                                                                        <span id="slnToLinkedEquNo" class="Arrow01"></span>
                                                                    </th>
                                                                    <th class="AreaIo">
                                                                        <asp:Label ID="Label1" runat="server" Text="連動設備" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th class="AreaIo"></th>

                                                                    <th>
                                                                        <span id="sInToCtrlAreaID" class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labInToCtrlAreaID" runat="server" Text="<%$ Resources:Resource, InMgn %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span id="sOutToCtrlAreaID" class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="labOutToCtrlAreaID" runat="server" Text="<%$ Resources:Resource, OutMgn %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th class="AreaIo">
                                                                        <span id="sInput_Trt" class="Arrow01"></span>
                                                                    </th>
                                                                    <th class="AreaIo">
                                                                        <asp:Label ID="labInput_Trt" runat="server" Text="<%$ Resources:labInput_Trt %>" Font-Bold="true"></asp:Label>
                                                                    </th>
                                                                </tr>
                                                                <tr>
                                                                    <th class="AreaIo"></th>
                                                                    <td class="AreaIo">
                                                                        <asp:DropDownList ID="ddlLinkEquNoList" runat="server" Width="180px" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                    <th class="AreaIo"></th>

                                                                    <th></th>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlInToCtrlAreaID" runat="server" Width="110px" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="0" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                            <%--<asp:ListItem Value="1" Text="DOOR"></asp:ListItem>--%>
                                                                        </asp:DropDownList></td>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <td>
                                                                        <asp:DropDownList ID="ddlOutToCtrlAreaID" runat="server" Width="110px" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="0" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                            <%--<asp:ListItem Value="1" Text="DOOR"></asp:ListItem>--%>
                                                                        </asp:DropDownList></td>
                                                                    <th></th>

                                                                    <th class="AreaIo"></th>
                                                                    <td class="AreaIo">
                                                                        <asp:DropDownList ID="ddlIsAndTrt" runat="server" Width="90px" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="1" Text="<%$ Resources:Resource, Yes %>"></asp:ListItem>
                                                                            <asp:ListItem Value="0" Selected="True" Text="<%$ Resources:Resource, No %>"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td style="padding-top: 1px">
                                                <asp:Button ID="Reader_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Reader_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Reader_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="Reader_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                                <input type="button" id="BtnPrint" value="<%=GetLocalResourceObject("btnPrint") %>" onclick="SetPrintQR()" class="IconSetCardNumber" />
                                                <input type="button" id="BtnPrint1" value="<%=GetLocalResourceObject("btnPrint1") %>" onclick="SetPrintQR1()" class="IconSetCardNumber" />
                                                <asp:Button ID="ParaButton" runat="server" Text="<%$ Resources: ReaderParaSetting %>" EnableViewState="False" CssClass="IconSet" />

                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
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
</asp:Content>


