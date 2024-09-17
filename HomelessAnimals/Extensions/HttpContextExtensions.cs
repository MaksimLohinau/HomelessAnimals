using HomelessAnimals.BusinessLogic.Models;
using System.Net;

namespace HomelessAnimals.Extensions
{
    public static class HttpContextExtensions
    {
        private readonly static IPAddress DefaultIPAddressForDevelopment = IPAddress.Parse("192.168.1.107");

        public static IPAddress GetIPAddress(this HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;

            return IPAddress.IsLoopback(ipAddress) ? DefaultIPAddressForDevelopment : ipAddress;
        }

        public static int? GetUserId(this HttpContext context)
        {
            var claim = context.User.Claims.FirstOrDefault(x => x.Type == nameof(AuthenticationInfo.PlayerId));

            return claim == null ? null : int.Parse(claim.Value);
        }
    }
}
