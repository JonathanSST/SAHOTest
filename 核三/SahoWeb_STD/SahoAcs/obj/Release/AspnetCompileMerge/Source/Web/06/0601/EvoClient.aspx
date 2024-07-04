<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EvoClient.aspx.cs" Theme="UI" Inherits="SahoAcs.EvoClient" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="/uc/Calendar.ascx" TagName="Calendar" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>
    <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <asp:HiddenField ID="SelectValue" runat="server" EnableViewState="False" />
    </div>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="tablePanel" runat="server" Width="1200" Height="500">
                <table style="width: 100%; height:30px;" border="0">
                    <tr style="height:30px;width:100%;color:white;">
                        <td style="width:8%;text-align:right;">
                            讀卡時間
                        </td>
                        <td style="width:10%;">
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeSDate" />
                        </td>
                        <td style="text-align:center; width:2%">至</td>
                        <td style="width:10%;">
                            <uc1:Calendar runat="server" ID="Calendar_CardTimeEDate" />
                        </td>
                        <td style="width:20%;">
                            <asp:Button ID="Button3" runat="server" Text="Search" CssClass="IconSearch" OnClick="Button1_Click" />
                        </td>
                        <td style="width:50%;"></td>
                    </tr>
                </table>
                <table style="width: 100%; height:30px;" border="0">
                    <tr style="height:30px;width:100%;color:white;">
                        <td colspan="4">
                             <asp:DropDownList ID="ddl" runat="server" style="width:100%;"></asp:DropDownList>
                        </td>   
                        <td style="width:45%;">
                            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="播放" />
                        </td>
                    </tr>
                </table>
                <table style="width: 100%; height:450px;" border="0">
                    <tr style="height:350px;">
                        <td colspan="5">
                            <div class="slideshow-container">
                                <div class="mySlides">
                                    <asp:Image ID="Image1" runat="server" style="width:auto; height:330px; border:none; border-radius:15px;" />  
                                </div>

                                <div class="mySlides">
                                    <video id="video1" runat="server" style="vertical-align:top; width:auto; height:330px; border:1px solid silver; border-radius:15px;" controls>
                                        <source id="vs1" type="video/mp4" />
                                    </video>                                    
                                </div>
                                <a class="prev" onclick="plusSlides(-1)">❮</a>
                                <a class="next" onclick="plusSlides(1)">❯</a>
                            </div>
                            <div class="dot-container">
                                <span class="dot" onclick="currentSlide(1)"></span>
                                <span class="dot" onclick="currentSlide(2)"></span>
                            </div>
                        </td>                    
                    </tr>    
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
