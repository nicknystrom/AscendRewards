using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ascend.Core;
using Ascend.Core.Services;
using Ascend.Core.Services.Caching;

namespace Ascend.Web.Admin.Controllers
{
    #region TransactionEditModel

    public class TransactionEditModel
    {
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
    }

    #endregion

    public partial class LedgerController : AdminController
    {
        public IEntityCache<Account> Accounts { get; set; }
        public IAccountingService Accounting { get; set; }

        [HttpGet]
        public virtual ActionResult Index(string account)
        {
            return PartialView("Index", Accounting.GetLedger(Accounts[account]));
        }

        [HttpPost]
        public virtual ActionResult Post(TransactionEditModel tx)
        {
            var x = 
                Accounting.CreateTransaction(
                    tx.DebitAccount,
                    tx.CreditAccount,
                    tx.Amount,
                    tx.Description,
                    null,
                    null,
                    null);

            Notifier.Notify(Severity.Success, "Transaction created.", "", tx);
            return PartialView("Index", Accounting.GetLedger(Accounts[x.Credit]));
        }
    }
}