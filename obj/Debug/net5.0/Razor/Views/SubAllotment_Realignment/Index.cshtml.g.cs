#pragma checksum "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b658f9dd5a7c386f7ca76bb93d703ea4218d0ddb"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_SubAllotment_Realignment_Index), @"mvc.1.0.view", @"/Views/SubAllotment_Realignment/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\_ViewImports.cshtml"
using fmis;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\_ViewImports.cshtml"
using fmis.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\_ViewImports.cshtml"
using fmis.Models.UserModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\_ViewImports.cshtml"
using System.Security.Claims;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
using System.Text.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
using System.Globalization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b658f9dd5a7c386f7ca76bb93d703ea4218d0ddb", @"/Views/SubAllotment_Realignment/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_SubAllotment_Realignment_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<fmis.Models.SubAllotment>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary btn-sm "), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "SubAllotment", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 5 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
  
    ViewData["Title"] = "Index";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral("\r\n<style>\r\n    .wtHider {\r\n        margin-bottom: 150px;\r\n    }\r\n</style>\r\n\r\n<input type=\"hidden\" class=\"uacs_from\"");
            BeginWriteAttribute("value", " value=\"", 491, "\"", 603, 1);
#nullable restore
#line 23 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
WriteAttributeValue("", 499, JsonSerializer.Serialize(Model.SubAllotmentAmounts.Where(x=>x.SubAllotmentId == @Model.SubAllotmentId)), 499, 104, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n<input type=\"hidden\" class=\"sub_allotment_realignment\"");
            BeginWriteAttribute("value", " value=\"", 661, "\"", 777, 1);
#nullable restore
#line 24 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
WriteAttributeValue("", 669, JsonSerializer.Serialize(Model.SubAllotmentRealignment.Where(x=>x.SubAllotmentId == @Model.SubAllotmentId)), 669, 108, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n<input type=\"hidden\" class=\"uacs\"");
            BeginWriteAttribute("value", " value=\"", 814, "\"", 906, 1);
#nullable restore
#line 25 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
WriteAttributeValue("", 822, JsonSerializer.Serialize(Model.Uacs.Where(x=>x.uacs_type == @Model.SubAllotmentId)), 822, 84, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n<input type=\"hidden\" id=\"sub_allotment_id\"");
            BeginWriteAttribute("value", " value=\"", 952, "\"", 981, 1);
#nullable restore
#line 26 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
WriteAttributeValue("", 960, Model.SubAllotmentId, 960, 21, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" />\r\n\r\n<h3 class=\"grey lighter smaller\">\r\n    SAA Realignment\r\n    <small>\r\n        <i class=\"ace-icon fa fa-angle-double-right\"></i>\r\n        ");
#nullable restore
#line 32 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
   Write(Model.Suballotment_title);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    </small>\r\n</h3>\r\n<hr />\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b658f9dd5a7c386f7ca76bb93d703ea4218d0ddb7753", async() => {
                WriteLiteral("<span class=\"glyphicon glyphicon-arrow-left\"></span> Back to SAA");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-AllotmentClassId", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 36 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                                                                                    WriteLiteral(Model.AllotmentClassId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["AllotmentClassId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-AllotmentClassId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["AllotmentClassId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 36 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                                                                                                                                        WriteLiteral(Model.AppropriationId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["AppropriationId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-AppropriationId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["AppropriationId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 36 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                                                                                                                                                                                             WriteLiteral(Model.BudgetAllotmentId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["BudgetAllotmentId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-BudgetAllotmentId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["BudgetAllotmentId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
<br />
<br />
<div class=""row  pull-left"">
    <div class=""space-5""></div>

    <div class=""col-xs-12 infobox-container"">
        <div class=""infobox infobox-green"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-money""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" style=""font-size:11pt; color:grey;"">
                    ");
#nullable restore
#line 49 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
               Write(Model.Beginning_balance.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </span>
                <div class=""infobox-content"">
                    <span class=""label label-success arrowed-in arrowed-in-right""> Beginning Balance </span>
                </div>
            </div>
        </div>
        <div class=""infobox infobox-orange2"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-bar-chart-o""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" id=""total_realignment_amount"" style=""font-size:11pt; color:grey;"">
                    ");
#nullable restore
#line 62 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
               Write(Model.realignment_amount.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </span>
                <div class=""infobox-content"">
                    <span class=""label label-warning arrowed-in arrowed-in-right""> Realignment Amount </span>
                </div>
            </div>
        </div>
        <div class=""infobox infobox-red"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-calculator""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" id=""total_remaining_balance"" style=""font-size:11pt; color: grey;"">
                    ");
#nullable restore
#line 75 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
               Write(Model.Remaining_balance.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </span>
                <div class=""infobox-content"">
                    <span class=""label label-danger arrowed-in arrowed-in-right""> Remaining Balance </span>
                </div>
            </div>
        </div>
    </div>
</div>
<br>
<br>
<br>
<br>
<br />
<div id=""example"" class=""hot""></div>
<br />
<script>
    var sub_allotment_id = $(""#sub_allotment_id"").val();

    var CHECK_AFTER_ROW = false;
    var CHECK_FIRST_ROW = false;
    var TRAP_THE_LOAD = false;
    var TRAP_THE_LOAD1 = false;
    var AMOUNT = 0;

    let guid = () => {
        let s4 = () => {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    function ajaxServerRealignment(ajax_data, url) {
        $.ajax({
            type: 'POST',
            url: url,
            headers: { ""RequestVerificationTo");
            WriteLiteral("ken\": \'");
#nullable restore
#line 113 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                               Write(GetAntiXsrfRequestToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' },
            data: { data: ajax_data },
            success: function (output) {
                console.log(output);
            }
        });
    }

    //FUNDS REALIGNMENT FROM
    var uacs = $.parseJSON($("".uacs_from"").val());
    var uacs_data_dropdown = [];
    var uacs_data_array = [];
    var get_account_title = [];
    var uacs_counter = 0;
    $.each(uacs, function () {
        uacs_data_dropdown[uacs_counter] = this.Uacs.Account_title;
        var json_data = new Object();
        json_data.SubAllotmentAmountId = this.SubAllotmentAmountId;
        json_data.suballotment_amount_token = this.suballotment_amount_token;
        uacs_data_array[this.Uacs.Account_title] = json_data;
        get_account_title[this.SubAllotmentAmountId] = this.Uacs.Account_title;
        uacs_counter++;
    });

    //FUNDS REALIGNEMNT TO
    var uacs = $.parseJSON($("".uacs"").val());
    var uacs_data_dropdown_2 = [];
    var uacs_data_array_2 = [];
    var get_account_title_2 = [];
    var");
            WriteLiteral(@" uacs_counter_2 = 0;
    $.each(uacs, function () {
        uacs_data_dropdown_2[uacs_counter_2] = this.Account_title;
        var json_data_2 = new Object();
        json_data_2.UacsId = this.UacsId;
        uacs_data_array_2[this.Account_title] = json_data_2;
        get_account_title_2[this.UacsId] = this.Account_title;
        uacs_counter_2++;
    });

    var sub_allotment_amount = 0;
    var sub_allotment_realignment = $.parseJSON($("".sub_allotment_realignment"").val());
    const data = [];
    var counter = 0;
    $.each(sub_allotment_realignment, function () {
        data[counter] = [
            get_account_title[this.SubAllotmentAmountId],
            this.SubAllotmentAmount.remaining_balance,
            get_account_title_2[this.Realignment_to],
            this.Realignment_amount,
            this.SubAllotmentId,
            this.token]
        counter++;
        sub_allotment_amount += this.Realignment_amount;
    })
    $("".sub_allotment_amount"").html(sub_allotment_amo");
            WriteLiteral(@"unt);

    const searchField = document.querySelector('#search_field2');
    const button = document.querySelector('#export-file');
    const container = document.getElementById('example');
    const hot = new Handsontable(container, {
        data: data,
        colWidths: [300, 150, 300, 150],
        rowHeaders: true,
        colHeaders: ['REALIGNMENT FROM','REMAINING BALANCE','REALIGNMENT TO', 'AMOUNT', 'ID', 'TOKEN'],
        columns: [
            {
                //REALIGNMENT FROM
                type: 'dropdown',
                source: uacs_data_dropdown
            },
            {
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                    culture: 'en-Ph'
                },
                readOnly: true
            },
            {
                //REALIGNMENT TO
                type: 'dropdown',
                source: uacs_data_dropdown_2
            },
            {
                type: 'numeric'");
            WriteLiteral(@",
                numericFormat: {
                    pattern: '0,0.00',
                    culture: 'en-Ph'
                },
            },
            {
                //HIDDEN ID
            },
            {
                //HIDDEN TOKEN
            },
        ],
        undo: true,
        search: {
            searchResultClass: 'search-result-custom'
        },
        contextMenu: ['row_above', 'row_below', 'remove_row'],
        beforeRemoveRow: function (index, column) {
            var selection = this.getSelected();
			var holder_data = this.getData();
            var single_token = holder_data[selection[0][0]][5];
			var first_column = selection[0][0];
			var last_column = selection[0][2];
			var many_token = [];
			for (var j = first_column; j <= last_column; j++) {
				many_token.push({
                    many_token: holder_data[j][5]
				});
			}

            var ajax_data = {
                ""sub_allotment_id"": sub_allotment_id,
				""single_token"": singl");
            WriteLiteral("e_token,\r\n\t\t\t\t\"many_token\": many_token\r\n            };\r\n\r\n            $.ajax({\r\n                type: \'POST\',\r\n                url: \'");
#nullable restore
#line 236 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                 Write(Url.Action("DeleteSubAllotmentRealignment", "SubAllotment_Realignment"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\',\r\n                headers: { \"RequestVerificationToken\": \'");
#nullable restore
#line 237 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                                   Write(GetAntiXsrfRequestToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' },
                data: { data: ajax_data },
                success: function (result) {
                    displayTotalCalculation(result);
                }
            });
		},
        afterUndo: function (index, column) {

        },
        afterCreateRow: function (row, column) {
            CHECK_FIRST_ROW = true;
            console.log(row);
            console.log(column);
            hot.setDataAtCell(row, 5, guid());
        },
        afterChange: function (changes, source) {
            if (!changes || source === 'loadData' || CHECK_AFTER_ROW) {
                CHECK_AFTER_ROW = false;
                return;
            }
            changes.forEach(([row, col, oldValue, newValue]) => {
                if (col == 0) { //when choosing a dropdown in fundrealignment from
                    console.log(""check the col 0"");
                    var suballotment_amount_token = uacs_data_array[hot.getDataAtCell(row, 0)].suballotment_amount_token;
                    $.get(");
            WriteLiteral("\'");
#nullable restore
#line 262 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                      Write(Url.Action("subAllotmentAmountRemainingBalance", "SubAllotment_Realignment"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + ""?suballotment_amount_token="" + suballotment_amount_token, function (suballotment_amount_remaining_balance) {
                        hot.setDataAtCell(row, 1, suballotment_amount_remaining_balance);
                        TRAP_THE_LOAD = true; //prevent to load 2 times
                    });
                }
                else if (col == 2) {
                    console.log(""check the col 2"");
                    TRAP_THE_LOAD = false; //allow to save the col 2 in else
                }
                else if (col == 3) { //when inputting the realigmnent amount
                    CHECK_FIRST_ROW = true;
                    if (TRAP_THE_LOAD1) {
                        TRAP_THE_LOAD1 = false;
                        console.log(""trap_the_load1"");
                        return;
                    }
                    else if (!hot.getDataAtCell(row, 0)) //trap if the uacs_from already selected
                        trapTheLoad(row, col, oldValue, ""Please Select the 'REALIGNMENT");
            WriteLiteral(@" FROM'!"");
                    else if (!hot.getDataAtCell(row, 2)) { //trap if the uacs_to already selected
                        trapTheLoad(row, col, oldValue, ""Please Select the 'REALIGNMENT TO'!"");
                    }
                    else {
                        var suballotment_amount_token = uacs_data_array[hot.getDataAtCell(row, 0)].suballotment_amount_token;
                        $.get('");
#nullable restore
#line 285 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                          Write(Url.Action("realignmentRemaining", "SubAllotment_Realignment"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' + ""?sub_allotment_id="" + sub_allotment_id + ""&suballotment_amount_token="" + suballotment_amount_token, function (result) {
                            displayTotalCalculation(result);
                            var post_remaining_balance = result.remaining_balance;
                            var post_realignment_amount = result.realignment_amount;
                            var post_suballotment_amount_remaining = result.subAllotmentAmounts[0].remaining_balance;
                            var post_suballotment_amount_realignment = result.subAllotmentAmounts[0].realignment_amount;

                            if (oldValue) { //new : 9999, OLD :1 , REMAINING : 9999
                                post_remaining_balance = post_remaining_balance + oldValue; //2000
                                post_realignment_amount = post_realignment_amount - oldValue;
                                post_suballotment_amount_remaining = post_suballotment_amount_remaining + oldValue;
                          ");
            WriteLiteral(@"      post_suballotment_amount_realignment = post_suballotment_amount_realignment - oldValue;
                            }
                            AMOUNT = newValue;

                            if (AMOUNT > post_suballotment_amount_remaining) {
                                trapTheCell(row, col, oldValue); //reset the realignment amount from the OLDVALUE,then dapat kani ang una mo display para makuha ang exact amount
                                trapTheLoad(row, 1, result.subAllotmentAmounts[0].remaining_balance, ""Insufficient SubAllotmentAmount Remaining Balance"");
                            }
                            else if (AMOUNT > post_remaining_balance)
                                trapTheLoad(row, col, oldValue, ""Insufficient SubAllotment Remaining Balance!"");
                            else {
                                //do the calculation
                                console.log(""do the calculation!"");
                                post_remaining_balance = p");
            WriteLiteral(@"ost_remaining_balance - AMOUNT;
                                post_realignment_amount = post_realignment_amount + AMOUNT;
                                post_suballotment_amount_remaining = post_suballotment_amount_remaining - AMOUNT;
                                post_suballotment_amount_realignment = post_suballotment_amount_realignment + AMOUNT;


                                var calculated_amount_data = {
                                    ""sub_allotment_id"": parseInt(sub_allotment_id),
                                    ""remaining_balance"": post_remaining_balance,
                                    ""realignment_amount"": post_realignment_amount,
                                    ""suballotment_amount_remaining_balance"": post_suballotment_amount_remaining,
                                    ""suballotment_amount_realignment"": post_suballotment_amount_realignment,
                                    ""realignment_token"": hot.getDataAtCell(row, 5),
                                    ");
            WriteLiteral(@"""amount"": AMOUNT,
                                    ""suballotment_amount_token"": suballotment_amount_token
                                };
                                //console.log(calculated_amount_data);
                                var url = '");
#nullable restore
#line 326 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                      Write(Url.Action("realignmentAmountSave", "SubAllotment_Realignment"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"'
                                $.post(url, calculated_amount_data, function (calculation) {
                                    hot.setDataAtCell(row, 1, post_suballotment_amount_remaining);
                                    TRAP_THE_LOAD = true; //prevent to load 2 times
                                    console.log(calculation);
                                    displayTotalCalculation(calculation);
                                });
                            }
                        });
                    }
                }
            });

            if (CHECK_FIRST_ROW) {
                CHECK_FIRST_ROW = false; //TRAP THE DOUBLE LOAD IN FIRST ROW
                console.log(""check first row!"");
            }
            else if (TRAP_THE_LOAD) {
                console.log(""TRAP_THE_LOAD"");
                TRAP_THE_LOAD = false;
            }
            else {
                console.log(""else"");
                var ajax_data = [];
                $.each(hot.getS");
            WriteLiteral(@"ourceData(), function () {
                    ajax_data.push({
                        Realignment_from: this[0] ? uacs_data_array[this[0]].SubAllotmentAmountId : """",
                        Realignment_to: this[2] ? uacs_data_array_2[this[2]].UacsId : """",
                        Realignment_amount: this[3],
                        SubAllotmentId: sub_allotment_id,
                        token: this[5]
                    });
                });
                console.log(ajax_data);
                ajaxServerRealignment(ajax_data, '");
#nullable restore
#line 360 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
                                             Write(Url.Action("SaveSubAllotmentRealignment", "SubAllotment_Realignment"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
            }
		},
        height: 'auto',
        hiddenColumns: {
            columns: [4,5],
            indicators: true
        },
        licenseKey: 'non-commercial-and-evaluation', // for non-commercial use only
    });
    var tableData = JSON.stringify(hot.getSourceData());

    function trapTheCell(row, col, oldValue) {
        TRAP_THE_LOAD1 = true; //trap so that can't save in DB
        TRAP_THE_LOAD = true; //trap so that can't save in DB
        hot.setDataAtCell(row, col, oldValue);
    }

    function trapTheLoad(row, col, oldValue, trap_message) {
        Lobibox.alert(""error"", {
            msg: trap_message,
            sound: false,
        });
        TRAP_THE_LOAD1 = true; //trap so that can't save in DB
        TRAP_THE_LOAD = true; //trap so that can't save in DB
        trapTheCell(row, col, oldValue)
        return;
    }

    function displayTotalCalculation(calculation) {
        $(""#total_remaining_balance"").html(parseFloat(calculation.remaining");
            WriteLiteral("_balance).toFixed(2));\r\n        $(\"#total_realignment_amount\").html(parseFloat(calculation.realignment_amount).toFixed(2));\r\n    }\r\n</script>\r\n\r\n");
        }
        #pragma warning restore 1998
#nullable restore
#line 10 "C:\Users\Jondy Gonzalez\Desktop\fmis\Views\SubAllotment_Realignment\Index.cshtml"
           
    public string GetAntiXsrfRequestToken()
    {
        return Xsrf.GetAndStoreTokens(Context).RequestToken;
    }

#line default
#line hidden
#nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<fmis.Models.SubAllotment> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
