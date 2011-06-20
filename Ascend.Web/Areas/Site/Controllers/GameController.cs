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
    public class GameViewModel
    {
        public ContentViewModel Content { get; set; }
        public int? TimeLimit { get; set; }
        
        public static GameViewModel FromDomain(Game g)
        {
            return new GameViewModel
            {
                Content = ContentViewModel.FromDomain(g.Content),
                TimeLimit = g.TimeLimit,
            };
        }
    }

    public class GameResultViewModel
    {
        public int Time { get; set; }
        public int Score { get; set; }
    }

    public class GameReviewModel
    {
        public Game Game { get; set; }
        public GameResult Result { get; set; }
        public int Time { get; set; }
        public int Score { get; set; }
        public int? PointsEarned { get; set; }

        public static GameReviewModel FromDomain(Game g, GameResult r)
        {
            return new GameReviewModel
                       {
                           Game = g,
                           Result = r,
                           Time = r.Time,
                           Score = r.Score,
                           PointsEarned = r.PointsEarned,
                       };
        }
    }

    
    public partial class GameController : ProgramController<Game>
    {
        public IGameRepository Gamees { get; set; }
        public IGameResultRepository Results { get; set; }

        protected override Game GetResource(string id)
        {
            return Gamees.Get(id);
        }

        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            var model = GameViewModel.FromDomain(CurrentResource);
            switch (CurrentResource.Type)
            {
                case GameType.Match3:
                    return View("Match3", model);
            }
            throw new HttpException((int) HttpStatusCode.NotFound, "Game type not found.");
        }

        [HttpPost]
        public virtual ActionResult Index(string id, GameResultViewModel model)
        {
            // score the Game
            var result = CurrentResource.Score(CurrentUser, model.Time, model.Score);

            // possibly award points
            if (result.PointsEarned.HasValue &&
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
            return Request.IsAjaxRequest()
                ? (ActionResult) Content(Url.Action(MVC.Site.Game.Review(id, result.Document.Id)))
                : RedirectToAction(MVC.Site.Game.Review(id, result.Document.Id));
        }

        [HttpGet]
        public virtual ActionResult Review(string id, string r)
        {
            // validate that we can view this result
            var result = Results.Get(r);
            if (result.Game != CurrentResource.Document.Id ||
                result.User != CurrentUser.Document.Id)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, "You cannot view this result.");
            }

            return View(GameReviewModel.FromDomain(CurrentResource, result));
        }
    }
}