using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RedBranch.Hammock;

namespace Ascend.Core
{
    public class Entity : IHasDocument
    {
        [JsonIgnore] public Document Document { get; set; }

        public EntityActivity Created { get; set; }
        public EntityActivity Updated { get; set; }
    }

    public class EntityActivity
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public string SourceId { get; set; }
    }

    /// <summary>
    /// Importable entity that can track locked field states, allowing users to edit
    /// fields in the entity and have those values not be overwritten during the next import.
    /// </summary>
    public class ImportableEntity : Entity
    {
        /// <summary>
        /// Gets or sets the locked fields which will not be overwritten when newly imported
        /// data is applied to this entity.
        /// </summary>
        /// <value>
        /// The locked fields.
        /// </value>
        public IList<string> LockedFields { get; set; }

        /// <summary>
        /// Sets the imported field indicated by <paramref name="field"/>, but only if the field
        /// is not part of the <see cref="LockedFields"/> set.
        /// </summary>
        /// <param name='field'>
        /// Field whose value is to be set.
        /// </param>
        /// <param name='value'>
        /// The value to set the field to.
        /// </param>
        public void SetImportedField(string field, object value)
        {
            if (null == field)
            {
                throw new ArgumentNullException("field", "Field name cannot be null.");
            }
            if (null != LockedFields && LockedFields.Contains(field))
            {
                return;
            }
            var property = this.GetType().GetProperty(field);
            if (null == property)
            {
                throw new InvalidOperationException(String.Format("Attempted to set property that did not exist: '{0}'.", field));
            }
            var method = property.GetSetMethod();
            if (null == method)
            {
                throw new InvalidOperationException(String.Format("Attempted to set read-only property: '{0}'.", field));
            }
            method.Invoke(this, new object[] { value });
        }
    }
}