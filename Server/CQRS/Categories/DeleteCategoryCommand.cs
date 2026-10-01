using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Categories
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public string Id { get; set; } = "";
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
            if (category == null) return false;

            _context.Categories.Remove(category);

            // If it's a main category, remove its subcategories as well
            if (!category.IsSubCategory)
            {
                var subs = await _context.Categories
                    .Where(c => c.IsSubCategory && c.ParentId == request.Id)
                    .ToListAsync(cancellationToken);
                
                _context.Categories.RemoveRange(subs);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
