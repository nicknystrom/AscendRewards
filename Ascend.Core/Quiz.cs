using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Quiz : Program
    {
        public IList<Question> Questions { get; set; }

        /// <summary>
        /// Number of times the quiz may be taken, regardless of outcome. Once the limit is reached,
        /// the quiz can no longer be taken by that user.
        /// </summary>
        public int? MaxTaken { get; set; }

        /// <summary>
        /// Number of times the quiz may award points to a user. Often set to 1.
        /// </summary>
        public int? MaxAwarded { get; set; }

        /// <summary>
        /// Timespan (usually in increments of days) between quiz attempts. Keeps the user from taking
        /// the quiz over and over until they get it right.
        /// </summary>
        public TimeSpan? Cooldown { get; set; }

        /// <summary>
        /// If true, the user will be allowed to review correctly and incorrectly answered
        /// questions at the end of the quiz.
        /// </summary>
        public bool AllowReview { get; set; }

        /// <summary>
        /// The minimum number of correctly answered questions required to award
        /// points. If zero, no correct answers are required; any result will yield points.
        /// If Null, <strong>all</strong> questions must be correctly answered.
        /// </summary>
        public int? CorrectAnswersRequired { get; set; }

        /// <summary>
        /// If not Null, a passing quiz (<see cref="CorrectAnswersRequired"/>) will yield
        /// exactly this point value. If unset, the point value of correctly answered questions
        /// is totalled and awarded to the user.
        /// </summary>
        public int? FlatPointValue { get; set; }

        public AvailabilityResult CanUserTakeQuiz(
            User u,
            DateTime date,
            IList<QuizResult> results)
        {
            if (MaxTaken.HasValue && results.Count >= MaxTaken)
            {
                return AvailabilityResult.No(
                    "This quiz has already been taken the maximum number of times.",
                    "This quiz can only be taken " + MaxTaken.Value + " times.");
            }
            if (MaxAwarded.HasValue && results.Count(x => !String.IsNullOrEmpty(x.Transaction)) >= MaxAwarded)
            {
                return AvailabilityResult.No(
                    "This quiz has already awarded you points the maximum number of times.",
                    "You can only be awarded points by this quiz " + MaxAwarded + " times.");
            }
            if (Cooldown.HasValue &&
                results.Count > 0 &&
                (results.Max(x => x.Taken) + Cooldown.Value < date))
            {
                return AvailabilityResult.No(
                    "This quiz cannot be taken again until " + (results.Max(x => x.Taken) + Cooldown.Value).ToShortDateString() + ".",
                    "You must wait at least " + Cooldown.Value.TotalDays.ToString("n0") + " days between attempts.");
            }
            return AvailabilityResult.Ok;
        }

        /// <summary>
        /// Determines whether each question was answered correctly, whether the overall quiz
        /// was passed or not, and the number of points to award. Does NOT save the QuizResult,
        /// nor actually award the points.
        /// </summary>
        public QuizResult Score(
            User user,
            int[] answers)
        {
            if (answers.Length != Questions.Count)
            {
                throw new ArgumentOutOfRangeException("answers", "Count of answers must equal the count of Questions in order to score the quiz.");
            }

            // create a result structure
            var r = new QuizResult
                        {
                            User = user.Document.Id,
                            Quiz = Document.Id,
                            Title = null == Content ? String.Empty : Content.Title,
                            Taken = DateTime.UtcNow,
                            Answers = new List<QuizResultAnswer>(),
                        };
           
            // score questions 1 by 1
            for (var i=0; i<Questions.Count; i++)
            {
                var q = Questions[i];
                var a = q.Answers[answers[i]];
                r.Answers.Add(new QuizResultAnswer
                                  {
                                      Question = q.Title,
                                      Answer = a.Title,
                                      Correct = a.Correct,
                                      Points = a.Correct ? q.Points : null,
                                  });
            }

            // determine overall passing grade and point reward
            r.RequiredToPass = CorrectAnswersRequired ?? Questions.Count;
            r.Passed = r.Answers.Count(x => x.Correct) >= r.RequiredToPass;
            if (r.Passed)
            {
                r.PointsEarned = FlatPointValue ?? (r.Answers.Sum(x => x.Points ?? 0));
            }
            return r;
        }
    }

    public class Question
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public IList<Answer> Answers { get; set; }

        /// <summary>
        /// Points to be awarded to the user if the quiz is passed and this question
        /// is correctly answered. Ignored if the Quiz's FlatPointValue is set, however.
        /// </summary>
        public int? Points { get; set; }
    }

    public class Answer
    {
        public bool Correct { get; set; }
        public string Title { get; set; }
    }

    public class QuizResult : Entity
    {
        public string User { get; set;}
        public string Quiz { get; set; }
        public string Title { get; set; }
        public DateTime Taken { get; set; }
        public IList<QuizResultAnswer> Answers { get; set; }

        public bool Passed { get; set; }
        public int? PointsEarned { get; set; }
        public int RequiredToPass { get; set; }

        /// <summary>
        /// References the transaction created from
        /// this quiz attempt.
        /// </summary>
        public string Transaction { get; set; }
    }
    
    public class QuizResultAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Correct { get; set; }
        public int? Points { get; set; }
    }

}
