using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceUI.ViewModel
{
    public class ApiConfig
    {
        public string ApiUrl { get; set; }
    }

    public class ApiAliasConfig
    {
        public string GetInvoicesPagging { get; set; }
        public string GetInvoice { get; set; }
        public string PostInvoice { get; set; }
        public string PutInvoice { get; set; }
        public string DeleteInvoice { get; set; }
        public string GetPurchaseOrders { get; set; }
        public string GetPurchaseOrder { get; set; }
        public string GetCustomers { get; set; }
        public string GetCustomer { get; set; }
        public string GetStaticDatasByGroup { get; set; }
    }
}
