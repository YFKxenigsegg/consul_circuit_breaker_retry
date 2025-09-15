using HandlebarsDotNet;

namespace CCBR.Services.NotificationService;

public interface ITemplateRenderer
{
    string Render(string template, Dictionary<string, object> data);
}

public class HandlebarsTemplateRenderer : ITemplateRenderer
{
    public string Render(string template, Dictionary<string, object> data)
    {
        var compiledTemplate = Handlebars.Compile(template);
        return compiledTemplate(data);
    }
}
