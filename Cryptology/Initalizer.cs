using Cryptology.BLL;
using Cryptology.Domain.Entity;

namespace Cryptology
{
    public static class Initializer
    {
        public static void InitializeServices (this IServiceCollection services)
        {
            services.AddScoped<GlobalService>();
            services.AddScoped<CaesarService>();
            services.AddScoped<TrithemiusService>();
            services.AddScoped<GammaService>();
            services.AddScoped<KnapsackService>();
            services.AddScoped<RsaService>();
        }
    }
}
