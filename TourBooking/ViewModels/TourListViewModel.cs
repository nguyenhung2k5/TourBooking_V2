using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TourBooking.Data;
using TourBooking.Models;

namespace TourBooking.ViewModels
{
    public class TourListViewModel : ViewModelBase
    {
        private string _keyword;
        private string _destination;
        private DateTime? _departureDate;
        private Tour _selectedTour;
        private string _lastError;

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            SearchCommand = new RelayCommand(_ => Search());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            RefreshCommand = new RelayCommand(_ => Load());
        }

        public ObservableCollection<Tour> Tours { get; private set; }

        public string Keyword
        {
            get { return _keyword; }
            set { SetProperty(ref _keyword, value); }
        }

        public string Destination
        {
            get { return _destination; }
            set { SetProperty(ref _destination, value); }
        }

        public DateTime? DepartureDate
        {
            get { return _departureDate; }
            set { SetProperty(ref _departureDate, value); }
        }

        public Tour SelectedTour
        {
            get { return _selectedTour; }
            set { SetProperty(ref _selectedTour, value); }
        }

        public string LastError
        {
            get { return _lastError; }
            private set { SetProperty(ref _lastError, value); }
        }

        public int TourCount
        {
            get { return Tours.Count; }
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public void Load()
        {
            Search();
        }

        public void Search()
        {
            LastError = string.Empty;

            try
            {
                using (var db = new AppDbContext())
                {
                    var query = db.Tours.Where(t => t.IsActive);

                    if (!string.IsNullOrWhiteSpace(Keyword))
                    {
                        var keyword = Keyword.Trim();
                        query = query.Where(t => t.TourCode.Contains(keyword)
                                              || t.TourName.Contains(keyword)
                                              || t.Destination.Contains(keyword));
                    }

                    if (!string.IsNullOrWhiteSpace(Destination))
                    {
                        var destination = Destination.Trim();
                        query = query.Where(t => t.Destination.Contains(destination));
                    }

                    if (DepartureDate.HasValue)
                    {
                        var date = DepartureDate.Value.Date;
                        var nextDate = date.AddDays(1);
                        query = query.Where(t => t.DepartureDate >= date && t.DepartureDate < nextDate);
                    }

                    Tours.Clear();
                    foreach (var tour in query.OrderBy(t => t.DepartureDate).ToList())
                    {
                        Tours.Add(tour);
                    }

                    OnPropertyChanged(nameof(TourCount));
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void ClearFilter()
        {
            Keyword = string.Empty;
            Destination = string.Empty;
            DepartureDate = null;
            Search();
        }
    }
}
