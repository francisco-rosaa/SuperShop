using System.Linq;
using System.Threading.Tasks;
using SuperShop.Data.Entities;
using SuperShop.Models;

namespace SuperShop.Data
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        IQueryable GetCountriesWithCities();

        Task<Country> GetCountryWithCitiesAsync(int id);

        Task<City> GetCityAsync(int id);

        Task AddCityAsync(CityViewModel model);

        Task<int> UpdateCityAsync(City city);

        Task<int> DeleteCityAsync(City city);

        Task<Country> GetCountryAsync(City city);
    }
}
