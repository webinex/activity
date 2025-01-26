﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Webinex.Activity.Server.Worker.DataAccess
{
    public interface IActivityStore
    {
        Task AddAsync(IEnumerable<ActivityStoreArgs> args);
    }
}