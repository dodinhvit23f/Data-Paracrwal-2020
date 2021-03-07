using System.Collections.Generic;
using System.IO;
using Project4Aptech.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
namespace Project4Aptech.Invoice
{
    public class InvoicePrepare
    {
        static int col = 6;
        Document _doc;
        Font _fontStyle;
         PdfPTable _table = new PdfPTable(col);
        PdfPCell _PdfPCell;
        MemoryStream stream = new MemoryStream();
        List<TransactionHistory> _trans = new List<TransactionHistory>();
        public byte[] Prepare(List<TransactionHistory> trans,string cus_id)
        {
            double debitSum = 0;
            double creditSum = 0;
            _trans = trans;
            _doc = new Document(PageSize.A4,0f, 0f, 0f, 0f);
            _doc.SetPageSize(PageSize.A4);
            _doc.SetMargins(20f,20f,20f,20f);
            _table.WidthPercentage = 100;
            _table.HorizontalAlignment = Element.ALIGN_LEFT;
            _fontStyle = FontFactory.GetFont("Tahoma",8f,1);
            PdfWriter.GetInstance(_doc, stream);
            _doc.Open();
            _table.SetWidths(new float[] {100f,70f,100f, 100f, 100f, 100f }  );
            _PdfPCell = new PdfPCell(new Phrase("Transaction Time", _fontStyle));
            _table.AddCell(_PdfPCell);
            _PdfPCell = new PdfPCell(new Phrase("Debit/Credit", _fontStyle));
            _table.AddCell(_PdfPCell);
            _PdfPCell = new PdfPCell(new Phrase("Message", _fontStyle));
            _table.AddCell(_PdfPCell);
            _PdfPCell = new PdfPCell(new Phrase("Amount", _fontStyle));
            _table.AddCell(_PdfPCell);
            _PdfPCell = new PdfPCell(new Phrase("Fee", _fontStyle));
            _table.AddCell(_PdfPCell);
            _table.CompleteRow();
            foreach (var item in _trans)
            {
                _PdfPCell = new PdfPCell(new Phrase(item.tran_time, _fontStyle));
                _table.AddCell(_PdfPCell);
                if (cus_id == item.SendAccount)
                {
                    _PdfPCell = new PdfPCell(new Phrase("Debit", _fontStyle));
                    debitSum += (double)item.Amount;
                }else if (cus_id == item.ReceiveAccount && cus_id != item.SendAccount)
                {
                    _PdfPCell = new PdfPCell(new Phrase("Credit", _fontStyle));
                    creditSum += (double)item.Amount;
                }
                _table.AddCell(_PdfPCell);
                if (!string.IsNullOrEmpty(item.Message))
                {
                    _PdfPCell = new PdfPCell(new Phrase(item.Message, _fontStyle));
                }
                else
                {
                    _PdfPCell = new PdfPCell(new Phrase("No message", _fontStyle));
                }
                _table.AddCell(_PdfPCell);
                _PdfPCell = new PdfPCell(new Phrase(item.Amount.ToString(), _fontStyle));
                _table.AddCell(_PdfPCell);
                _PdfPCell = new PdfPCell(new Phrase(item.fee.ToString(), _fontStyle));
                _table.AddCell(_PdfPCell);
                _table.CompleteRow();
            }
            _PdfPCell = new PdfPCell(new Phrase("Debit Sum:"+debitSum.ToString(), _fontStyle));
            _table.AddCell(_PdfPCell);
            _PdfPCell = new PdfPCell(new Phrase("Credit Sum:"+creditSum.ToString(), _fontStyle));
            _table.AddCell(_PdfPCell);
            _table.CompleteRow();
            _doc.Add(_table);
            _doc.Close();
            
            return stream.ToArray();
        }
    }
    //Prepare
}