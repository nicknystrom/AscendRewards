using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class SurveyRepository : Repository<Survey>, ISurveyRepository
    {
        public SurveyRepository(Session sx) : base(sx)
        {
        }
    }

    public class SurveyResultRepository : Repository<SurveyResult>, ISurveyResultRepository
    {
        public SurveyResultRepository(Session sx) : base(sx)
        {
        }

        public IList<SurveyResult> GetResults(Survey q, User u)
        {
            return Where(x => x.User).Eq(u.Document.Id)
                    .And(x => x.Survey).Eq(q.Document.Id)
                    .ToList();
        }
    }
}
