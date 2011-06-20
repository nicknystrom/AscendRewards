using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services.Validations.General
{
    [Category("Users")]
    [DisplayName("Users can access their homepages.")]
    [Description("Ensures that every active system user can succesfully load their assigned homepage.")]
    public class UsersCanAccessHomepages : IValidation
    {
        public IEnumerable<ValidationResult> Validate()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ValidationResult> Fix(string option)
        {
            throw new NotImplementedException();
        }
    }
}
