using MediatR;
using Server.Data;
using Server.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Categories
{
    public class CreateCategoryCommand : IRequest<string>
    {
        public string Id { get; set; } = "";
        public string Name_TR { get; set; } = "";
        public string Name_EN { get; set; } = "";
        public string Name_DE { get; set; } = "";
        public bool IsSubCategory { get; set; } = false;
        public string? ParentId { get; set; }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, string>
    {
        private readonly AppDbContext _context;

        public CreateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = new CategoryEntity
            {
                Id = request.Id,
                Name_TR = request.Name_TR,
                Name_EN = request.Name_EN,
                Name_DE = request.Name_DE,
                IsSubCategory = request.IsSubCategory,
                ParentId = request.IsSubCategory ? request.ParentId : null
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
