using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class RewardService : IRewardService
    {
        IRewardRepository _RewardRepository;
        public RewardService(IRewardRepository rewardRepository)
        {
            _RewardRepository = rewardRepository;
        }
        public void AddReward(Reward reward)
        {
            if (reward != null)
            {
                _RewardRepository.AddReward(reward);
            }
        }
        public void DeleteReward(int id)
        {
            if (id > 0)
            {
                _RewardRepository.DeleteReward(id);
            }
        }
        public IEnumerable<Reward> GetAllRewards()
        {
            return _RewardRepository.GetAllRewards();

        }
        public Reward GetRewardById(int id)
        {
            if (id > 0)
            {
                return _RewardRepository.GetRewardById(id);
            }
            return null;
        }
        public void SaveChanges()
        {
            _RewardRepository.SaveChanges();
        }
        public void UpdateReward(Reward reward)
        {
            if (reward != null)
            {
                _RewardRepository.UpdateReward(reward);
            }
        }
    }
}
