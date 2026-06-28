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

        
        private bool _isAllTours = true;
        private bool _isAvailableOnly = false;
        private bool _isSoldOutOnly = false;

        
        private int _currentPage = 1;
        private int _totalPages = 1;
        private const int PageSize = 5; 
        private int _totalTourCount = 0;

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            SearchCommand = new RelayCommand(_ => { CurrentPage = 1; Search(); });
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            RefreshCommand = new RelayCommand(_ => Load());
            PrevPageCommand = new RelayCommand(_ => PreviousPage(), _ => CanPreviousPage());
            NextPageCommand = new RelayCommand(_ => NextPage(), _ => CanNextPage());
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

        public bool IsAllTours
        {
            get { return _isAllTours; }
            set 
            { 
                if (SetProperty(ref _isAllTours, value) && value)
                {
                    IsAvailableOnly = false;
                    IsSoldOutOnly = false;
                }
            }
        }

        public bool IsAvailableOnly
        {
            get { return _isAvailableOnly; }
            set 
            { 
                if (SetProperty(ref _isAvailableOnly, value) && value)
                {
                    IsAllTours = false;
                    IsSoldOutOnly = false;
                }
            }
        }

        public bool IsSoldOutOnly
        {
            get { return _isSoldOutOnly; }
            set 
            { 
                if (SetProperty(ref _isSoldOutOnly, value) && value)
                {
                    IsAllTours = false;
                    IsAvailableOnly = false;
                }
            }
        }

        
        public int CurrentPage
        {
            get { return _currentPage; }
            set 
            { 
                if (SetProperty(ref _currentPage, value))
                {
                    Search();
                }
            }
        }

        public int TotalPages
        {
            get { return _totalPages; }
            private set { SetProperty(ref _totalPages, value); }
        }

        public int TourCount
        {
            get { return _totalTourCount; }
            private set { SetProperty(ref _totalTourCount, value); }
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand PrevPageCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }

        public void Load()
        {
            CurrentPage = 1;
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

                    
                    if (IsAvailableOnly)
                    {
                        query = query.Where(t => t.AvailableSlots > 0);
                    }
                    else if (IsSoldOutOnly)
                    {
                        query = query.Where(t => t.AvailableSlots == 0);
                    }

                    
                    TourCount = query.Count();

                    
                    TotalPages = (int)Math.Ceiling((double)TourCount / PageSize);
                    if (TotalPages == 0) TotalPages = 1;

                    
                    if (CurrentPage > TotalPages) _currentPage = TotalPages;
                    if (CurrentPage < 1) _currentPage = 1;

                    
                    int skip = (CurrentPage - 1) * PageSize;
                    var pagedList = query.OrderBy(t => t.DepartureDate)
                                         .Skip(skip)
                                         .Take(PageSize)
                                         .ToList();

                    Tours.Clear();
                    foreach (var tour in pagedList)
                    {
                        Tours.Add(tour);
                    }

                    OnPropertyChanged(nameof(CurrentPage));
                    OnPropertyChanged(nameof(TotalPages));
                    OnPropertyChanged(nameof(TourCount));
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
        }

        private void PreviousPage()
        {
            if (CanPreviousPage())
            {
                CurrentPage--;
            }
        }

        private bool CanPreviousPage()
        {
            return CurrentPage > 1;
        }

        private void NextPage()
        {
            if (CanNextPage())
            {
                CurrentPage++;
            }
        }

        private bool CanNextPage()
        {
            return CurrentPage < TotalPages;
        }

        private void ClearFilter()
        {
            Keyword = string.Empty;
            Destination = string.Empty;
            DepartureDate = null;
            IsAllTours = true;
            IsAvailableOnly = false;
            IsSoldOutOnly = false;
            CurrentPage = 1;
            Search();
        }
    }
}
