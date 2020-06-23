﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BDTest.Maps;
using BDTest.ReportGenerator.RazorServer.Interfaces;
using BDTest.ReportGenerator.RazorServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BDTest.ReportGenerator.RazorServer.Controllers
{
    [Route("bdtest")]
    public class BDTestController : Controller
    {
        private readonly IDataController _dataController;
        private readonly ILogger<BDTestController> _logger;
        
        public BDTestController(IDataController dataController, ILogger<BDTestController> logger)
        {
            _dataController = dataController;
            _logger = logger;
        }

        [HttpPost]
        [Route("data")]
        public async Task<IActionResult> Index([FromBody] BDTestOutputModel bdTestOutputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var id = bdTestOutputModel.Id ?? Guid.NewGuid().ToString("N");

            await _dataController.StoreData(bdTestOutputModel, id);

            return RedirectToAction("Summary", "BDTest", new { id });
        }

        [HttpGet]
        [Route("report/{id}")]
        public IActionResult GetReport([FromRoute] string id)
        {
            return RedirectToAction("Summary", "BDTest", new { id });
        }

        [HttpGet]
        [Route("report/{id}/summary")]
        public Task<IActionResult> Summary([FromRoute] string id)
        {
            return GetView(id, model => View("Summary", model));
        }

        [HttpGet]
        [Route("report/{id}/stories")]
        public Task<IActionResult> Stories([FromRoute] string id)
        {
            return GetView(id, model => View("Stories", model));
        }

        [HttpGet]
        [Route("report/{id}/all-scenarios")]
        public Task<IActionResult> AllScenarios([FromRoute] string id)
        {
            return GetView(id, model => View("AllScenarios", model));
        }

        [HttpGet]
        [Route("report/{id}/timings")]
        public Task<IActionResult> Timings([FromRoute] string id)
        {
            return GetView(id, model => View("TestTimesSummary", model));
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Redirect()
        {
            return RedirectToAction("TestRuns");
        }

        [HttpGet]
        [Route("report/test-runs")]
        public async Task<IActionResult> TestRuns([FromQuery] string reportIds)
        {
            var records = await _dataController.GetRunsBetweenTimes(DateTime.MinValue, DateTime.MaxValue);

            return View("TestRunList", records.OrderByDescending(record => record.DateTime).ToList());
        }

        [HttpGet]
        [Route("report/test-run-times")]
        public async Task<IActionResult> TestRunTimes([FromQuery] string reportIds)
        {
            var reportIdsArray = reportIds?.Split(',') ?? Array.Empty<string>();

            if (!reportIdsArray.Any())
            {
                return RedirectToAction("TestRuns", "BDTest");
            }
            
            var foundReports = (await Task.WhenAll(reportIdsArray.Select(_dataController.GetData))).ToList();

            if (!foundReports.Any())
            {
                return NotFound("No reports found");
            }

            return View("MultipleTestRunsTimes", foundReports);
        }
        
        [HttpGet]
        [Route("report/test-run-flakiness")]
        public async Task<IActionResult> TestRunFlakiness([FromQuery] string reportIds)
        {
            var reportIdsArray = reportIds?.Split(',') ?? Array.Empty<string>();

            if (!reportIdsArray.Any())
            {
                return RedirectToAction("TestRuns", "BDTest");
            }
            
            var foundReports = (await Task.WhenAll(reportIdsArray.Select(_dataController.GetData))).ToList();

            if (!foundReports.Any())
            {
                return NotFound("No reports found");
            }

            return View("MultipleTestRunsFlakiness", foundReports);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private async Task<IActionResult> GetView(string id, Func<BDTestOutputModel, IActionResult> viewAction)
        {
            var model = await _dataController.GetData(id);
            
            if (model == null)
            {
                return NotFound($"Report Not Found: {id}");
            }

            ViewBag.Id = id;

            return viewAction(model);
        }
    }
}