<%@ Page Title="" Language="C#" Theme="UI"  MasterPageFile="~/Site1.Master" AutoEventWireup="true" 
    CodeBehind="WorkDataExport.aspx.cs" Inherits="SahoAcs.Web._06._0674.WorkDataExport" %>
<%@ Register Src="../../../uc/CalendarFrm.ascx" TagName="CalendarFrm" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div id="ValueKeep">
        <asp:HiddenField ID="hideUserID" runat="server" EnableViewState="False" />
        <input type="hidden" id="Date" name="Date" value="" />     
    </div>
    <link rel="stylesheet" href="https://cdn.rawgit.com/openlayers/openlayers.github.io/master/en/v5.2.0/css/ol.css" type="text/css" />
     <style>
        fieldset {
            border: solid;
            border-radius: 10px;
            color: aliceblue;
        }
            fieldset legend {
                font-size: 20px;
                color: aliceblue;
            }
            #BtnImport{

            }
    </style>
     <table style="width:90% ">
        <tr>
            <td>
                <fieldset>
                    <legend>出勤資料匯出</legend>
                        <table class="Item" >
                            <tr>
                                <th colspan="3">
                                    <span class="Arrow01"></span>
                                    <asp:Label ID="Label1" runat="server" Text="匯出日期區間"></asp:Label>
                                </th>                                
                                <th colspan="3">
                            
                                </th>                                                                        
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <uc2:CalendarFrm ID="ExportDate" runat="server" />
                                </td>
                                <td>~</td>
                                <td>
                                    <uc2:CalendarFrm ID="ExportDate2" runat="server" />
                                </td>
                                <td>                                
                                    <input type="button" value="匯出資料" id="Btn" 
                                        style="font-size:small;color:aliceblue" name="BtnExport" 
                                        onclick="ExportData()"
                                        class="IconExport" />
                                    <input type="button" value="計算差勤" id="BtnCalc" 
                                        style="font-size:small;color:aliceblue" name="BtnCalc" class="IconSet" />
                                    <span id="SpanBusing" style="display:none;color:white;background-color:red">資料匯入中...</span>
                                    <span id="SpanCalc" style="display:none;color:white;background-color:red">資料計算中...</span>
                                </td>        
                            </tr>
                        </table>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
