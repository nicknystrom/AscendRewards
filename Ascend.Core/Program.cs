using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Services;
using Newtonsoft.Json;

namespace Ascend.Core
{
    public abstract class Program : Resource
    {
        [JsonIgnore]
        public string Title { get { return null == Content ? "-" : Content.Title.Or("-"); } }

        public Content Content { get; set; }

        public Issuance Issuance { get; set; }
    }

    public class Issuance
    {
        /// <summary>
        /// The source of points for a program. Null for a Nominator issuance type,
        /// or a specific budget account for ProgramBudget issuance.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Determines from where a program's points are drawn, if any.
        /// </summary>
        public IssuanceType Type { get; set; }

        /// <summary>
        /// If set, the program can only issue awards in this amount, period. Overrides any game
        /// or quiz point calculation.
        /// </summary>
        public int? FixedIssuance { get; set; }

        /// <summary>
        /// If set, the lower range of award allowed, to prevent 'insulstingly' low awards. Overrides any game
        /// or quiz point calculation.
        /// </summary>
        public int? MinIssuance { get; set; }

        /// <summary>
        /// If set, the high range of award allowed, to prvent excessive awards. Overrides any game
        /// or quiz point calculation.
        /// </summary>
        public int? MaxIssuance { get; set; }

        /// <summary>
        /// If set, the suggested level of award for this program.
        /// </summary>
        public int? DefaultIssuance { get; set; }
    }

    public enum IssuanceType
    {
        /// <summary>
        /// The program does not issue points.
        /// </summary>
        None = 0,

        /// <summary>
        /// The program issues points from a specific budget account, which will be created automatically if needed.
        /// </summary>
        ProgramBudget = 2,

        /// <summary>
        /// The program issues points from the nominators own budget account rather than a shared program budget.
        /// </summary>
        NominatorBudget = 3,
    }
}
