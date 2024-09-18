﻿using Microsoft.AspNetCore.Mvc;
using Serilog;
using STIMULUS_V2.Shared.Interface.ChildInterface;
using STIMULUS_V2.Shared.Models.Entities;
using System.ComponentModel;

namespace STIMULUS_V2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursController : Controller
    {
        private readonly ICoursService coursService;

        public CoursController(ICoursService coursService)
        {
            this.coursService = coursService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Cours cours)
        {
            var response = await coursService.Create(cours);
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"Create([FromBody] Cours cours = {cours}) \n  Response: {apiResponse}");
            return apiResponse;
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await coursService.Delete(id);
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"Delete(int id = {id}) \n  Response: {apiResponse}");
            return apiResponse;
        }

        [HttpGet("Fetch/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await coursService.Get(id);
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"Get(int id = {id}) \n  Response: {apiResponse}");
            return apiResponse;
        }

        [HttpGet("Fetch/All")]
        public async Task<IActionResult> GetAll()
        {
            var response = await coursService.GetAll();
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"GetAll() \n  Response: {apiResponse}");
            return apiResponse;
        }

        [HttpGet("Fetch/All/{id}")]
        public async Task<IActionResult> GetAllById(int id)
        {
            var response = await coursService.GetAllById(id);
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"GetAllById(int id = {id}) \n  Response: {apiResponse}");
            return apiResponse;
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cours cours)
        {
            var response = await coursService.Update(id, cours);
            var log = Log.ForContext<CoursController>();
            var apiResponse = StatusCode(response.StatusCode, response);
            log.Information($"Update(int id = {id}, [FromBody] Cours cours = {cours}) \n  Response: {apiResponse}");
            return apiResponse;
        }
    }
}
