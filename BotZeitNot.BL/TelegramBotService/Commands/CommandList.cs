using BotZeitNot.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BotZeitNot.BL.TelegramBotService.Commands
{
    public class CommandList : ICommandList
    {
        public List<Command> Commands { get; }

        public CommandList(IUnitOfWorkFactory unitOfWorkFactory)
        {
            Commands = new List<Command>();

            var typelist = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.BaseType != null && (t.Namespace == "BotZeitNot.BL.TelegramBotService.Commands.List" && t.BaseType.Name == "Command"))
                .ToArray();

            foreach (var type in typelist)
            {
                if (!type.IsClass || type.Name == "HelpCommand") 
                    continue;
                var com = Activator.CreateInstance(Type.GetType(type.FullName), unitOfWorkFactory);
                Set(com as Command);
            }

            var typeHelp = typelist.First(t => t.IsClass && t.Name == "HelpCommand");
            var comHelp = Activator.CreateInstance(Type.GetType(typeHelp.FullName), Commands);
            Set(comHelp as Command);

        }

        public void Set(Command command) => Commands.Add(command);

        public Command GetCommand(string commandMessage)
        {
            if (commandMessage.StartsWith("/search "))
            {
                return Commands.First(c => c.Name == "/search");
            }

            return Commands.FirstOrDefault(item => item.Contains(commandMessage));
        }
    }
}
