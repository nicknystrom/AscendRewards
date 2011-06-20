using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(Session sx) : base(sx)
        {
        }
    }

    public class QuizResultRepository : Repository<QuizResult>, IQuizResultRepository
    {
        public QuizResultRepository(Session sx) : base(sx)
        {
        }

        public IList<QuizResult> GetResults(Quiz q, User u)
        {
            return Where(x => x.User).Eq(u.Document.Id)
                    .And(x => x.Quiz).Eq(q.Document.Id)
                    .ToList();
        }
    }
}
