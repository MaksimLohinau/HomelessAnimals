using HomelessAnimals.BusinessLogic.Interfaces;
using HomelessAnimals.Shared.Models;
using Microsoft.Extensions.Options;

namespace HomelessAnimals.BusinessLogic.EmailMessageBuilders
{
    public class EmailTemplateBuilder : IEmailTemplateBuilder
    {
        private readonly EmailTemplatesSettings _emailTemplatesSettings;

        public EmailTemplateBuilder(IOptions<EmailTemplatesSettings> emailTemplatesSettings)
        {
            _emailTemplatesSettings = emailTemplatesSettings.Value;
        }

        public async Task<string> CreateHtmlTemplate(string templateName, params object[] values)
        {
            var basePath = _emailTemplatesSettings.Path;

            var stringTemplate = await File.ReadAllTextAsync($"{basePath}/{templateName}.html");

            return string.Format(stringTemplate, values);
        }
    }
}
