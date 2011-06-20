using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    public interface IMessagingSender
    {
        EmailSendAttempt Send(Email email);
    }
}
