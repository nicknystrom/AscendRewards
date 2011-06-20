using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Game : Program
    {
        public GameType Type { get; set; }
        public int? Award { get; set; }
        public int? TimeLimit { get; set; }

        public GameResult Score(User currentUser, int time, int score)
        {
            return new GameResult
                       {
                           User = currentUser.Document.Id,
                           Game = this.Document.Id,
                           Title = Content.Title,
                           Taken = DateTime.UtcNow,
                           Time = time,
                           Score = score,
                           PointsEarned = null == TimeLimit
                            ? this.Award
                            : time <= TimeLimit ? this.Award : null,
                       };
        }
    }

    public class GameResult : Entity
    {
        public string User { get; set; }
        public string Game { get; set; }
        public string Title { get; set; }
        public DateTime Taken { get; set; }
        
        public int Time { get; set; }
        public int Score { get; set; }

        public int? PointsEarned { get; set; }
        
        /// <summary>
        /// References the transaction created from
        /// this game play attempt.
        /// </summary>
        public string Transaction { get; set; }
   
    }

    public enum GameType
    {
        Match3 = 10,   
    }
}
