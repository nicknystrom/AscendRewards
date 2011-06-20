using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core;
using Ascend.Core.Repositories;
using RedBranch.Hammock;

namespace Ascend.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(Session sx) : base(sx)
        {
        }

        public IEnumerable<Account> GetAccountsForBudgetRefresh()
        {
            return WithView(
                "_accounts-with-budgets",
                @"
                function (doc) {
                  if (doc._id.indexOf('account-') === 0 &&
                      doc.Budget != undefined &&
                      doc.RefreshLimit != undefined &&
                      doc.RefreshLimit > 0 &&
                      doc.RefreshInterval != undefined &&
                      doc.RefreshInterval > 0) {
                    emit(null, null);
                  }
                }
                ")
                .All()
                .WithDocuments()
                .ToList();
        }
    }

    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(Session sx) : base(sx)
        {
        }

        public IList<Transaction> GetTransactionsForAccount(string account)
        {
            return WithView(
                "_ledger",
                @"
                function (doc) {
                  if (doc._id.indexOf('transaction-') === 0) {
                    emit(doc.Debit, doc.Amount * -1);
                    emit(doc.Credit, doc.Amount);
                  }
                }")
                .From(account)
                .To(account)
                .WithDocuments()
                .ToList();
        }
    }
}
