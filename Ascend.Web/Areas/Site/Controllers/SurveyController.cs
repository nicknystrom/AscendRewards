using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;
using Ascend.Infrastructure;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class SurveyViewModel
    {
        public ContentViewModel Content { get; set; }
        public IEnumerable<SurveyViewModelQuestion> Questions { get; set; }

        public static SurveyViewModel FromDomain(Survey s)
        {
            var markdown = new Markdown();
            return new SurveyViewModel
            {
                Content = ContentViewModel.FromDomain(s.Content),
                Questions = s.Questions.Select(
                    x => new SurveyViewModelQuestion
                    {
                        Title = x.Title,
                        Content = String.IsNullOrEmpty(x.Content) ? null : markdown.Transform(x.Content),
                        Freeform = x.Freeform,
                        Answers = x.Answers == null ? null : x.Answers.Select(y => y.Title),
                    }),
            };
        }
    }

    public class SurveyViewModelQuestion
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Freeform { get; set; }
        public IEnumerable<string> Answers { get; set; }
    }

    public class SurveyResultViewModel
    {
        public string[] Answers { get; set; }
    }

    public class SurveyReviewModel
    {
        public Survey Survey { get; set; }
        public SurveyResult Result { get; set; }

        public int Points { get; set; }

        public static SurveyReviewModel FromDomain(Survey q, SurveyResult r)
        {
            return new SurveyReviewModel
                       {
                           Survey = q,
                           Result = r,
                           Points = r.PointsEarned ?? 0,
                       };
        }
    }

    
    public partial class SurveyController : ProgramController<Survey>
    {
        public ISurveyRepository Surveys { get; set; }
        public ISurveyResultRepository Results { get; set; }

        protected override Survey GetResource(string id)
        {
            return Surveys.Get(id);
        }

        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            var count = Results.GetResults(CurrentResource, CurrentUser).Count;
            if (CurrentResource.MaxTimes.HasValue && count > CurrentResource.MaxTimes)
            {
                return View("Unavailable", AvailabilityResult.No("This survey can only be taken " + CurrentResource.MaxTimes + " times."));
            }
            return View(SurveyViewModel.FromDomain(CurrentResource));
        }

        [HttpPost]
        public virtual ActionResult Index(string id, SurveyResultViewModel model)
        {
            var count = Results.GetResults(CurrentResource, CurrentUser).Count;
            if (CurrentResource.MaxTimes.HasValue && count > CurrentResource.MaxTimes)
            {
                return View("Unavailable", AvailabilityResult.No("This survey can only be taken " + CurrentResource.MaxTimes + " times."));
            }

            // score the Survey
            var result = CurrentResource.Score(CurrentUser, model.Answers);

            // possibly award points
            if (result.PointsEarned.HasValue && result.PointsEarned > 0)
            {
                var tx = TryAwardPoints(result.PointsEarned.Value);
                if (null != tx)
                {
                    result.Transaction = tx.Document.Id;
                }
            }
            Results.Save(result);

            // show the review screen
            return RedirectToAction(MVC.Site.Survey.Review(id, result.Document.Id));
        }

        [HttpGet]
        public virtual ActionResult Review(string id, string r)
        {
            // validate that we can view this result
            var result = Results.Get(r);
            if (result.Survey != CurrentResource.Document.Id ||
                result.User != CurrentUser.Document.Id)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot view this result.");
            }

            return View(SurveyReviewModel.FromDomain(CurrentResource, result));
        }
    }
}