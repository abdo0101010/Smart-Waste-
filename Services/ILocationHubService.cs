using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public interface ILocationHubService: ILocationHubRepository
    {

        public Task JoinAdminGroup(string connectionId);

    }
}
