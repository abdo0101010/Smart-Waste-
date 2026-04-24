using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.Models;
using SmartWaste.Repositories;

namespace SmartWaste.Services
{
    public class RecyclerService: IRecyclerService
    {
        IRecyclerRepository _RecyclerRepository;
        public RecyclerService(IRecyclerRepository recyclerRepository)
        {
            _RecyclerRepository = recyclerRepository;
        }

        public void AddRecycler(Recycler recycler)
        {
            if(recycler != null)
            {
                _RecyclerRepository.AddRecycler(recycler);
            }
        }

        public void DeleteRecycler(int id)
        {
            if(id > 0)
            {
                _RecyclerRepository.DeleteRecycler(id);
            }
        }

        public IEnumerable<Recycler> GetAllRecyclers()
        {
          return  _RecyclerRepository.GetAllRecyclers();
             
        }

        public List<Recycler> GetAllRecyclersWithPickupRequests()
        {
            return _RecyclerRepository.GetAllRecyclersWithPickupRequests();
        }

        public Recycler GetRecyclerById(int id)
        {
            if(id > 0)
            {
                return _RecyclerRepository.GetRecyclerById(id);
            }
            return null;
        }

        public void SaveChanges()
        {
            _RecyclerRepository.SaveChanges();
        }

        public void UpdateRecycler(Recycler recycler)
        {
           if(recycler != null)
            {
                _RecyclerRepository.UpdateRecycler(recycler);
            }
        }

        public decimal? GetAvgRatingRecyclers()
        {
            
            return _RecyclerRepository.GetAvgRatingRecyclers();
        }

        public int GetTotalRecyclers()
        {
            return _RecyclerRepository.GetTotalRecyclers();
        }

        public int GetTotalRecyclingActive()
        {
            return _RecyclerRepository.GetTotalRecyclingActive();
        }

        public List<ReyclerDetailsAdimDto> GetReyclerDetails()
        {
            return _RecyclerRepository.GetReyclerDetails();
        }

        public List<RecyclerWithTotaltripDTO> GetSortingRecyclersByRating()
        {
            return _RecyclerRepository.GetSortingRecyclersByRating();
        }
        public void UpdateRecyclerStatus(int recyclerId, string newStatus)
        {
            _RecyclerRepository.UpdateRecyclerStatus(recyclerId, newStatus);
        }
        public Recycler GetRecyclerByName(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return _RecyclerRepository.GetRecyclerByName(Name);
            }
            return null;
        }
        public void CreateRecycler(RecyclerCreationDTO recyclerCreationDTO)
        {
            if (recyclerCreationDTO != null)
            {

                _RecyclerRepository.CreateRecycler(recyclerCreationDTO);
            }
            else
            {
                throw new ArgumentNullException(nameof(recyclerCreationDTO), "RecyclerCreationDTO cannot be null.");
            }
        }
    }
}
