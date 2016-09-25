using System;
using System.Linq;
using System.Web.Mvc;
using Services.FileIO;
using Services.FormHelpers;
using Services.Models.Helpers;
using Newtonsoft.Json;
using Services.Models.FinanceModels;

namespace HouseFinance.Controllers
{
    public class FinanceController : Controller
    {
        // GET: Finance/AddBill
        public ActionResult AddBill()
        {
            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            return View(new BillFormModel());
        }

        [HttpGet]
        public string GetBillData()
        {
            var bills = new GenericFileHelper(FilePath.Bills).GetAll<Bill>();

            var jsonResponse = JsonConvert.SerializeObject(bills);

            return jsonResponse;
        }

        // POST: Finance/AddBill
        [HttpPost]
        public ActionResult AddBill(BillFormModel billForm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExceptionMessage = "The Data entered was Invalid.";

                return View(billForm);
            }

            try
            {
                foreach (var person in billForm.SelectedPeople)
                {
                    if (person.Selected)
                    {
                        billForm.Bill.People.Add(person.Person.Id);
                    }
                }

                BillValidator.CheckIfValidBill(billForm.Bill);
                
                new GenericFileHelper(FilePath.Bills).AddOrUpdate<Bill>(billForm.Bill);
            }
            catch (Exception exception)
            {
                TempData["Exception"] = exception.Message;

                return RedirectToActionPermanent("AddBill");
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddPayment(Guid? billId)
        {
            if (billId == null)
            {
                return RedirectToActionPermanent("Index", "Home");
            }

            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            var payment = new PaymentFormHelper()
            {
                BillId = (Guid)billId
            };

            return View("AddPayment", payment);
        }

        [HttpPost]
        public ActionResult AddPayment(PaymentFormHelper formPayment)
        {
            var fileHelper = new GenericFileHelper(FilePath.Bills);

            if (!ModelState.IsValid)
            {
                return View("AddPayment", formPayment);
            }

            try
            {
                var realBill = fileHelper.Get<Bill>(formPayment.BillId);

                BillHelper.AddOrUpdatePayment(ref realBill, formPayment.Payment);

                fileHelper.AddOrUpdate<Bill>(realBill);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;

                return RedirectToActionPermanent("AddPayment", new { billId = formPayment.BillId });
            }

            return RedirectToActionPermanent("BillDetails", new {billId = formPayment.BillId, name = fileHelper.Get<Bill>(formPayment.BillId).Name});
        }

        // GET: Finance/EditPayment
        [HttpGet]
        public ActionResult EditPayment(Guid? billId, Guid? paymentId)
        {
            if (billId == null)
            {
                return RedirectToActionPermanent("Index", "Home");
            }

            if (paymentId == null)
            {
                return RedirectToActionPermanent("AddPayment", new { billId });
            }

            if (TempData.ContainsKey("Exception"))
            {
                ViewBag.ExceptionMessage = TempData["Exception"];
            }

            var payment = new PaymentFormHelper()
            {
                BillId = (Guid)billId,
                Payment = new GenericFileHelper(FilePath.Payments).Get<Payment>((Guid)paymentId)
            };

            return View("EditPayment", payment);
        }

        [HttpPost]
        public ActionResult EditPayment(PaymentFormHelper formPayment)
        {
            var fileHelper = new GenericFileHelper(FilePath.Bills);

            if (!ModelState.IsValid)
            {
                return View("EditPayment", formPayment);
            }

            try
            {
                var realBill = fileHelper.Get<Bill>(formPayment.BillId);

                BillHelper.AddOrUpdatePayment(ref realBill, formPayment.Payment);

                fileHelper.AddOrUpdate<Bill>(realBill);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;

                return RedirectToActionPermanent("EditPayment", new { billId = formPayment.BillId });
            }

            return RedirectToActionPermanent("BillDetails", new { billId = formPayment.BillId, name = fileHelper.Get<Bill>(formPayment.BillId).Name });
        }

        // GET: Finance/DeletePayment
        [Route("Finance/DeletePayment/{billId}")]
        public ActionResult DeleteBill(Guid billId)
        {
            try
            {
                new GenericFileHelper(FilePath.Bills).Delete<Bill>(billId);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;
            }

            return RedirectToActionPermanent("Index", "Home");
        }

        // GET: Finance/DeletePayment
        [Route("Finance/DeletePayment/{billId}/{paymentId}")]
        public ActionResult DeletePayment(Guid billId, Guid paymentId)
        {
            var billFileHelper = new GenericFileHelper(FilePath.Bills);
            var paymentFileHelper = new GenericFileHelper(FilePath.Payments);

            var bill = billFileHelper.Get<Bill>(billId);
            var payment = paymentFileHelper.Get<Payment>(paymentId);

            for (var i = 0; i < bill.AmountPaid.Count; i++)
            {
                if (paymentId == bill.AmountPaid.ElementAt(i))
                {
                    bill.AmountPaid.RemoveAt(i);
                    paymentFileHelper.Delete<Payment>(paymentId);
                    break;
                }
            }

            billFileHelper.AddOrUpdate<Bill>(bill);

            return RedirectToActionPermanent("BillDetails", new { billId = bill.Id, name = bill.Name });
        }

        [Route("Finance/BillDetails/{billId?}")]
        public ActionResult BillDetails(Guid? billId)
        {
            var fileHelper = new GenericFileHelper(FilePath.Bills);

            return View(fileHelper.Get<Bill>(billId ?? new Guid()));
        }
    }
}