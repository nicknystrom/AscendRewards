using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IQuizRepository : IRepository<Quiz>
    {
    }

    public interface IQuizResultRepository : IRepository<QuizResult>
    {
        IList<QuizResult> GetResults(Quiz q, User u);
    }
}