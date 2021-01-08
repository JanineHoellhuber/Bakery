using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bakery.Wpf.ViewModels
{
  public class MainWindowViewModel : BaseViewModel
  {
        private ObservableCollection<ProductDto> _products;
        private string _priceFrom;
        private string _priceTo;
        private ProductDto _selectedProduct;
        //private List<ProductDto> _productList;
        private double _avgPrice;


        public ObservableCollection<ProductDto> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        public string PriceFrom
        {
            get => _priceFrom;
            set
            {
                _priceFrom = value;
                OnPropertyChanged(nameof(PriceFrom));
            }
        }
        public string PriceTo
        {
            get => _priceTo;
            set
            {
                _priceTo = value;
                OnPropertyChanged(nameof(PriceTo));
            }
        }
        public ProductDto SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
    public MainWindowViewModel(IWindowController controller) : base(controller)
    {
      
       LoadCommands();
    }

    public double APrice()
        {
            return _avgPrice = Products.Average(p => p.Price);

        }

    private void LoadCommands()
    {

    }


    public static async Task<MainWindowViewModel> Create(IWindowController controller)
    {
      var model = new MainWindowViewModel(controller);
      await model.LoadProducts();
      return model;
    }

    /// <summary>
    /// Produkte laden. Produkte können nach Preis gefiltert werden. 
    /// Preis liegt zwischen from und to
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    private async Task LoadProducts()
    {
            IUnitOfWork uow = new UnitOfWork();
            double priceFrom = 0;
            double priceTo = 0;

            double.TryParse(PriceFrom, out priceFrom);
            double.TryParse(PriceTo, out priceTo);

            var product = await uow.Products.GetWithFilterAsync(priceFrom, priceTo);

      

            Products = new ObservableCollection<ProductDto>(product);

            SelectedProduct = Products.FirstOrDefault();

            AvgPrice = APrice();
            
        }


        public double AvgPrice
        {

            get => _avgPrice;
             set
             {
                _avgPrice = value;
                OnPropertyChanged();
             }
        }

        private ICommand _cmdSearch;

        public ICommand CmdSearch
        {
            get
            {
                if (_cmdSearch == null)
                {
                    _cmdSearch = new RelayCommand(
                       execute: _ =>
                       {
                           LoadProducts();
                       },
                       canExecute: _ => true
                       );
                }
                return _cmdSearch;
            }
            set => _cmdSearch = value;

        }

        private ICommand _cmdEditProduct;

    public ICommand CmdEditProduct
    {
            get
            {
                if(_cmdEditProduct == null)
                {
                    _cmdEditProduct = new RelayCommand(
                       execute: _ =>
                       {
                           Controller.ShowWindow(new EditAndNewProductViewModel(Controller, SelectedProduct), true);
                           LoadProducts();
                       },
                       canExecute: _ => true
                       );
                }
                return _cmdEditProduct;
            }
            set => _cmdEditProduct = value;

    }

        private ICommand _cmdNewProduct;

        public ICommand CmdNewProduct
        {
            get
            {
                if (_cmdNewProduct == null)
                {
                    _cmdNewProduct = new RelayCommand(
                       execute: _ =>
                       {
                           Controller.ShowWindow(new EditAndNewProductViewModel(Controller, null), true);
                           LoadProducts();
                       },
                       canExecute: _ => true
                       );
                }
                return _cmdNewProduct;
            }
            set => _cmdNewProduct = value;
        }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      yield return ValidationResult.Success;
    }
  }
}
