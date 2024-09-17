using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.DataAccess.Interfaces;
using HomelessAnimals.Shared.Enums;
using System.Security.Claims;
using Enums = HomelessAnimals.Shared.Enums;

namespace HomelessAnimals.BusinessLogic.Services
{
    public class ScopeVerificationService : IScopeVerificationService
    {
        private readonly IDataAccessFactory _dataFactory;

        public ScopeVerificationService(IDataAccessFactory dataFactory)
        {
            _dataFactory = dataFactory;
        }

        public bool ValidateScopeForAnimalManage(IEnumerable<Claim> claims, int animalId)
        {
            var scopes = GetScopes(claims);

            if (scopes.Any(s => s.Level == ScopeLevel.System))
                return true;

            return scopes.Any(s => s.Level == ScopeLevel.Animal && s.ResourceId == animalId);
        }

        public bool ValidateScopeForAnimalAdminReassignment(IEnumerable<Claim> claims)
        {
            var scopes = GetScopes(claims);

            return scopes.Any(s => s.Level == ScopeLevel.System);
        }

        public bool ValidateScopeForPhoneNumberView(IEnumerable<Claim> claims, int volunteerId)
        {
            var scopes = GetScopes(claims);

            if (scopes.Any(s => s.Level == ScopeLevel.System || s.Level == ScopeLevel.Animal))
                return true;

            return scopes.Any(s => s.Level == ScopeLevel.Volounteer && s.ResourceId == volunteerId);
        }


        private static List<Scope> GetScopes(IEnumerable<Claim> claims)
        {
            return claims
                .Where(c => c.Type == nameof(AuthenticationInfo.Scopes))
                .Select(c =>
                {
                    var segments = c.Value.Split('/');

                    return new Scope
                    {
                        Level = Enum.Parse<Enums.ScopeLevel>(segments[0]),
                        ResourceId = segments.Length > 1 ? int.Parse(segments[1]) : null,
                    };
                })
                .ToList();
        }
    }
}
