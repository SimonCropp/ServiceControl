﻿namespace ServiceControl.Audit.Persistence.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dapper;
    using Infrastructure;
    using NServiceBus.CustomChecks;
    using NServiceBus.Logging;
    using ServiceControl.Audit.Auditing;
    using ServiceControl.Audit.Auditing.MessagesView;
    using ServiceControl.Audit.Monitoring;
    using ServiceControl.SagaAudit;

    //TODO - not done as not sure on the table structure required here
    class SqlDbAuditDataStore : IAuditDataStore
    {
        readonly SqlDbConnectionManager connectionManager;

        public SqlDbAuditDataStore(SqlDbConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        public async Task<QueryResult<IList<MessagesView>>> GetMessages(HttpRequestMessage request, PagingInfo pagingInfo)
        {
            var messagesViews = new List<MessagesView>();

            await connectionManager.Perform(async connection =>
            {
                var rows = await connection.QueryAsync("SELECT * FROM [MessagesView]").ConfigureAwait(false);

                foreach (var row in rows)
                {
                    messagesViews.Add(new MessagesView
                    {
                        BodySize = row.BodySize,
                        BodyUrl = row.BodyUrl,
                        ConversationId = row.ConversationId,
                        CriticalTime = row.CriticalTime,
                        DeliveryTime = row.DeliveryTime,
                        Id = row.Id,
                        InstanceId = row.InstanceId,
                        IsSystemMessage = row.IsSystemMessage,
                        MessageId = row.MessageId,
                        MessageIntent = row.MessageIntent,
                        MessageType = row.MessageType,
                        ProcessedAt = row.ProcessedAt,
                        ProcessingTime = row.ProcessingTime,
                        Status = row.Status,
                        TimeSent = row.TimeSent
                        //Headers = row.Headers,
                        //InvokedSagas = row.InvokedSagas,
                        //OriginatesFromSaga = row.OriginatesFromSaga,
                        //ReceivingEndpoint = row.ReceivingEndpoint,
                        //SendingEndpoint = row.SendingEndpoint
                    });
                }
            }).ConfigureAwait(false);

            return new QueryResult<IList<MessagesView>>(messagesViews, new QueryStatsInfo(string.Empty, messagesViews.Count));
        }

        public async Task<QueryResult<IList<KnownEndpointsView>>> QueryKnownEndpoints()
        {
            var knownEndpoints = new List<KnownEndpointsView>();

            await connectionManager.Perform(async connection =>
            {
                var rows = await connection.QueryAsync("SELECT * FROM [MessagesView]").ConfigureAwait(false);

                foreach (var row in rows)
                {
                    knownEndpoints.Add(new KnownEndpointsView
                    {
                        Id = row.Id,
                        HostDisplayName = row.HostDisplayName
                        //EndpointDetails = row.EndpointDetails
                    });
                }
            }).ConfigureAwait(false);

            return new QueryResult<IList<KnownEndpointsView>>(knownEndpoints, new QueryStatsInfo(string.Empty, knownEndpoints.Count));
        }

        public Task<QueryResult<IList<MessagesView>>> QueryMessages(HttpRequestMessage request, string searchParam, PagingInfo pagingInfo) => throw new NotImplementedException();
        public async Task<QueryResult<IList<MessagesView>>> QueryMessagesByConversationId(HttpRequestMessage request, string conversationId, PagingInfo pagingInfo)
        {
            var messagesViews = new List<MessagesView>();

            await connectionManager.Perform(async connection =>
            {
                var rows = await connection.QueryAsync(
                    @"SELECT * FROM [dbo].[MessagesView] WHERE [ConversationId] = @Id",
                    new
                    {
                        Id = conversationId
                    }).ConfigureAwait(false);

                foreach (var row in rows)
                {
                    messagesViews.Add(new MessagesView
                    {
                        BodySize = row.BodySize,
                        BodyUrl = row.BodyUrl,
                        ConversationId = row.ConversationId,
                        CriticalTime = row.CriticalTime,
                        DeliveryTime = row.DeliveryTime,
                        Id = row.Id,
                        InstanceId = row.InstanceId,
                        IsSystemMessage = row.IsSystemMessage,
                        MessageId = row.MessageId,
                        MessageIntent = row.MessageIntent,
                        MessageType = row.MessageType,
                        ProcessedAt = row.ProcessedAt,
                        ProcessingTime = row.ProcessingTime,
                        Status = row.Status,
                        TimeSent = row.TimeSent
                        //Headers = row.Headers,
                        //InvokedSagas = row.InvokedSagas,
                        //OriginatesFromSaga = row.OriginatesFromSaga,
                        //ReceivingEndpoint = row.ReceivingEndpoint,
                        //SendingEndpoint = row.SendingEndpoint
                    });
                }
            }).ConfigureAwait(false);

            return new QueryResult<IList<MessagesView>>(messagesViews, new QueryStatsInfo(string.Empty, messagesViews.Count));
        }

        public async Task<QueryResult<IList<MessagesView>>> QueryMessagesByReceivingEndpoint(HttpRequestMessage request, string endpointName, PagingInfo pagingInfo)
        {
            var messagesViews = new List<MessagesView>();

            await connectionManager.Perform(async connection =>
            {
                var rows = await connection.QueryAsync(
                    @"SELECT * FROM [dbo].[MessagesView] WHERE [ReceivingEndpointName] = @receivingEndpointName",
                    new
                    {
                        receivingEndpointName = endpointName
                    }).ConfigureAwait(false);

                foreach (var row in rows)
                {
                    messagesViews.Add(new MessagesView
                    {
                        BodySize = row.BodySize,
                        BodyUrl = row.BodyUrl,
                        ConversationId = row.ConversationId,
                        CriticalTime = row.CriticalTime,
                        DeliveryTime = row.DeliveryTime,
                        Id = row.Id,
                        InstanceId = row.InstanceId,
                        IsSystemMessage = row.IsSystemMessage,
                        MessageId = row.MessageId,
                        MessageIntent = row.MessageIntent,
                        MessageType = row.MessageType,
                        ProcessedAt = row.ProcessedAt,
                        ProcessingTime = row.ProcessingTime,
                        Status = row.Status,
                        TimeSent = row.TimeSent
                        //Headers = row.Headers,
                        //InvokedSagas = row.InvokedSagas,
                        //OriginatesFromSaga = row.OriginatesFromSaga,
                        //ReceivingEndpoint = row.ReceivingEndpoint,
                        //SendingEndpoint = row.SendingEndpoint
                    });
                }
            }).ConfigureAwait(false);

            return new QueryResult<IList<MessagesView>>(messagesViews, new QueryStatsInfo(string.Empty, messagesViews.Count));
        }

        public Task<QueryResult<IList<MessagesView>>> QueryMessagesByReceivingEndpointAndKeyword(HttpRequestMessage request, SearchEndpointApi.Input input, PagingInfo pagingInfo) => throw new NotImplementedException();
        public async Task<QueryResult<SagaHistory>> QuerySagaHistoryById(Guid input)
        {
            SagaHistory sagaHistory = default;

            await connectionManager.Perform(async connection =>
            {
                var rows = await connection.QueryAsync(
                    @"SELECT * FROM [dbo].[SagaHistory] WHERE [SagaId] = @Id",
                    new
                    {
                        Id = input
                    }).ConfigureAwait(false);

                if (rows.AsList().Count > 0)
                {
                    sagaHistory = new SagaHistory
                    {
                        Id = rows.AsList()[0].Id,
                        SagaId = rows.AsList()[0].SagaId,
                        SagaType = rows.AsList()[0].SagaType,
                        //Changes = rows.AsList()[0].Changes
                    };
                }

            }).ConfigureAwait(false);

            if (sagaHistory == null)
            {
                return await Task.FromResult(QueryResult<SagaHistory>.Empty()).ConfigureAwait(false);
            }

            return await Task.FromResult(new QueryResult<SagaHistory>(sagaHistory, new QueryStatsInfo(string.Empty, 1))).ConfigureAwait(false);

        }

        public Task<HttpResponseMessage> TryFetchFromIndex(HttpRequestMessage request, string messageId) => throw new NotImplementedException();

        public Task MigrateEndpoints(int pageSize = 1024) => throw new NotImplementedException();

        public Task<CheckResult> PerformFailedAuditImportCheck(string errorMessage) => throw new NotImplementedException();

        public Task SaveFailedAuditImport(FailedAuditImport message) => throw new NotImplementedException();

        static readonly ILog Logger = LogManager.GetLogger(typeof(SqlDbAuditDataStore));
    }
}