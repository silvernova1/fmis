#pragma checksum "D:\fmis\Views\Budget\Obligation.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "64e861687d9d911958092beba394559b135f7e76"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Budget_Obligation), @"mvc.1.0.view", @"/Views/Budget/Obligation.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"64e861687d9d911958092beba394559b135f7e76", @"/Views/Budget/Obligation.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    public class Views_Budget_Obligation : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/assets/images/loading.gif"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("style", new global::Microsoft.AspNetCore.Html.HtmlString("margin-left:20%;margin-top:30%;"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("msg-photo"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("alt", new global::Microsoft.AspNetCore.Html.HtmlString("Alex\'s Avatar"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral(@"<table class=""table table-hover table-responsive"">
    <thead>
        <tr>
            <th scope=""col"">#</th>
            <th scope=""col"">Date</th>
            <th scope=""col"">Dv</th>
            <th scope=""col"">Po No</th>
            <th scope=""col"">Pr No</th>
            <th scope=""col"">Payee</th>
            <th scope=""col"">Address</th>
            <th scope=""col"">Particulars</th>
            <th scope=""col"">Ors No</th>
            <th scope=""col"">Fund Source</th>
            <th scope=""col"">Gross</th>
            <th scope=""col"">Exp Code</th>
            <th scope=""col"">Amount</th>

        </tr>
    </thead>
    <tbody>
        <tr>
            <th scope=""row"">1</th>
            <td>Mark</td>
            <td>Otto</td>
        </tr>
        <tr>
            <th scope=""row"">2</th>
            <td>Jacob</td>
            <td>Thornton</td>

        </tr>
        <tr>
            <th scope=""row"">3</th>
            <td colspan=""2"">Larry the Bird</td>
        </tr>
    </tbody");
            WriteLiteral(">\r\n</table>\r\n\r\n<div class=\"modal\" id=\"loading_modal\" tabindex=\"-1\" role=\"dialog\">\r\n    <div class=\"modal-dialog\" role=\"document\">\r\n        <center>");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("img", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "64e861687d9d911958092beba394559b135f7e766108", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"</center>
    </div>
</div>

<script type=""text/javascript"">
    jQuery(function ($) {
        try {
            Dropzone.autoDiscover = false;

            var myDropzone = new Dropzone('#dropzone', {
                previewTemplate: $('#preview-template').html(),

                thumbnailHeight: 120,
                thumbnailWidth: 120,
                maxFilesize: 10,

                //addRemoveLinks : true,
                //dictRemoveFile: 'Remove',

                dictDefaultMessage:
                    '<span class=""bigger-150 bolder""><i class=""ace-icon fa fa-caret-right red""></i> Drop files</span> to upload \
                    <span class=""smaller-80 grey"">(or click)</span> <br /> \
                    <i class=""upload-icon ace-icon fa fa-cloud-upload blue fa-3x""></i>'
                ,

                thumbnail: function (file, dataUrl) {
                    if (file.previewElement) {
                        $(file.previewElement).removeClass(""dz-file-preview"");
    ");
            WriteLiteral(@"                    var images = $(file.previewElement).find(""[data-dz-thumbnail]"").each(function () {
                            var thumbnailElement = this;
                            thumbnailElement.alt = file.name;
                            thumbnailElement.src = dataUrl;
                        });
                        setTimeout(function () { $(file.previewElement).addClass(""dz-image-preview""); }, 1);
                    }
                }

            });


            //simulating upload progress
            var minSteps = 6,
                maxSteps = 60,
                timeBetweenSteps = 100,
                bytesPerStep = 100000;

            myDropzone.uploadFiles = function (files) {
                var self = this;
                $('#submit_excel').click(function (e) {
                    $('#loading_modal').modal('show');
                    var fdata = new FormData();
                    var fileUpload = files;
                    fdata.append(fileUpload[0].n");
            WriteLiteral("ame, fileUpload[0]);\r\n                    console.log(fdata);\r\n                    setTimeout(function () {\r\n                        $.ajax({\r\n                            type: \'POST\',\r\n                            url: \'");
#nullable restore
#line 98 "D:\fmis\Views\Budget\Obligation.cshtml"
                             Write(Url.Action("ImportSection", "UploadSection"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                            data: fdata,
                            contentType: false,
                            processData: false,
                            success: function (output) {
                                $('#loading_modal').modal('hide');
                                Lobibox.notify(""success"", {
                                    msg: ""Successfully Uploaded..."",
                                    sound: false
                                });
                                console.log(output);
                            }
                        });
                    }, 1000);

                });

                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    totalSteps = Math.round(Math.min(maxSteps, Math.max(minSteps, file.size / bytesPerStep)));

                    for (var step = 0; step < totalSteps; step++) {
                        var duration = timeBetweenSteps * (step + 1);
            ");
            WriteLiteral(@"            setTimeout(function (file, totalSteps, step) {
                            return function () {
                                file.upload = {
                                    progress: 100 * (step + 1) / totalSteps,
                                    total: file.size,
                                    bytesSent: (step + 1) * file.size / totalSteps
                                };

                                self.emit('uploadprogress', file, file.upload.progress, file.upload.bytesSent);
                                if (file.upload.progress == 100) {
                                    file.status = Dropzone.SUCCESS;
                                    self.emit(""success"", file, 'success', null);
                                    self.emit(""complete"", file);
                                    self.processQueue();
                                }
                            };
                        }(file, totalSteps, step), duration);
                    }
 ");
            WriteLiteral(@"               }
            }


            //remove dropzone instance when leaving this page in ajax mode
            $(document).one('ajaxloadstart.page', function (e) {
                try {
                    myDropzone.destroy();
                } catch (e) { }
            });

        } catch (e) {
            alert('Dropzone.js does not support older browsers!');
        }

    });
</script>
");
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