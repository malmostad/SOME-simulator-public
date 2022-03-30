using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;
using SomeSimulator.DTOs;

namespace SomeSimulator.Mapper
{
    public class DataMapper : Profile
    {
        public DataMapper()
        {
            
            CreateMap<AMessageEntity, AMessageDto>().ReverseMap();
            CreateMap<EntityBase, EntityBaseDto>().ReverseMap();
            
            CreateMap<Comment, CommentDto>()
                .ForMember(d=>d.Phases, opt=>opt.MapFrom(p=>p.PhaseLink.Select(x=>x.PhaseId)))
                .ReverseMap()
                .ForMember(d => d.Avatar, opt => opt.MapFrom(p => "/circle.svg"));
                
            
            CreateMap<Phase, PhaseDto>()
                .ForMember(p => p.ScenarioEvents, opt => opt.MapFrom(p => p.ScenarioEvents.OrderBy(s => s.TimePercent)))
                .ReverseMap();
            
            CreateMap<Post, PostDto>()
                .ForMember(d=>d.Phases, opt=>opt.MapFrom(p=>p.PhaseLink.Select(x=>x.PhaseId)))
                .ReverseMap()
                .ForMember(d => d.Avatar, opt => opt.MapFrom(p => "/circle.svg"));
            
            CreateMap<MessageFlow, MessageFlowDto>().ReverseMap();

            CreateMap<Scenario, ScenarioDto>()
                .ForMember(s => s.Posts, opt => opt.MapFrom(s => s.Phases.SelectMany(p => p.PostLink).Select(pl
                    => pl.Post).Distinct().ToList()));

            CreateMap<MessageFlow, MessageFlowDto>().ReverseMap();

            CreateMap<ScenarioEvent, ScenarioEventDto>()
                .ReverseMap();
        }
    }
}