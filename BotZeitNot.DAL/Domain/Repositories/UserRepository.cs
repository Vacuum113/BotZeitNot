﻿using BotZeitNot.DAL.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using BotZeitNot.DAL.Domain.SpecificStorage;

namespace BotZeitNot.DAL.Domain.Repositories
{
    public class UserRepository : Repository<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        { }

        public bool ContainsUserByTelegramId(int id)
        {
            User user = Table.FirstOrDefault(u => u.TelegramId == id);
            return user != default;
        }
    }
}