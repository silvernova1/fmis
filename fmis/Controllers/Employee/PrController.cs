﻿using fmis.Data.MySql;
using fmis.Data;
using Microsoft.AspNetCore.Mvc;
using fmis.Filters;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fmis.Models.pr;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using fmis.Models.ppmp;


using System;
using fmis.Models;
using fmis.Models.Accounting;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using Rotativa.AspNetCore;
using Font = iTextSharp.text.Font;
using System.Globalization;
using System.Collections;
using iTextSharp.tool.xml;
using Image = iTextSharp.text.Image;
using Grpc.Core;
using fmis.ViewModel;
using fmis.DataHealpers;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Microsoft.Extensions.Hosting.Internal;
using iTextSharp.text.pdf.draw;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using DocumentFormat.OpenXml.InkML;
using fmis.Data.Accounting;
using System.Text;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Runtime.InteropServices.WindowsRuntime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace fmis.Controllers.Employee
{
    public class PrController : Controller
    {
        private readonly PpmpContext _ppmpContext;
        private readonly DtsContext _dts;
        private readonly MyDbContext _context;

        public PrController(PpmpContext ppmpContext, DtsContext dts, MyDbContext context)
        {
            _ppmpContext = ppmpContext;
            _dts = dts;
            _context = context;
        }
        [Route("Employee/Pr")]
        public async Task<IActionResult> Index()
        {
            ViewBag.filter = new FilterSidebar("end_user", "DV", "");
            ExpenseDropDownList();
            ItemsDropDownList();
            UsersDropDownList();

            var pr = await _context.Pr.Include(x => x.PrItems).ToListAsync();
            ViewBag.ItemDesc = _ppmpContext.item.FirstOrDefault();


            return View(pr);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Pr pr)
        {
            _context.Add(pr);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetUnit(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_measurement);
            }
            return NotFound();
        }
        [HttpGet]
        public IActionResult GetUnitCost(int id)
        {

            var item = _ppmpContext.item.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                return Ok(item.Unit_cost);
            }
            return NotFound();
        }
        public void ExpenseDropDownList(object selected = null)
        {
            var Query = from e in _ppmpContext.expense
                        orderby e.Id
                        select e;
            ViewBag.ExpenseId = new SelectList(Query, "Id", "Description", selected);
        }
        public void ItemsDropDownList(object selected = null)
        {
            var Query = from i in _ppmpContext.item
                        orderby i.Id
                        select i;
            ViewBag.ItemId = new SelectList(Query, "Id", "Description", selected);
        }
        public void UsersDropDownList(object selected = null)
        {
            var employee = from x in _dts.users
                        orderby x.Id
                        select new
                        {
                            Id = x.Id,
                            Fullname = x.Fname + " " + x.Lname + " " + x.Lname
                        };
            ViewBag.Approval = new SelectList(employee, "Id", "Fullname");
        }
        public List<Item> GetItemsId(int expenseId)
        {
            return _ppmpContext.item.Where(c => c.Expense_id == expenseId).ToList();
        }


		public IActionResult PrintPr(string[] token, int id)
		{
			using (MemoryStream stream = new System.IO.MemoryStream())
			{

				string ExportData = "This is pdf generated";
				StringReader reader = new StringReader(ExportData);
				Document doc = new iTextSharp.text.Document(PageSize.A4);
				PdfWriter writer = PdfWriter.GetInstance(doc, stream);
				doc.Open();


				doc.NewPage();



				Paragraph nextline = new Paragraph("\n");
				doc.Add(nextline);
				PdfPTable table = new PdfPTable(2);
				table.PaddingTop = 5f;
				table.WidthPercentage = 100f;
				float[] columnWidths = { 1, 15 };
				table.SetWidths(columnWidths);

				Image logo = Image.GetInstance("wwwroot/assets/images/doh_logo_updated.png");
				logo.ScaleAbsolute(50f, 50f);
				PdfPCell logo_cell = new PdfPCell(logo);
				logo_cell.DisableBorderSide(8);

				logo_cell.PaddingLeft = 30f;
				logo_cell.PaddingTop = 5f;
				logo_cell.PaddingBottom = 5f;

				table.AddCell(logo_cell);


				Font arial_font_8 = FontFactory.GetFont("", 8, Font.NORMAL, BaseColor.BLACK);
				Font italic_font_8 = FontFactory.GetFont("", 8, Font.ITALIC, BaseColor.BLACK);
				Font italic_font_7 = FontFactory.GetFont("", 7, Font.ITALIC, BaseColor.BLACK);
				Font arial_font_9 = FontFactory.GetFont("", 9, Font.NORMAL, BaseColor.BLACK);
				Font arial_font_7 = FontFactory.GetFont("", 7, Font.NORMAL, BaseColor.BLACK);
				Font arial_font_6 = FontFactory.GetFont("", 6, Font.NORMAL, BaseColor.BLACK);
				Font arial_font_9b = FontFactory.GetFont("", 9, Font.BOLD, BaseColor.BLACK);
				Font arial_font_10 = FontFactory.GetFont("Times New Roman", 10, Font.NORMAL, BaseColor.BLACK);
				Font arial_font_11 = FontFactory.GetFont("Times New Roman", 11, Font.NORMAL, BaseColor.BLACK);
				Font arial_font_12 = FontFactory.GetFont("Times New Roman", 12, Font.BOLD, BaseColor.BLACK);
				Font header = FontFactory.GetFont("Times New Roman", 10, Font.BOLD, BaseColor.BLACK);

				FontFactory.Register("C:\\Windows\\Fonts\\timesbd.ttf", "Times New Roman Bold");

				var times_new_roman_b12 = FontFactory.GetFont("Times New Roman Bold", 12f);
				var times_new_roman_b11 = FontFactory.GetFont("Times New Roman Bold", 11f);
				var times_new_roman_b10 = FontFactory.GetFont("Times New Roman Bold", 10f);
				var times_new_roman_b9 = FontFactory.GetFont("Times New Roman Bold", 9f);
				var times_new_roman_b8 = FontFactory.GetFont("Times New Roman Bold", 8f);


				FontFactory.Register("C:\\Windows\\Fonts\\times.ttf", "Times New Roman Regular");
				var times_new_roman_r12 = FontFactory.GetFont("Times New Roman Regular", 12f);
				var times_new_roman_r11 = FontFactory.GetFont("Times New Roman Regular", 11f);
				var times_new_roman_r10 = FontFactory.GetFont("Times New Roman Regular", 10f);
				var times_new_roman_r9 = FontFactory.GetFont("Times New Roman Regular", 9f);
				var times_new_roman_r8 = FontFactory.GetFont("Times New Roman Regular", 8f);
				var times_new_roman_r7 = FontFactory.GetFont("Times New Roman Regular", 7f);

				var table2 = new PdfPTable(1);
				table2.DefaultCell.Border = 0;

				table2.AddCell(new PdfPCell(new Paragraph("Appendix 60", italic_font_7)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT });
				table2.AddCell(new PdfPCell(new Paragraph("PURCHASE REQUEST", arial_font_12)) { PaddingRight = 35, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
				table2.AddCell(new PdfPCell(new Paragraph("DOH - Central Visayas Center For Health Development", arial_font_9)) { PaddingRight = 35, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
				table2.AddCell(new PdfPCell(new Paragraph("(Agency)", arial_font_9)) { PaddingRight = 35, Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });


				var no_left_bor = new PdfPCell(table2);
				no_left_bor.DisableBorderSide(4);
				table.AddCell(no_left_bor);

				var table3 = new PdfPTable(1);
				float[] table3widths = { 1 };
				table3.SetWidths(table3widths);
				table3.WidthPercentage = 100f;
				table3.DefaultCell.Border = 0;

				table.AddCell(table3);
				doc.Add(table);


				iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance("wwwroot/assets/images/final_textbox_f.png");
				PdfPCell cell = new PdfPCell(myImage);

				var table_row_3 = new PdfPTable(4);
				float[] tbt_row3_width = { 25, 16, 7, 8 };
				table_row_3.WidthPercentage = 100f;
				table_row_3.SetWidths(tbt_row3_width);
				table_row_3.AddCell(new PdfPCell(new Paragraph("Division:", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});

				table_row_3.AddCell(new PdfPCell(new Paragraph("PR No.: ", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE,
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("Date:", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("69/69/2069", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("Section/Unit: ", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});

				table_row_3.AddCell(new PdfPCell(new Paragraph("SAI No.: ", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE,
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("Date:", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});

				table_row_3.AddCell(new PdfPCell(new Paragraph("ALOBS No.: ", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE,
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("Date:", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_3.AddCell(new PdfPCell(new Paragraph("", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 15,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});

				doc.Add(table_row_3);



				var table_row_5 = new PdfPTable(6);
				float[] tbt_row5_width = { 3, 4, 20, 4, 5, 6 };
				table_row_5.WidthPercentage = 100f;
				table_row_5.SetWidths(tbt_row5_width);

				table_row_5.AddCell(new PdfPCell(new Paragraph("Item No.", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});
				table_row_5.AddCell(new PdfPCell(new Paragraph("Unit Of Issue", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});
				table_row_5.AddCell(new PdfPCell(new Paragraph("Item Description", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});
				table_row_5.AddCell(new PdfPCell(new Paragraph("Quantity", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});
				table_row_5.AddCell(new PdfPCell(new Paragraph("Estimated Unit Cost", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});
				table_row_5.AddCell(new PdfPCell(new Paragraph("Estimated Cost", arial_font_8))
				{
					HorizontalAlignment = Element.ALIGN_CENTER,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 25
				});



				doc.Add(table_row_5);
				var table_row_6 = new PdfPTable(6);
				float[] tbt_ro6_width = { 3, 4, 20, 4, 5, 6 };
				table_row_6.WidthPercentage = 100f;
				table_row_6.SetWidths(tbt_ro6_width);
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });
				table_row_6.AddCell(new PdfPCell(new Paragraph("\n" + "", arial_font_9)) { Border = 13, FixedHeight = 320f, HorizontalAlignment = Element.ALIGN_CENTER });

				doc.Add(table_row_6);

				var table_row_7 = new PdfPTable(4);
				float[] tbt_row7_width = { 10, 5, 5, 5 };
				table_row_7.WidthPercentage = 100f;
				table_row_7.SetWidths(tbt_row7_width);
				table_row_7.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
				{
					Border = 14,
					HorizontalAlignment = Element.ALIGN_CENTER
				});

				PdfPTable tbt_total_amt = new PdfPTable(1);
				float[] tbt_total_amt_width = { 10 };
				tbt_total_amt.WidthPercentage = 100f;
				tbt_total_amt.SetWidths(tbt_total_amt_width);
				tbt_total_amt.AddCell(new PdfPCell(new Paragraph("\n", arial_font_9))
				{
					Border = 0,
					HorizontalAlignment = Element.ALIGN_CENTER
				});
				tbt_total_amt.AddCell(new PdfPCell(new Paragraph("", arial_font_9))
				{
					HorizontalAlignment = Element.ALIGN_RIGHT
				});
				table_row_7.AddCell(new PdfPCell(tbt_total_amt) { Border = 14 });

				doc.Add(table_row_7);

				var table_row_8 = new PdfPTable(1);
				var p = new Paragraph();
				float[] tbt_ro8_width = { 10 };
				table_row_8.DefaultCell.FixedHeight = 200f;
				table_row_8.WidthPercentage = 100f;
				table_row_8.SetWidths(tbt_ro8_width);

				table_row_8.AddCell(new PdfPCell(new Paragraph("CERTIFICATION \n      " + "This is to certify that diligent efforts have been exerted to ensure that the price/s indicated above (in relation to the specifications) is/are within the prevailing market price/s.", arial_font_6))
				{
					Border = 13,
					FixedHeight = 25f,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					HorizontalAlignment = Element.ALIGN_CENTER,
				});

				doc.Add(table_row_8);

				var table_row_13 = new PdfPTable(2);
				float[] tbt_row_13_width = { 4, 42 };
				table_row_13.WidthPercentage = 100f;
				table_row_13.SetWidths(tbt_row_13_width);
				table_row_13.AddCell(new PdfPCell(new Paragraph("Purpose: ", arial_font_7))
				{
					FixedHeight = 20f,
					HorizontalAlignment = Element.ALIGN_LEFT,
					VerticalAlignment = Element.ALIGN_MIDDLE
				});
				table_row_13.AddCell(new PdfPCell(new Paragraph("", arial_font_7))
				{
					Border = 13,
					FixedHeight = 15f,
					HorizontalAlignment = Element.ALIGN_LEFT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
				});
				doc.Add(table_row_13);


				var table_row_16 = new PdfPTable(3);
				float[] tbt_row_16_width = { 4, 30, 12 };
				table_row_16.WidthPercentage = 100f;
				table_row_16.SetWidths(tbt_row_16_width);
				table_row_16.AddCell(new PdfPCell(new Paragraph("Fund Chargable To: ", arial_font_7))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 30f
				});
				table_row_16.AddCell(new PdfPCell(new Paragraph("", arial_font_7))
				{
					Border = 13,
					VerticalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 30f
				});
				table_row_16.AddCell(new PdfPCell(new Paragraph("", arial_font_7))
				{
					VerticalAlignment = Element.ALIGN_MIDDLE,
					HorizontalAlignment = Element.ALIGN_CENTER,
					FixedHeight = 30f
				});

				doc.Add(table_row_16);


				var table_row_17 = new PdfPTable(4);
				float[] tbt_row_17_width = { 4, 18, 12, 12 };
				table_row_17.WidthPercentage = 100f;
				table_row_17.SetWidths(tbt_row_17_width);
				table_row_17.AddCell(new PdfPCell(new Paragraph("\nSignature:\n\nPrinted Name: ", arial_font_7))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 40f,
				});
				table_row_17.AddCell(new PdfPCell(new Paragraph("Requested By:" + "\n\n\n\n" + "Eugenia Mercedes R. Cañal, MD, MBHA-HA, FPSMS, PHSAE", arial_font_7))
				{
					Border = 13,
					VerticalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 40f
				});


				table_row_17.AddCell(new PdfPCell(new Paragraph("Recommending Approval By:" + "\n\n\n\n" + "Sophia M. Mancao, MD, DPSP, RN, MAN", arial_font_7))
				{
					VerticalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 40f
				});
				table_row_17.AddCell(new PdfPCell(new Paragraph("Approved:" + "\n\n\n\n" + "Jaime S. Bernadas, MD, MGM, CESO III", arial_font_7))
				{
					VerticalAlignment = Element.ALIGN_LEFT,
					FixedHeight = 40f
				});

				doc.Add(table_row_17);


				var table_row_18 = new PdfPTable(4);
				float[] tbt_row_18_width = { 4, 18, 12, 12 };
				table_row_18.WidthPercentage = 100f;
				table_row_18.SetWidths(tbt_row_18_width);
				table_row_18.DefaultCell.Border = 0;
				table_row_18.AddCell(new PdfPCell(new Paragraph("Designation: ", arial_font_7))
				{
					HorizontalAlignment = Element.ALIGN_LEFT,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					FixedHeight = 15f,
					Padding = 1
				});
				table_row_18.AddCell(new PdfPCell(new Paragraph("Medical Officer IV", arial_font_7))
				{
					Border = 13,
					VerticalAlignment = Element.ALIGN_MIDDLE,
					HorizontalAlignment = Element.ALIGN_CENTER,
					FixedHeight = 15f
				});
				table_row_18.AddCell(new PdfPCell(new Paragraph("Director III", arial_font_7))
				{
					VerticalAlignment = Element.ALIGN_MIDDLE,
					HorizontalAlignment = Element.ALIGN_CENTER,
					FixedHeight = 15f
				});
				table_row_18.AddCell(new PdfPCell(new Paragraph("Director IV", arial_font_7))
				{
					VerticalAlignment = Element.ALIGN_MIDDLE,
					HorizontalAlignment = Element.ALIGN_CENTER,
					FixedHeight = 15f
				});

				doc.Add(table_row_18);



				var table_end = new PdfPTable(1);
				float[] tbt_end_width = { 10 };
				table_end.WidthPercentage = 100f;
				table_end.SetWidths(tbt_end_width);
				table_end.AddCell(new PdfPCell(new Paragraph("" + "", arial_font_9))
				{
					Border = 13,
					FixedHeight = 1f,
					HorizontalAlignment = Element.ALIGN_LEFT
				});
				doc.Add(table_end);

				XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, reader);
				doc.Close(); return File(stream.ToArray(), "application/pdf");

			}

		}
	}
}
