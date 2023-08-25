#pragma checksum "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "fdfeee81e50685c624e89ba3af19d0208e9bc030"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_UacsTrustFund_MOOE_Trust_Fund), @"mvc.1.0.view", @"/Views/UacsTrustFund/MOOE_Trust_Fund.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"fdfeee81e50685c624e89ba3af19d0208e9bc030", @"/Views/UacsTrustFund/MOOE_Trust_Fund.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c636cde0ce5b472ea04d81eadec7dcf23c48d750", @"/Views/_ViewImports.cshtml")]
    public class Views_UacsTrustFund_MOOE_Trust_Fund : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<fmis.Models.UacsTrustFund>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 5 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
  
    ViewData["Title"] = "MOOE";
    Layout = "~/Views/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral("\r\n<style>\r\n    .filterHeader {\r\n        color: black;\r\n    }\r\n\r\n    input {\r\n        width: 750px;\r\n    }\r\n</style>\r\n\r\n<input type=\"hidden\" class=\"uacs\"");
            BeginWriteAttribute("value", " value=\"", 489, "\"", 510, 1);
#nullable restore
#line 27 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
WriteAttributeValue("", 497, ViewBag.temp, 497, 13, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@">
<h3 class=""grey lighter smaller"">
    (Trust Fund - MOOE) Account Title & Expense Code
</h3>
<hr>
<br />
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
        })");
            WriteLiteral(@";
    };
    // Build elements which will be displayed in header.
    const getInitializedElements = colIndex => {
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
#line 83 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
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
    var uacs = $.parseJSON($("".uacs"").val());
    const data = [];
    var counter = 0;
    $.each(uacs, function () {
        data[counter] = [this.Account_title, this.Expense_code, this.Id, this.token]
        counter++;
    });
    const searchField = document.querySelector('#search_field2');
    const button = document.querySelector('#export-file');
    const container = document.getElementById('example');
    const hot = new Handsontable(container, {
        data: data,
        colWidths: [600, 500],
        colHeaders: ['ACCOUNT TITLE', 'EXPENSE CODE', 'ID', 'TOKEN'],
        columns: [
            {
            },
            {
            },
            {
                //HIDDEN ID
            },
            {
               //HIDDEN TOKEN
            }
        ],
 ");
            WriteLiteral(@"       rowHeaders: true,
        undo: true,
        search: {
            searchResultClass: 'search-result-custom'
        },
        contextMenu: ['row_above', 'row_below', 'remove_row'],
        beforeRemoveRow: function (index, column) {
            var selection = this.getSelected();
            var holder_data = this.getData();
            var single_token = holder_data[selection[0][0]][3];
            var first_column = selection[0][0];
            var last_column = selection[0][2];
            var many_token = [];
            for (var j = first_column; j <= last_column; j++) {
                many_token.push({
                    many_token: holder_data[j][3]
                })
            }
            if (single_token) {
                var ajax_data = {
                    ""single_token"": single_token,
                    ""many_token"": many_token
                };
                console.log(ajax_data);
                ajaxServer(ajax_data,'");
#nullable restore
#line 141 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
                                 Write(Url.Action("DeleteUacsTrustFund", "UacsTrustFund"));

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
            hot.setDataAtCell(row, 3, guid());
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
                        Account_title: this[0] ? this[0] : "" "",
                        Expense_code: this[1] ? this[1] : "" "",
                        Id: this[2] ? this[2] : "" "",
                        token: this[3]
                    }
                );
            });
            ajaxServer(ajax_data, '");
#nullable restore
#line 167 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
                              Write(Url.Action("SaveUacsMOOETrustFund", "UacsTrustFund"));

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
            columns: [2,3],
            indicators: true
        },
        licenseKey: 'non-commercial-and-evaluation',
    });

    var tableData = JSON.stringify(hot.getSourceData());
");
            WriteLiteral("</script>");
        }
        #pragma warning restore 1998
#nullable restore
#line 10 "D:\fmis\Views\UacsTrustFund\MOOE_Trust_Fund.cshtml"
           
    public string GetAntiXsrfRequestToken()
    {
        return Xsrf.GetAndStoreTokens(Context).RequestToken;
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public Microsoft.AspNetCore.Antiforgery.IAntiforgery Xsrf { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<fmis.Models.UacsTrustFund>> Html { get; private set; }
    }
}
#pragma warning restore 1591
