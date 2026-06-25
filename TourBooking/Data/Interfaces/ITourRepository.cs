using System;
using System.Collections.Generic;
using TourBooking.Models;

namespace TourBooking.Data.Interfaces
{
    public interface ITourRepository : IGenericRepository<Tour>
    {
        IEnumerable<Tour> GetActiveTours();
        IEnumerable<Tour> SearchTours(string keyword, string destination, DateTime? date);
    }
}