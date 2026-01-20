using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    public class SupplierController
    {
        private readonly SupplierService _supplierService;

        public SupplierController()
        {
            _supplierService = new SupplierService();
        }

        public List<Supplier> GetAllSuppliers()
        {
            return _supplierService.GetAllSuppliers();
        }

        public Supplier GetSupplierById(int id)
        {
            return _supplierService.GetSupplierById(id);
        }

        public bool AddSupplier(Supplier supplier)
        {
            return _supplierService.AddSupplier(supplier);
        }

        public bool UpdateSupplier(Supplier supplier)
        {
            return _supplierService.UpdateSupplier(supplier);
        }

        public bool DeleteSupplier(int id)
        {
            return _supplierService.DeleteSupplier(id);
        }

        public bool SoftDeleteSupplier(int id)
        {
            return _supplierService.DeleteSupplier(id);
        }
    }
}
