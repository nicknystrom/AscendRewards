using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(Session sx) : base(sx)
        {
        }
    }

    public class GameResultRepository : Repository<GameResult>, IGameResultRepository
    {
        public GameResultRepository(Session sx) : base(sx)
        {
        }

        public IList<GameResult> GetResults(Game g, User u)
        {
            return Where(x => x.User).Eq(u.Document.Id)
                    .And(x => x.Game).Eq(g.Document.Id)
                    .ToList();
        }
    }
}
