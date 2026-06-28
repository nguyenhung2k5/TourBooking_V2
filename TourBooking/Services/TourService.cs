using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.Services
{
    public class TourService
    {
        public List<Tour> GetActiveTours()
        {
            using (var db = new AppDbContext())
            {
                return db.Tours.Where(t => t.IsActive).OrderBy(t => t.DepartureDate).ToList();
            }
        }

        public List<Tour> FilterTours(string destination, DateTime? departureDate)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Tours.Where(t => t.IsActive);

                if (!string.IsNullOrWhiteSpace(destination))
                {
                    var dest = destination.Trim();
                    query = query.Where(t => t.Destination.Contains(dest));
                }

                if (departureDate.HasValue)
                {
                    var date = departureDate.Value.Date;
                    var nextDate = date.AddDays(1);
                    query = query.Where(t => t.DepartureDate >= date && t.DepartureDate < nextDate);
                }

                return query.OrderBy(t => t.DepartureDate).ToList();
            }
        }

        public bool UpdateAvailableSlots(int tourId, int quantityChange)
        {
            using (var db = new AppDbContext())
            {
                var tour = db.Tours.FirstOrDefault(t => t.TourId == tourId);
                if (tour == null) return false;

                if (tour.AvailableSlots + quantityChange < 0 || tour.AvailableSlots + quantityChange > tour.TotalSlots)
                {
                    return false;
                }

                tour.AvailableSlots += quantityChange;
                db.SaveChanges();
                return true;
            }
        }
    }
}
