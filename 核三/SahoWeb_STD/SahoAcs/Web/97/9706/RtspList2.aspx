<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RtspList2.aspx.cs" Inherits="SahoAcs.Web._97._9706.RtspList2" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../../Css/bootstrap.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>
            <th>
                <span class="Arrow01"></span>
                <asp:Label ID="lblEvoHost" runat="server" Text="影片服務端位置"></asp:Label>
            </th>
            <td></td>
        </tr>
        <tr>
            <th>               
                <input type="text" name="RtspHost" id="RtspHost" value="<%=RtspHost %>" style="width: 250px" disabled="disabled" />
            </th>            
            <td>                
                <%--<input type="button" id="AddButton" name="AddButton" value="新增"  class="IconNew" />--%>
            </td>
        </tr>
    </table>
    <table class="Item">
        <tr>
		<th>
                <span class="Arrow01"></span>
                <span>攝影機位置清單：</span>
            </th>
		</tr>
        <tr>
            <th style="height: 250px; width: 820px; vertical-align: top" >
                <div class="container">
                    <div class="row">                        
                        <%foreach (var o in this.GlobalRtspList)
                            { %>
                            <div class="col-lg-4">
                                <br />
                                <span><%=o.RtspMemo %></span><br />
                                <input type="hidden" id="hResID" name="hResID" value="<%=o.ResourceID %>" />
                                <input type="hidden" id="ChkTarget" name="ChkTarget" value="<%=o.ChkTarget %>" />
                                <input type="hidden" id="hUrl" name="hUrl" value="<%=o.RtspVideo %>" />                                
                                <input type="button" name="BtnOpen" value="開啟影像"  class="IconSearch" />                                
                            </div>
                        <%} %>                        
                    </div>
                </div>
            </th>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <%--<input type="button" value="儲存" id="SaveButton" class="IconSave" />--%>
            </td>
            </tr>
            </table>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
