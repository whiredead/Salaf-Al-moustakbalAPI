
namespace ProjectPfe.Services
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ProjectPfe.Data;
    using ProjectPfe.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ProjectPfe.DTOs;
    using AutoMapper;
    
    public class Service
    {
        private readonly IdentityContext _context;
        private readonly IMapper _mapper;
        public Service(IdentityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<BarDto>> GetBarsAsync()
        {
            var bars = await _context.Bars.Select( x => new BarDto {
                Title = x.Title,
                Id = x.Id,
                hasChild=x.hasChild,
                MenusDto = x.Menus.Select(menu => new MenuDto
                {
                    Id = menu.Id,
                    Name = menu.Name,
                    HasChild = menu.HasChild,
                    ParentId = menu.ParentId,
                    BarId = menu.BarId,
                }).ToList()

            } ).ToListAsync();


            return bars;
        }
        public async Task<BarDto> GetBarAsyncById(int id)
        {
            var bar = await _context.Bars
                .Where(b => b.Id == id)
                .Select(b => new BarDto
                {
                    Title = b.Title,
                    Id = b.Id,
                    hasChild=b.hasChild,
                    MenusDto = b.Menus
                        .Where(m => m.BarId == b.Id) // Filter menus based on BarId
                        .Select(menu => new MenuDto
                        {
                            Id = menu.Id,
                            Name = menu.Name,
                            HasChild = menu.HasChild,
                            ParentId = menu.ParentId,
                            BarId = menu.BarId,
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return bar;
        }

    }

}
