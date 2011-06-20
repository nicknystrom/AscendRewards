using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services.Validations.General
{
    [Category("System")]
    [DisplayName("Database is up-to-date.")]
    [Description("Checks for any pending migration that must be performed.")]
    public class DatabaseIsUpdated : IValidation
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
