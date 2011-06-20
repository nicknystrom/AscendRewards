using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Survey : Program
    {
        public IList<SurveyQuestion> Questions { get; set; }
        public int? MaxTimes { get; set; }
        public int? Points { get; set; }

        public SurveyResult Score(
            User user,
            string[] answers)
        {
            if (answers.Length != Questions.Count)
            {
                throw new ArgumentOutOfRangeException("answers", "Count of answers must equal the count of Questions in order to score the survey.");
            }

            // create a result structure
            var r = new SurveyResult
            {
                User = user.Document.Id,
                Survey = Document.Id,
                Title = Content.Title,
                Taken = DateTime.UtcNow,
                PointsEarned = Points,
                Answers = new List<SurveyResultAnswer>(),
            };

            // score questions 1 by 1
            for (var i = 0; i < Questions.Count; i++)
            {
                var q = Questions[i];
                r.Answers.Add(new SurveyResultAnswer
                {
                    Question = q.Title,
                    Answer = q.Freeform ? answers[i] : q.Answers[int.Parse(answers[i])].Title,
                });
            }

            return r;
        }
    }

    public class SurveyQuestion
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Freeform { get; set; }
        public IList<SurveyAnswer> Answers { get; set; }
    }

    public class SurveyAnswer
    {
        public string Title { get; set; }
    }

    public class SurveyResult : Entity
    {
        public string User { get; set;}
        public string Survey { get; set; }
        public string Title { get; set; }
        public DateTime Taken { get; set; }
        public IList<SurveyResultAnswer> Answers { get; set; }
        public int? PointsEarned { get; set; }
        public string Transaction { get; set; }
    }
    
    public class SurveyResultAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

}
