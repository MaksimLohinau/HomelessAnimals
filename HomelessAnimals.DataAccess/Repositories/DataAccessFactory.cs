using HomelessAnimals.DataAccess.Interfaces;

namespace HomelessAnimals.DataAccess.Repositories
{
    public class DataAccessFactory : IDataAccessFactory
    {
        private readonly HomelessAnimalsContext _context;
        public IVolunteerProfileRepository VolunteerProfileRepository { get;  }
        public IAnimalProfileRepository AnimalProfileRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IResetPasswordTokenRepository ResetPasswordTokenRepository { get; }
        public ISignUpRequestRepository SignUpRequestRepository { get; }

        public DataAccessFactory(HomelessAnimalsContext context)
        {
            _context = context;
            VolunteerProfileRepository = new VolunteerRepository(_context);
            AnimalProfileRepository = new AnimalRepository(_context);
            AccountRepository = new AccountRepository(_context);
            ResetPasswordTokenRepository = new ResetPasswordTokenRepository(_context);
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
