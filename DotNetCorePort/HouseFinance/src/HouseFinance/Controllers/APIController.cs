using System;
using System.Collections.Generic;
using System.Net.Http;
using HouseFinance.Core.Authentication;
using HouseFinance.Core.Bills;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.FileManagement;
using HouseFinance.Core.Services.Discord;
using HouseFinance.Core.Shopping;
using HouseFinance.Models.API;
using HouseFinance.Models.Bills;
using HouseFinance.Models.Shopping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace HouseFinance.Controllers
{
    public class ApiController : Controller
    {
        private readonly DiscordService _discordService;
        private readonly BillRepository _billRepository;

        public ApiController()
        {
            _discordService = new DiscordService(new HttpClient());
            _billRepository = new BillRepository();
        }

        [HttpGet]
        [Route("Api/Bills")]
        public GetBillListResponse GetBillList()
        {
            var response = new GetBillListResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bills = BillListBuilder.BuildBillList();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpGet]
        [Route("Api/v2/Bills")]
        public GetBillListResponseV2 GetBillListV2()
        {
            var response = new GetBillListResponseV2();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bills = _billRepository.GetAllBasicBillDetails();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Bills")]
        public GetBillResponse GetBill([FromBody]GetBillRequest billRequest)
        {
            var response = new GetBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bill = BillDetailsBuilder.BuildBillDetails(Guid.Parse(billRequest.BillId));
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills")]
        public GetBillResponseV2 GetBillV2([FromBody]GetBillRequestV2 billRequest)
        {
            var response = new GetBillResponseV2();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Bill = _billRepository.GetBasicBillDetails(billRequest.BillId);
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Bills/Add")]
        public CommunicationResponse AddBill([FromBody]Bill billRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                BillValidator.CheckIfValidBill(billRequest);
                var genericFileHelper = new GenericFileHelper(FilePath.Bills);
                genericFileHelper.AddOrUpdate<Bill>(billRequest);

                _discordService.AddBillNotification(billRequest.Name, billRequest.Due, billRequest.AmountOwed);

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills/Add")]
        public AddBillResponse AddBillV2([FromBody]AddBillRequest billRequest)
        {
            var response = new AddBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                BillValidator.CheckIfValidBill(billRequest);
                response.Id = _billRepository.AddBill(billRequest);

                _discordService.AddBillNotification(billRequest.Name, billRequest.Due, billRequest.TotalAmount);
                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/Bills/Update")]
        public CommunicationResponse UpdateBill([FromBody]UpdateBillRequest billRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var genericFileHelper = new GenericFileHelper(FilePath.Bills);

                var oldBill = genericFileHelper.Get<Bill>(billRequest.Id);
                if (oldBill == null)
                {
                    response.AddError("The requested bill does not exist");
                    return response;
                }

                var newBill = new Bill
                {
                    Id = billRequest.Id,
                    AmountOwed = billRequest.AmountOwed ?? oldBill.AmountOwed,
                    Due = billRequest.Due ?? oldBill.Due,
                    People = billRequest.People ?? oldBill.People,
                    Name = billRequest.Name ?? oldBill.Name,
                    RecurringType = billRequest.RecurringType ?? oldBill.RecurringType,
                    AmountPaid = oldBill.AmountPaid
                };

                BillValidator.CheckIfValidBill(newBill);
                genericFileHelper.AddOrUpdate<Bill>(newBill);

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/v2/Bills/Update")]
        public CommunicationResponse UpdateBillV2([FromBody]UpdateBillRequestV2 billRequest)
        {
            var response = new AddBillResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _billRepository.UpdateBill(billRequest);

                if (rowUpdated == false)
                {
                    response.AddError("The given Bill Id does not correspond to an existing Bill");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The bill '{billRequest.Name}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/Bills/Delete")]
        public CommunicationResponse DeleteBill([FromBody]DeleteBillRequest deleteBillRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var genericFileHelper = new GenericFileHelper(FilePath.Bills);
                var bill = genericFileHelper.Get<Bill>(deleteBillRequest.BillId);

                if (bill == null)
                {
                    response.AddError($"Cannot delete the bill (ID: {deleteBillRequest.BillId}) because it does not exist");
                    return response;
                }

                genericFileHelper.Delete<Bill>(deleteBillRequest.BillId);

                response.Notifications = new List<string>
                {
                    $"The bill '{bill.Name}' has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/v2/Bills/Delete")]
        public CommunicationResponse DeleteBillV2([FromBody]DeleteBillRequestV2 deleteBillRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var rowUpdated = _billRepository.DeleteBill(deleteBillRequest.BillId);

                if (rowUpdated == false)
                {
                    response.AddError($"Cannot delete the bill (ID: {deleteBillRequest.BillId}) because it does not exist");
                    return response;
                }

                response.Notifications = new List<string>
                {
                    $"The bill (ID: {deleteBillRequest.BillId}) has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Bills/Payments")]
        public CommunicationResponse AddPayment([FromBody]AddPaymentRequest paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Amount = paymentRequest.Amount,
                    Created = paymentRequest.Created,
                    PersonId = Guid.Parse(paymentRequest.PersonId)
                };

                PaymentValidator.CheckIfValidPayment(payment);
                var paymentFileHelper = new GenericFileHelper(FilePath.Payments);
                paymentFileHelper.AddOrUpdate<Payment>(payment);

                var billFileHelper = new GenericFileHelper(FilePath.Bills);
                var realBill = billFileHelper.Get<Bill>(Guid.Parse(paymentRequest.BillId));

                BillHelper.AddOrUpdatePayment(ref realBill, payment);

                billFileHelper.AddOrUpdate<Bill>(realBill);

                response.Notifications = new List<string>
                {
                    "The payment has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/v2/Bills/Payments")]
        public CommunicationResponse AddPaymentV2([FromBody]AddPaymentRequestV2 paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                PaymentValidator.CheckIfValidPayment(paymentRequest);
                _billRepository.AddPayment(paymentRequest);

                response.Notifications = new List<string>
                {
                    "The payment has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/Bills/Payments")]
        public CommunicationResponse UpdatePayment([FromBody]UpdatePaymentRequest paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var paymentFileHelper = new GenericFileHelper(FilePath.Payments);
                var existingPayment = paymentFileHelper.Get<Payment>(paymentRequest.PaymentId);

                if (existingPayment == null)
                {
                    response.AddError("The requested payment does not exist");
                    return response;
                }

                var newPayment = new Payment
                {
                    Id = paymentRequest.PaymentId,
                    Amount = paymentRequest.Amount,
                    Created = existingPayment.Created,
                    PersonId = existingPayment.PersonId
                };
                PaymentValidator.CheckIfValidPayment(newPayment);
                paymentFileHelper.AddOrUpdate<Payment>(newPayment);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{newPayment.Id}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpDelete]
        [Route("Api/Bills/Payments")]
        public CommunicationResponse DeletePayment([FromBody]DeletePaymentRequest paymentRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var billFileHelper = new GenericFileHelper(FilePath.Bills);
                var bill = billFileHelper.Get<Bill>(paymentRequest.BillId);
                var paymentFileHelper = new GenericFileHelper(FilePath.Payments);
                var payment = paymentFileHelper.Get<Payment>(paymentRequest.PaymentId);

                if (bill == null)
                {
                    response.AddError($"Cannot delete the payment (ID: {paymentRequest.PaymentId}) because the bill (ID {paymentRequest.BillId}) does not exist");
                    return response;
                }
                if (payment == null)
                {
                    response.AddError($"Cannot delete the payment (ID: {paymentRequest.PaymentId}) because it does not exist");
                    return response;
                }

                paymentFileHelper.Delete<Payment>(paymentRequest.PaymentId);
                bill.AmountPaid.Remove(payment.Id);
                billFileHelper.AddOrUpdate<Bill>(bill);
                response.Notifications = new List<string>
                {
                    $"The shopping item '{payment.Id}' has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpGet]
        [Route("Api/Shopping")]
        public GetShoppingResponse GetShoppingItems()
        {
            var response = new GetShoppingResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                response.Items = ShoppingListBuilder.BuildShoppingList();
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPost]
        [Route("Api/Shopping")]
        public CommunicationResponse AddShoppingItem([FromBody]ShoppingItem shoppingRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                ShoppingValidator.CheckIfValidItem(shoppingRequest);
                var shoppingFileHelper = new GenericFileHelper(FilePath.Shopping);

                if (shoppingFileHelper.Get<ShoppingItem>(shoppingRequest.Id) != null)
                {
                    response.AddError("Cannot update a shopping item via POST. Please use a PATCH request");
                    return response;
                }

                shoppingFileHelper.AddOrUpdate<ShoppingItem>(shoppingRequest);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been added"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [HttpPatch]
        [Route("Api/Shopping")]
        public CommunicationResponse UpdateShoppingItem([FromBody]UpdateShoppingItemRequest shoppingRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var shoppingFileHelper = new GenericFileHelper(FilePath.Shopping);
                var existingShoppingItem = shoppingFileHelper.Get<ShoppingItem>(shoppingRequest.Id);

                if (existingShoppingItem == null)
                {
                    response.AddError("The requested shopping item does not exist");
                    return response;
                }

                var newShoppingItem = new ShoppingItem
                {
                    Id = shoppingRequest.Id,
                    Name = shoppingRequest.Name ?? existingShoppingItem.Name,
                    Added = existingShoppingItem.Added,
                    AddedBy = existingShoppingItem.AddedBy,
                    ItemFor = shoppingRequest.ItemFor ?? existingShoppingItem.ItemFor,
                    Purchased = shoppingRequest.Purchased ?? existingShoppingItem.Purchased
                };
                ShoppingValidator.CheckIfValidItem(newShoppingItem);
                shoppingFileHelper.AddOrUpdate<ShoppingItem>(newShoppingItem);

                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingRequest.Name}' has been updated"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }
        
        [HttpDelete]
        [Route("Api/Shopping")]
        public CommunicationResponse DeleteShoppingItem([FromBody]DeleteShoppingItemRequest deleteShoppingItemRequest)
        {
            var response = new CommunicationResponse();
            if (Authenticate(Request.Headers["Authorization"]) == false)
            {
                response.AddError("The API Key was invalid");
                return response;
            }

            try
            {
                var genericFileHelper = new GenericFileHelper(FilePath.Shopping);
                var shoppingItem = genericFileHelper.Get<ShoppingItem>(deleteShoppingItemRequest.ShoppingItemId);

                if (shoppingItem == null)
                {
                    response.AddError($"Cannot delete the shopping item (ID: {deleteShoppingItemRequest.ShoppingItemId}) because it does not exist");
                    return response;
                }

                genericFileHelper.Delete<ShoppingItem>(deleteShoppingItemRequest.ShoppingItemId);
                response.Notifications = new List<string>
                {
                    $"The shopping item '{shoppingItem.Name}' has been deleted"
                };
            }
            catch (Exception exception)
            {
                response.AddError($"An unexpected exception occured: {exception}");
            }

            return response;
        }

        [Route("Experimental/DatabaseMigration")]
        public void DatabaseMigrationForBills()
        {
            var bills = new GenericFileHelper(FilePath.Bills).GetAll<Bill>();
            var repository = new BillRepository();
            repository.EnterAllIntoDatabase(bills);
        }

        private bool Authenticate(StringValues authorizationHeader)
        {
            var apiKey = authorizationHeader.ToString().Replace("Token ", "");
            return Authentication.CheckKey(apiKey);
        }
    }

    public class UpdatePaymentRequest
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
    }
}