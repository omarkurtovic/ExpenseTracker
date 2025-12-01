using ExpenseTrackerWebApp.Features.SharedKernel.Behaviors;
using ExpenseTrackerWebApp.Features.SharedKernel.Behaviours;

namespace ExpenseTrackerWebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}
