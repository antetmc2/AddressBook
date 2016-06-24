using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AddressBook.App.Startup))]
namespace AddressBook.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
