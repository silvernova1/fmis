#pragma checksum "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f0e22eaf509f40274d2c3f27fc16dc815b689ee6"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Yearly_reference_Index), @"mvc.1.0.view", @"/Views/Yearly_reference/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f0e22eaf509f40274d2c3f27fc16dc815b689ee6", @"/Views/Yearly_reference/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Yearly_reference_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<fmis.Models.Yearly_reference>>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-sm btn-success"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Create", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml"
  
    ViewBag.Title = "YearlyReference";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<h3 class=\"grey lighter smaller\">\r\n    Budget Year Reference\r\n</h3>\r\n<hr>\r\n<p>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "f0e22eaf509f40274d2c3f27fc16dc815b689ee64494", async() => {
                WriteLiteral("\r\n        <i class=\"ace-icon fa fa-plus\"></i>Create New\r\n    ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
</p>
<br />
<table class=""table table-striped"" style=""width: 900px"">
    <thead>
        <tr>

            <th>
               YEAR REFERENCE
            </th>
            <th>
               ACTION
            </th>
        </tr>
    </thead>
    <tbody>
");
#nullable restore
#line 31 "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml"
         foreach (var item in Model)
         {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n\r\n                <td>\r\n                    ");
#nullable restore
#line 36 "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml"
               Write(Html.DisplayFor(modelItem => item.YearlyReference));

#line default
#line hidden
#nullable disable
            WriteLiteral(" Budget\r\n                </td>\r\n                <td>\r\n                    <a class=\"btn btn-xs btn-danger\"");
            BeginWriteAttribute("href", " href=\"", 849, "\"", 933, 1);
#nullable restore
#line 39 "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml"
WriteAttributeValue("", 856, Url.Action("Delete","Yearly_reference",new { id = @item.YearlyReferenceId }), 856, 77, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@"
                           onclick="" if (confirm('Are you sure you want to delete this into the database?')) {
                                          // Save it!
                                         Lobibox.notify('error', {
                                               title: 'Deleted',
                                                msg: 'Year Successfully Deleted!',
                                        });
                                        }
                                            else
                                        {
                                          return false
                                        };"">
                        <i class=""ace-icon fa fa-trash""></i>Delete
                    </a>
                </td>
            </tr>
");
#nullable restore
#line 55 "D:\fmis\fmis\Views\Yearly_reference\Index.cshtml"
         }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<fmis.Models.Yearly_reference>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
