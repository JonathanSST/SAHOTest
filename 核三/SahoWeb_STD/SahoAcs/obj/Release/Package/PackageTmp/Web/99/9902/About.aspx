<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master" CodeBehind="About.aspx.cs"
    Inherits="SahoAcs.About" Debug="true" EnableEventValidation="false" Theme="UI" %>
<%@ Import Namespace="SahoAcs.DBClass" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: center;">
        <div style="color: white;font-size:20px">
            關於 ... 
        </div>
        <div style="color: white; font-size:20px">
            SMS 多元管理平台
        </div>
		<br/>
        <div>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Img/Baner1.png" Width="430px"/>
        </div>
		<br/>
        <div style="color: cornsilk; font-size:20px">
            版本：&nbsp;SMS_Web&nbsp;&nbsp; v2023.10.05 <%=this.GetSysVersion() %><br />
            資料庫原始版本：&nbsp;<%=this.DbVer %><br />
            資料庫更新版本：&nbsp;<%=this.DbUpdVer %><br />
            資料庫異動時間：&nbsp;<%=this.DbChgTime %><br />
            <%=string.Format("授權使用者人數：{0}；目前系統使用者人數：{1}",DongleVaries.GetMaxUser(),DongleVaries.GetCurrentUser()) %><br />
            <%=string.Format("授權控制器台數：{0}；目前控制器台數：{1}",DongleVaries.GetMaxCtrls(),DongleVaries.GetCurrentCtrl()) %><br />
            <%=string.Format("授權發卡數量：{0}；目前發卡數量：{1}",DongleVaries.GetMaxPerson(),DongleVaries.GetCurrentCard()) %><br />
            <%=string.Format("授權行動裝置數量：{0}；目前授權數量：{1}",DongleVaries.GetMaxMobile(),DongleVaries.GetCurrentMobile()) %><br />
            <%=WebAppService.GetKeyInfo() %>
        </div>
        <div style="color: cornsilk; font-size:20px">
            <i>Saho</i>&nbsp; 商合行 股份有限公司
        </div>
    </div>
</asp:Content>
