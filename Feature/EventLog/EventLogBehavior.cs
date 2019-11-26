using HAS.Profile.ApplicationServices.Messaging;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace HAS.Profile.Feature.EventLog
{
    public class EventLogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IQueueService _queueService;

        public EventLogBehavior(IQueueService queueService, IConfiguration configuration)
        {
            _queueService = queueService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Model.EventLog commandLog = null;

            var response = await next();

            if (request is ICommandEvent)
            {
                commandLog = Model.EventLog.Create(request);
            }

            if (commandLog != null)
            {
                commandLog.AddResult(response);
                await _queueService.AddMessage(commandLog);
            }

            return response;
        }
    }
}
