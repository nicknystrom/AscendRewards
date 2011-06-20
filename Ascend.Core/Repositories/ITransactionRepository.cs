using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        IEnumerable<Account> GetAccountsForBudgetRefresh();
    }

    public interface ITransactionRepository : IRepository<Transaction>
    {
        IList<Transaction> GetTransactionsForAccount(string account);
    }
}