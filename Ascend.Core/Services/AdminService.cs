using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedBranch.Hammock;

namespace Ascend.Core.Services
{
    public interface IAdminService
    {
        bool Authenticate(string user, string pwd);
    }

    public class AdminService : IAdminService
    {
        Session _session;
        static object _lock = new object();
        static Admin _admin = null;

        public AdminService(Session tenants)
        {
            _session = tenants;
        }

        public bool Authenticate(string user, string pwd)
        {
            if (null == _admin)
                lock (_lock)
                    if (null == _admin)
                        _admin = _session.Load<Admin>("admin");
        
            return null != _admin &&
                   _admin.Accounts.ContainsKey(user) &&
                   _admin.Accounts[user].Password == pwd;
        }
    }
}
