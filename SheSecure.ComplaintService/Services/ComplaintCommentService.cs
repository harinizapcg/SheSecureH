using SheSecure.ComplaintService.DTOs.Requests;
using SheSecure.ComplaintService.DTOs.Responses;
using SheSecure.ComplaintService.Entities;
using SheSecure.ComplaintService.Interfaces;

namespace SheSecure.ComplaintService.Services
{
    public class ComplaintCommentService
        : IComplaintCommentService
    {
        private readonly IComplaintCommentRepository _repository;

        public ComplaintCommentService(
            IComplaintCommentRepository repository)
        {
            _repository = repository;
        }

        public async Task<ComplaintCommentResponseDTO>
            AddCommentAsync(AddComplaintCommentDTO dto)
        {
            var comment = new ComplaintComment
            {
                ComplaintId = dto.ComplaintId,
                UserId = dto.UserId,
                Comment = dto.Comment,
                IsInternal = dto.IsInternal
            };

            var savedComment =
                await _repository.AddCommentAsync(comment);

            return new ComplaintCommentResponseDTO
            {
                Id = savedComment.Id,
                ComplaintId = savedComment.ComplaintId,
                UserId = savedComment.UserId,
                Comment = savedComment.Comment,
                IsInternal = savedComment.IsInternal,
                CreatedAt = savedComment.CreatedAt
            };
        }

        public async Task<List<ComplaintCommentResponseDTO>>
            GetCommentsByComplaintIdAsync(int complaintId)
        {
            var comments =
                await _repository
                    .GetCommentsByComplaintIdAsync(
                        complaintId);

            return comments.Select(x =>
                new ComplaintCommentResponseDTO
                {
                    Id = x.Id,
                    ComplaintId = x.ComplaintId,
                    UserId = x.UserId,
                    Comment = x.Comment,
                    IsInternal = x.IsInternal,
                    CreatedAt = x.CreatedAt
                }).ToList();
        }
    }
}