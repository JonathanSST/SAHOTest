<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapSet.aspx.cs" Inherits="SahoAcs.Web._98._9825.EquMapSet" Theme="UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
             <td style="height: 250px; width: 980px; vertical-align: top" id="EquArea">
                <table class="TableS1" style="width:980px">
                    <tbody>
                        <tr class="GVStyle">
                            <th scope="col" style="width: 190px">圖幅名稱</th>
                            <th scope="col" style="width: 270px">圖幅路徑</th>
                            <th scope="col" style="width: 70px">是否顯示</th>
                            <th scope="col">編輯</th>
                        </tr>
                        <tr>
                             <td id="ContentPlaceHolder1_td_showGridView2" colspan ="4">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 980px; overflow-y: scroll;" >
                                    <div id="DataResult">
                                         <table class="GVStyle" cellspacing="0" rules="all" border="1"
                                            id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                             <tbody>
                                                  <%foreach (var s in MapDataList)
                                                  { %>
                                                <tr>
                                                    <td style="width: 194px; text-align: left"><%=s.PicDesc %></td>
                                                    <td style="width: 274px; text-align: left"><%=s.PicName %></td>
                                                    <td style="width: 74px;  text-align: center" >
                                                         <%if (s.IsOpen.Equals(1))
                                                            { %>是<%}
                                                        else
                                                            { %>否<%} 
                                                         %>
                                                    </td>
                                                    <td style="text-align: center ">
                                                       <input type="button" id="BtnMapEdit" value="地圖編輯" class="IconSet" />                                                    
                                                       <input type="button" id="BtnEdit" value="資料編輯" class="IconSet"  />                                                    
                                                        <input type="hidden" id="EditID" value="<%=s.PicID %>" />
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
                    <td colspan="4">
                        <a id="btnFirst" href="#" style="text-decoration: none;" onclick="ShowPage(1)">第一頁</a>
                        <a id="btnPrev" href="#" style="text-decoration: none;"  onclick="ShowPage(<%=this.PrePage%>)">前一頁</a>
                        <%for (int pageIndex = this.StartPage; pageIndex < EndPage; pageIndex++)
                        { %>
                        <%if (pageIndex == this.PageIndex)
                        { %>
                                    <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;color:white"><%=pageIndex %></a>
                        <%}
                        else
                        {%>
                                <a id="btn_<%=pageIndex %>" onclick="ShowPage(<%=pageIndex %>)" href="#" style="text-decoration: none;"><%=pageIndex %></a>
                                <%} %>
                        <%} %>
                            <%=string.Format("{0} / {1}　                總共 {2} 筆", this.PagedList.PageNumber, this.PagedList.PageCount, this.PagedList.TotalItemCount) %>       
                        <a id="btnNext" href="#" style="text-decoration: none; " onclick="ShowPage(<%=this.NextPage%>)">下一頁</a>
                        <a id="btnLast" href="#" style="text-decoration: none;" onclick="ShowPage(<%=this.PagedList.PageCount %>)">最末頁</a>
                    </td>
                </tr>
                <%} %>
                    </tbody>
                </table>
             </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <input type="button" value="新增" onclick="AddMap()" class="IconNew" />
            </td>
        </tr>
    </table>
    <input type="hidden" name="PageEvent" value="Save" />
    <input type="hidden" name="PageMapSrc" id="PageMapSrc" value="<%=this.MapSrc %>" />
</asp:Content>
