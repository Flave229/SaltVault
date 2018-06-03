﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.ToDo;

namespace SaltVault.IntegrationTests.ToDo.GivenARequestToGetToDoItems
{
    [TestClass]
    public class WhenTheUserHasNoToDoTasks
    {
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private Guid _validSessionId;
        private GetToDoResponse _getToDoTaskResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString());

            string responseContent = _endpointHelper.GetToDoItems();
            _getToDoTaskResponse = JsonConvert.DeserializeObject<GetToDoResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheBillListIsEmpty()
        {
            Assert.AreEqual(0, _getToDoTaskResponse.ToDoTasks.Count);
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
            _fakeTestingAccountHelper.CleanUp(_validSessionId);
        }
    }
}