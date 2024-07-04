<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="IOMgnSetting.aspx.cs" Inherits="SahoAcs.IOMgnSetting" Theme="UI" EnableEventValidation="false" %>

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
                            <asp:HiddenField ID="IOMstIDValue" runat="server" />
                            <asp:HiddenField ID="IOModuleIDValue" runat="server" />
                            <asp:HiddenField ID="SenIDValue" runat="server" />
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

                <div runat="server" id="Div_IOMaster">
                    <table style="margin-left: 15px; width: 98%">
                        <%--IO連線裝置介面--%>
                        <tr>
                            <td id="IOMasterUI_Title" style="text-align: center; width: 100%" class="title01">
                                <div runat="server" id="Div_IOMasterUI_Title">
                                    <asp:Label ID="Master_Label_Title" runat="server" Text="<%$ Resources: IOMasterSet %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
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
                                                            <asp:Label ID="Master_Label_No" runat="server" Text="<%$Resources: IOMstNo %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_Name" runat="server" Text="<%$Resources: IOMstName %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Master_Label_Dci" runat="server" Text="<%$Resources:IOMstConn %>"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="IOMaster_Input_No" runat="server" CssClass="TextBoxRequired" Width="120px"></asp:TextBox>
                                                        </th>

                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="IOMaster_Input_Name" runat="server" Width="220px" CssClass="TextBoxRequired" MaxLength="20"></asp:TextBox>
                                                        </th>

                                                        <th></th>

                                                        <th></th>
                                                        <th>
                                                            <asp:Label ID="IOMaster_Input_Dci" runat="server" Width="150px"></asp:Label>
                                                        </th>
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
                                                                <asp:TextBox ID="IOMaster_Input_IPParam_IP" runat="server" Style="width: 120px" CssClass="TextBoxRequired"></asp:TextBox>
                                                            </th>

                                                            <th></th>

                                                            <th></th>
                                                            <th style="width: 100px; text-align: left">
                                                                <asp:TextBox ID="IOMaster_Input_IPParam_Port" runat="server" CssClass="TextBoxRequired" Width="80%"></asp:TextBox>
                                                            </th>
                                                            <th></th>
                                                            <th></th>
                                                            <th>
                                                                <asp:RadioButton ID="IOMaster_Input_AutoRerun_Y" runat="server" Text="<%$ Resources:Resource, Yes %>" value="0" GroupName="MasterAutoRerun" Width="60px" />
                                                                <asp:RadioButton ID="IOMaster_Input_AutoRerun_N" runat="server" Text="<%$ Resources:Resource, No %>" value="1" GroupName="MasterAutoRerun" Width="60px" Checked="True" />
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
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="IOMaster_labArea" runat="server" Text="<%$Resources: labArea %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="IOMaster_labBuilding" runat="server" Text="<%$Resources: labBuilding %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="IOMaster_labFloor" runat="server" Text="<%$ Resources:Resource, Floor %>"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="IOMasterDDLArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectIOMstArea()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 180px">
                                                            <asp:DropDownList ID="IOMasterDDLBuilding" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectIOMstBuilding()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="IOMasterDDLFloor" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
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
                                                            <asp:Label ID="Master_Label_CtrlModel" runat="server" Text="控制器機型"></asp:Label>
                                                        </th>
                                                        <th></th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="IOMaster_Input_Model" runat="server" Width="120px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 100px; text-align: left;">
                                                            <asp:DropDownList ID="IOMaster_Input_Status" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Enabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Disabled %>" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Restrict %>" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>

                                                        <th></th>
                                                        <td>
                                                            <asp:DropDownList ID="IOMaster_Input_CtrlModel" runat="server" BackColor="#FFE5E5" Font-Bold="True"></asp:DropDownList>
                                                        </td>
                                                        <th></th>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="IOMaster_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="IOMaster_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="IOMaster_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="IOMaster_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>

                <div runat="server" id="Div_IOModule">
                    <table style="margin-left: 15px; width: 98%">
                        <%--I/O模組介面--%>
                        <tr>
                            <td id="IOModuleUI_Title" style="text-align: center" class="title01">
                                <asp:Label ID="IOModule_Label_Title" runat="server" Text="<%$Resources:IOModuleSet %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
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
                                                            <asp:Label ID="Iom_Label_No" runat="server" Text="<%$Resources:IomNO %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Label_Name" runat="server" Text="<%$Resources:IomName %>"></asp:Label>
                                                        </th>
                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Label_Addr" runat="server" Text="<%$Resources:IomAddr %>"></asp:Label>
                                                        </th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Lable_Ctrl" runat="server" Text="<%$Resources:IomCtrl %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Iom_Input_No" runat="server" CssClass="TextBoxRequired" Width="100px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Iom_Input_Name" runat="server" Width="200px"></asp:TextBox>
                                                        </th>
                                                        <th></th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Iom_Input_Addr" runat="server" CssClass="TextBoxRequired" Width="80px"></asp:TextBox>
                                                        </th>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:Label ID="Iom_Input_Ctrl" runat="server" Text="" Width="100px"></asp:Label>
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
                                                            <asp:Label ID="Iom_Lable_Usage" runat="server" Text="<%$Resources:IomUsage %>"></asp:Label>
                                                        </th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Label2" runat="server" Text="<%$Resources:Resource,DeviceIP %>"></asp:Label>
                                                        </th>

                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <th style="vertical-align: top; width: 360px;">
                                                            <asp:RadioButtonList ID="Iom_Input_Usage" runat="server" RepeatDirection="Horizontal" onchange="ChangeIOUsage()">
                                                                <asp:ListItem Selected="True" Value="0">一般IO使用</asp:ListItem>
                                                               <%-- <asp:ListItem Value="1" >設備內建控制器使用</asp:ListItem>--%>
                                                            </asp:RadioButtonList>
                                                        </th>

                                                        <th></th>
                                                        <%--<td style="vertical-align: top; width: 180px;">
                                                            <asp:DropDownList ID="IomDDLIOMst" Width="95%" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>--%>
                                                        <th style="vertical-align: top;">
                                                            <asp:Label ID="Iom_Input_IOMst" runat="server" Text="" Width="100px"></asp:Label>
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
                                                            <asp:Label ID="Iom_Lable_CtrlK3" runat="server" Text="<%$Resources:IomCtrlK3 %>"></asp:Label>
                                                        </th>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Lable_Bits" runat="server" Text="<%$Resources:IomBits %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <td style="vertical-align: top; width: 360px;">
                                                            <asp:DropDownList ID="IomDDLCtrlK3" Width="95%" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:DropDownList ID="IomDDLCtrlK3_Other" Width="95%" runat="server" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>
                                                        <th style="vertical-align: top;">
                                                            <asp:TextBox ID="Iom_Input_Bits" runat="server" CssClass="TextBoxRequired" Width="80px"></asp:TextBox>
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
                                                            <asp:Label ID="Iom_Lable_Area" runat="server" Text="<%$Resources: labArea %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Iom_Lable_Building" runat="server" Text="<%$Resources: labBuilding %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Lable_Floor" runat="server" Text="<%$ Resources:Resource, Floor %>"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="IomDDLArea" runat="server" Width="95%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectIomArea()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 190px">
                                                            <asp:DropDownList ID="IomDDLBuilding" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectIomBuilding()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="IomDDLFloor" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
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
                                                            <asp:Label ID="Iom_Lable_Status" runat="server" Text="<%$Resources:Resource,Status %>"></asp:Label>
                                                        </th>
                                                        <th></th>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Iom_Label_Desc" runat="server" Text="<%$Resources:IomDesc %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <td style="vertical-align: top; width: 140px">
                                                            <asp:DropDownList ID="Iom_Input_Status" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Enabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Disabled %>" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Restrict %>" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Iom_Input_Desc" TextMode="MultiLine" Rows="2" runat="server" Width="340px"></asp:TextBox>
                                                        </th>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="Iom_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Iom_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Iom_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="Iom_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
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

                <div runat="server" id="Div_Sensor">
                    <table style="margin-left: 15px; width: 98%">
                        <%--偵測器介面--%>
                        <tr>
                            <%--<td id="ReaderUI_Title" style="text-align:center" style="background: #3B5998">--%>
                            <td id="SensorUI_Title" style="text-align: center" class="title01">
                                <asp:Label ID="Sensor_Label_Title" runat="server" Text="<%$ Resources:Sen_Label_Title %>" Font-Size="Medium" Font-Bold="true" ForeColor="White"></asp:Label>
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
                                                             <table class="Item">
                                                                <tr>
                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>

                                                                        <asp:Label ID="Sen_Label_No" runat="server" Text="<%$ Resources:SenNo %>" ></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Sen_Label_Name" runat="server" Text="<%$ Resources:SenName %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Sen_Label_IoBit" runat="server" Text="<%$ Resources:SenIoBit %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>
                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 140px">
                                                                        <asp:TextBox ID="Sen_Input_No" runat="server" Width="90%" CssClass="TextBoxRequired"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 250px">
                                                                        <asp:TextBox ID="Sen_Input_Name" runat="server" Width="95%"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <th style="vertical-align: top; width: 100px">
                                                                        <asp:TextBox ID="Sen_Input_Bit" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>
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
                                                                        <asp:Label ID="Sen_Label_AS" runat="server" Text="<%$ Resources:SenActiveSignal %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                    <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Sen_Label_AlmType" runat="server" Text="<%$ Resources:SenAlmType %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                     <th>
                                                                        <span class="Arrow01"></span>
                                                                    </th>
                                                                    <th>
                                                                        <asp:Label ID="Sen_Lable_AlmSeconds" runat="server" Text="<%$ Resources:SenAlmSeconds %>"></asp:Label>
                                                                    </th>
                                                                    <th></th>

                                                                </tr>
                                                                <tr>
                                                                    <th></th>
                                                                    <td style="vertical-align: top; width: 140px">
                                                                        <asp:DropDownList ID="Sen_Input_ActiveSignal" runat="server" Width="90%" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                            <asp:ListItem Text="0" Value="0"></asp:ListItem>
                                                                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <th></th>

                                                                    <th></th>
                                                                    <td style="width: 190px">
                                                                        <asp:DropDownList ID="SenDDLAlmType" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                            <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <th></th>

                                                                     <th></th>
                                                                    <th style="vertical-align: top; width: 100px">
                                                                        <asp:TextBox ID="Sen_Input_AlmSeconds" runat="server" Width="95%" CssClass="TextBoxRequired"></asp:TextBox>
                                                                    </th>
                                                                    <th></th>
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
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Sen_Label_Area" runat="server" Text="<%$Resources: labArea %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span></th>
                                                        <th>
                                                            <asp:Label ID="Sen_Label_Building" runat="server" Text="<%$Resources: labBuilding %>"></asp:Label>
                                                        </th>

                                                        <th></th>

                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Sen_Label_Floor" runat="server" Text="<%$ Resources:Resource, Floor %>"></asp:Label>
                                                        </th>
                                                    </tr>

                                                    <tr>
                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="SenDDLArea" runat="server" Width="95%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectSenArea()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 190px">
                                                            <asp:DropDownList ID="SenDDLBuilding" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True"
                                                                onchange="SelectSenBuilding()">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>

                                                        <th></th>

                                                        <th></th>
                                                        <td style="width: 140px">
                                                            <asp:DropDownList ID="SenDDLFloor" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Value="" Text="<%$ Resources:Resource, ddlSelectDefault %>"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
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
                                                            <asp:Label ID="Sen_Label_Status" runat="server" Text="<%$Resources:Resource,Status %>"></asp:Label>
                                                        </th>
                                                        <th></th>
                                                        <th>
                                                            <span class="Arrow01"></span>
                                                        </th>
                                                        <th>
                                                            <asp:Label ID="Sen_Label_Desc" runat="server" Text="<%$Resources:SenDesc %>"></asp:Label>
                                                        </th>
                                                    </tr>
                                                    <tr>
                                                        <th></th>
                                                        <td style="vertical-align: top; width: 140px">
                                                            <asp:DropDownList ID="Sen_Input_Status" runat="server" Width="95%" BackColor="#FFE5E5" Font-Bold="True">
                                                                <asp:ListItem Text="<%$ Resources:Resource,ddlSelectDefault %>" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="<%$ Resources:Resource,Enabled %>" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Disabled %>" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="<%$Resources:Resource,Restrict %>" Value="2"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <th></th>
                                                        <th></th>
                                                        <th>
                                                            <asp:TextBox ID="Sen_Input_Desc" TextMode="MultiLine" Rows="2" runat="server" Width="340px"></asp:TextBox>
                                                        </th>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        
                                        <tr>
                                            <td style="padding-top: 1px">
                                                <asp:Button ID="Sen_B_Add" runat="server" Text="<%$ Resources:Resource, btnAdd%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Sen_B_Edit" runat="server" Text="<%$ Resources:Resource, btnSave%>" EnableViewState="False" CssClass="IconSave" />
                                                <asp:Button ID="Sen_B_Delete" runat="server" Text="<%$ Resources:Resource, btnDelete%>" EnableViewState="False" CssClass="IconDelete" />
                                                <asp:Button ID="Sen_B_Cancel" runat="server" Text="<%$ Resources:Resource, btnCancel %>" EnableViewState="False" CssClass="IconCancel" />
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

   <%-- <asp:Panel ID="PanelPopup1" runat="server" SkinID="PanelPopup" Width="260px"
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
    </asp:Panel>--%>
  <%--  <asp:Label ID="PopupTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <asp:Label ID="CancelTrigger1" runat="server" EnableViewState="False"></asp:Label>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server"></cc1:ModalPopupExtender>--%>
    <%--<asp:TextBox ID="txt_NodeTypeList" runat="server"></asp:TextBox>
    <asp:TextBox ID="txt_NodeIDList" runat="server"></asp:TextBox>
    <asp:TextBox ID="NodeAct" runat="server"></asp:TextBox>
    <asp:TextBox ID="CreateSource" runat="server"></asp:TextBox>
    <asp:TextBox ID="ClickItem" runat="server"></asp:TextBox>--%>
</asp:Content>


