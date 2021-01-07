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
        private List<ProductDto> _productList;

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
            var products = await uow.Products
                .GetAllAsync();

            var pro = products
                .Select(p => new ProductDto(p));

            //_productList = new List<ProductDto>(pro);

            Products = new ObservableCollection<ProductDto>(pro);

            SelectedProduct = Products.FirstOrDefault();


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

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      yield return ValidationResult.Success;
    }
  }
}
