using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Account : Entity
    {
        public string Name { get; set; }
        public string Purpose { get; set; }
        public AccountType Type { get; set; }
        public Budget Budget { get; set; }
    }

    public enum AccountType
    {
        Control,
        User,
        Budget,
        Program,
    }

    public class Budget
    {
        public int? RefreshLimit { get; set; }
        public BudgetRefreshInterval RefreshInterval { get; set; }

        public DateTime? LastRefreshed { get; set; }
        public DateTime? NextRefresh { get; set; }
    }

    public enum BudgetRefreshInterval
    {
        None = 0,
        Weekly = 1,
        Monthly = 2,
        SemiAnnually = 3,
        Annually = 4,
    }

    public class Transaction : Entity
    {
        public string Description { get; set; }
        public IDictionary<string, object> Data { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// The account from which the points are drawn.
        /// </summary>
        public string Debit { get; set; }
        
        /// <summary>
        /// The account to which the points will be added.
        /// </summary>
        public string Credit { get; set; }

        /// <summary>
        /// The number of points transfered from the debit to the credit account.
        /// Always positive.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Reference to the Quiz, Recognition, etc that generated this
        /// transaction. Useful when these awards draw directly from the control
        /// account and not through a budget account.
        /// </summary>
        public string Program { get; set; }
    }
}
