using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PipelineService;
using PipelineService.Models;
using PipelineService.Tests;
using PipelineServicePrototype;
using ServiceClient.Models;

namespace ServiceClient.Controllers
{
    public class HomeController : Controller
    {
        private IPipelineService _pipelineService;
        private IPipeline<User> _pipeline;

        public HomeController(IPipelineService pipelineService, IPipeline<User> pipeline)
        {
            _pipelineService = pipelineService;
            _pipeline = pipeline;
        }

        public async Task<ActionResult> Index()
        {
            User user = new User();
            user.Username = "dills122";
            user.Name = "Dylan Steele";

            await _pipeline.Post(user);
            await _pipeline.Complete();
            await _pipeline.WaitForResultsAsync();
            var users = _pipeline.GetResults().Result;
            await _pipeline.Flush();

            //TestPipeline pipeline = new TestPipeline();
            //pipeline.Post(user);
            //pipeline.InputBlock.Complete();
            //await pipeline.CompletionTask;
            //var t = pipeline.Result;
            //List<User> test = new List<User>();
            //test.Add(user);
            ViewBag.Users = users;

            //TimingTest timingTest = new TimingTest();

            //GenerateOrder randomOrder = new GenerateOrder(OrderType.CarLoan);

            //_pipelineService.FillPipeline(randomOrder.order);
            //_pipelineService.FillPipeline(new GenerateOrder(OrderType.HomeLoan).order);
            //_pipelineService.FillPipeline(new GenerateOrder(OrderType.OtherLoan).order);
            //_pipelineService.FillPipeline(new GenerateOrder(OrderType.PersonalLoan).order);
            //_pipelineService.FillPipeline(new GenerateOrder(OrderType.CarLoan).order);

            //_pipelineService.WaitForResults();

            //var test = _pipelineService.GetResults().Result;
            ////Flush the results of the pipeline after receiving them
            //_pipelineService.FlushPipeline();
            //List<Order> receivedOrders = new List<Order>();
            //foreach (object inputOrder in test)
            //{
            //    var order = (Order)inputOrder;
            //    if (!string.IsNullOrEmpty(order.ClientFName))
            //    {
            //        receivedOrders.Add((Order)inputOrder);
            //    }
            //}
            //ViewBag.Orders = receivedOrders;

            return View();
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
