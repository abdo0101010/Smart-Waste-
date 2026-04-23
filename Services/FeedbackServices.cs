using Microsoft.AspNetCore.Http.HttpResults;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class FeedbackServices:IFeedbackServices
    {
        IFeedbackRepository _feedbackRepository;
        public FeedbackServices(FeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public void AddFeedback(Feedback feedback)
        {
            if(feedback != null){

             _feedbackRepository.AddFeedback(feedback);
            }
        }
        public void RemoveFeedback(int id) {
            if (id != 0)
            {

                _feedbackRepository.DeleteFeedback(id);
            }
        }
        public Feedback GetFeedbackById(int id) {

            if (id > 0)
            {

              return  _feedbackRepository.GetFeedbackById(id);
            }
            return null;
        }

       

        public void UpdateFeedback(Feedback feedback)
        {

            if (feedback != null)
            {
                _feedbackRepository.UpdateFeedback(feedback);
            }
          
        }

        public void DeleteFeedback(int id)
        {
           
            if (id > 0)
            {
                _feedbackRepository.DeleteFeedback(id);
            }

        }

        public IEnumerable<Feedback> GetAllFeedback()
        {
           return _feedbackRepository.GetAllFeedback();
        }

        public void SaveChanges()
        {
            _feedbackRepository.SaveChanges();
        }
    }
}
