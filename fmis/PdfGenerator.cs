using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using System.Collections.Generic;
using System;
using System.Text;

namespace fmis
{
    public class PdfGenerator
    {


        public void GeneratePdf(string filePath)
        {
            using (var writer = new PdfWriter(filePath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var document = new Document(pdf);
                    var pageEventHandler = new CustomPageEventHandler();
                    pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, pageEventHandler);

                    // Generate the long content string
                    string shortContent = "This is a long paragraph. ";
                    int repetitions = 5000;
                    StringBuilder longContentBuilder = new StringBuilder(shortContent.Length * repetitions);
                    for (int i = 0; i < repetitions; i++)
                    {
                        longContentBuilder.Append(shortContent);
                    }
                    string longContent = longContentBuilder.ToString();

                    // Add content to the document
                    document.Add(new Paragraph(longContent));

                    // Add more content or modify the document as needed

                    // Close the document
                    document.Close();
                }
            }
        }

        public List<string> SplitContentIntoChunks(string content, int chunkSize = 2000)
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < content.Length; i += chunkSize)
            {
                int size = Math.Min(chunkSize, content.Length - i);
                chunks.Add(content.Substring(i, size));
            }
            return chunks;
        }

        public bool IsPageFull(Document document, CustomPageEventHandler pageEventHandler)
        {
            PdfDocument pdfDoc = document.GetPdfDocument();
            PdfPage page = pdfDoc.GetLastPage();

            float availableSpace = page.GetPageSize().GetHeight() - pageEventHandler.GetFooterHeight();
            float usedSpace = pageEventHandler.GetFooterHeight() + (float)document.GetRenderer().GetCurrentArea().GetBBox().GetHeight();

            return usedSpace > availableSpace;
        }

        public class CustomPageEventHandler : IEventHandler
        {
            private const float FooterHeight = 40; // Adjust this value based on your footer's height

            public float GetFooterHeight()
            {
                return FooterHeight;
            }

            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                // Add header to the page
                float x = pdfDoc.GetDefaultPageSize().GetX() + 20;
                float y = pdfDoc.GetDefaultPageSize().GetTop() - 20;
                DrawText(page, x, y, "Header Text");

                // Add footer to the page
                x = pdfDoc.GetDefaultPageSize().GetX() + 20;
                y = pdfDoc.GetDefaultPageSize().GetBottom() + 20;
                DrawText(page, x, y, "Footer Text - Page " + pdfDoc.GetPageNumber(page));
            }

            private void DrawText(PdfPage page, float x, float y, string text)
            {
                PdfCanvas canvas = new PdfCanvas(page);
                canvas.BeginText();
                canvas.MoveText(x, y);
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(), 12);
                canvas.ShowText(text);
                canvas.EndText();
            }
        }


    }
}
