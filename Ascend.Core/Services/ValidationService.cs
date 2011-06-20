using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ascend.Core.Services
{
    public interface IValidation
    {
        IEnumerable<ValidationResult> Validate();
        IEnumerable<ValidationResult> Fix(string option);
    }

    public interface IValidationService
    {
        IEnumerable<IValidationMetadata> GetValidations();
        IEnumerable<ValidationResult> Validate(string name);
        IEnumerable<ValidationResult> Fix(string name, string option);
    }

    public interface IValidationMetadata
    {
        string Name { get; }
        string Category { get; }
        string Title { get; }
        string Description { get; }
    }

    public class ValidationResult
    {
        public ValidationResultSeverity Severity { get; set; }
        public string Problem { get; set; }
        public string Details { get; set; }
        public Dictionary<string, string> Fixes { get; set; }
    }

    public enum ValidationResultSeverity
    {
        /// <summary>
        /// Most or all of the application is unlikely to function due to this issue.
        /// </summary>
        Critical,
        /// <summary>
        /// At least one feature of the application is unlikely to function due to this issue.
        /// </summary>
        Serious,
        /// <summary>
        /// This issue is likely to have a noticable impact to usability.
        /// </summary>
        Minor,
        /// <summary>
        /// This issue is likely to have only a minor, non-blocking impact.
        /// </summary>
        Trivial,
    }

    public class ValidationService  : IValidationService
    {
        class Metadata : IValidationMetadata
        {
            public Metadata(Type t)
            {
                Name = t.Name;
                Title = t.Value<DisplayNameAttribute, string>(a => a.DisplayName, t.Name);
                Category = t.Value<CategoryAttribute, string>(a => a.Category, t.Namespace.Split('.').Last());
                Description = t.Value<DescriptionAttribute, string>(a => a.Description, "");
            }

            public string Name { get; private set; }
            public string Category { get; private set; }
            public string Title { get; private set; }
            public string Description { get; private set; }
        }

        public IEnumerable<IValidation> Validations { get; set; }

        public IEnumerable<IValidationMetadata> GetValidations()
        {
            return Validations.Distinct().Select(x => new Metadata(x.GetType()));
        }

        public IEnumerable<ValidationResult> Validate(string name)
        {
            return Validations.First(x => new Metadata(x.GetType()).Name == name).Validate();
        }

        public IEnumerable<ValidationResult> Fix(string name, string option)
        {
            return Validations.First(x => new Metadata(x.GetType()).Name == name).Fix(option);
        }
    }
}
