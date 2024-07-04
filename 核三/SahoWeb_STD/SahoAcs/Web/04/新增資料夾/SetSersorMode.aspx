<%@ Page Title="" Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SetSersorMode.aspx.cs" Inherits="SahoAcs.SetSersorMode" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <%--   <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />--%>
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <asp:HiddenField ID="sLocArea" runat="server" />
        <asp:HiddenField ID="sLocBuilding" runat="server" />
        <asp:HiddenField ID="sLocFloor" runat="server" />
        <asp:HiddenField ID="sCtrlModel" runat="server" />
        <asp:HiddenField ID="tmpLocArea" runat="server" />
        <asp:HiddenField ID="tmpLocBuilding" runat="server" />
        <asp:HiddenField ID="txtAreaList" runat="server" />
        <asp:HiddenField ID="txtBuildingList" runat="server" />
        <asp:HiddenField ID="txtFloorList" runat="server" />
        <asp:HiddenField ID="sSaveCheckList" runat="server" />
        <asp:HiddenField ID="sSenStatus" runat="server" />
    </div>
    <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <th style="display: none;">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_KeyWord" runat="server" Text="<%$Resources:ResourceCtrls,lblKeyword %>"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLable_Ctrl" runat="server" Text="機種"></asp:Label>
                        </th>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLable_Loc" runat="server" Text="位置"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td style="width: 140px">
                            <asp:DropDownList ID="ddlCtrlMode" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 140px">
                            <asp:DropDownList ID="ddlLocArea" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                onchange="SelectArea()">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 140px">
                            <asp:DropDownList ID="ddlLocBuilding" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True"
                                onchange="SelectBuilding()">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 140px">
                            <asp:DropDownList ID="ddlLocFloor" runat="server" Width="90%" CssClass="DropDownListStyle" EnableViewState="False" BackColor="#FFE5E5" Font-Bold="True">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <input type="button" class="IconSearch" value="<%=GetGlobalResourceObject("Resource","btnQuery") %>" onclick="SetMode(1)" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width: 1050px">
            <tbody>
                <tr class="GVStyle">
                    <th scope="col" class="TitleRow" style="width: 50px">
                        <%--<asp:CheckBox ID="HeaderCheckState" runat="server"></asp:CheckBox>--%>
                        <input name="HeaderCheckState" id="HeaderCheckState" type="checkbox" />
                    </th>
                    <th scope="col" class="TitleRow" style="width: 60px">識別碼
                    </th>
                    <th scope="col" class="TitleRow" style="width: 100px">IO模組編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 180px">IO模組名稱
                    </th>
                    <th scope="col" class="TitleRow" style="width: 100px">偵測器編號
                    </th>
                    <th scope="col" class="TitleRow" style="width: 180px">偵測器名稱
                    </th>
                    <th scope="col" class="TitleRow" style="width: 250px">位置
                    </th>
                    <th scope="col" class="TitleRow">狀態
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="9">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1250px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if (this.DataResult.Rows.Count == 0)
                                            { %>
                                        <tr class="DataRow">
                                            <td colspan="9">
                                                <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <%foreach (System.Data.DataRow r in this.DataResult.Rows)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">
                                            <td style="width: 54px; text-align: center">
                                                <input name="RowCheckState[]" type="checkbox" value="<%=r["IOMstID"].ToString()+","+r["IOMID"].ToString()+","+r["SenID"].ToString() %>" />
                                            </td>
                                            <td style="width: 64px; text-align: center">
                                                <%=r["SenID"].ToString() %>
                                            </td>
                                            <td style="width: 104px; text-align: center">
                                                <%=r["IOMNo"].ToString() %>
                                            </td>
                                            <td style="width: 184px; text-align: center">
                                                <%=r["IOMName"].ToString() %>
                                            </td>
                                            <td style="width: 104px; text-align: center">
                                                <%=r["SenNo"].ToString() %>
                                            </td>
                                            <td style="width: 184px; text-align: center">
                                                <%=r["SenName"].ToString() %>
                                            </td>
                                            <td style="width: 254px; text-align: center">
                                                <%=r["LocLongName"].ToString() %>
                                            </td>
                                            <td style="text-align: center">
                                                <%=r["RowStatus"].ToString() %>
                                            </td>

                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
                <%if (this.PagedList != null)
                    { %>
                <tr class="GVStylePgr">
                    <td colspan="11">
                        <a id="btnFirst" href="#" style="text-decoration: none;" onclick="ShowPage(1)">第一頁</a>
                        <a id="btnPrev" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                            { %>
                        <%if (pageIndex == this.PageIndex)
                            { %>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none; color: white"><%=pageIndex %></a>
                        <%}
                            else
                            {%>
                        <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                        <%} %>
                        <%} %>
                        <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>
                        <a id="btnNext" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div>
    <br />
    <div class="Item">
        <div>
            <asp:RadioButton ID="SenStatus_1" runat="server" Text="啟用" value="1" GroupName="SenStatus" Width="60px" Checked="True" />
            <asp:RadioButton ID="SenStatus_0" runat="server" Text="停用" value="0" GroupName="SenStatus" Width="60px" />
            <asp:RadioButton ID="SenStatus_2" runat="server" Text="限制" value="2" GroupName="SenStatus" Width="60px" />
            <input type="button" name="SetButton" value="設 定" id="SetButton" class="IconSet" onclick="SetStatus()" />
        </div>
    </div>

</asp:Content>
