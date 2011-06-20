using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascend.Core
{
    public class AvailabilityResult
    {
        public bool Available { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }

        public static AvailabilityResult No(string reason)
        {
            return No(reason, null);
        }

        public static AvailabilityResult No(string reason, string details)
        {
            return new AvailabilityResult
            {
                Available = false,
                Reason = reason,
                Details = details,
            };
        }

        public readonly static AvailabilityResult Ok = new AvailabilityResult
        {
            Available = true,
        };
    }

    public enum AvailabilityMode
    {
        AvailableToPublic = 0,
        AvailableToAllUsers = 1,
        AvailableOnlyTo = 2,
        AvailableToEveryoneBut = 3,
    }

    public class Availability
    {
        /// <summary>
        /// Indicates how to interpret Users and Groups, including Public and AllUsers access modes.
        /// </summary>
        public AvailabilityMode Mode { get; set; }

        /// <summary>
        /// List of users that this Resource is available to or excluded from, depending on Mode.
        /// </summary>
        public string[] Users { get; set; }

        /// <summary>
        /// List of Groups that this Resource is available to or excluded from, depending on Mode.
        /// </summary>
        public string[] Groups { get; set; }

        /// <summary>
        /// If set, the resource is available only after the specified date (UTC).
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// If set, the resource is available before the specified date (UTC).
        /// </summary>
        private DateTime? _to;
        public DateTime? To
        {
            get { return _to; }
            set { _to = value; }
        }

        /// <summary>
        /// Determine if a user is allowed access to this resource, at a given point in time.
        /// </summary>
        /// <param name="u">User</param>
        /// <param name="date">Date</param>
        /// <returns>
        /// False, unless either the given user is in the Users list, the user's current group is
        /// in the Groups list, or the AvailableToPublic or AvailableToAllUsers flags are set,  and
        /// the supplied date is within the From and To properties of the Availability and the 
        /// Resource is Enabled.
        /// </returns>
        public AvailabilityResult IsAllowedAccess(User u, DateTime date)
        {
            // disallow access based on date range settings
            if (From.HasValue && date < From.Value)
            {
                return AvailabilityResult.No(
                    "This resource is not yet available.",
                    "This resource will become available on " + From.Value.ToShortDateString() + ". Please check back then.");
            }
            if (To.HasValue && date > To.Value)
            {
                return AvailabilityResult.No(
                    "This resource is no longer available.",
                    "This resource was made unavaible on " + To.Value.ToShortDateString() + ". Please ask your adminstrator to remove any links to this resource.");
            }

            // allow access based on the group membership & mode
            var isInUsers = null != u && null != Users && Users.Contains(u.Document.Id);
            var isInGroups = null != u && null != Groups && null != u.Group && Groups.Contains(u.Group);
            switch (Mode)
            {
                case AvailabilityMode.AvailableToPublic: return AvailabilityResult.Ok;
                case AvailabilityMode.AvailableToAllUsers:
                    if (null != u) return AvailabilityResult.Ok;
                    break;
                case AvailabilityMode.AvailableOnlyTo:
                    if (isInUsers || isInGroups) return AvailabilityResult.Ok;
                    break;
                case AvailabilityMode.AvailableToEveryoneBut:
                    if (!isInUsers && !isInGroups) return AvailabilityResult.Ok;
                    break;
            }

            return AvailabilityResult.No(
                "You do not have access to this resource.",
                "Please ask your adminstrator to grant access to this resource.");
        }

        static Availability _public = new Availability { Mode = AvailabilityMode.AvailableToPublic };
        public static Availability Public { get { return _public; } }
    }

}
