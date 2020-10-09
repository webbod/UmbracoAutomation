using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.SiteBuilder.Component;

namespace Umbraco.SiteBuilder.Composer
{
    public class SiteBuilderComposer : ComponentComposer<SiteBuilderComponent>
    {
        public override void Compose(Composition composition)
        {
            composition.Components().Append<SiteBuilderComponent>();
            base.Compose(composition);
        }
    }
}
