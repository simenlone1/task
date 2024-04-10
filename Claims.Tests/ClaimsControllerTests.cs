using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Claims.Auditing;
using Claims.Core.DTO.Requests;
using Claims.Presistence.Cosmos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Claims.Tests
{
    public class MoqCosmosRepository : ICosmosRepository<Cover>
    {
        public List<Cover> covers = new();

        public Task<IEnumerable<Cover>> GetItemsAsync()
        {
            return Task.FromResult((IEnumerable<Cover>)covers);
        }

        public Task<Cover?> GetItemAsync(Guid id)
        {
            return Task.FromResult(covers.FirstOrDefault(m => m.Id == id));
        }

        public Task AddItemAsync(Cover item)
        {
            covers.Add(item);
            return Task.CompletedTask;
        }

        public Task<bool> DeleteItemAsync(Guid id)
        {
            var contained = covers.Any(x => x.Id == id);
            covers = covers.Where(m => m.Id != id).ToList();
            return Task.FromResult(contained);
        }
    }

    public class ClaimsControllerTests
    {
        public WebApplicationFactory<Program> CreateTestApplicationFactory()
        {
            return new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddDbContext<AuditContext>(options => options.UseInMemoryDatabase("mssqllocaldb"));

                        var moqCosmosRepository = new MoqCosmosRepository();
                        services.AddSingleton<ICosmosRepository<Cover>>(moqCosmosRepository);
                    });
                });
            
        }
        
        [Theory]
        [InlineData("2222-01-01", "2222-01-01", CoverType.Yacht, 1375)] 
        [InlineData("2222-01-01", "2222-01-01", CoverType.PassengerShip, 1500)] 
        [InlineData("2222-01-01", "2222-01-01", CoverType.ContainerShip, 1625)] 
        [InlineData("2222-01-01", "2222-01-01", CoverType.BulkCarrier, 1625)] 
        [InlineData("2222-01-01", "2222-01-01", CoverType.Tanker, 1875)] 
        [InlineData("2222-01-01", "2222-02-01", CoverType.Yacht, 43875.0)] 
        [InlineData("2222-01-01", "2222-02-01", CoverType.PassengerShip, 47950)] 
        [InlineData("2222-01-01", "2222-09-01", CoverType.ContainerShip, 390350.00)] 
        [InlineData("2222-01-01", "2222-09-01", CoverType.BulkCarrier, 390350.0)] 
        [InlineData("2222-01-01", "2222-09-01", CoverType.Tanker, 451350.0)] 
        public async Task ComputePremium(string startDate, string endDate, CoverType coverType, decimal expectedPremium)
        {
            
            var application = CreateTestApplicationFactory();
            var client = application.CreateClient();
            var premiumRequest = new PremiumRequest()
            {
                StartDate = DateOnly.Parse(startDate),
                EndDate = DateOnly.Parse(endDate),
                Type = coverType
            };

            var premium = await (await client.PostAsJsonAsync("/Covers/ComputePremium", premiumRequest))
               .Content
               .ReadFromJsonAsync<decimal>();
            Assert.Equal(expectedPremium, premium);
       }
        
        [Fact]
        public async Task GetCovers()
        {
            var application = CreateTestApplicationFactory();
            var client = application.CreateClient();

            var createdCovers = new List<Cover>();
            for (var i = 0; i < 4; i++)
            {
                var coverRequest = new CreateCoverRequest
                {
                    StartDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 1, 1 + i),
                    EndDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 4, 1 + i),
                    Type = CoverType.Yacht
                };
                createdCovers.Add((await (await client.PostAsJsonAsync("/Covers", coverRequest)).Content
                    .ReadFromJsonAsync<Cover>())!);
            }

            var res = await client.GetAsync("/Covers");

            res.EnsureSuccessStatusCode();
            var result = await res.Content.ReadFromJsonAsync<List<Cover>>();
            Assert.Equal(JsonSerializer.Serialize(createdCovers), JsonSerializer.Serialize(result));
            
        }
        [Fact]
        public async Task GetCoverById()
        {
            var application = CreateTestApplicationFactory();
            var client = application.CreateClient();
            var coverRequest = new CreateCoverRequest
            {
                StartDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 1, 1),
                EndDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 4, 1),
                Type = CoverType.Yacht
            };

            var createdCover = await (await client.PostAsJsonAsync("/Covers", coverRequest)).Content
                .ReadFromJsonAsync<Cover>();


            var res = await client.GetAsync($"/Covers/{createdCover.Id}");
            res.EnsureSuccessStatusCode();

            var fetchCover = await res.Content.ReadFromJsonAsync<Cover>();
            Assert.NotNull(fetchCover);
            Assert.Equal(coverRequest.StartDate, fetchCover.StartDate);
            Assert.Equal(coverRequest.EndDate, fetchCover.EndDate);
            Assert.Equal(coverRequest.Type, fetchCover.Type);
            Assert.Equal(createdCover.Id, fetchCover.Id);
        }

        [Fact]
        public async Task GetCoverByIdFailure()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();
            var res = await client.GetAsync($"/Cover/1f579037-d1c6-4246-b9a2-5ed9ca69cc15");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task CreateCover()
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();

            var coverRequest = new CreateCoverRequest
            {
                StartDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 1, 1),
                EndDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 4, 1),
                Type = CoverType.Yacht
            };

            var res = await client.PostAsJsonAsync("/Covers", coverRequest);

            res.EnsureSuccessStatusCode();

            var cover = await res.Content.ReadFromJsonAsync<Cover>();

            Assert.NotNull(cover);
            Assert.NotNull(cover.Id);
            Assert.Equal(coverRequest.StartDate, cover.StartDate);
            Assert.Equal(coverRequest.EndDate, cover.EndDate);
            Assert.Equal(coverRequest.Type, cover.Type);
        }

        [Theory]
        [InlineData("2222-01-01", "2225-01-01", "Insurance period exceeds 1 year")]
        [InlineData("1999-01-01", "2225-01-01", "Date is in an invalid format or set in the past")]
        public async Task CreateCoverFailure(string startDate, string endDate,
            string error)
        {
            var application = CreateTestApplicationFactory();

            var client = application.CreateClient();

            var createCoverRequest = new CreateCoverRequest
            {
                StartDate = DateOnly.Parse(startDate),
                EndDate = DateOnly.Parse(endDate),
                Type = CoverType.Yacht
            };

            var res = await client.PostAsJsonAsync("/Covers", createCoverRequest);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
            Assert.Contains(error, await res.Content.ReadAsStringAsync());
        }


            [Fact]
            public async Task DeleteCover()
            {
                var application = CreateTestApplicationFactory();

                var client = application.CreateClient();
                var coverRequest = new CreateCoverRequest
                {
                    StartDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 1, 1),
                    EndDate = new DateOnly(DateTimeOffset.UtcNow.Year + 1, 4, 1),
                    Type = CoverType.Yacht
                };
                var cover = await (await client.PostAsJsonAsync("/Covers", coverRequest)).Content.ReadFromJsonAsync<Cover>();
                var res = await client.DeleteAsync($"/Covers/{cover.Id}");
                res.EnsureSuccessStatusCode();
            }

            [Fact]
            public async Task DeleteCoverFailure()
            {
                var application = CreateTestApplicationFactory();

                var client = application.CreateClient();
                var res = await client.DeleteAsync($"/Covers/1f579037-d1c6-4246-b9a2-5ed9ca69cc15");
                Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
            }
    }
}