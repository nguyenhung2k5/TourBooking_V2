using System;
using System.Collections.Generic;
using System.Linq;
using TourBooking.Models;
using TourBooking.Data.Interfaces;

namespace TourBooking.Data.Repositories
{
    public class TourRepository : GenericRepository<Tour>, ITourRepository
    {
        public TourRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Tour> GetActiveTours()
        {
            return _dbSet.Where(t => t.IsActive).ToList();
        }

        public IEnumerable<Tour> SearchTours(string keyword, string destination, DateTime? date)
        {
            var query = _dbSet.Where(t => t.IsActive);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.TourName.Contains(keyword) || t.TourCode.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(destination))
            {
                query = query.Where(t => t.Destination.Contains(destination));
            }

            if (date.HasValue)
            {
                query = query.Where(t => t.DepartureDate.Year == date.Value.Year
                                      && t.DepartureDate.Month == date.Value.Month
                                      && t.DepartureDate.Day == date.Value.Day);
            }

            return query.ToList();
        }
    }
}