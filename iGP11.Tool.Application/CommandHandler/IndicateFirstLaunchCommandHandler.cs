using System.Threading.Tasks;

using iGP11.Library.DDD;
using iGP11.Library.DDD.Action;
using iGP11.Tool.Application.Api;
using iGP11.Tool.Domain.Model.UsageStatistics;
using iGP11.Tool.Shared.Event;

namespace iGP11.Tool.Application.CommandHandler
{
    public class IndicateFirstLaunchCommandHandler : IDomainCommandHandler<IndicateFirstLaunchCommand>
    {
        private readonly IUsageStatisticsRepository _usageStatisticsRepository;

        public IndicateFirstLaunchCommandHandler(IUsageStatisticsRepository usageStatisticsRepository)
        {
            _usageStatisticsRepository = usageStatisticsRepository;
        }

        public async Task HandleAsync(DomainCommandContext context, IndicateFirstLaunchCommand command)
        {
            var usageStatistics = await _usageStatisticsRepository.LoadAsync();
            usageStatistics.FirstLaunchTime = command.Time;

            await _usageStatisticsRepository.SaveAsync(usageStatistics);
            await context.PublishAsync(new FirstLaunchIndicatedEvent(command.Time));
        }
    }
}