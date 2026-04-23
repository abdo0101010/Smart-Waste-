using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class RewardRepository: IRewardRepository
    {

        smartwasteContext _context;
        public RewardRepository(smartwasteContext context)
        {
            _context = context;
        }

        public void AddReward(Reward reward)
        {
            _context.Rewards.Add(reward);
            SaveChanges();
        }
        public void DeleteReward(int id)
        {
            var reward = _context.Rewards.Find(id);
            _context.Rewards.Remove(reward);
            SaveChanges();
        }
        public Reward GetRewardById(int id)
        {
            return _context.Rewards.Find(id);
        }
        public void UpdateReward(Reward reward)
        {
            _context.Rewards.Update(reward);
            SaveChanges();
        }
        public IEnumerable<Reward> GetAllRewards()
        {
            return _context.Rewards.ToList();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();

        }

        
    }
}
