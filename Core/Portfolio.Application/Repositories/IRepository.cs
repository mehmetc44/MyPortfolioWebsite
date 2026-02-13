using System;
using Portfolio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Application.Repositories;
public interface IRepository<T> where T : BaseEntity
{
    DbSet<T> Table { get; }
}

