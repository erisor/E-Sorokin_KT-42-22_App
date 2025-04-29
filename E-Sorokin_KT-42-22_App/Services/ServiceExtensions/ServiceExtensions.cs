using E_Sorokin_KT_42_22_App.Services;

namespace E_Sorokin_KT_42_22_App.Services.ServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<DepartmentService>();
            services.AddScoped<TeacherService>();
            services.AddScoped<DisciplineService>();
            services.AddScoped<WorkLoadService>();
            
            return services;
        }
    }
}