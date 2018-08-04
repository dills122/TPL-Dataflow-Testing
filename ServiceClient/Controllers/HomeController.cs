using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelineService;
using PipelineService.Models;
using ServiceClient.Models;

namespace ServiceClient.Controllers
{
    public class HomeController : Controller
    {
        private IPipelineService _pipelineService;
        public HomeController(IPipelineService pipelineService)
        {
            _pipelineService = pipelineService;
        }

        public IActionResult Index()
        {
            GenerateOrder randomOrder = new GenerateOrder(OrderType.CarLoan);

            _pipelineService.FillPipeline(randomOrder.order);
            _pipelineService.FillPipeline(new GenerateOrder(OrderType.HomeLoan).order);
            _pipelineService.FillPipeline(new GenerateOrder(OrderType.OtherLoan).order);
            _pipelineService.FillPipeline(new GenerateOrder(OrderType.PersonalLoan).order);
            _pipelineService.FillPipeline(new GenerateOrder(OrderType.CarLoan).order);

            _pipelineService.WaitForResults();

            var test = _pipelineService.GetResults().Result;
            //Flush the results of the pipeline after receiving them
            _pipelineService.FlushPipeline();
            List<Order> receivedOrders = new List<Order>();
            foreach (object inputOrder in test)
            {
                var order = (Order)inputOrder;
                if (!string.IsNullOrEmpty(order.ClientFName))
                {
                    receivedOrders.Add((Order)inputOrder);
                }
            }
            ViewBag.Orders = receivedOrders;
            return View(receivedOrders);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
