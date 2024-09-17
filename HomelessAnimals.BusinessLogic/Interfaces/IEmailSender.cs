namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IEmailSender
    {
        Task Send(string recipient, string subject, string message);
    }
}
