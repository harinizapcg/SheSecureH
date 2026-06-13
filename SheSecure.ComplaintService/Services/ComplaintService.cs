using SheSecure.ComplaintService.DTOs.Requests;
using SheSecure.ComplaintService.DTOs.Responses;
using SheSecure.ComplaintService.Entities;
using SheSecure.ComplaintService.Interfaces;

namespace SheSecure.ComplaintService.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _repository;
        //private readonly IComplaintStatusHistoryRepository _historyRepository;
        public ComplaintService(IComplaintRepository repository)
        {
            _repository = repository;
        }

        public async Task<ComplaintResponseDTO> CreateComplaintAsync(
            CreateComplaintDTO dto,
            string employeeId)
        {
            var complaint = new Complaint
            {
                EmployeeId = employeeId,
                Category = dto.Category,
                Subject = dto.Subject,
                Description = dto.Description,
                Priority = dto.Priority,
                IsAnonymous = dto.IsAnonymous,
                Status = "Submitted",
                ComplaintNumber = GenerateComplaintNumber()
            };

            var createdComplaint =
                await _repository.CreateComplaintAsync(complaint);

            return new ComplaintResponseDTO
            {
                Id = createdComplaint.Id,
                ComplaintNumber = createdComplaint.ComplaintNumber,
                Subject = createdComplaint.Subject,
                Status = createdComplaint.Status,
                CreatedAt = createdComplaint.CreatedAt
            };
        }

        public async Task<List<ComplaintResponseDTO>> GetAllComplaintsAsync()
        {
            var complaints = await _repository.GetAllComplaintsAsync();

            return complaints.Select(x => new ComplaintResponseDTO
            {
                Id = x.Id,
                ComplaintNumber = x.ComplaintNumber,
                Subject = x.Subject,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task<ComplaintResponseDTO> GetComplaintByIdAsync(int id)
        {
            var complaint = await _repository.GetComplaintByIdAsync(id);

            if (complaint == null)
                throw new Exception("Complaint not found");

            return new ComplaintResponseDTO
            {
                Id = complaint.Id,
                ComplaintNumber = complaint.ComplaintNumber,
                Subject = complaint.Subject,
                Status = complaint.Status,
                CreatedAt = complaint.CreatedAt
            };
        }

        public async Task UpdateComplaintStatusAsync(
            UpdateComplaintStatusDTO dto)
        {
            var complaint =
                await _repository.GetComplaintByIdAsync(dto.ComplaintId);

            if (complaint == null)
                throw new Exception("Complaint not found");

            complaint.Status = dto.Status;
            complaint.ResolutionNotes = dto.ResolutionNotes;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateComplaintAsync(complaint);
        }

        public async Task AssignComplaintAsync(AssignComplaintDTO dto)
        {
            var complaint =
                await _repository.GetComplaintByIdAsync(dto.ComplaintId);

            if (complaint == null)
                throw new Exception("Complaint not found");

            complaint.AssignedTo = dto.AssignedTo;

            await _repository.UpdateComplaintAsync(complaint);
        }

        private string GenerateComplaintNumber()
        {
            return $"CMP-{DateTime.UtcNow.Ticks}";
        }
    }
}