using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InvoiceUI.Models;
using DataTables.AspNet.AspNetCore;
using Newtonsoft.Json;
using DataTables.AspNet.Core;
using System.Net.Http;
using InvoiceUI.ViewModel;
using Microsoft.Extensions.Options;
using InvoiceUI.Extentions;
using System.Net.Http.Headers;
using System.Text;
using InvoiceUI.StaticData;
using InvoiceUI.ViewComponents;

namespace InvoiceUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        ApiConfig _apiConfig;
        ApiAliasConfig _apiAliasConfig;
        private HttpClient _client;
        IViewRenderService _viewRenderService;
        public HomeController(ILogger<HomeController> logger,
            IOptions<ApiConfig> apiConfig,
            IOptions<ApiAliasConfig> apiAliasConfig,
            IViewRenderService viewRenderService)
        {
            _logger = logger;
            _apiConfig = apiConfig.Value;
            _apiAliasConfig = apiAliasConfig.Value;
            _client = new HttpClient();
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<JsonResult> getDataOption()
        {
            try
            {
                var dataoptionUOM = await getStaticDatas(StaticDataGroup.UOM);
                var dataoptionCurrency = await getStaticDatas(StaticDataGroup.Currency);
                var dataoptionLanguage = await getStaticDatas(StaticDataGroup.Language);

                ApiResult<List<string>> apiResultCs = new ApiResult<List<string>>();
                string url = $"{_apiConfig.ApiUrl}{_apiAliasConfig.GetCustomers}";
                apiResultCs = await _client.GetAsync<List<string>>(url);
                
                return Json(new { error = false, dataOptUom = dataoptionUOM, 
                    dataOptCurrency = dataoptionCurrency, 
                    dataOptLanguange = dataoptionLanguage,
                    dataOptCustomer = apiResultCs.Payload
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        public async Task<JsonResult> getDataCustomerById(string id)
        {
            try
            {
                ApiResult<CustomerViewModel> apiResultCs = new ApiResult<CustomerViewModel>();
                string url = $"{_apiConfig.ApiUrl}{_apiAliasConfig.GetCustomers}/{id}";
                apiResultCs = await _client.GetAsync<CustomerViewModel>(url);

                return Json(new
                {
                    error = false,
                    customerLogo = apiResultCs.Payload?.CUstomerLogo,
                    customerAddress = apiResultCs.Payload?.CustomerAddress
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        public JsonResult AddNewDetail(string detailName, decimal qty, decimal rate, decimal amount, string uomId, string uomName, string invId) 
        {
            try
            {
                var partialViewDetail = _viewRenderService.RenderPartialViewToString(this, "_DetailInvoice", new InvoiceDetailViewModel
                {
                    Id = null,
                    InvoiceId = invId,
                    UomId = uomId,
                    UomName = uomName,
                    InvoiceDetailName = detailName,
                    InvoiceDetailRate = rate,
                    InvoiceDetailQty = qty,
                    AddDate = DateTime.Now,
                    EditDate = DateTime.Now
                });

                return Json(new
                {
                    error = false,
                    dataDetai= partialViewDetail,
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        //[HttpPost]
        public async Task<JsonResult> saveInvoice([FromBody] InvoiceViewModel model) 
        {
            try
            {
                var contentType = "application/json";

                ApiResult<InvoiceViewModel> resultApi = new ApiResult<InvoiceViewModel>();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var dataParameter = JsonConvert.SerializeObject(model);
                StringContent contentGetAppointmentOL = new StringContent(dataParameter, Encoding.UTF8, "application/json");

                resultApi = await _client.PostAsync<InvoiceViewModel>($"{_apiConfig.ApiUrl}{_apiAliasConfig.PostInvoice}", contentGetAppointmentOL);
                if (!resultApi.isSuccessful)
                {
                    throw new ArgumentException(resultApi.message);
                }

                return Json(new
                {
                    error = false,
                    data = resultApi.Payload
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        public async Task<JsonResult> updateInvoice([FromBody] InvoiceViewModel model)
        {
            try
            {
                var contentType = "application/json";

                ApiResult<InvoiceViewModel> resultApi = new ApiResult<InvoiceViewModel>();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var dataParameter = JsonConvert.SerializeObject(model);
                StringContent contentGetAppointmentOL = new StringContent(dataParameter, Encoding.UTF8, "application/json");

                resultApi = await _client.PutAsync<InvoiceViewModel>($"{_apiConfig.ApiUrl}{_apiAliasConfig.PutInvoice}/{model.Id}", contentGetAppointmentOL);
                if (!resultApi.isSuccessful)
                {
                    throw new ArgumentException(resultApi.message);
                }

                return Json(new
                {
                    error = false,
                    data = resultApi.Payload
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var dataoptionUOM = await getStaticDatas(StaticDataGroup.UOM);
                var dataoptionCurrency = await getStaticDatas(StaticDataGroup.Currency);
                var dataoptionLanguage = await getStaticDatas(StaticDataGroup.Language);

                ApiResult<List<string>> apiResultCs = new ApiResult<List<string>>();
                string url = $"{_apiConfig.ApiUrl}{_apiAliasConfig.GetCustomers}";
                apiResultCs = await _client.GetAsync<List<string>>(url);

                ViewBag.Id = id;
                ViewBag.Uom = JsonConvert.SerializeObject(dataoptionUOM);
                ViewBag.Currency = JsonConvert.SerializeObject(dataoptionCurrency);
                ViewBag.Language = JsonConvert.SerializeObject(dataoptionLanguage);
                ViewBag.Customer = JsonConvert.SerializeObject(apiResultCs.Payload);
                return View();
            }
            catch (Exception)
            {
                ViewBag.Uom = "";
                ViewBag.Currency = "";
                ViewBag.Language = "";
                ViewBag.Customer = "";
                ViewBag.Id = id;
                return View();
            }
        }

        public async Task<JsonResult> getInvoiceId(string id)
        {
            try
            {
                ApiResult<InvoiceViewModel> apiResult = new ApiResult<InvoiceViewModel>();
                string url = $"{_apiConfig.ApiUrl}{_apiAliasConfig.GetInvoice}/{id}";
                apiResult = await _client.GetAsync<InvoiceViewModel>(url);

                var partialViewDetail = _viewRenderService.RenderPartialViewToString(this, "_ListDetailInvoice", apiResult.Payload.invoiceDetails);

                return Json(new
                {
                    error = false,
                    dataDetail = partialViewDetail,
                    dataHeader = apiResult.Payload
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, errorMsg = ex.Message });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> DataTableInvoice(IDataTablesRequest request, string param)
        {
            try
            {
                var oParam = JsonConvert.DeserializeObject<ParamSearchInvoiceViewModel>(param);
                var model = await GetResultDataInvoices(request, oParam.PurchaseOrderCode, oParam.InvoiceCode, oParam.InvoiceDate);
                return model;
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> DataTablePopUp(IDataTablesRequest request, string param)
        {
            try
            {
                var oParam = JsonConvert.DeserializeObject<ParamSearchPurchaseOrderViewModel>(param);
                var model = await GetResultDataPo(request, oParam.PurchaseOrderCode, oParam.PurchaseOrderPic);
                return model;
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private async Task<DataTablesJsonResult> GetResultDataInvoices(IDataTablesRequest request,
                                                                        string purchaseOrderCode, string invoiceCode,
                                                                        DateTime? invoiceDate)
        {
            int oPage = 1;
            int returnPageNumber = 0;
            int returnTotalRecords = 0;

            try
            {
                if (request.Start > 0)
                {
                    oPage = (request.Start / 10) + 1;
                }

                var contentType = "application/json";
                var dataParamToApi = new ParamSearchInvoiceViewModel();
                dataParamToApi.InvoiceDate = invoiceDate != null ? invoiceDate.Value.Date : invoiceDate;
                dataParamToApi.InvoiceCode = invoiceCode;
                dataParamToApi.PurchaseOrderCode = purchaseOrderCode;
                dataParamToApi.page = oPage;
                dataParamToApi.itemPerPage = 10;
                
                ApiResult<GetInvoicesViewModel> resultApi = new ApiResult<GetInvoicesViewModel>();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var dataParameter = JsonConvert.SerializeObject(dataParamToApi);
                StringContent contentGetAppointmentOL = new StringContent(dataParameter, Encoding.UTF8, "application/json");

                resultApi = await _client.PostAsync<GetInvoicesViewModel>($"{_apiConfig.ApiUrl}{_apiAliasConfig.GetInvoicesPagging}", contentGetAppointmentOL);
                if (!resultApi.isSuccessful)
                {
                    throw new ArgumentException(resultApi.message);
                }

                var datafromApi = resultApi.Payload.DataInvoices.AsQueryable();

                if (datafromApi.Any())
                {
                    returnPageNumber = oPage;
                    returnTotalRecords = resultApi.Payload.CountData;
                }

                #region Shorted
                IOrderedQueryable<InvoiceViewModel> dataSorted = null;
                #endregion
                var response = DataTablesResponse.Create(request, returnPageNumber, returnTotalRecords, dataSorted != null ? dataSorted : datafromApi);

                return new DataTablesJsonResult(response, true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private async Task<DataTablesJsonResult> GetResultDataPo(IDataTablesRequest request,
                                                                        string purchaseOrderCode, string poPic)
        {
            int oPage = 1;
            int returnPageNumber = 0;
            int returnTotalRecords = 0;

            try
            {
                if (request.Start > 0)
                {
                    oPage = (request.Start / 10) + 1;
                }

                var contentType = "application/json";
                var dataParamToApi = new ParamSearchPurchaseOrderViewModel();
                dataParamToApi.PurchaseOrderPic = poPic;
                dataParamToApi.PurchaseOrderCode = purchaseOrderCode;
                dataParamToApi.page = oPage;
                dataParamToApi.itemPerPage = 10;

                ApiResult<GetPurchaseOrdersViewModel> resultApi = new ApiResult<GetPurchaseOrdersViewModel>();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                var dataParameter = JsonConvert.SerializeObject(dataParamToApi);
                StringContent contentGetAppointmentOL = new StringContent(dataParameter, Encoding.UTF8, "application/json");

                resultApi = await _client.PostAsync<GetPurchaseOrdersViewModel>($"{_apiConfig.ApiUrl}{_apiAliasConfig.GetPurchaseOrders}", contentGetAppointmentOL);
                if (!resultApi.isSuccessful)
                {
                    throw new ArgumentException(resultApi.message);
                }

                var datafromApi = resultApi.Payload.DataPurchaseOrders.AsQueryable();

                if (datafromApi.Any())
                {
                    returnPageNumber = oPage;
                    returnTotalRecords = resultApi.Payload.CountData;
                }

                #region Shorted
                IOrderedQueryable<PurchaseOrderViewModel> dataSorted = null;
                #endregion
                var response = DataTablesResponse.Create(request, returnPageNumber, returnTotalRecords, dataSorted != null ? dataSorted : datafromApi);

                return new DataTablesJsonResult(response, true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private async Task<List<string>> getStaticDatas(string group)
        {
            try
            {
                ApiResult<List<string>> apiResult = new ApiResult<List<string>>();
                string url = $"{_apiConfig.ApiUrl}{_apiAliasConfig.GetStaticDatasByGroup}/{group}";
                apiResult = await _client.GetAsync<List<string>>(url);
                return apiResult.Payload;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

    }
}
