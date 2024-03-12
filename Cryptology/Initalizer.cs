using Cryptology.BLL;

namespace Cryptology
{
    public static class Initializer
    {
        public static void InitializeServices (this IServiceCollection services)
        {
            services.AddScoped<GlobalService>();
            services.AddScoped<CaesarService>();
            services.AddScoped<TrithemiusService>();
        }
    }
}
