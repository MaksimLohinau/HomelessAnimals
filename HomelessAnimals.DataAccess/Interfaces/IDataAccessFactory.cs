namespace HomelessAnimals.DataAccess.Interfaces
{
    public interface IDataAccessFactory
    {
        public IVolunteerProfileRepository VolunteerProfileRepository { get; }
        public IAnimalProfileRepository AnimalProfileRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IResetPasswordTokenRepository ResetPasswordTokenRepository { get; }
        public ISignUpRequestRepository SignUpRequestRepository { get; }
        Task SaveChanges();
    }
}
