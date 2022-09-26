﻿using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Activities
{
  public class List
  {
    public class Query : IRequest<Result<PagedList<ActivityDto>>> {

      public PagingParams Params { get; set; }
      //public ActivityParams Params { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
    {
      private readonly DataContext _context;
      private readonly IMapper _mapper;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
      {
        _context = context;
        _mapper = mapper;
        _userAccessor = userAccessor;
      }

      public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
      {
        // var activities = await _context.ActivitiesTable
        //    .Include(a => a.Attendees)
        //    .ThenInclude(u => u.AppUser)
        //    .ToListAsync(cancellationToken);
        // var data = _mapper.Map<List<ActivityDto>>(activities);
        // return Result<List<ActivityDto>>.Success(data);

        var query = _context.ActivitiesTable
              //.Where(d => d.Date >= request.Params.StartDate)
              .OrderBy(d => d.Date)
              .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
              .AsQueryable();
        //.ToListAsync(cancellationToken);

        //if (request.Params.IsGoing && !request.Params.IsHost)
        //{
        //  query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
        //}

        //if (request.Params.IsHost && !request.Params.IsGoing)
        //{
        //  query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
        //}

        return Result<PagedList<ActivityDto>>.Success(
          await PagedList<ActivityDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize));
      }
    }
  }
}
