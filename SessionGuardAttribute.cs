using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionGuardAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _targetRole;

        public SessionGuardAttribute(string targetRole)
        {
            _targetRole = targetRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var role = session.GetString("AssignedUserRole");
            var token = session.GetString("AuthTokenKey");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            if (!string.IsNullOrEmpty(_targetRole) && !_targetRole.Equals(role, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { error = "Unauthorized." });
            }
        }
    }
}
