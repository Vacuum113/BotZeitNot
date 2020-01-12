﻿using BotZeitNot.DAL.Domain.Entity;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class SeasonRepository : Repository<Season, int>
    {
        public SeasonRepository(ApplicationDbContext context) : base(context)
        { }

    }
}