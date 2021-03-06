using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface ISurveyRepository : IRepository<Survey>
    {
    }

    public interface ISurveyResultRepository : IRepository<SurveyResult>
    {
        IList<SurveyResult> GetResults(Survey q, User u);
    }
}