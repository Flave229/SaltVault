using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core.Exception;
using SaltVault.Core.ToDo;
using SaltVault.Core.ToDo.Models;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Bills;
using SaltVault.WebApp.Models.ToDo;

namespace SaltVault.WebApp.Controllers
{
    public class ToDoController : Controller
    {
        private readonly IUserService _userService;
        private readonly IToDoRepository _toDoRepository;

        public ToDoController(IToDoRepository toDoRepository, IUserService userService)
        {
            _userService = userService;
            _toDoRepository = toDoRepository;
        }

        [HttpGet]
        [Route("Api/v2/ToDo")]
        public GetToDoResponse GetToDoList()
        {
            var response = new GetToDoResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to get To Do Tasks");
                    return response;
                }
                response.ToDoTasks = _toDoRepository.GetToDoList(user.HouseId);
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/ToDo")]
        public AddToDoResponse AddToDoItem([FromBody]AddToDoTaskRequest toDoTaskRequest)
        {
            var response = new AddToDoResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ActiveUser user = _userService.GetUserInformationFromAuthHeader(Request.Headers["Authorization"].ToString());
                if (user.HouseId == 0)
                {
                    response.Notifications.Add("You must belong to a household to add To Do Tasks");
                    return response;
                }
                response.Id = _toDoRepository.AddToDoTask(toDoTaskRequest, user.HouseId);

                response.Notifications = new List<string>
                {
                    $"The task '{toDoTaskRequest.Title}' has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoTaskRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoTaskRequest);
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/v2/ToDo")]
        public CommunicationResponse UpdateToDoItem([FromBody]UpdateToDoRequest toDoRequest)
        {
            var response = new AddBillResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                _toDoRepository.UpdateToDoTask(toDoRequest);

                response.Notifications = new List<string>
                {
                    $"The Task '{toDoRequest.Title}' has been updated"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", toDoRequest);
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/ToDo")]
        public CommunicationResponse DeleteToDoItem([FromBody]DeleteToDoRequest deleteToDoRequest)
        {
            var response = new CommunicationResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                var rowUpdated = _toDoRepository.DeleteToDoTask(deleteToDoRequest.Id);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the To Do Task (ID: {deleteToDoRequest.Id}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The To Do Task (ID: {deleteToDoRequest.Id}) has been deleted"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteToDoRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteToDoRequest);
            }

            return response;
        }
    }
}