using Domain.Interfaces;
using Meilisearch;

namespace Infrastructure
{
    public class MeilisearchService : IMeilisearchService
    {
        private readonly MeilisearchClient client;
        public MeilisearchService(MeilisearchClient meilisearchClient)
        {
            client = meilisearchClient;
        }

        public async Task GetAllIndexes()
        {
            await client.Index("movies").UpdateFilterableAttributesAsync(new[] { "id", "genres" });
        }
    }
}