using Microsoft.EntityFrameworkCore;
using SmartWaste.DTO.RecuclerDTOS;
using SmartWaste.Models;

namespace SmartWaste.Repositories
{
    public interface IRecyclerRepository
    {
        public void AddRecycler(Recycler recycler);

        public Recycler GetRecyclerById(int id);

        public void UpdateRecycler(Recycler recycler);

        public void DeleteRecycler(int id);

        public IEnumerable<Recycler> GetAllRecyclers();

        public List<Recycler> GetAllRecyclersWithPickupRequests();
      
        public void SaveChanges();
        public Recycler GetRecyclerByName(string Name);


        public decimal? GetAvgRatingRecyclers();
        public int GetTotalRecyclers();
        public int GetTotalRecyclingActive();
                public List<ReyclerDetailsAdimDto> GetReyclerDetails();
        public List<RecyclerWithTotaltripDTO> GetSortingRecyclersByRating();
        public void UpdateRecyclerStatus(int recyclerId, string newStatus);
        public void CreateRecycler(RecyclerCreationDTO recyclerCreationDTO);

    }
}
