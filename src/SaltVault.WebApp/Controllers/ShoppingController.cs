using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SaltVault.Core;
using SaltVault.Core.Exception;
using SaltVault.Core.Shopping;
using SaltVault.Core.Users;
using SaltVault.WebApp.Models;
using SaltVault.WebApp.Models.Shopping;
using UpdateShoppingItemRequest = SaltVault.Core.Shopping.Models.UpdateShoppingItemRequest;

namespace SaltVault.WebApp.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly IUserService _userService;
        private readonly IShoppingRepository _shoppingRepository;

        public ShoppingController(IShoppingRepository shoppingRepository, IUserService userService)
        {
            _userService = userService;
            _shoppingRepository = shoppingRepository;
        }

        [HttpGet]
        [Route("Api/v2/Shopping")]
        public GetShoppingResponse GetShoppingItemsV2(int? page, int? resultsPerPage)
        {
            var response = new GetShoppingResponse();

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
                    response.Notifications.Add("You must belong to a household to add shopping items");
                    return response;
                }
                Pagination pagination = new Pagination
                {
                    Page = page ?? 0,
                    ResultsPerPage = resultsPerPage ?? int.MaxValue
                };
                response.ShoppingList = _shoppingRepository.GetAllItems(pagination, user.HouseId);
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
        [Route("Api/v2/Shopping")]
        public CommunicationResponse AddShoppingItemV2([FromBody]AddShoppingItemRequest shoppingRequest)
        {
            var response = new CommunicationResponse();

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
                    response.Notifications.Add("You must belong to a household to add Shopping Items");
                    return response;
                }
                ShoppingItem item = new ShoppingItem
                {
                    ItemFor = shoppingRequest.ItemFor,
                    Added = DateTime.Now,
                    AddedBy = user.PersonId,
                    Name = shoppingRequest.Name,
                    Purchased = false
                };
                ShoppingValidator.CheckIfValidItem(item);
                _shoppingRepository.AddItem(item, user.HouseId);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been added"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest);
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/v2/Shopping")]
        public CommunicationResponse UpdateShoppingItemV2([FromBody]UpdateShoppingItemRequest shoppingRequest)
        {
            var response = new CommunicationResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                ShoppingValidator.CheckIfValidItem(shoppingRequest);
                _shoppingRepository.UpdateItem(shoppingRequest);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been updated"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", shoppingRequest);
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Shopping")]
        public CommunicationResponse DeleteShoppingItemV2([FromBody]DeleteShoppingItemRequest deleteShoppingItemRequest)
        {
            var response = new CommunicationResponse();

            try
            {
                if (_userService.AuthenticateSession(Request.Headers["Authorization"].ToString()) == false)
                {
                    response.AddError("The authorization credentails were invalid", ErrorCode.SESSION_INVALID);
                    return response;
                }

                _shoppingRepository.DeleteItem(deleteShoppingItemRequest.ShoppingItemId);

                response.Notifications = new List<string>
                {
                    "The shopping item has been deleted"
                };
            }
            catch (ErrorCodeException exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteShoppingItemRequest, exception.Code);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}", deleteShoppingItemRequest);
            }

            return response;
        }
    }
}