#pragma checksum "D:\fmis\fmis\Views\Prexc\GAS.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c43f03d99b3c23a45ae2958295fd996ad19f58f4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Prexc_GAS), @"mvc.1.0.view", @"/Views/Prexc/GAS.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c43f03d99b3c23a45ae2958295fd996ad19f58f4", @"/Views/Prexc/GAS.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Prexc_GAS : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<fmis.Models.Prexc>>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
  
    ViewData["Title"] = "GAS";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral("<style>\r\n    .filterHeader {\r\n        color: black;\r\n    }\r\n\r\n    input {\r\n        width: 750px;\r\n    }\r\n</style>\r\n\r\n<input type=\"hidden\" class=\"prexc\"");
            BeginWriteAttribute("value", " value=\"", 477, "\"", 498, 1);
#nullable restore
#line 25 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
WriteAttributeValue("", 485, ViewBag.temp, 485, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n<h3 class=\"grey lighter smaller\">\r\n    (GAS) PPA Description & Code\r\n</h3>\r\n<hr>\r\n");
            WriteLiteral(@"<br />
<div id=""example"" class=""hot""></div>
<br />
<div class=""controls"" hidden>
    <button id=""export-file"">Download CSV</button>
</div>

<script>

    let guid = () => {
        let s4 = () => {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    const debounceFn = Handsontable.helper.debounce((colIndex, event) => {
        const filtersPlugin = hot.getPlugin('filters');

        filtersPlugin.removeConditions(colIndex);
        filtersPlugin.addCondition(colIndex, 'contains', [event.target.value]);
        filtersPlugin.filter();
    }, 100);

    const addEventListeners = (input, colIndex) => {
        input.addEventListener('keydown', event => {
            debounceFn(colIndex, event);
        });
    };

    // Build elements which will be displayed in header.
    const getInitializedElem");
            WriteLiteral(@"ents = colIndex => {
        const div = document.createElement('div');
        const input = document.createElement('input');

        div.className = 'filterHeader';

        addEventListeners(input, colIndex);

        div.appendChild(input);

        return div;
    };

    // Add elements to header on `afterGetColHeader` hook.
    const addInput = (col, TH) => {
        // Hooks can return a value other than number (for example `columnSorting` plugin uses this).
        if (typeof col !== 'number') {
            return col;
        }

        if (col >= 0 && TH.childElementCount < 2) {
            TH.appendChild(getInitializedElements(col));
        }
    };

    function ajaxServer(ajax_data, url) {
        $.ajax({
            type: 'POST',
            url: url,
            headers: { ""RequestVerificationToken"": '");
#nullable restore
#line 93 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
                                               Write(GetAntiXsrfRequestToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' },
            data: { data: ajax_data },
            success: function (output) {
                $('#loading_modal').modal('hide');
               
                console.log(output);
            }
        });
    }

    function ajaxServer(ajax_data, url) {
        $.ajax({
            type: 'POST',
            url: url,
            headers: { ""RequestVerificationToken"": '");
#nullable restore
#line 107 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
                                               Write(GetAntiXsrfRequestToken());

#line default
#line hidden
#nullable disable
            WriteLiteral(@"' },
            data: { data: ajax_data },
            success: function (output) {
                $('#loading_modal').modal('hide');
                console.log(output);
            }
        });
    }

    var prexc = $.parseJSON($("".prexc"").val());
    const data = [];
    var counter = 0;
    $.each(prexc, function () {
        data[counter] = [this.pap_initial, this.pap_title, this.pap_code1, this.Id, this.token]
        counter++;
    })
    const searchField = document.querySelector('#search_field2');
    const button = document.querySelector('#export-file');
    const container = document.getElementById('example');
    const hot = new Handsontable(container, {
        data: data,
        colWidths: [100,600, 500],
        colHeaders: ['PPA INITIAL','PPA DESCRIPTION', 'PPA CODE', 'ID', 'TOKEN'],
        columns: [
            {

            },
            {

            },
            {

            },
            {
                // 2nd cell ID column had been hidd");
            WriteLiteral(@"en
            },
            {
                // 3nd cell ID column had been hidden
            },

        ],
        rowHeaders: true,
        undo: true,
        search: {
            searchResultClass: 'search-result-custom'
        },
        contextMenu: ['row_above', 'row_below', 'remove_row'],
        beforeRemoveRow: function (index, column) {
            var selection = this.getSelected();
            var holder_data = this.getData();
            var single_token = holder_data[selection[0][0]][4];
            var first_column = selection[0][0];
            var last_column = selection[0][2];
            var many_token = [];
            for (var j = first_column; j <= last_column; j++) {
                many_token.push({
                    many_token: holder_data[j][4]
                })
            }

            if (single_token) {
                var ajax_data = {
                    ""single_token"": single_token,
                    ""many_token"": many_token
        ");
            WriteLiteral("        };\r\n\r\n                console.log(ajax_data);\r\n\r\n                ajaxServer(ajax_data,\'");
#nullable restore
#line 175 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
                                 Write(Url.Action("DeletePrexc", "Prexc"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
            }
        },
        afterUndo: function (index, column) {

        },
        afterCreateRow: function (row, column) {

            console.log(row);
            console.log(column);
            hot.setDataAtCell(row, 4, guid());
        },
        afterChange: function (changes, source) {
            if (source === 'loadData') {
                return; 
            }

            console.log(changes);

            var ajax_data = [];
            $.each(hot.getSourceData(), function () {
                ajax_data.push(
                    {
                        pap_initial: this[0] ? this[0] : "" "",
                        pap_title: this[1] ? this[1] : "" "",
                        pap_code1: this[2] ? this[2] : "" "",
                        Id: this[3] ? this[3] : "" "",
                        token: this[4]
                    }
                );
            });

            ajaxServer(ajax_data, '");
#nullable restore
#line 207 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
                              Write(Url.Action("SavePrexcGAS", "Prexc"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"');
        },
        height: 'auto',
        className: 'as-you-type-demo',
        filters: true,
        afterGetColHeader: addInput,
        beforeOnCellMouseDown(event, coords) {
            // Deselect the column after clicking on input.
            if (coords.row === -1 && event.target.nodeName === 'INPUT') {
                event.stopImmediatePropagation();
                this.deselectCell();
            }
        },
        hiddenColumns: {
            columns: [3,4],
            indicators: true
        },
        licenseKey: 'non-commercial-and-evaluation', // for non-commercial use only
    });

");
            WriteLiteral(@"
    const exportPlugin = hot.getPlugin('exportFile');
    button.addEventListener('click', () => {
        exportPlugin.downloadFile('csv', {
            bom: false,
            columnDelimiter: ',',
            columnHeaders: true,
            exportHiddenColumns: true,
            exportHiddenRows: true,
            fileExtension: 'csv',
            filename: 'UACS',
            mimeType: 'text/csv',
            rowDelimiter: '\r\n',
            rowHeaders: true
        });
    });

    var tableData = JSON.stringify(hot.getSourceData());
    console.log(hot.getSourceData());

</script>
");
        }
        #pragma warning restore 1998
#nullable restore
#line 9 "D:\fmis\fmis\Views\Prexc\GAS.cshtml"
           
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<fmis.Models.Prexc>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
