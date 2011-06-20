using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Infrastructure;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Site.Controllers
{
    public class QuizViewModel
    {
        public ContentViewModel Content { get; set; }
        public IEnumerable<QuizViewModelQuestion> Questions { get; set; }

        public static QuizViewModel FromDomain(Quiz q)
        {
            var markdown = new Markdown();
            return new QuizViewModel
            {
                Content = ContentViewModel.FromDomain(q.Content),
                Questions = q.Questions.Select(
                    x => new QuizViewModelQuestion
                    {
                        Title = x.Title,
                        Content = String.IsNullOrEmpty(x.Content) ? null : markdown.Transform(x.Content),
                        Answers = x.Answers.Select(y => y.Title),
                    }),
            };
        }
    }

    public class QuizViewModelQuestion
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IEnumerable<string> Answers { get; set; }
    }

    public class QuizResultViewModel
    {
        public int[] Answers { get; set; }
    }

    public class QuizReviewModel
    {
        public Quiz Quiz { get; set; }
        public QuizResult Result { get; set; }

        public bool Review { get; set; }
        public bool Passed { get; set; }
        public int Correct { get; set; }
        public int Needed { get; set; }
        public int Points { get; set; }

        public static QuizReviewModel FromDomain(Quiz q, QuizResult r)
        {
            return new QuizReviewModel
                       {
                           Quiz = q,
                           Result = r,
                           Review = q.AllowReview,
                           Passed = r.Passed,
                           Correct = r.Answers.Count(x => x.Correct),
                           Needed = r.RequiredToPass,
                           Points = r.PointsEarned ?? 0,
                       };
        }
    }

    
    public partial class QuizController : ProgramController<Quiz>
    {
        public IQuizRepository Quizes { get; set; }
        public IQuizResultRepository Results { get; set; }

        protected override Quiz GetResource(string id)
        {
            return Quizes.Get(id);
        }

        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            var x = CurrentResource.CanUserTakeQuiz(
                CurrentUser,
                DateTime.UtcNow,
                Results.GetResults(CurrentResource, CurrentUser));
            if (!x.Available)
            {
                return View("Unavailable", x);
            }

            return View(QuizViewModel.FromDomain(CurrentResource));
        }

        [HttpPost]
        public virtual ActionResult Index(string id, QuizResultViewModel model)
        {
            var x = CurrentResource.CanUserTakeQuiz(
                CurrentUser,
                DateTime.UtcNow,
                Results.GetResults(CurrentResource, CurrentUser));
            if (!x.Available)
            {
                return View("Unavailable", x);
            }

            // score the quiz
            var result = CurrentResource.Score(CurrentUser, model.Answers);

            // possibly award points
            if (result.Passed &&
                result.PointsEarned.HasValue &&
                result.PointsEarned > 0)
            {
                var tx = TryAwardPoints(result.PointsEarned.Value);
                if (null != tx)
                {
                    result.Transaction = tx.Document.Id;
                }
            }
            Results.Save(result);

            // show the review screen
            return RedirectToAction(MVC.Site.Quiz.Review(id, result.Document.Id));
        }

        [HttpGet]
        public virtual ActionResult Review(string id, string r)
        {
            // validate that we can view this result
            var result = Results.Get(r);
            if (result.Quiz != CurrentResource.Document.Id ||
                result.User != CurrentUser.Document.Id)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot view this result.");
            }

            return View(QuizReviewModel.FromDomain(CurrentResource, result));
        }
    }
}