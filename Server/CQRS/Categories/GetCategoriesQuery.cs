using MediatR;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Categories
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>> { }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _context.Categories.ToListAsync(cancellationToken);
            
            // Get main categories
            var mainCategories = allCategories
                .Where(c => !c.IsSubCategory)
                .Select(CategoryMappingConfig.MapToDto)
                .ToList();
            
            // Map subcategories to their parents
            foreach (var main in mainCategories)
            {
                main.SubCategories = allCategories
                    .Where(c => c.IsSubCategory && c.ParentId == main.Id)
                    .Select(CategoryMappingConfig.MapToDto)
                    .ToList();
            }

            return mainCategories;
        }
    }
}
