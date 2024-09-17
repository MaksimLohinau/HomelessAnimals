using HomelessAnimals.Shared.Constants;
using Microsoft.AspNetCore.Authorization;

namespace HomelessAnimals.Policies
{
    public class Policy
    {
        public static void AddPolicies(AuthorizationOptions options)
        {


            options.AddPolicy(Permissions.Animal.Create,
                x => x.RequireClaim(nameof(Permissions), Permissions.Animal.Create));

            options.AddPolicy(Permissions.Animal.Delete,
                x => x.RequireClaim(nameof(Permissions), Permissions.Animal.Delete));

            options.AddPolicy(Permissions.Animal.Edit,
                x => x.RequireClaim(nameof(Permissions), Permissions.Animal.Edit));

            options.AddPolicy(Permissions.SignUp.View,
                x => x.RequireClaim(nameof(Permissions), Permissions.SignUp.View));

            options.AddPolicy(Permissions.SignUp.ChangeStatus,
                x => x.RequireClaim(nameof(Permissions), Permissions.SignUp.ChangeStatus));

            options.AddPolicy(Permissions.Volunteer.Edit,
                x => x.RequireClaim(nameof(Permissions), Permissions.Volunteer.Edit));

            options.AddPolicy(Permissions.Volunteer.Delete,
                x => x.RequireClaim(nameof(Permissions), Permissions.Volunteer.Delete));

            options.AddPolicy(Permissions.Volunteer.ViewEmail,
                x => x.RequireClaim(nameof(Permissions), Permissions.Volunteer.ViewEmail));

            options.AddPolicy(Permissions.Volunteer.ViewPhoneNumber,
                x => x.RequireClaim(nameof(Permissions), Permissions.Volunteer.ViewPhoneNumber));
        }
    }
}
