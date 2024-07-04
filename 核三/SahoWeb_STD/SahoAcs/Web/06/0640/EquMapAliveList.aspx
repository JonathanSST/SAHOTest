<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EquMapAliveList.aspx.cs" Inherits="SahoAcs.Web._06._0640.EquMapAliveList"  Theme="UI"  %>
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
                            <th scope="col">顯示</th>
                        </tr>
                        <tr>
                            <td id="ContentPlaceHolder1_td_showGridView2" colspan ="3">
                                <div id="ContentPlaceHolder1_tablePanel2" style="height: 305px; width: 980px; overflow-y: scroll;" >
                                    <div id="DataResult">
                                        <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_GridView2" style="border-collapse: collapse;">
                                              <tbody>
                                                   <%foreach (var s in MapDataList)
                                                  { %>
                                                  <tr>
                                                       <td style="width: 194px; text-align: left"><%=s.PicDesc %></td>
                                                      <td style="width: 274px; text-align: left"><%=s.PicName %></td>
                                                      <td style="text-align: center ">
                                                           <input type="button" id="BtnMapShow" value="地圖顯示" class="IconOk" />  
                                                            <input type="hidden" id="EditID" value="<%=s.PicID %>" />
                                                      </td>
                                                  </tr>
                                                    <%
                                                  } %>
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
          <%if (this.PagedList != null)
                        { %>
                <tr class="GVStylePgr">
                    <td colspan="3">
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
    </table>
</asp:Content>
