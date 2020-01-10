using BotZeitNot.BL.TelegramBotService.Commands;
using BotZeitNot.BL.TelegramBotService.Commands.CommandList;
using BotZeitNot.Domain.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BotZeitNot.BL.Tests
{
    [TestClass]
    public class CommadListTests
    {
        private Mock<IUnitOfWorkFactory> _mockIUnitOfWorkFactory;

        public CommadListTests()
        {
            var services = new ServiceCollection();

            _mockIUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();

            _mockIUnitOfWorkFactory.Setup(m => m.Create()).Returns(new Mock<IUnitOfWork>().Object);

        }

        [TestMethod]
        public void CommandList_Return_StartCommand()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("/start");

            Assert.AreEqual("/start", command.Name);
        }

        [TestMethod]
        public void CommandList_Return_SearchCommand()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("/search Мистер Робот");

            Assert.AreEqual("/search", command.Name);
        }

        [TestMethod]
        public void CommandList_HelpCommand()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("/help");

            Assert.AreEqual("/help", command.Name);
        }

        [TestMethod]
        public void CommandList_Wrong_CommandParam()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("/start/start");

            Assert.AreEqual(null, command);
        }

        [TestMethod]
        public void CommandList_Wrong_Params()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("sad");

            Assert.AreEqual(null, command);
        }

        [TestMethod]
        public void CommandList_Wrong_Params1()
        {
            var commandList = new CommandList(_mockIUnitOfWorkFactory.Object);

            Command command = commandList.GetCommand("/sad");

            Assert.AreEqual(null, command);
        }
    }
}
