using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SehirRehberi.Api.Models;

namespace SehirRehberi.Api.Data
{
    public class AppRepository : IAppRepository
    {
        private readonly DataContext _context;

        public AppRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public List<City> GetCities()
        {
            var cities = _context.Cities.Include(c => c.Photos).ToList();
            return cities;
        }

        public City GetCityById(int cityId)
        {
            var city = _context.Cities.Include(c => c.Photos).FirstOrDefault(c => c.Id == cityId);
            return city;
        }

        public Photo GetPhoto(int cityId)
        {
            var photo = _context.Photos.Include(p => p.City).FirstOrDefault(p => p.Id == cityId);
            return photo;
        }

        public List<Photo> GetPhotosByCity(int id)
        {
            var photos = _context.Photos.Where(p => p.CityId == id).ToList();
            return photos;
        }

        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
