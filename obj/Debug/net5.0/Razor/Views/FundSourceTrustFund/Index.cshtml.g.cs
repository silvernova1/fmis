#pragma checksum "D:\fmis\Views\FundSourceTrustFund\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6b47c4d83c40114cc404001e5dd88f6cb96e2913"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_FundSourceTrustFund_Index), @"mvc.1.0.view", @"/Views/FundSourceTrustFund/Index.cshtml")]
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
#line 1 "D:\fmis\Views\_ViewImports.cshtml"
using fmis;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\fmis\Views\_ViewImports.cshtml"
using fmis.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\fmis\Views\_ViewImports.cshtml"
using fmis.Models.UserModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\fmis\Views\_ViewImports.cshtml"
using System.Security.Claims;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\fmis\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
using System.Globalization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6b47c4d83c40114cc404001e5dd88f6cb96e2913", @"/Views/FundSourceTrustFund/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    public class Views_FundSourceTrustFund_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<fmis.Models.BudgetAllotmentTrustFund>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("button"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "FundSourceTrustFund", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Create", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-success btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString(" btn btn-primary btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "BudgetAllotmentTrustFund", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "FundsRealignmentTrustFund", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
  
    ViewData["Title"] = "FundSourceTrustFund";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 8 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
 if (Model.FundSourceTrustFunds.Count() > 0)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <h3 class=\"grey lighter smaller\">\r\n        Fund Source (Trust Fund)\r\n        <small>\r\n            <i class=\"ace-icon fa fa-angle-double-right\"></i>\r\n            ");
#nullable restore
#line 14 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
       Write(Model.Yearly_reference.YearlyReference);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </small>\r\n        <small>\r\n            <i class=\"ace-icon fa fa-angle-double-right\"></i>\r\n            ");
#nullable restore
#line 18 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
       Write(Model.FundSourceTrustFunds.FirstOrDefault().Appropriation.AppropriationSource);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </small>\r\n        <small>\r\n            <i class=\"ace-icon fa fa-angle-double-right\"></i>\r\n            ");
#nullable restore
#line 22 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
       Write(Model.FundSourceTrustFunds.FirstOrDefault().AllotmentClass.Allotment_Class);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </small>\r\n\r\n    </h3>\r\n");
#nullable restore
#line 26 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<hr />
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
#line 38 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Model.FundSourceTrustFunds.Sum(x => x.Beginning_balance).ToString("C", new CultureInfo("en-PH")));

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
        <div class=""infobox infobox-red"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-calculator""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" style=""font-size:11pt; color: grey;"">
                    ");
#nullable restore
#line 51 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Model.FundSourceTrustFunds.Sum(x => x.Remaining_balance).ToString("C", new CultureInfo("en-PH")));

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
        <div class=""infobox infobox-orange2"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-bar-chart-o""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" style=""font-size:11pt; color:grey;"">
                    ");
#nullable restore
#line 64 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Model.FundSourceTrustFunds.Sum(x => x.realignment_amount).ToString("C", new CultureInfo("en-PH")));

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
        <div class=""infobox infobox-blue"">
            <div class=""infobox-icon"">
                <i class=""ace-icon fa fa-bar-chart-o""></i>
            </div>
            <div class=""infobox-data"">
                <span class=""infobox-data-number"" style=""font-size:11pt; color:grey;"">
                    ");
#nullable restore
#line 77 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Model.FundSourceTrustFunds.Sum(x => x.obligated_amount).ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </span>
                <div class=""infobox-content"">
                    <span class=""label label-primary arrowed-in arrowed-in-right""> Obligated Amount </span>
                </div>
            </div>
        </div>
    </div>
</div>
<br />
<br />
<br />
<br />
<div class=""modal"" id=""createModal"" role=""dialog"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-body"" id=""fund_source_create_trust_fund"">

            </div>
        </div>
    </div>
</div>

<div class=""modal"" id=""editModal""  role=""dialog"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-body"" id=""fund_source_edit_trust_fund"">

            </div>
        </div>
    </div>
</div>

<div class=""modal fade"" id=""DeleteModalCenter"" tabindex=""-1"" role=""dialog"" aria-labelledby=""exampleModalCenterTitle"" aria-hidden=""true"">
    <div class=""modal-dialog modal-s");
            WriteLiteral("m\" role=\"document\">\r\n        <div class=\"modal-content\">\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n<p>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6b47c4d83c40114cc404001e5dd88f6cb96e291313041", async() => {
                WriteLiteral("\r\n        <i class=\"ace-icon fa fa-plus \"></i>Create New\r\n    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.Action = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "onclick", 3, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 4578, "createFundSourceTrustFund(", 4578, 26, true);
#nullable restore
#line 118 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
AddHtmlAttributeValue("", 4604, Model.BudgetAllotmentTrustFundId, 4604, 33, false);

#line default
#line hidden
#nullable disable
            AddHtmlAttributeValue("", 4637, ")", 4637, 1, true);
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(" |\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6b47c4d83c40114cc404001e5dd88f6cb96e291315272", async() => {
                WriteLiteral("\r\n        <span class=\"glyphicon glyphicon-arrow-left\"></span> Back to Budget Allotment\r\n    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</p>\r\n\r\n");
#nullable restore
#line 126 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
 if (Model.FundSourceTrustFunds.Count() > 0)
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <table class=""table"">
        <thead>
            <tr>
                <th>
                    Fund Source Code
                </th>
                <th>
                    Fund Source Title
                </th>
                <th>
                    Appropriation (Source)
                </th>
                <th>
                    Responsibility Center
                </th>
                <th>
                    Actions
                </th>
                <th>
                    Funds Source (Realignment)
                </th>
                <th>
                    Beginning Balance
                </th>
                <th>
                    Remaining Balance
                </th>
                <th>
                    Realignment Amount
                </th>
                <th>
                    Obligated Amount
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 164 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
             foreach (var item in Model.FundSourceTrustFunds)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td>\r\n                        ");
#nullable restore
#line 168 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                   Write(Html.DisplayFor(modelItem => item.FundSourceTrustFundTitleCode));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 171 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                   Write(Html.DisplayFor(modelItem => item.FundSourceTrustFundTitle));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 174 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                   Write(item.Appropriation.AppropriationSource);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");
#nullable restore
#line 177 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                   Write(item.RespoCenter.Respo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        <button class=\"btn btn-primary btn-xs\"");
            BeginWriteAttribute("onclick", " onclick=\"", 6624, "\"", 6723, 5);
            WriteAttributeValue("", 6634, "getEditFundSourceTrustFund(", 6634, 27, true);
#nullable restore
#line 180 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
WriteAttributeValue("", 6661, Model.BudgetAllotmentTrustFundId, 6661, 33, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6694, ",", 6694, 1, true);
#nullable restore
#line 180 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
WriteAttributeValue("", 6695, item.FundSourceTrustFundId, 6695, 27, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6722, ")", 6722, 1, true);
            EndWriteAttribute();
            WriteLiteral("> <span class=\"glyphicon glyphicon-edit\"></span></button>\r\n");
            WriteLiteral("                    </td>\r\n                    <td>\r\n                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6b47c4d83c40114cc404001e5dd88f6cb96e291320587", async() => {
                WriteLiteral("<span class=\"glyphicon glyphicon-transfer\"></span> Funds Realignment");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_7.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_7);
            if (__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues == null)
            {
                throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-fundsource_trustfund_id", "Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper", "RouteValues"));
            }
            BeginWriteTagHelperAttribute();
#nullable restore
#line 184 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                                                                                             WriteLiteral(item.FundSourceTrustFundId);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["fundsource_trustfund_id"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-fundsource_trustfund_id", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["fundsource_trustfund_id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            BeginWriteTagHelperAttribute();
#nullable restore
#line 184 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                                                                                                                                              WriteLiteral(ViewBag.budget_id);

#line default
#line hidden
#nullable disable
            __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["BudgetId"] = __tagHelperStringValueBuffer;
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-BudgetId", __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.RouteValues["BudgetId"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_6.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_8);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                    </td>\r\n\r\n                    <td>\r\n                        <b class=\"green\">\r\n                            <span>");
#nullable restore
#line 189 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                             Write(item.Beginning_balance.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                        </b>\r\n                    </td>\r\n                    <td>\r\n                        <b class=\"red\">\r\n                            <span>");
#nullable restore
#line 194 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                             Write(item.realignment_amount.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                        </b>\r\n                    </td>\r\n                    <td>\r\n                        <b class=\"orange\">\r\n                            <span>");
#nullable restore
#line 199 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                             Write(item.realignment_amount.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                        </b>\r\n                    </td>\r\n                    <td>\r\n                        <b class=\"blue\">\r\n                            <span>");
#nullable restore
#line 204 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                             Write(item.obligated_amount.ToString("C", new CultureInfo("en-PH")));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                        </b>\r\n                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 208 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 211 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
}
else
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <div class=\"alert alert-block alert-danger\">\r\n        <i class=\"ace-icon fa fa-warning red\"></i>\r\n        No Records found\r\n    </div>\r\n");
#nullable restore
#line 218 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
}

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<script>


     function createFundSourceTrustFund(BudgetAllotmentTrustFundId) {
         $(""#createModal"").modal({ backdrop: 'static', keyboard: false });
         $(""#fund_source_create_trust_fund"").html(loading);
         var AllotmentClassId = ");
#nullable restore
#line 226 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                           Write(ViewBag.AllotmentClassId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var AppropriationId = ");
#nullable restore
#line 227 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                          Write(ViewBag.AppropriationId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var url = \'");
#nullable restore
#line 228 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Url.Action("Create", "FundSourceTrustFund"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
         url += ""?AllotmentClassId="" + AllotmentClassId + ""&AppropriationId="" + AppropriationId + ""&BudgetAllotmentTrustFundId="" + BudgetAllotmentTrustFundId;
         $.get(url, function (result) {
            setTimeout(function () {
                $(""#fund_source_create_trust_fund"").html(result);
            }, 500);
         });
     }

     function getEditFundSourceTrustFund(BudgetAllotmentTrustFundId, fund_source_id_trust_fund) {
         $(""#editModal"").modal({ backdrop: 'static', keyboard: false });
         $(""#fund_source_edit_trust_fund"").html(loading);
         var AllotmentClassId = ");
#nullable restore
#line 240 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                           Write(ViewBag.AllotmentClassId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var AppropriationId = ");
#nullable restore
#line 241 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
                          Write(ViewBag.AppropriationId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var url = \'");
#nullable restore
#line 242 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Url.Action("Edit", "FundSourceTrustFund"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
         //url += ""?budget_id="" + BudgetAllotmentTrustFundId + ""&fund_source_trust_fund_id="" + fund_source_id_trust_fund
         url += ""?AllotmentClassId="" + AllotmentClassId + ""&AppropriationId="" + AppropriationId + ""&BudgetAllotmentTrustFundId="" + BudgetAllotmentTrustFundId + ""&fund_source_id_trust_fund="" + fund_source_id_trust_fund;
         $.get(url, function (result) {
            setTimeout(function () {
                $(""#fund_source_edit_trust_fund"").html(result);
            }, 500);
         });
     }

    function deleteFundSourceTrustFund(BudgetAllotmentTrustFundId) {
         $(""#deleteModal"").modal({ backdrop: 'static', keyboard: false });
         $(""#fund_source_delete_trust_fund"").html(loading);
         var url = '");
#nullable restore
#line 255 "D:\fmis\Views\FundSourceTrustFund\Index.cshtml"
               Write(Url.Action("Delete", "FundSourceTrustFund"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
        url += ""?budget_id="" + BudgetAllotmentTrustFundId
         $.get(url, function (result) {
            setTimeout(function () {
                $(""#fund_source_delete_trust_fund"").html(result);
            }, 500);
         });
     }

    let generate_token = () => {
        let s4 = () => {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    function fundSourceToken() {
        fundsource_token = generate_token();
        $(""#funds_input_token"").val(fundsource_token);
    }
</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<fmis.Models.BudgetAllotmentTrustFund> Html { get; private set; }
    }
}
#pragma warning restore 1591