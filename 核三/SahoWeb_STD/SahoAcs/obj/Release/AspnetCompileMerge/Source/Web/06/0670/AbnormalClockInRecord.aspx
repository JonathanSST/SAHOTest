<%@ Page Title=""  Theme="UI" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AbnormalClockInRecord.aspx.cs" Inherits="SahoAcs.Web._06._0670.AbnormalClockInRecord" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="BuildingValue" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="DefaultCardLen" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="hDataRowCount" runat="server" EnableViewState="False" />
        <input type="hidden" id="SortDataContext" name="SortDataContext" value="" />
        <input type="hidden" id="PageEvent" name="PageEvent" value="Query" />
        <input type="hidden" id="PageIndex" name="PageIndex" value="1" />
        <input type="hidden" id="CardDateS" name="DateS" value="" />
        <input type="hidden" id="CardDateE" name="DateE" value="" />
        <input type="hidden" id="MgaName" name="MgaName" value="" />        
        <input type="hidden" id="QueryMode" name="QueryMode" value="" />
        <input type="hidden" id="SortName" name="SortName" value="<%=this.SortName %>" />
        <input type="hidden" id="SortType" name="SortType" value="<%=this.SortType %>" />
        <input type="hidden" id ="PsnID" name="PsnID" value="<%=this.PsnID %>" />
        <input type="hidden" name="AuthList" id="AuthList" value="<%=this.AuthList %>" />             
    </div>
     <table class="Item">
        <tr>
            <td>
                <table>
                    <tr>
                        <%if (this.PsnNo.Equals(""))
                            { %>
                        <th>
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label1" runat="server" Text="工號"></asp:Label>
                        </th>
                        <%} %>
                        <th colspan="3">
                            <span class="Arrow01"></span>
                            <asp:Label ID="QueryLabel_CardTime" runat="server" Text="異常時間起迄"></asp:Label>
                        </th>     
                        <th> 
                            <span class="Arrow01"></span>
                            <asp:Label ID="Label2" runat="server" Text="發送否"></asp:Label>
                        </th>
                        <td></td>
                    </tr>
                    <tr>
                        <%if (this.PsnNo.Equals(""))
                            { %>
                        <td>
                            <input type="text" id="PsnName" name="PsnName"  />
                            <input type="hidden" id="PsnNo" name="PsnNo" />                            
                        </td>
                        <%} %>
                        <td>
                           <uc2:CalendarFrm ID="CardDayS" runat="server" />
                        </td>
                        <td style="font-size: 16px; color: white"><%=Resources.Resource.lblTo %></td>
                        <td>
                            <uc2:CalendarFrm ID="CardDayE" runat="server" />
                        </td>   
                        <td>
                            <select class="DropDownListStyle" id="dllIsSend" name="dllIsSend" style="width:108px">
                                <option value="0">未發送</option>
                                <option value="1">已發送</option>
                            </select>
                        </td>
                        <td>                            
                            <input type="button" id="BtnQuery" value="<%=Resources.Resource.btnQuery %>" class="IconSearch" onclick="SetMode(1);" />
                            <input type="button" id="BtnSetting" value="電子郵件設定" class="IconSet" />
                            <input type="button" id="BtnModify" value="儲存異常原因" class="IconSave"/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
     <div id="ContentPlaceHolder1_UpdatePanel1">
        <table class="TableS1" style="width:1000px">
            <tbody>
                <tr class="GVStyle">   
                    <th scope="col" style="width:30px;">
                        <input type="checkbox" name="ChkAll" id="ChkAll" onclick="SetCheckAll(this)" />
                    </th>
                   <th scope="col" class="TitleRow" style="width:100px">
                        日期
                    </th>
                    <th scope="col" class="TitleRow" style="width:200px">
                        單位
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        姓名
                    </th>
                    <th scope="col" class="TitleRow" style="width:100px">
                        工號
                    </th>
                   <%-- <th scope="col" class="TitleRow" style="width:100px">
                        最後刷卡日期時間
                    </th>--%>
                    <th scope="col" class="TitleRow">
                        異常原因
                    </th>
                </tr>
                <tr>
                    <td id="ContentPlaceHolder1_td_showGridView" colspan="6">
                        <div id="ContentPlaceHolder1_tablePanel" class="MinHeight" style="width: 1000px; overflow-y: scroll;">
                            <div>
                                <table class="GVStyle" cellspacing="0" rules="all" border="1" id="ContentPlaceHolder1_MainGridView" style="width: 100%; border-collapse: collapse;">
                                    <tbody>
                                        <%if(this.PagedList.Count==0) { %>
                                            <tr class="DataRow">
                                                <td colspan="6">
                                                    <%=GetGlobalResourceObject("Resource","NonData").ToString() %>
                                                </td>
                                            </tr>
                                        <%} %>
                                        <%foreach (var o in this.PagedList)
                                            { %>
                                        <tr class="DataRow" id="GV_Row199158" onmouseover="onMouseMoveIn(0,this,'','')" onmouseout="onMouseMoveOut(0,this)" style="color: rgb(0, 0, 0);">                                                                                        
                                             <td style="width:34px;text-align:center">
                                                 <input type="checkbox"
                                                     name="ChkOne" id="ChkOne" 
                                                     value="<%=o.RecordID %>"  onclick="chkChange(this)" />
                                             </td>
                                             <td style="width:104px">
                                                 <%=o.WorkDate %>
                                            </td>
                                            <td style="width:204px">
                                                <%=o.OrgStrucName%>
                                            </td>
                                             <td style="width:104px">
                                                <%=o.PsnName%>
                                            </td>
                                             <td style="width:104px">
                                                <%=o.PsnNo%>
                                            </td>
                                            <%--  <td style="width:104px">
                                                  <%=o.WorkDate%>
                                                  <%=o.RealTimeS + "~" + o.RealTimeE%>
                                            </td>        --%>                        
                                              <td>
                                                  <input type="text" id="AbnormalDesc" name="AbnormalDesc" value="<%=string.IsNullOrEmpty(o.AbnormalDesc)?o.StatuDesc:o.AbnormalDesc %>" style="width:98%" />
                                                  <input type="hidden" id="RecordID" name="RecordID" value="<%=o.RecordID %>" />
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
                            <td colspan="14">
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
                      <%} 
                %>
            </tbody>
        </table>
    </div> 
    <br />
     <div>        
       <input type="button" name="BtnSend" value="發 送" id="BtnSend" class="IconExport" onclick="Send()" />
    </div>
   <div id="PersonListArea" style="display:none;position:absolute; background-color:navy">
        <div id="ScrollArea" style="height:200px; overflow-y:scroll">
        <%foreach (var o in this.PersonList)
            { %>
            <%--<div id="PsnArea" class="PsnArea"><span id="NameSpan" style="font-size:10pt;color:white"><%=o.PsnName %>(<%=o.PsnNo %>)</span><input type="hidden" id="HiddenPsnNo" value="<%=o.PsnNo %>" /></div>--%>
        <%} %>
            </div>
    </div>
    
     <div id="popOverlay1" style="width: 100%; height: 100%; display: none; position: fixed; top: 0; left: 0; z-index: 29999; overflow: hidden; -webkit-transform: translate3d(0,0,0); background-color: #000; opacity: 0.5">
    </div>
    <div id="ParaExtDiv1" style="display: none; position: absolute; z-index: 30000; background-color: #1275BC; border-style: solid; border-width: 2px; border-color: #069">
        <table>
                <tr>
                    <td>
                        <fieldset id="DeviceField" runat="server" style="width: 285px;">
                            <legend id="DeviceList_Legend" runat="server">寄件信箱設定</legend>
                            <table class="TableS3" style="width: 410px">
                                <tr>
                                    <th style="text-align:left" scope="col">SMTP位置</th>                                    
                                </tr>
                                <tr>
                                    <td>
                                        <input type="text" id="Smtp" name="Smtp" value="<%=this.SmtpName %>" style="width:95%" />
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align:left" scope="col">登入帳號</th>                                    
                                </tr>
                                <tr>
                                    <td>
                                        <input type="text" id="SmtpAccount" name="SmtpAccount" value="<%=this.SmtpAccount %>" style="width:95%" />
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align:left" scope="col">密碼</th>                                    
                                </tr>
                                <tr>
                                    <td>
                                        <input type="password" autocomplete="off" id="SmtpPwd" name="SmtpPwd" value="<%=this.SmtpPwd %>" style="width:95%" />
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align:left" scope="col">寄件人</th>                                    
                                </tr>
                                <tr>
                                    <td>
                                        <input type="text" id="MailFrom" name="MailFrom" value="<%=this.MailFrom %>" style="width:95%" />
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align:left" scope="col"><input type="checkbox" id="UseSsl" name="UseSsl"  value="1"/> 加密連線
                                        <input type="hidden" id="IsSsl" name="IsSsl" value="<%=this.UseSsl %>" />
                                    </th>                                    
                                </tr>                                
                            </table>
                        </fieldset>
                    </td>
                </tr>                
                <tr>
                    <td>
                        <input type="button" id="BtnSave" value="儲存" class="IconSave" />
                        <input type="button" id="BtnAlive" value="離開" class="IconCancel"/>
                    </td>
                </tr>
            </table>
    </div>
</asp:Content>
