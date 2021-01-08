using Bakery.Core.Contracts;
using Bakery.Core.DTOs;
using Bakery.Core.Entities;
using Bakery.Persistence;
using Bakery.Wpf.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;

namespace Bakery.Wpf.ViewModels
{
    class EditAndNewProductViewModel : BaseViewModel
    {
        private IWindowController _windowController;
        private ProductDto _selectedProduct;
        private Product _product;
        private bool create;
        private string _productNr;
        private string _price;
        private string _name;

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
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

        public string ProductNr
        {
            get => _productNr;
            set
            {
                _productNr = value;
                OnPropertyChanged(nameof(ProductNr));
            }
        }
        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        [MinLength(1,ErrorMessage = "Produktname ist zu kurz")]
        [MaxLength(20, ErrorMessage = "Produktname darf maximal 20 Zeichen lang sein")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                ValidateViewModelProperties();
            }
        }

        public EditAndNewProductViewModel(IWindowController windowController, ProductDto product) : base(windowController)
        {
            _windowController = windowController;
            _selectedProduct = product;
            if(_selectedProduct != null)
            {
                InitSelectedProduct();
            }
            else
            {
                create = true;
            }
        }

        private void InitSelectedProduct()
        {
            Product = new Product
            {
                Id = _selectedProduct.Id,
                ProductNr = _selectedProduct.ProductNr,
                Price = _selectedProduct.Price,
                Name = _selectedProduct.Name
            };
            ProductNr = _selectedProduct.ProductNr;
            Price = _selectedProduct.Price.ToString();
            Name = _selectedProduct.Name;

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
             if(Name.Length < 1)
             {
                 yield return new ValidationResult("Produktname ist zu kurz", new string[] { nameof(Name) });
             }
             if(Name.Length > 20)
             {
                 yield return new ValidationResult("$Produktname darf maximal 20 Zeichen lang sein", new string[] { nameof(Name) });
             }
            /*return new List<ValidationResult>
           {
               ValidationResult.Success
           };*/
        }

        private ICommand _cmdSave;

        public ICommand CmdSave
        {
            get
            {
                if(_cmdSave == null)
                {
                    _cmdSave = new RelayCommand(
                        execute: async _ =>
                         {
                             ValidateViewModelProperties();

                             try
                             {
                                 await using IUnitOfWork uow = new UnitOfWork();

                                 if (!create)
                                 {
                                     Product productInDb = await uow.Products.GetProductByIdAsync(Product.Id);
                                     productInDb.ProductNr = ProductNr;
                                     productInDb.Price = Double.Parse(Price);
                                     productInDb.Name = Name;

                                 }
                                 else
                                 {
                                     Product = new Product()
                                     {
                                         ProductNr = ProductNr,
                                         Name = Name,
                                         Price = Double.Parse(Price)
                                     };
                                     await uow.Products.AddAsync(Product);
                                 }
                                 await uow.SaveChangesAsync();
                                 Controller.CloseWindow(this);
                             }
                             catch (ValidationException ex)
                             {
                                 //if(ex.Value is IEnumerable<string> properties)
                                 //{
                                 //    foreach (var property in properties)
                                 //    {
                                 //        Errors.Add(property, new List<string> { ex.ValidationResult.ErrorMessage });

                                 //    }
                                 //}
                                 //else
                                 //{
                                 //    DbError = ex.ValidationResult.ToString();
                                 //}
                                 DbError = ex.Message;
                             }
                         },
                        canExecute: _ => !HasErrors
                        );
                }
                return _cmdSave;
            }
            set => _cmdSave = value;
        }
        private ICommand _cmdUndo;

        public ICommand CmdUndo
        {
            get
            {
                if(_cmdUndo == null)
                {
                    _cmdUndo = new RelayCommand(
                        execute: _ =>
                        {
                            if (create)
                            {
                                ProductNr = string.Empty;
                                Name = string.Empty;
                                Price = string.Empty;
                                return;
                            }
                            ProductNr = _product.ProductNr;
                            Name = _product.Name;
                            Price = _product.Price.ToString();
                        },
                        canExecute: _ => true
                        );
                }
                return _cmdUndo;
            }
            set => _cmdUndo = value;
        }

    }
}
