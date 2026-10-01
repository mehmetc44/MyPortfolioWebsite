using MediatR;
using Server.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Categories
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public string Id { get; set; } = "";
        public string Name_TR { get; set; } = "";
        public string Name_EN { get; set; } = "";
        public string Name_DE { get; set; } = "";
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly AppDbContext _context;

        public UpdateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
            if (category == null) return false;

            category.Name_TR = request.Name_TR;
            category.Name_EN = request.Name_EN;
            category.Name_DE = request.Name_DE;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
