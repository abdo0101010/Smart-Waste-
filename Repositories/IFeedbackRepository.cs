using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IFeedbackRepository
    {
        public void AddFeedback(Feedback feedback);
        public Feedback GetFeedbackById(int id);
        public void UpdateFeedback(Feedback feedback);
        public void DeleteFeedback(int id);
        public IEnumerable<Feedback> GetAllFeedback();
        public void SaveChanges();
    }
}
