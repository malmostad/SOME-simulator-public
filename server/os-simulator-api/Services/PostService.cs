using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data;
using SoMeSimulator.Data.Models;
using SoMeSimulator.Data.Models.SessionLogs;
using SomeSimulator.DTOs;
using SomeSimulator.Interfaces;

namespace SomeSimulator.Services
{
    public class PostService: ICrudInterface<PostDto>
    {
        private readonly IMapper _mapper;
        private readonly IEntityFactory _factory;
        private readonly SoMeContext _dbContext;

        public PostService(IMapper mapper, IEntityFactory factory, SoMeContext dbContext)
        {
            _mapper = mapper;
            _factory = factory;
            _dbContext = dbContext;
        }

        public async Task<PostDto> GetByIdAsync(int id)
        {
            return _mapper.Map<PostDto>(await _dbContext.Posts.FindAsync(id));
        }

        public async Task<PostDto> CreateAsync(PostDto postDto)
        {
            var mappedPost = _mapper.Map<Post>(postDto);
            List<Phase> phases = new List<Phase>();
            foreach (var p in postDto.Phases)
            {
                var phase = await _dbContext.Phases.FirstOrDefaultAsync(x => x.Id == p);
                phases.Add(phase);
            }
            
            var newPost = _factory
                .Post(mappedPost.Text, mappedPost.Sender, "/circle.svg", mappedPost.MessageFlow, phases);
            newPost.Heading = mappedPost.Heading;
            
            await _dbContext.Posts.AddAsync(newPost);
            await _dbContext.SaveChangesAsync();
            
            
            return _mapper.Map<Post, PostDto>(newPost);
        }

        public async Task<PostDto> UpdateAsync(PostDto postDto)
        {
            var post = await _dbContext.Posts                          
                .Include(p => p.PhaseLink)                  
                .SingleAsync(p => p.Id == postDto.Id);
            post.PhaseLink.Clear();
            
            foreach (var p in postDto.Phases)
            {
                var phase = await _dbContext.Phases.FirstOrDefaultAsync(x => x.Id == p);
                var pp = new PhasePost()
                {
                    Post = post,
                    PostId = post.Id,
                    Phase = phase,
                    PhaseId = p
                };
                post.PhaseLink.Add(pp);
            }

            post.Avatar = postDto.Avatar;
            post.MessageFlow = _mapper.Map<MessageFlow>(postDto.MessageFlow);
            post.Heading = postDto.Heading;
            post.Sender = postDto.Sender;
            post.Text = postDto.Text;
            
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();
            
            
            return _mapper.Map<Post, PostDto>(post);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext
                .Posts
                .FindAsync(id);
            _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}