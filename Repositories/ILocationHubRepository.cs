namespace SmartWaste.Repositories
{
    public interface ILocationHubRepository
    {
        Task UpdateDriverLocation(int driverId, double lat, double lng, string status, int capacity);
        Task JoinAdminGroup();
        IAsyncEnumerable<object> GetTruckLocationStream(int driverId, CancellationToken cancellationToken);
    }
}
