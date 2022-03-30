using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;
using PersonDto = SoMeSimulator.Data.PersonDto;

namespace SomeSimulator.Services
{
    public class CommentService: ICrudInterface<CommentDto>
    {
        private readonly IMapper _mapper;
        private readonly IEntityFactory _factory;
        private readonly SoMeContext _dbContext;

        public CommentService(IMapper mapper, IEntityFactory factory, SoMeContext dbContext)
        {
            _mapper = mapper;
            _factory = factory;
            _dbContext = dbContext;
        }
        public async Task<CommentDto> GetByIdAsync(int id)
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            var dto = _mapper.Map<CommentDto>(comment);
            return dto;
        }

        public async Task<CommentDto> CreateAsync(CommentDto commentDto)
        {
            var scenario = await _dbContext.Scenarios.FirstOrDefaultAsync(x => x.Id == commentDto.ScenarioId);
            var personDto = new PersonDto(commentDto.Sender, "/circle.svg");
            var messageFlow = _mapper.Map<MessageFlow>(commentDto.MessageFlow);
            var phases = new List<Phase>();
            foreach (var p in commentDto.Phases)
            {
                var phase = await _dbContext.Phases.FirstOrDefaultAsync(x => x.Id == p);
                phases.Add(phase);
            }

            var newComment = _factory.Comment(scenario, commentDto.Text, commentDto.Props, messageFlow, phases, personDto );

            await _dbContext.Comments.AddAsync(newComment);
            await _dbContext.SaveChangesAsync();
            
            return _mapper.Map<Comment, CommentDto>(newComment);
        }

        public async Task<CommentDto> UpdateAsync(CommentDto commentDto)
        {
            var comment = await _dbContext.Comments                          
                .Include(p => p.PhaseLink)                  
                .SingleAsync(p => p.Id == commentDto.Id);
            comment.PhaseLink.Clear();
            
            foreach (var p in commentDto.Phases)
            {
                var phase = await _dbContext.Phases.FirstOrDefaultAsync(x => x.Id == p);
                var pc = new PhaseComment()
                {
                    Comment = comment,
                    CommentId = comment.Id,
                    Phase = phase,
                    PhaseId = p
                };
                comment.PhaseLink.Add(pc);
            }
            
            comment.Avatar = commentDto.Avatar;
            comment.Props = commentDto.Props;
            comment.Scenario = await _dbContext.Scenarios.FindAsync(commentDto.ScenarioId);
            comment.Sender = commentDto.Sender;
            comment.Text = commentDto.Text;
            comment.MessageFlow = _mapper.Map<MessageFlow>(commentDto.MessageFlow);
            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
            
            return _mapper.Map<Comment, CommentDto>(comment);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext
                .Comments
                .FindAsync(id);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}