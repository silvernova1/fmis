#pragma checksum "D:\fmis\Views\Carlo\Temp\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "19ca92b99788c0def5058bafb2fafe33d9da3d9f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Carlo_Temp_Index), @"mvc.1.0.view", @"/Views/Carlo/Temp/Index.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"19ca92b99788c0def5058bafb2fafe33d9da3d9f", @"/Views/Carlo/Temp/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    public class Views_Carlo_Temp_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<style>\r\n");
            WriteLiteral(@"
</style>

<h3 class=""grey lighter smaller"">
    TEMP HANDSONTABLE
</h3>
<hr>

<div class=""modal"" id=""createModal"" role=""dialog"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-body"" id=""temp_create"">

            </div>
        </div>
    </div>
</div>
<div class=""modal"" id=""editModal"" role=""dialog"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-body"" id=""temp_edit"">

            </div>
        </div>
    </div>
</div>





<script>
       function createTemp(BudgetAllotmentId) {
         $(""#createModal"").modal({ backdrop: 'static', keyboard: false });
         $(""#temp_create"").html(loading);

         var AllotmentClassId = ");
#nullable restore
#line 41 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
                           Write(ViewBag.AllotmentClassId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n           var AppropriationId = ");
#nullable restore
#line 42 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
                            Write(ViewBag.AppropriationId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n\r\n              \r\n         var url = \'");
#nullable restore
#line 45 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
               Write(Url.Action("Create", "Temp"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
         url += ""?AllotmentClassId=""+AllotmentClassId+""&AppropriationId=""+AppropriationId+""&BudgetAllotmentId="" + BudgetAllotmentId;
         $.get(url, function (result) {
            setTimeout(function () {
                $(""#temp_create"").html(result);
            }, 500);
         });
     }

     function editTemp(BudgetAllotmentId, fund_source_id) {
         $(""#editModal"").modal({ backdrop: 'static', keyboard: false });
         $(""#temp_edit"").html(loading);
         var AllotmentClassId = ");
#nullable restore
#line 57 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
                           Write(ViewBag.AllotmentClassId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var AppropriationId = ");
#nullable restore
#line 58 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
                          Write(ViewBag.AppropriationId);

#line default
#line hidden
#nullable disable
            WriteLiteral(";\r\n         var url = \'");
#nullable restore
#line 59 "D:\fmis\Views\Carlo\Temp\Index.cshtml"
               Write(Url.Action("Edit", "Temp"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"';
         //url += ""?budget_id="" + BudgetAllotmentId + ""&fund_source_id="" + fund_source_id
         url += ""?AllotmentClassId="" + AllotmentClassId + ""&AppropriationId="" + AppropriationId + ""&BudgetAllotmentId="" + BudgetAllotmentId + ""&fund_source_id="" + fund_source_id;
         $.get(url, function (result) {
            setTimeout(function () {
                $(""#temp_edit"").html(result);
            }, 500);
         });
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
