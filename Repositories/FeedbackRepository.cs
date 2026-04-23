using Microsoft.EntityFrameworkCore;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public class FeedbackRepository: IFeedbackRepository
    {
        smartwasteContext _context;
        public FeedbackRepository(smartwasteContext context)
        {
            _context = context;
        }

        public void AddFeedback(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            SaveChanges();
        }
          
        public Feedback GetFeedbackById(int id)
        {
            return _context.Feedbacks.Find(id);
        }
           
        public void UpdateFeedback(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            SaveChanges();
        }
        public void DeleteFeedback(int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
                SaveChanges();
            }
        }

        public IEnumerable<Feedback> GetAllFeedback()
        {
            return _context.Feedbacks.ToList();
        }



        public void SaveChanges()
        {
            _context.SaveChanges();
        }





    }
}
