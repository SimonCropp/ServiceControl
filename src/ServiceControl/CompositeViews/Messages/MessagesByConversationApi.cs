namespace ServiceControl.CompositeViews.Messages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Nancy;
    using Raven.Client;
    using Raven.Client.Linq;
    using ServiceControl.Infrastructure.Extensions;

    public class MessagesByConversationApi : ScatterGatherApiMessageView<string>
    {
        public override async Task<QueryResult<List<MessagesView>>> LocalQuery(Request request, string input)
        {
            using (var session = Store.OpenAsyncSession())
            {
                RavenQueryStatistics stats;

                var results = await session.Query<MessagesViewIndex.SortAndFilterOptions, MessagesViewIndex>()
                    .Statistics(out stats)
                    .Where(m => m.ConversationId == input)
                    .Sort(request)
                    .Paging(request)
                    .TransformWith<MessagesViewTransformer, MessagesView>()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return new QueryResult<List<MessagesView>>(results.ToList(), stats.ToQueryStatsInfo());
            }
        }
    }
}