using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;

namespace SomeSimulator.Services
{
    public class ScenarioService:ICrudInterface<ScenarioDto>
    {
        private readonly SoMeContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioService(SoMeContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ScenarioDto> GetByIdAsync(int id)
        {
            var entity = await _dbContext.Scenarios
                .Include(x => x.Comments)
                .Include(x => x.Phases).ThenInclude(p => p.CommentLink).ThenInclude(cl => cl.Comment)
                .Include(x => x.Phases).ThenInclude(p => p.PostLink).ThenInclude(pl => pl.Post)
                .Include(x => x.Phases).ThenInclude(p => p.ScenarioEvents)
                .AsNoTracking()
                .FirstOrDefaultAsync(x=>x.Id == id);
            
            entity.Phases= entity.Phases.OrderBy(x => x.StartPercent).ToList();
            
            var dto = _mapper.Map<ScenarioDto>(entity);
            return dto;
        }

        public Task<ScenarioDto> CreateAsync(ScenarioDto item)
        {
            throw new System.NotImplementedException();
        }

        public Task<ScenarioDto> UpdateAsync(ScenarioDto item)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}