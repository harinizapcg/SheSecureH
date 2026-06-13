using Microsoft.AspNetCore.Mvc;
using SheSecure.WellnessSafetyService.DTOs.Requests;
using SheSecure.WellnessSafetyService.Interfaces;

namespace SheSecure.WellnessSafetyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WellnessRequestController
        : ControllerBase
    {
        private readonly
            IWellnessRequestService _service;

        public WellnessRequestController(
            IWellnessRequestService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult>
            CreateRequest(
                [FromBody]
                CreateWellnessRequestDTO dto)
        {
            var result =
                await _service.CreateRequestAsync(dto);

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult>
            GetAllRequests()
        {
            var result =
                await _service.GetAllRequestsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            var result =
                await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(
                    "Wellness request not found");
            }

            return Ok(result);
        }

        [HttpPut("status")]
        public async Task<IActionResult>
            UpdateStatus(
                [FromBody]
                UpdateWellnessRequestStatusDTO dto)
        {
            await _service.UpdateStatusAsync(dto);

            return Ok(
                "Wellness request updated successfully");
        }
    }
}