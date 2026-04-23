using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IRewardRepository
    {
        public void AddReward(Reward reward);
        public Reward GetRewardById(int id);
        public void UpdateReward(Reward reward);
        public void DeleteReward(int id);
        public IEnumerable<Reward> GetAllRewards();
        public void SaveChanges();
    }
}
