namespace HomelessAnimals.BusinessLogic.Interfaces
{
    public interface IEmailTemplateBuilder
    {
        Task<string> CreateHtmlTemplate(string templateName, params object[] values);
    }
}
