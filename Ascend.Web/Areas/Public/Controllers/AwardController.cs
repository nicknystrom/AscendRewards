using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services.Caching;

using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;

namespace Ascend.Web.Areas.Public.Controllers
{
    public class UserAwardViewModel
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string NominatorName { get; set; }
        public string RecipientName { get; set; }
        public int? Amount { get; set; }
        public string Message { get; set; }
        public Certificate Certificate { get; set; }
    }

    public class PdfAwardresult : ActionResult
    {
        private const int Dpi = 72;

        public UserAwardViewModel Model { get; set; }

        public PdfAwardresult(UserAwardViewModel model)
        {
            Model = model;
        }

        void DrawBox(
            Document doc,
            PdfContentByte cb,
            Certificate c,
            CertificateArea box,
            string text,
            bool wrap)
        {
            if (null == box || !box.Enabled || String.IsNullOrEmpty(text))
            {
                return;
            }

            var origin = (float)box.Left;
            var alignment = PdfContentByte.ALIGN_LEFT;
            switch (box.Alignment)
            {
                case "center":
                    alignment = PdfContentByte.ALIGN_CENTER;
                    origin = (box.Left + box.Width/2);
                    break;
                case "right":
                    alignment = PdfContentByte.ALIGN_RIGHT;
                    origin = box.Left + box.Width;
                    break;
            }

            var size = 0 == box.FontSize ? 12f : box.FontSize;
            cb.SetFontAndSize(
                BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false),
                size);
            cb.BeginText();
            if (wrap)
            {
                var y = 0;
                var line = 0;
                for (var x = 0; x < text.Length; x++)
                {
                    var sub = text.Substring(y, x - y);
                    var width = cb.GetEffectiveStringWidth(sub, false);
                    if (width >= 10f * Dpi * box.Width / c.BackgroundSize.Width)
                    {
                        // backup to the last whitespace char
                        while (x > y && Char.IsLetterOrDigit(text[x]))
                        {
                            x--;
                        }
                        sub = text.Substring(y, x - y);
                        cb.ShowTextAligned(
                            alignment,
                            sub,
                            (0.5f*Dpi) + (10f*Dpi*origin/c.BackgroundSize.Width),
                            (0.5f*Dpi) + (7.5f*Dpi) - (7.5f*Dpi*box.Top/c.BackgroundSize.Height) - size - (line * size),
                            0f
                        );
                        y = x;
                        line++;
                    }
                }
            }
            else
            {
                cb.ShowTextAligned(
                    alignment,
                    text,
                    (0.5f * Dpi) + (10f * Dpi * origin / c.BackgroundSize.Width),
                    (0.5f * Dpi) + (7.5f * Dpi) - (7.5f * Dpi * box.Top / c.BackgroundSize.Height) - size,
                    0f
                );
            }
            cb.EndText();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/pdf";

            var doc = new Document(PageSize.LETTER.Rotate());
            var writer = PdfWriter.GetInstance(doc, response.OutputStream);
            doc.Open();

            var c = Model.Certificate;
            var cb = writer.DirectContent;

            var bg = Image.GetInstance(Model.Certificate.BackgroundUrl.ToAbsoluteUrl(context.HttpContext.Request));
            bg.SetAbsolutePosition(0.5f * Dpi, 0.5f * Dpi);
            bg.ScaleAbsoluteWidth(10.0f * Dpi);
            bg.ScaleAbsoluteHeight(7.5f * Dpi);
            cb.AddImage(bg);

            DrawBox(doc, cb, c, c.FromBox, Model.NominatorName, false);
            DrawBox(doc, cb, c, c.ToBox, Model.RecipientName, false);
            DrawBox(doc, cb, c, c.AwardBox, Model.Amount.HasValue ? String.Format("You have received {0:n0} points!", Model.Amount.Value) : "", false);
            DrawBox(doc, cb, c, c.MessageBox, Model.Message, false);
            DrawBox(doc, cb, c, c.DateBox, Model.Date.ToShortDateString(), false);
            
            doc.Close();
        }
    }

    public partial class AwardController : PublicController
    {
        public IUserSummaryCache Users { get; set; }
        public IUserAwardRepository UserAwards { get; set; }

        private UserAwardViewModel GetViewModel(string id)
        {
            var award = UserAwards.Get(id);
            return new UserAwardViewModel
            {
                Id = award.Document.Id,
                Date = award.Created.Date,
                NominatorName = (Users.TryGet(award.Nominator) ?? new UserSummary { Login = "Unknown User" }).DisplayName,
                RecipientName = (Users.TryGet(award.Recipient) ?? new UserSummary { Login = "Unknown User" }).DisplayName,
                Amount = award.Amount,
                Message = award.Message,
                Certificate = award.Certificate,
            };
        }

        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            return new PdfAwardresult(GetViewModel(id));
            //return PartialView(GetViewModel(id));
        }
    }
}