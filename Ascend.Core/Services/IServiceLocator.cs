using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    /// <summary>
    /// Provide the thinnest possibel service locator interface.
    /// </summary>
    public interface IServiceLocator
    {
        TService Resolve<TService>();
    }

    public static class ServiceLocator
    {
        public static IServiceLocator CurrentServiceLocator { get; set; }

        public static TService Resolve<TService>()
        {
            if (null == CurrentServiceLocator)
                throw new InvalidOperationException("You must set a CurrentServiceLocator first.");
            return CurrentServiceLocator.Resolve<TService>();
        }
    }
}
