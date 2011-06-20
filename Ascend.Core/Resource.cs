using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class Resource : Entity
    {
        /// <summary>
        /// Indicates that the Resource is available to the user, or hidden. A disabled
        /// Resource does not display on any menu, ever, and gives 404 if it is navigated to.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Indicates that the Resource is archived. It is still available to the
        /// user, but in a read-only form, and typically on a seperate menu.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// This Resource is only available during the given time frame. Optional, and
        /// either the From or To may be left empty to indicate no boundary in that direction.
        /// </summary>
        public Availability Availability { get; set; }

        public AvailabilityResult IsAllowedAccess(User u, DateTime date)
        {
            // disallow access based on enabled & date range settings
            if (!Enabled)
            {
                return AvailabilityResult.No(
                    "This resource is not enabled.",
                    "This resource is currently disabled. Please ask an administrator to enabled this resource or to remove any links referencing it.");
            }

            // if the availability is left blank, allow access by default
            return null == Availability
                ? AvailabilityResult.Ok
                : Availability.IsAllowedAccess(u, date);
        }
    }
}
