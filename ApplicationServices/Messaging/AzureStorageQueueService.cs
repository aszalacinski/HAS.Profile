﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HAS.Profile.ApplicationServices.Messaging
{
    public class AzureStorageQueueService : IQueueService
    {
        private CloudStorageAccount _storageAccount { get; set; }
        private CloudQueueClient _queueClient { get; set; }
        private CloudQueue _queue { get; set; }

        private AzureStorageQueueService() { }

        public static AzureStorageQueueService Create(string connectionString)
        {
            var _service = new AzureStorageQueueService();
            _service._storageAccount = CloudStorageAccount.Parse(connectionString);
            _service._queueClient = _service._storageAccount.CreateCloudQueueClient();

            return _service;
        }

        public CloudStorageAccount StorageAccount
        {
            get
            {
                return _storageAccount;
            }
        }

        public CloudQueueClient QueueClient
        {
            get
            {
                return _queueClient;
            }
        }

        public CloudQueue Queue
        {
            get
            {
                return _queue;
            }
        }

        public async Task CreateQueue(string queueName)
        {
            _queue = _queueClient.GetQueueReference(queueName);
            await _queue.CreateIfNotExistsAsync();
        }

        public async Task AddMessage<T>(T messageObj) => await _queue.AddMessageToJsonAsync<T>(messageObj);
    }

    public static class MPYRegistrationServiceExtensions
    {
        public static async Task AddMessageToJsonAsync<T>(this CloudQueue cloudQueue, T objectToAdd)
        {
            var msgAsJson = JsonSerializer.Serialize(objectToAdd);
            var cloudQueueMsg = new CloudQueueMessage(msgAsJson);
            await cloudQueue.AddMessageAsync(cloudQueueMsg);
        }
    }
}
