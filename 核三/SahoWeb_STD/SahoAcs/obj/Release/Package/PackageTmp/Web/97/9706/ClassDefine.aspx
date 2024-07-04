<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ClassDefine.aspx.cs" Inherits="SahoAcs.Web._97._9706.ClassDefine" Theme="UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="Item">
        <tr>            
            <th>
                <span class="Arrow01"></span>
                <span>初始排班月份</span>
            </th>
            <td></td>
        </tr>
        <tr>
            <td>
                <input type="text" id="ClassMonth" name="ClassMonth" value="<%=DateTime.Now.ToString("yyyy/MM") %>"/>
                <input type="hidden" id="DefMonth" name="DefMonth" value="" />
            </td>
            <th>
                <input type="button" id="BtnQuery" name="BtnQuery" value="取得未初始化人員" class="IconSearch" />
                <input type="button" id="BtnCreate" name="BtnCreate" value="排班初始化"  class="IconSet"/>※系統將根據初始兩日的排班，之後每月依順序建立班表
            </th>
        </tr>
    </table>
    <table>
        <tr>
            <td style="height: 250px; width: 820px; vertical-align: top" id="EquArea">
                <!--依管理區自動帶入輪班人員的資訊--->
                <table class="TableS1">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 105px">工號</th>
                            <th scope="col" style="width: 105px">姓名</th>
                            <th scope="col" style="width: 105px">前二日班別(24日)</th>
                            <th scope="col">前一日班別(25日)</th>                            
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" style="padding: 0" colspan="5">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 820px; overflow-y: scroll;">
                                    <div id="ResultData">
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                            <tbody>
                                                <%foreach (var work in this.ClassList)
                                                { %>
                                                <tr>
                                                    <td style="width: 109px">
                                                        <%=work.PsnNo %>
                                                        <input type="hidden" id="PsnNo" name="PsnNo" value="<%=work.PsnNo %>" />
                                                    </td>
                                                    <td style="width: 109px">
                                                        <%=work.PsnName %>
                                                    </td>
                                                    <td style="width: 109px">
                                                        <select id="ClassNo2" name="ClassNo2" class="DropDownListStyle">
                                                            <%foreach (var o in this.ClassNoList)
                                                                {%>
                                                            <%if (o.Equals(work.ClassNo2))
                                                                { %>
                                                            <option selected="selected" value="<%=o %>"><%=o.Equals("R")?"休":o %></option>
                                                            <%}
                                                                else
                                                                { %>
                                                            <option value="<%=o %>"><%=o.Equals("R")?"休":o %></option>
                                                            <%} %>
                                                            <%} %>
                                                        </select>
                                                    </td>
                                                    <td>
                                                        <select id="ClassNo1" name="ClassNo1" class="DropDownListStyle">
                                                            <%foreach (var o in this.ClassNoList)
                                                                {%>
                                                            <%if (o.Equals(work.ClassNo1))
                                                                { %>
                                                            <option selected="selected" value="<%=o %>"><%=o.Equals("R")?"休":o %></option>
                                                            <%}
                                                                else
                                                                { %>
                                                            <option value="<%=o %>"><%=o.Equals("R")?"休":o %></option>
                                                            <%} %>
                                                            <%} %>
                                                        </select>
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
                <span style="color:white; font-size:14pt">※ 每次執行50人次的初始化設定</span>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <%--<asp:Button ID="AddButton" runat="server" Text="<%$ Resources:Resource, btnAdd %>" CssClass="IconNew" />--%>                
            </td>
        </tr>
    </table>
    <div id="dvContent" style="position: absolute; left: 20px; top: 30px; z-index: 1000000000"></div>
    <input type="hidden" name="PageEvent" value="Save" />
</asp:Content>
