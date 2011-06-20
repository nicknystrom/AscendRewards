using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascend.Core.Repositories;
using Ascend.Core.Services.Caching;
using RedBranch.Hammock;

namespace Ascend.Core.Services
{
    #region IAccountingService

    public interface IAccountingService
    {
        Account GetPointsAccount(User user);
        Ledger GetLedger(Account account);
        Ledger GetPointsLedger(User user);
        Ledger GetBudgetLedger(User user);
        Ledger GetBudgetLedger(User user, bool create, Budget defaults);

        Transaction CreateTransaction(
            string debit,
            string credit,
            int amount,
            string description,
            string program,
            DateTime? date,
            IDictionary<string, object> data);

        Transaction CreateProgramAward(
            Program program,
            User nominator,
            User nominee,
            int amount,
            string description);

        Transaction CreateOrderPayment(
            User user,
            Order order);

        Transaction CreateBudgetTransfer(
            User from,
            User to,
            int amount,
            string message);

        void RefreshBudgets(DateTime utc);
        Account GetBudgetAccount(User user, bool create, Budget defaults);
        IList<UserSummary> GetUsersManangerCanDisitributeBudgetTo(User manager);

        void SetUserBudget(User user, Budget budget);

        /// <summary>
        /// Given a program and (optional) nominator, determine what the max number of
        /// awarded points could be. Takes into account program issuance parameters and
        /// actual point balances of budget accounts.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="nominator"></param>
        /// <returns>The maximum amount of allowed points that could be awarded, or null
        /// if a fixed or a minimum point value is not availble in the source account.</returns>
        int? GetMaxProgramAward(
            Program program,
            User nominator);

        /// <summary>
        /// Given a program and (optional) nominator, determines the account from which
        /// awarded points will be drawn.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="nominator"></param>
        /// <returns></returns>
        Account GetProgramAwardSource(
            Program program,
            User nominator);

        Account UpdateProgramAwardSource(Program p, Budget budget);
        bool CanDistributeBudget(User fromManager, UserSummary toUser);
    }

    #endregion
    #region Ledger

    public class Ledger
    {
        public Account Account { get; set;}
        public IList<Transaction> Transactions { get; set; }

        public bool IsDebit(Transaction x)
        {
            return x.Debit == Account.Document.Id;
        }

        public bool IsCredit(Transaction x)
        {
            return x.Credit == Account.Document.Id;
        }

        public int Balance
        {
            get { return TotalCredits - TotalDebits; }
        }

        public int TotalDebits
        {
            get { return Debits.Sum(x => x.Amount); }
        }

        public int TotalCredits
        {
            get { return Credits.Sum(x => x.Amount); }
        }

        public IEnumerable<Transaction> Debits
        {
            get { return Transactions.Where(IsDebit); }
        }

        public IEnumerable<Transaction> Credits
        {
            get { return Transactions.Where(IsCredit); }
        }

        public bool AllowCredit(Transaction tx)
        {
            return true;
        }

        public bool AllowDebit(Transaction tx)
        {
            var balanceCurrent = Balance;
            var balanceNew = balanceCurrent - tx.Amount;

            if (Account.Type == AccountType.User ||
                Account.Type == AccountType.Budget)
            {
                // user accounts may not have a negative balance
                if (balanceNew < 0) return false;
            }
            
            return true;
        }

        public void Timeline(Action<int, Transaction> func)
        {
            var b = 0;
            foreach (var t in Transactions.OrderBy(x => x.Date))
            {
                b += IsCredit(t) ? t.Amount : -1 * t.Amount;
                func(b, t);
            }
        }
    }

    #endregion
    #region ProgramLiabilityLedger

    public class ProgramLiabilityLedger
    {
        Ledger _control, _expense;

        public ProgramLiabilityLedger(
            Ledger control,
            Ledger expense)
        {
            _control = control;
            _expense = expense;
        }

        public int TotalLiability
        {
            get { return _control.TotalDebits - _expense.TotalCredits; }
        }

        /// <summary>
        /// Day, total points distributed, total spent, current liability.
        /// </summary>
        public void Timeline(Action<DateTime, int, int, int> func)
        {
            var merged = _control.Debits
                    .Concat(_expense.Credits)
                    .OrderBy(x => x.Date);

            var issued = 0;
            var spent = 0;
            var d = DateTime.MinValue;
            
            foreach (var tx in merged)
            {
                // prevent issuing a bogus first entry
                if (d == DateTime.MinValue)
                {   
                    d = tx.Date.Date;
                }

                // is this tx on a new day?
                if (tx.Date.Date > d)
                {
                    func(d, issued, spent, issued-spent);
                    d = tx.Date.Date;
                }

                // ok, same day, lets bump some of these numbers up
                if (tx.Debit == _control.Account.Document.Id)
                {
                    issued += tx.Amount;
                }
                else
                {
                    spent += tx.Amount;
                }
            }

            // output the final line
            func(d, issued, spent, issued - spent);
        }
    }

    #endregion

    public class AccountingService : IAccountingService
    {
        public IApplicationConfiguration Application { get; set; }
        public ICacheStore Cache { get; set; }
        public IEntityCache<Account> Accounts { get; set; }

        public ITransactionRepository TransactionRepository { get; set; }
        public IAccountRepository AccountRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IUserSummaryCache UserSummaries { get; set; }

        #region Users & their points

        /// <summary>
        /// Retrieves the points account for a given user, creating that account
        /// record if it does not yet exist.
        /// </summary>
        /// <param name="user">The user whose points account should be retrieved.</param>
        /// <returns>The points account for the given user.</returns>
        public Account GetPointsAccount(User user)
        {
            if (String.IsNullOrEmpty(user.PointsAccount))
            {
                // create the points account and associate to the user
                var a = new Account
                {
                    Document = new Document
                    {
                        Id = Document.For<Account>(user.Document.Id),
                    },
                    Name = user.DisplayName + "'s Points Account",
                    Type = AccountType.User,
                };
                AccountRepository.Save(a);
                user.PointsAccount = a.Document.Id;
                UserRepository.Save(user);
                return a;
            }
            return Accounts[user.PointsAccount];
        }

        /// <summary>
        /// Retrieves the points ledger for a given user. The account will be created
        /// if it does not already exist.
        /// </summary>
        /// <param name="user">The user whose points ledger should be retrieved.</param>
        /// <returns>The points ledger for the given user.</returns>
        public Ledger GetPointsLedger(User user)
        {
            return GetLedger(GetPointsAccount(user));
        }

        /// <summary>
        /// Pays for a user's order by transfering the order's total from the user's
        /// points account to the general expense account.
        /// </summary>
        /// <param name="user">The user who placed the order.</param>
        /// <param name="order">The order to be payed for.</param>
        /// <returns>The resulting transaction.</returns>
        public Transaction CreateOrderPayment(
            User user,
            Order order)
        {
            return CreateTransaction(
                GetPointsAccount(user).Document.Id,
                Application.GeneralExpenseAccount,
                order.Total,
                String.Format(
                    "Payment for {0} items, ordered on {1}.",
                    order.Items.Length,
                    order.Ordered.ToShortDateString()),
                null,
                order.Ordered,
                new Dictionary<string, object> {{ "order", order.Document.Id }});
        }

        #endregion
        #region Users & their budgets

        /// <summary>
        /// Performs a transfer from a manager's budget account to another user's
        /// points account. No validation is performed with regards to whether the
        /// particular user should be able to receive points from the manager.
        /// </summary>
        /// <param name="from">The manager from whose budget account points will be drawn.</param>
        /// <param name="to">The user into whose points account the points will be deposited.</param>
        /// <param name="amount">The number of points to transfer.</param>
        /// <param name="message">Descriptive reason for the transfer. A default message will be supplied if null or empty.</param>
        /// <returns>The resulting transaction.</returns>
        public Transaction CreateBudgetTransfer(
            User from,
            User to,
            int amount,
            string message)
        {
            return CreateTransaction(
                GetBudgetAccount(from, true, null).Document.Id,
                GetPointsAccount(to).Document.Id,
                amount,
                message.Or("Budget award from " + from.DisplayName),
                from.Document.Id,
                DateTime.UtcNow,
                null);
        }

        /// <summary>
        /// Gets the budget account for user, creating that account with the given budget
        /// parameters if specified.
        /// </summary>
        /// <param name="user">The user whose budget account is to be retrieved.</param>
        /// <param name="create">True if a budget account should be created if it doesn't already exist.</param>
        /// <param name="defaults">The budget parameters to use for a new account if one is created.</param>
        /// <returns>The budget accoutn for the specified user, or null if the user has no budget
        /// account and create was false.</returns>
        public Account GetBudgetAccount(User user, bool create, Budget defaults)
        {
            if (String.IsNullOrEmpty(user.BudgetAccount))
            {
                if (!create)
                {
                    return null;
                }

                // create the budget account and associate to the user
                var a = new Account
                {
                    Document = new Document
                    {
                        Id = Document.For<Account>(user.Document.Id + "-budget"),
                    },
                    Name = user.DisplayName + "'s Budget Account",
                    Type = AccountType.Budget,
                    Budget = defaults,
                };
                AccountRepository.Save(a);
                user.BudgetAccount = a.Document.Id;
                UserRepository.Save(user);
                return a;
            }
            return Accounts[user.BudgetAccount];
        }

        /// <summary>
        /// Gets the budget ledger for a given user. Will create the a new budget
        /// with the given default if that user does not already have a budget.
        /// </summary>
        /// <param name="user">The user whose budget is to be retrieved.</param>
        /// <param name="create">True to allow a budget to be created.</param>
        /// <param name="defaults">The budget parameters to use if a new budget is to be created.</param>
        /// <returns>The budget ledger for the given user.</returns>
        public Ledger GetBudgetLedger(User user, bool create, Budget defaults)
        {
            var account = GetBudgetAccount(user, create, defaults);
            if (null == account)
            {
                return null;
            }
            return GetLedger(account);
        }

        /// <summary>
        /// Gets the budget ledger for a user. Does not automatically create a budget account.
        /// </summary>
        /// <param name="user">The user whose budget ledger will be retrieved.</param>
        /// <returns>The budget ledger for the given user, or null if user has no budget.</returns>
        public Ledger GetBudgetLedger(User user)
        {
            return GetBudgetLedger(user, false, null);
        }


        public void SetUserBudget(User user, Budget budget)
        {
            var create = (null != budget &&
                          budget.RefreshLimit.HasValue &&
                          budget.RefreshLimit.Value > 0);
            var ledger = GetBudgetLedger(user, create, budget);
            if (null != ledger &&
                null != ledger.Account)
            {
                ledger.Account.Budget = budget;
                AccountRepository.Save(ledger.Account);
            }
        }

        #endregion
        #region Programs & their budgets

        /// <summary>
        /// Given a program and (optional) nominator, determine what the max number of
        /// awarded points could be. Takes into account program issuance parameters and
        /// actual point balances of budget accounts.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="nominator"></param>
        /// <returns>The maximum amount of allowed points that could be awarded, or null
        /// if a fixed or a minimum point value is not availble in the source account.</returns>
        public int? GetMaxProgramAward(
            Program program,
            User nominator)
        {
            // if the program has no source, we can't award anything
            var account = GetProgramAwardSource(program, nominator);
            if (null == account)
            {
                return null;
            }

            var ledger = GetLedger(account);
            var balance = ledger.Balance;

            // fixed issuance?
            if (program.Issuance.FixedIssuance.HasValue)
            {
                return balance < program.Issuance.FixedIssuance
                    ? null
                    : program.Issuance.FixedIssuance;
            }

            // min issuance?
            if (program.Issuance.MinIssuance.HasValue &&
                balance < program.Issuance.MinIssuance)
            {
                return null;
            }

            // enough to points to award.. but possibly capped by max issuance
            if (program.Issuance.MaxIssuance.HasValue)
            {
                return Math.Min(balance, program.Issuance.MaxIssuance.Value);
            }

            return balance;
        }

        /// <summary>
        /// Given a program and (optional) nominator, determines the account from which
        /// awarded points will be drawn.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="nominator"></param>
        /// <returns></returns>
        public Account GetProgramAwardSource(
            Program program,
            User nominator)
        {
            if (null == program) throw new ArgumentNullException("program");
            if (null == program.Issuance)
            {
                return null;
                //throw new InvalidOperationException("Program awards cannot be made from programs with no issuance parameters specified.");
            }

            switch (program.Issuance.Type)
            {
                case IssuanceType.None:
                    return null;

                case IssuanceType.NominatorBudget:
                    if (null == nominator)
                    {
                        throw new InvalidOperationException("Program is designed to award points from the nominator's budget, but no nominator specified.");
                    }
                    if (String.IsNullOrEmpty(nominator.BudgetAccount))
                    {
                        throw new InvalidOperationException("Program is designed to award points from the nominator's budget, but nominator has no budget.");
                    }
                    return Accounts.Try(x => x[nominator.BudgetAccount]);

                case IssuanceType.ProgramBudget:
                    if (String.IsNullOrEmpty(program.Issuance.Account))
                    {
                        throw new InvalidOperationException("Program is designed to award points from a program budget account, but none exist.");
                    }
                    return Accounts.Try(x => x[program.Issuance.Account]);
            }            

            throw new Exception("Unknown program issuance type.");
        }


        public Account UpdateProgramAwardSource(Program p, Budget budget)
        {
            if (null == p.Issuance) return null;

            if (p.Issuance.Type == IssuanceType.NominatorBudget ||
                p.Issuance.Type == IssuanceType.None)
            {
                p.Issuance.Account = null;
                return null;
            }

            // p.Issuance.Type == IssuanceType.ProgramBudget
            if (String.IsNullOrWhiteSpace(p.Issuance.Account))
            {
                p.Issuance.Account = "account-" + p.Document.Id;
            }
            var a = Accounts.TryGet(p.Issuance.Account);
            if (null == a)
            {
                a = new Account
                        {
                            Document = new Document { Id = p.Issuance.Account },
                            Type = AccountType.Program,
                            Name = "Program Budget Account for " + p.Title,
                            Budget = budget,
                        };
                
            }
            else
            {
                a.Budget = budget;
            }
            AccountRepository.Save(a);

            return a;
        }

        /// <summary>
        /// Transfers points from the program's point source account (can be either: control account,
        /// the nominator's user budget account, or a specific program budget account, depending on the
        /// program's setup) into the user's points account.
        /// </summary>
        /// <param name="program"></param>
        /// <param name="nominator"></param>
        /// <param name="nominee"></param>
        /// <param name="amount"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Transaction CreateProgramAward(
            Program program,
            User nominator,
            User nominee,
            int amount,
            string description)
        {
            var source = GetProgramAwardSource(program, nominator);
            var ledger = GetLedger(source);
            var max = GetMaxProgramAward(program, nominator);
            if (null == max)
            {
                throw new InvalidOperationException("Insufficient points to satisfy the minimum award level for this program.");
            }
            if (amount > ledger.Balance)
            {
                throw new InvalidOperationException("Insufficient points in source account to complete this program award.");
            }
            if (program.Issuance.FixedIssuance.HasValue &&
                program.Issuance.FixedIssuance.Value != amount)
            {
                throw new InvalidOperationException("Program can only award a fixed point value.");
            }
            if (amount > max)
            {
                throw new InvalidOperationException("Award value is higher than the maximum award allowed for this program.");
            }

            return CreateTransaction(
                source.Document.Id,
                GetPointsAccount(nominee).Document.Id,
                amount,
                description,
                program.Document.Id,
                null,
                null);
        }

        #endregion
        #region Core methods: GetLedger, CreateTransaction, RefreshBudgets

        public Ledger GetLedger(Account account)
        {
            if (null == account) return null; 
            
            var key = "ledger-" + account.Document.Id;
            if (Cache.ContainsKey(key))
            {
                return Cache.Get<Ledger>(key);
            }

            var l = new Ledger
            {
                Account = account,
                Transactions = TransactionRepository.GetTransactionsForAccount(account.Document.Id),
            };

            // cache user account for 30 minutes, cache the general control and
            // expense accounts for longer
            var ts = (account.Type == AccountType.Control)
                         ? TimeSpan.FromHours(12)
                         : TimeSpan.FromMinutes(30);

            Cache.Put(key, l, ts);
            return l;
        }

        public Transaction CreateTransaction(
            string debit,
            string credit,
            int amount,
            string description,
            string program,
            DateTime? date,
            IDictionary<string, object> data)
        {
            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException("amount", "Transactions must be for an amount greater than zero.");
            }

            // create the transaction record
            var tx = new Transaction
            {
                Description = description,
                Data = data,

                Date = (date ?? DateTime.UtcNow),
                Program = program,
                Debit = debit,
                Credit = credit,
                Amount = amount,
            };

            // validate against each ledger
            var ledgerDebit = GetLedger(Accounts[debit]);
            var ledgerCredit = GetLedger(Accounts[credit]);
            if (!ledgerDebit.AllowDebit(tx))
            {
                throw new InvalidOperationException("Debit cannot be made against account.");
            }
            if (!ledgerCredit.AllowCredit(tx))
            {
                throw new InvalidOperationException("Credit cannot be made against account.");
            }

            // persist and update both ledgers
            TransactionRepository.Save(tx);
            ledgerDebit.Transactions.Add(tx);
            ledgerCredit.Transactions.Add(tx);
            return tx;
        }

        public bool CanDistributeBudget(User fromManager, UserSummary toUser)
        {
            // user directly report to current user or be in one of the
            // groups this user manages
            return toUser.Manager == fromManager.Document.Id ||
                     (!String.IsNullOrEmpty(toUser.Group) &&
                      fromManager.ManagedGroups != null &&
                      fromManager.ManagedGroups.Contains(toUser.Group));
        }

        public IList<UserSummary> GetUsersManangerCanDisitributeBudgetTo(User manager)
        {
            return UserSummaries.All.Where(x => x.Id != manager.Document.Id && CanDistributeBudget(manager, x)).ToList();
        }

        public void RefreshBudgets(DateTime utc)
        {
            var forRefresh = AccountRepository.GetAccountsForBudgetRefresh();
            foreach (var a in forRefresh.Where(x => null != x.Budget &&
                                                    x.Budget.RefreshLimit.HasValue &&
                                                    x.Budget.RefreshLimit.Value > 0 &&
                                                    x.Budget.RefreshInterval != BudgetRefreshInterval.None))
            {
                // all accounts for refresh have a limit and an interval, however even if
                // the 'NextRefresh' is not set, refresh the budget right now
                if (!a.Budget.NextRefresh.HasValue ||
                     a.Budget.NextRefresh.Value <= utc)
                {
                    // top up points if needed
                    var ledger = GetLedger(a);
                    var balance = ledger.Balance;
                    var effective = a.Budget.NextRefresh ?? utc;
                    if (balance < a.Budget.RefreshLimit)
                    {
                        CreateTransaction(
                            Application.GeneralControlAccount,
                            ledger.Account.Document.Id,
                            a.Budget.RefreshLimit.Value - balance,
                            "Scheduled Budget Refresh (" + effective.ToString("MMMM yyyy") + ")",
                            a.Document.Id,
                            effective,
                            null);
                    }

                    // set the next refresh regardless
                    switch (a.Budget.RefreshInterval)
                    {
                        case BudgetRefreshInterval.None:
                            a.Budget.NextRefresh = null;
                            break;
                        case BudgetRefreshInterval.Weekly:
                            a.Budget.NextRefresh = effective.AddDays(7);
                            break;
                        case BudgetRefreshInterval.Monthly:
                            a.Budget.NextRefresh =
                                new DateTime(
                                    effective.AddMonths(1).Year,
                                    effective.AddMonths(1).Month,
                                    1);
                            break;
                        case BudgetRefreshInterval.SemiAnnually:
                            a.Budget.NextRefresh =
                                new DateTime(
                                    effective.AddMonths(6).Year,
                                    effective.AddMonths(6).Month,
                                    1);
                            break;
                        case BudgetRefreshInterval.Annually:
                            a.Budget.NextRefresh =
                                new DateTime(
                                    effective.AddYears(1).Year,
                                    effective.AddYears(1).Month,
                                    1);
                            break;
                    }

                    AccountRepository.Save(a);
                }
            }
        }

        #endregion
    }
}
