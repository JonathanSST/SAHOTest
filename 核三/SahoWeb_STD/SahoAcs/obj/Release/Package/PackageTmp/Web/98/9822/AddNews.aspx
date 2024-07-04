<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNews.aspx.cs" Inherits="SahoAcs.Web.AddNews" %>

<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>
<%@ Register Src="/uc/CalendarFrm.ascx" TagName="NewCalendar" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div>
            <table class="popItem">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <th>
                                    <span class="Arrow01"></span>
                                    <span>公告訊息主旨</span>
                                </th>
                                <th>
                                    <span class="Arrow01"></span>
                                    <span>公告日期</span>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <input type="text" name="NewsTitle" id="NewsTitle" value="<%=news.NewsTitle %>" style="width: 230px" />
                                    <input type="hidden" name="NewsID" id="NewsID" value="<%=this.news.NewsID %>" />
                                </td>
                                <td>
                                     <uc2:NewCalendar runat="server" ID="NewsDate" />
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
                                    <asp:Label ID="popLabel_LogStatus" runat="server" Text="公告內容" Font-Bold="true"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <td>
                                    <%--<input type="text" name="NewsContent" id="NewsContent" value="<%=this.news.NewsContent %>" style="width: 290px" />--%>
                                    <textarea style="width:290px;height:300px" name="NewsContent" id="NewsContent"><%=this.news.NewsContent %></textarea>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input type="button" name="BtnSave" value="<%=Resources.Resource.btnSave %>" id="BtnSave" class="IconSave" />
                        <input type="button" name="BtnCancel" value="<%=Resources.Resource.btnCancel %>" id="BtnCancel" class="IconCancel" />
                    </td>
                </tr>
            </table>
        </div>

    </form>
</body>
</html>
