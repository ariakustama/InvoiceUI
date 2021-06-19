using Microsoft.AspNetCore.Mvc;
using InvoiceUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceUI.ViewComponents
{
    public class BreadcrumbViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<BreadcrumbViewModel> breadcrumb)
        {
            if (breadcrumb != null)
            {
                if (breadcrumb.Any())
                {
                    var Action = ViewContext.RouteData.Values["action"];
                    var Controller = ViewContext.RouteData.Values["controller"];

                    switch (Action)
                    {
                        case "Create":
                            breadcrumb.RemoveRange(2, 1);
                            break;
                        case "Edit":
                            breadcrumb.RemoveRange(2, 1);
                            break;
                        default:
                            break;
                    }
                }

                ViewBag.BreadcrumbDetail = breadcrumb;
            }
            else
            {
                ViewBag.BreadcrumbDetail = new HashSet<BreadcrumbViewModel>();
            }

            return View();
        }
    }
}
