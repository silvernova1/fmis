#pragma checksum "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "651f6ea3226b0137fd6cfe4eb76bcf596cd80466"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Reports_DownloadSaob2), @"mvc.1.0.view", @"/Views/Reports/DownloadSaob2.cshtml")]
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
#line 1 "D:\fmis\fmis\Views\_ViewImports.cshtml"
using fmis;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\fmis\fmis\Views\_ViewImports.cshtml"
using fmis.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\fmis\fmis\Views\_ViewImports.cshtml"
using fmis.Models.UserModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\fmis\fmis\Views\_ViewImports.cshtml"
using System.Security.Claims;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\fmis\fmis\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"651f6ea3226b0137fd6cfe4eb76bcf596cd80466", @"/Views/Reports/DownloadSaob2.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Reports_DownloadSaob2 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "excel", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml"
  
    ViewData["Title"] = "SAOB";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n﻿");
#nullable restore
#line 5 "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml"
   
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"<div class=""submit-progress hidden"">
    <i class=""fa fa-2x fa-spinner fa-spin""></i>
    <label>Please wait while Downloading Data...</label>
</div>

<div class=""space space-10""></div>
<div class=""col-md-3"" style=""margin-left: 25px"">
    <div class=""widget-box"">
        <div class=""widget-header"">

            <h4 class=""widget-title""> <span class=""glyphicon glyphicon-list-alt""></span> Saob Reports</h4>

            <span class=""widget-toolbar"">
                <a href=""#"" data-action=""collapse"">
                    <i class=""ace-icon fa fa-chevron-up""></i>
                </a>
            </span>
        </div>
        <div class=""widget-body"">
            <div class=""widget-main"">
                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "651f6ea3226b0137fd6cfe4eb76bcf596cd804665492", async() => {
                WriteLiteral("\r\n                    ");
#nullable restore
#line 29 "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml"
               Write(Html.AntiForgeryToken());

#line default
#line hidden
#nullable disable
                WriteLiteral(@"
                    <label>Format: </label>
                    <div class=""row"">
                        <div class=""col-xs-12 col-sm-12"">
                            <select name=""format"" class=""form-control"" tabindex=""-1"" aria-disabled=""true"" disabled>
                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "651f6ea3226b0137fd6cfe4eb76bcf596cd804666289", async() => {
                    WriteLiteral("EXCEL");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_0.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"
                            </select>
                        </div>
                    </div>
                    <div class=""space space-8""></div>
                    <label>Date From: </label>
                    <div class=""row"">
                        <div class=""col-xs-12 col-sm-12"">
                            <div>
                                <input class=""form-control"" type=""date"" name=""date_from"" id=""dateFrom"" data-date-format=""dd-mm-yyyy"" required>
                            </div>
                        </div>
                    </div>
                    <div class=""space space-8""></div>
                    <label>Date To: </label>
                    <div class=""row"">
                        <div class=""col-xs-12 col-sm-12"">
                            <div>
                                <input class=""form-control"" type=""date"" name=""date_to"" id=""dateTo"" data-date-format=""dd-mm-yyyy"" required>
                            </div>
                        </div>
     ");
                WriteLiteral("               </div>\r\n                    <div class=\"space space-8\"></div>\r\n                    <div class=\"row\">\r\n                        <div");
                BeginWriteAttribute("class", " class=\"", 2438, "\"", 2446, 0);
                EndWriteAttribute();
                WriteLiteral(@">
                            <div class=""col-xs-12 col-sm-12"">
                                <button type=""submit"" onclick=""return DisplayProgressMessage(this, 'Downloading...');"" class=""btn btn-success btn-sm pull-right""> <span class=""glyphicon glyphicon-download-alt""></span> Download</button>
                            </div>
                        </div>
                    </div>
                ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "action", 1, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
#nullable restore
#line 28 "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml"
AddHtmlAttributeValue("", 839, Url.Action("DownloadExcel","Reports"), 839, 38, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n\r\n");
            WriteLiteral(@"

<style>
    .submit-progress-bg {
        background-color: lightgray;
        opacity: .5;
    }

    .submit-progress {
        position: fixed;
        top: 50%;
        left: 50%;
        height: 6em;
        padding-top: 2.3em;
        z-index: 1;
        /* The following properties are the ones most likely to change */
        width: 20em;
        /* Set 'margin-left' to a negative number that is 1/2 of 'width' */
        margin-left: -10em;
        padding-left: 2.1em;
        background-color: black;
        color: white;
        -webkit-border-radius: 0.4em;
        -moz-border-radius: 0.4em;
        border-radius: 0.4em;
        box-shadow: 0.4em 0.4em rgba(0,0,0,0.6);
        -webkit-box-shadow: 0.4em 0.4em rgba(0,0,0,0.6);
        -moz-box-shadow: 0.4em 0.4em rgba(0,0,0,0.6);
    }

    /* Changing style for spinner */
    .submit-progress {
        padding-top: 2em;
        width: 23em;
        margin-left: -11.5em; /* Set to a negative number that is 1/2 of th");
            WriteLiteral("e width */\r\n    }\r\n\r\n        .submit-progress i {\r\n            margin-right: 0.5em;\r\n        }\r\n\r\n    .hidden {\r\n        display: none;\r\n    }\r\n</style>\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 128 "D:\fmis\fmis\Views\Reports\DownloadSaob2.cshtml"
      
        await Html.RenderPartialAsync("_ValidationScriptsPartial");
    

#line default
#line hidden
#nullable disable
                WriteLiteral(@"    <script type=""text/javascript"">
        function DisplayProgressMessage(ctl, msg) {
            //if use the submit button, you can use event.preventDefault to prevent the default submit form behavior.
            event.preventDefault();
            // Turn off the ""Save"" button and change text
            $(ctl).prop(""disabled"", true).val(msg);
            // Gray out background on page
            $(""body"").addClass(""submit-progress-bg"");
            // Wrap in setTimeout so the UI can update the spinners
            $("".submit-progress"").removeClass(""hidden"");
            //submit the form.
            setTimeout(function () {
                $(""form"").submit();
            }, 1);
            return true;

        }
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
