using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;

namespace SomeSimulator.Services
{
    public class ScenarioEventService: ICrudInterface<ScenarioEventDto>
    {
        private readonly SoMeContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEntityFactory _factory;

        public ScenarioEventService(SoMeContext dbContext, IMapper mapper, IEntityFactory factory)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _factory = factory;
        }

        public async Task<ScenarioEventDto> GetByIdAsync(int id)
        {
            return _mapper.Map<ScenarioEventDto>(await _dbContext.ScenarioEvents.FindAsync(id));
        }
        public async Task<ScenarioEventDto> CreateAsync(ScenarioEventDto eventDto)
        {
            var entity = _mapper.Map<ScenarioEvent>(eventDto);
            var phase = _dbContext.Phases.First(x => x.Id == eventDto.PhaseId);
            phase.ScenarioEvents.Add(entity);
            _dbContext.ScenarioEvents.Add(entity);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ScenarioEvent, ScenarioEventDto>(entity);
        }

        public async Task<ScenarioEventDto> UpdateAsync(ScenarioEventDto eventDto)
        {
            var entity = _mapper.Map<ScenarioEvent>(eventDto);
            var phase = await _dbContext.Phases.FirstOrDefaultAsync(x => x.Id == eventDto.PhaseId);
            entity.Phase = phase;
            _dbContext.ScenarioEvents.Update(entity);
            await _dbContext.SaveChangesAsync();
            
            return _mapper.Map<ScenarioEvent, ScenarioEventDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext
                .ScenarioEvents
                .FindAsync(id);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}