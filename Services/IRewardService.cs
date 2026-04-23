using SmartWaste.Models;

namespace SmartWaste.Services
{
    public interface IRewardService
    {
        public void AddReward(Reward reward);
        public Reward GetRewardById(int id);
        public void UpdateReward(Reward reward);
        public void DeleteReward(int id);
        public IEnumerable<Reward> GetAllRewards();
        public void SaveChanges();
    }
}
