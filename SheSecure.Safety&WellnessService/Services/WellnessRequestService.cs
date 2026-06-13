using SheSecure.WellnessSafetyService.DTOs.Requests;
using SheSecure.WellnessSafetyService.DTOs.Responses;
using SheSecure.WellnessSafetyService.Entities;
using SheSecure.WellnessSafetyService.Interfaces;

namespace SheSecure.WellnessSafetyService.Services
{
    public class WellnessRequestService
        : IWellnessRequestService
    {
        private readonly
            IWellnessRequestRepository _repository;

        public WellnessRequestService(
            IWellnessRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<WellnessRequestResponseDTO>
            CreateRequestAsync(
                CreateWellnessRequestDTO dto)
        {
            var request = new WellnessRequest
            {
                EmployeeId = dto.EmployeeId,
                RequestType = dto.RequestType,
                Description = dto.Description,
                Priority = dto.Priority
            };

            var savedRequest =
                await _repository.CreateRequestAsync(
                    request);

            return new WellnessRequestResponseDTO
            {
                Id = savedRequest.Id,
                EmployeeId = savedRequest.EmployeeId,
                RequestType = savedRequest.RequestType,
                Description = savedRequest.Description,
                Priority = savedRequest.Priority,
                Status = savedRequest.Status,
                AssignedTo = savedRequest.AssignedTo,
                CreatedAt = savedRequest.CreatedAt
            };
        }

        public async Task<
            List<WellnessRequestResponseDTO>>
            GetAllRequestsAsync()
        {
            var requests =
                await _repository.GetAllRequestsAsync();

            return requests.Select(x =>
                new WellnessRequestResponseDTO
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    RequestType = x.RequestType,
                    Description = x.Description,
                    Priority = x.Priority,
                    Status = x.Status,
                    AssignedTo = x.AssignedTo,
                    CreatedAt = x.CreatedAt
                }).ToList();
        }

        public async Task<
            WellnessRequestResponseDTO?>
            GetByIdAsync(int id)
        {
            var request =
                await _repository.GetByIdAsync(id);

            if (request == null)
            {
                return null;
            }

            return new WellnessRequestResponseDTO
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                RequestType = request.RequestType,
                Description = request.Description,
                Priority = request.Priority,
                Status = request.Status,
                AssignedTo = request.AssignedTo,
                CreatedAt = request.CreatedAt
            };
        }

        public async Task UpdateStatusAsync(
            UpdateWellnessRequestStatusDTO dto)
        {
            var request =
                await _repository.GetByIdAsync(
                    dto.RequestId);

            if (request == null)
            {
                throw new Exception(
                    "Wellness request not found");
            }

            request.Status = dto.Status;
            request.AssignedTo = dto.AssignedTo;

            await _repository.UpdateRequestAsync(
                request);
        }
    }
}