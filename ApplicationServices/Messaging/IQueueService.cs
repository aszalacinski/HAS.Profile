﻿using System.Threading.Tasks;

namespace HAS.Profile.ApplicationServices.Messaging
{
    public interface IQueueService
    {
        Task CreateQueue(string queueName);
        Task AddMessage<T>(T messageObj);
    }
}
