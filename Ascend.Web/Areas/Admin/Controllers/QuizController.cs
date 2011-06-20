
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

using Ascend.Core;
using Ascend.Core.Repositories;
using Ascend.Core.Services;

namespace Ascend.Web.Areas.Admin.Controllers
{
	#region QuizCreateModel

	public class QuizCreateModel
	{
		[Required, StringLength(100)] public string Title { get; set; }

		public Quiz CreateQuiz()
		{
			return new Quiz {
				Content = new Content {
                    Title = Title,
                },
				Document = new Document { Id = Document.For<Quiz>(Title.ToSlug()) },
			};
		}
	}
	
	#endregion
	#region QuizEditModel
	
	public class QuizEditModel : ProgramEditModel
	{
		[DisplayName("Max. times quiz can be taken")] public int? MaxTaken { get; set; }
        [DisplayName("Max. times quiz can award points to a user")] public int? MaxAwarded { get; set; }
        [DisplayName("Days between quiz attempts")] public int? Cooldown { get; set; }

        [DisplayName("Allow the quiz to be reviewed.")] public bool AllowReview { get; set; }
        [DisplayName("Flat quiz value")] public int? FlatPointValue { get; set; }
        [DisplayName("Correct answers required to pass, blank means 100%.")] public int? CorrectAnswersRequired { get; set; }

        public IList<Question> Questions { get; set; }

		public static QuizEditModel FromDomain(Quiz q, IAccountingService accounting)
		{
			var x = new QuizEditModel {
                MaxTaken = q.MaxTaken,
                MaxAwarded = q.MaxAwarded,
                Cooldown = (null == q.Cooldown) ? null : (int?)q.Cooldown.Value.TotalDays,
                AllowReview = q.AllowReview,
                FlatPointValue = q.FlatPointValue,
                CorrectAnswersRequired = q.CorrectAnswersRequired,
                Questions = q.Questions,
			};
            x.Init(q, accounting);
            return x;
		}

        public void Apply(Quiz q, IAccountingService accounting)
		{
		    q.Questions = Questions;
		    q.MaxTaken = MaxTaken;
		    q.MaxAwarded = MaxAwarded;
		    q.Cooldown = Cooldown.HasValue ? (TimeSpan?)TimeSpan.FromDays(Cooldown.Value) : null;
		    q.AllowReview = AllowReview;
		    q.FlatPointValue = FlatPointValue;
		    q.CorrectAnswersRequired = CorrectAnswersRequired;
            base.Apply(q, accounting);
        }
	}
	
	#endregion
	
	
    public partial class QuizController : AdminController
    {
		public IQuizRepository Quizs { get; set; }
        public IAccountingService Accounting { get; set; }

        public IUserSummaryCache UserSummaries { get; set; }
        public IGroupSummaryCache GroupSummaries { get; set; }

		[HttpGet]
		public virtual ActionResult Index()
        {
            return View(Quizs.All().WithDocuments());
        }

        [HttpPost]
        public virtual ActionResult Index(QuizCreateModel q)
        {
			ViewData["q"] = q;
			if (!ModelState.IsValid)
			{
                return View(Quizs.All().WithDocuments());
			}
			
			var x = q.CreateQuiz();
            try
            {
				Quizs.Save(x);
                return RedirectToAction(Actions.Edit, new { id = x.Document.Id });
            }
            catch (Exception ex)
            {
				Notifier.Notify(ex);
                return View(Quizs.All().WithDocuments());
            }
        }

		[HttpGet]
        public virtual ActionResult Edit(string id)
        {
		    var model = QuizEditModel.FromDomain(Quizs.Get(id), Accounting);
            ViewData["groups"] = GroupSummaries;
            ViewData["users"] = UserSummaries[model.Availability.Users ?? new string[0]];
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit(string id, QuizEditModel q)
        {
			var x = Quizs.Get(id);
			if (!ModelState.IsValid)
			{
				return View(q);
			}
            try
            {
                q.Apply(x, Accounting);
				Quizs.Save(x);
                return RedirectToAction(Actions.Index);
            }
            catch (Exception ex)
            {
                ViewData["groups"] = GroupSummaries;
                ViewData["users"] = UserSummaries[q.Availability.Users ?? new string[0]];
				Notifier.Notify(ex);
                return View(q);
            }
        }
    }
}
