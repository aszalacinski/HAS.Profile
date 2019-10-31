using HAS.Profile.ApplicationServices.Messaging;
using HAS.Profile.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

            if (request is ICommandEvent)
            {
                commandLog = Model.EventLog.Create(request);
            }

            var response = await next();

            if(commandLog != null)
            {
                commandLog.AddResult(response);
                await _queueService.AddMessage(commandLog);
            }

            return response;
        }
    }
}
