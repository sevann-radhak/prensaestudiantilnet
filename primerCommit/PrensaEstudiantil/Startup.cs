using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PrensaEstudiantil.Startup))]
namespace PrensaEstudiantil
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
