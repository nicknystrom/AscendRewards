using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Ascend.Core.Services
{
    public interface IUserMessaging
    {
        EmailSendAttempt SendActivation(RequestContext request, User u);
        EmailSendAttempt SendWelcome(RequestContext request, User u);
        EmailSendAttempt SendReset(RequestContext request, User u, string password);
        EmailSendAttempt SendAward(RequestContext request, UserAward award);
    }
}
