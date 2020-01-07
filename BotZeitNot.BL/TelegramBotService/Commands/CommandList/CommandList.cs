﻿using BotZeitNot.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotZeitNot.BL.TelegramBotService.Commands.CommandList
{
    public class CommandList : ICommandList
    {
        public List<Command> Commands { get; }

        public CommandList(IUnitOfWorkFactory unitOfWorkFactory)
        {
            Commands = new List<Command>();

            Type[] typelist = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "BotZeitNot.BL.TelegramBotService.Commands.CommandList.List" && t.BaseType.Name == "Command")
                .ToArray();

            foreach (Type type in typelist)
            {
                if (type.IsClass)
                {
                    var com = Activator.CreateInstance(Type.GetType(type.FullName), unitOfWorkFactory);
                    Set(com as Command);
                }
            }
        }

        public void Set(Command command) => Commands.Add(command);

        public Command GetCommand(string commandMessage)
        {
            if(commandMessage.StartsWith("/search "))
            {
                return Commands.First(c => c.Name == "/search");
            }

            foreach(var item in Commands)
            {
                if(item.Contains(commandMessage))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
