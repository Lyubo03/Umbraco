namespace HighlyDeveloped.Core
{
    using HighlyDeveloped.Core.Interfaces;
    using HighlyDeveloped.Core.Services;
    using Umbraco.Core.Composing;
    using Umbraco.Core;

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class RegisterServicesComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IEmailService, EmailService>(Lifetime.Request);
            composition.Register<ISearchService, SearchService>(Lifetime.Request);
        }
    }
}