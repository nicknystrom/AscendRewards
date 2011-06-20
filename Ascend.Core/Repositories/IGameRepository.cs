using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
    }

    public interface IGameResultRepository : IRepository<GameResult>
    {
        IList<GameResult> GetResults(Game g, User u);
    }
}