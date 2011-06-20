using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

using Ascend.Core;
using Ascend.Core.Services;
using RedBranch.Hammock;

namespace Ascend.Web.Areas.Site.Controllers
{
    public abstract class ProgramController<TProgram> : ResourceController<TProgram> where TProgram : Program
    {
        protected Transaction TryAwardPointsTo(User nominee, int points)
        {
            try
            {
                return
                Accounting.CreateProgramAward(
                    CurrentResource,
                    CurrentUser,
                    nominee,
                    points,
                    CurrentResource.Content.Title
                );
            }
            catch (Exception)
            {
                Notifier.Notify(
                    Severity.Error,
                    "We couldn't award points to your nominee.", "We're sorry, but there was configuration problem with this program. Please contact technical support so that the issue can be resolved.",
                    CurrentResource);
            }
            return null;
        }

        protected Transaction TryAwardPoints(int points)
        {
            try
            {
                return
                Accounting.CreateProgramAward(
                    CurrentResource,
                    null,
                    CurrentUser,
                    points,
                    CurrentResource.Content.Title
                );
            }
            catch (Exception)
            {
                Notifier.Notify(
                    Severity.Error,
                    "We couldn't award your points.", "We're sorry, but there was configuration problem with this program. Please contact technical support so that the issue can be resolved.",
                    CurrentResource);
            }
            return null;
        }
    }
}