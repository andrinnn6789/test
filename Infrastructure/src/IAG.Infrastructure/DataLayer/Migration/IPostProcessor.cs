using System;

using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IAG.Infrastructure.DataLayer.Migration;

public interface IPostProcessor : IProcessor
{
    void Process(DatabaseFacade db, IServiceProvider serviceProvider);
}