using System.Collections.Generic;
using WarehouseManagement.Models;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    public class CustomerController
    {
        private readonly CustomerService _customerService;

        public CustomerController()
        {
            _customerService = new CustomerService();
        }

        public List<Customer> GetAllCustomers()
        {
            return _customerService.GetAllCustomers();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerService.GetCustomerById(id);
        }

        public bool AddCustomer(Customer customer)
        {
            return _customerService.AddCustomer(customer);
        }

        public bool UpdateCustomer(Customer customer)
        {
            return _customerService.UpdateCustomer(customer);
        }

        public bool DeleteCustomer(int id)
        {
            return _customerService.DeleteCustomer(id);
        }

        public bool SoftDeleteCustomer(int id)
        {
            return _customerService.DeleteCustomer(id);
        }
    }
}
