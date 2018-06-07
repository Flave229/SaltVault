using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.ToDo;

namespace SaltVault.IntegrationTests.ToDo.GivenARequestToGetToDoItems
{
    [TestClass]
    public class WhenTheUserHasToDoTasks
    {
        private FakeAccountHelper _fakeAccountHelper;
        private Guid _validSessionId;
        private GetToDoResponse _getToDoTaskResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            Guid validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(validSessionId.ToString())
                .AddToDoTask(typeof(WhenTheUserHasToDoTasks).Name);
            
            string responseContent = _endpointHelper.GetToDoItems();
            _getToDoTaskResponse = JsonConvert.DeserializeObject<GetToDoResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheToDoTaskContainsItems()
        {
            Assert.AreEqual(1, _getToDoTaskResponse.ToDoTasks.Count);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Assert.IsFalse(_getToDoTaskResponse.HasError);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _fakeAccountHelper.CleanUp();
        }
    }
}