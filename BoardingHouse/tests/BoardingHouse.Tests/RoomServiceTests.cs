using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Application.Common.Exceptions;
using BoardingHouse.Application.Rooms;
using BoardingHouse.Domain.Entities;
using Xunit;

namespace BoardingHouse.Tests;

public sealed class RoomServiceTests
{
    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public async Task Invalid_pagination_is_rejected_before_accessing_repository(int pageNumber, int pageSize)
    {
        var repository = new FakeRoomRepository();
        var service = new RoomService(repository);

        await Assert.ThrowsAsync<ValidationException>(() => service.GetPageAsync(pageNumber, pageSize));

        Assert.Equal(0, repository.Calls);
    }

    [Fact]
    public async Task Page_maps_room_fields_and_propagates_pagination_and_cancellation()
    {
        var room = new RoomEntity
        {
            Id = Guid.NewGuid(), FloorId = Guid.NewGuid(), RoomNumber = "A101",
            Area = 25, BasePrice = 3000000, MaxOccupants = 2,
            Description = "Room with a balcony", Status = "Available"
        };
        var repository = new FakeRoomRepository { Rooms = [room], TotalCount = 21 };
        using var cancellation = new CancellationTokenSource();

        var result = await new RoomService(repository).GetPageAsync(2, 20, cancellation.Token);

        var dto = Assert.Single(result.Items);
        Assert.Equal(room.Id, dto.Id);
        Assert.Equal(room.FloorId, dto.FloorId);
        Assert.Equal(room.RoomNumber, dto.RoomNumber);
        Assert.Equal(room.Area, dto.Area);
        Assert.Equal(room.BasePrice, dto.BasePrice);
        Assert.Equal(room.MaxOccupants, dto.MaxOccupants);
        Assert.Equal(room.Description, dto.Description);
        Assert.Equal(room.Status, dto.Status);
        Assert.Equal(21, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(20, result.PageSize);
        Assert.Equal((2, 20), repository.RequestedPage);
        Assert.All(repository.Tokens, token => Assert.Equal(cancellation.Token, token));
    }

    [Fact]
    public async Task Empty_result_has_zero_total_pages()
    {
        var result = await new RoomService(new FakeRoomRepository()).GetPageAsync(1, 20);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task Missing_room_throws_not_found()
    {
        var id = Guid.NewGuid();
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => new RoomService(new FakeRoomRepository()).GetByIdAsync(id));
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public async Task Detail_forwards_id_and_cancellation()
    {
        var room = new RoomEntity { Id = Guid.NewGuid(), RoomNumber = "A101", Status = "Available" };
        var repository = new FakeRoomRepository { Rooms = [room] };
        using var cancellation = new CancellationTokenSource();

        var result = await new RoomService(repository).GetByIdAsync(room.Id, cancellation.Token);

        Assert.Equal(room.Id, result.Id);
        Assert.Equal(room.Id, repository.RequestedId);
        Assert.Equal(cancellation.Token, Assert.Single(repository.Tokens));
    }

    private sealed class FakeRoomRepository : IRoomRepository
    {
        public IReadOnlyList<RoomEntity> Rooms { get; init; } = [];
        public int TotalCount { get; init; }
        public int Calls { get; private set; }
        public (int, int) RequestedPage { get; private set; }
        public Guid RequestedId { get; private set; }
        public List<CancellationToken> Tokens { get; } = [];

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            Calls++;
            Tokens.Add(cancellationToken);
            return Task.FromResult(TotalCount);
        }

        public Task<IReadOnlyList<RoomEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            Calls++;
            Tokens.Add(cancellationToken);
            RequestedPage = (pageNumber, pageSize);
            return Task.FromResult(Rooms);
        }

        public Task<RoomEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Calls++;
            Tokens.Add(cancellationToken);
            RequestedId = id;
            return Task.FromResult(Rooms.SingleOrDefault(room => room.Id == id));
        }
    }
}
