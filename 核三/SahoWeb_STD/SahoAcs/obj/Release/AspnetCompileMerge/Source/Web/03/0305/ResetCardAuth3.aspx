<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ResetCardAuth3.aspx.cs" Inherits="SahoAcs.Web.ResetCardAuth3" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">    
    <table>
        <tr>
            <td style="height: 250px; width: 780px; vertical-align: top" id="EquArea">
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 170px">重整卡別</th>
                            <th scope="col" style="width: 155px">卡片數量</th>                            
                            <th scope="col" style="width: 120px">尚待重整張數</th>
                            <th scope="col" style="">執行重整</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="4">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 780px; overflow-y: scroll;">
                                    <div>
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="DataGridList" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var s in this.CardTypeList)
                                                    { %>
                                                <tr>
                                                    <td style="width: 174px; text-align: left"><%=s.CardTypeName %>                                                        
                                                    </td>
                                                    <td style="width: 159px; text-align: left"><%=s.CardAmt %></td>
                                                    <td style="width: 124px; text-align: left"><%=s.WaitCount %></td>
                                                    <td style="text-align: center">
                                                        <input type="button" value="送出重整" id="BtnAdd" class="IconSet" name="BtnAdd"/>
                                                        <input type="hidden" value="<%=s.CardTypeNo %>" id="CardType"  name="CardType"/>
                                                    </td>
                                                </tr>
                                                <%} %>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>
                <div style="color:white;background-color:red; display:none" id="ProcDiv">處理中</div>
            </td>
            </tr>
            </table>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
