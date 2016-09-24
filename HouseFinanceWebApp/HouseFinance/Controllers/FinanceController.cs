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
            var billFileHelper = new BillFileHelper();
            var bills = billFileHelper.GetAll();
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

                var billFileHelper = new BillFileHelper();
                billFileHelper.AddOrUpdate(billForm.Bill);
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
            var billFileHelper = new BillFileHelper();

            if (!ModelState.IsValid)
            {
                return View("AddPayment", formPayment);
            }

            try
            {
                var realBill = (Bill)billFileHelper.Get(formPayment.BillId);

                BillHelper.AddOrUpdatePayment(ref realBill, formPayment.Payment);

                billFileHelper.AddOrUpdate(realBill);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;

                return RedirectToActionPermanent("AddPayment", new { billId = formPayment.BillId });
            }

            return RedirectToActionPermanent("BillDetails", new {billId = formPayment.BillId, name = (billFileHelper.Get(formPayment.BillId) as Bill).Name});
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
                Payment = PaymentFileHelper.GetPayment((Guid)paymentId)
            };

            return View("EditPayment", payment);
        }

        [HttpPost]
        public ActionResult EditPayment(PaymentFormHelper formPayment)
        {
            var billFileHelper = new BillFileHelper();

            if (!ModelState.IsValid)
            {
                return View("EditPayment", formPayment);
            }

            try
            {
                var realBill = (Bill)billFileHelper.Get(formPayment.BillId);

                BillHelper.AddOrUpdatePayment(ref realBill, formPayment.Payment);

                billFileHelper.AddOrUpdate(realBill);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex.Message;

                return RedirectToActionPermanent("EditPayment", new { billId = formPayment.BillId });
            }

            return RedirectToActionPermanent("BillDetails", new { billId = formPayment.BillId, name = (billFileHelper.Get(formPayment.BillId) as Bill).Name });
        }

        // GET: Finance/DeletePayment
        [Route("Finance/DeletePayment/{billId}")]
        public ActionResult DeleteBill(Guid billId)
        {
            try
            {
                var billFileHelper = new BillFileHelper();

                billFileHelper.Delete(billId);
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
            var billFileHelper = new BillFileHelper();

            var bill = (Bill)billFileHelper.Get(billId);
            var payment = PaymentFileHelper.GetPayment(paymentId);

            for (var i = 0; i < bill.AmountPaid.Count; i++)
            {
                if (paymentId == bill.AmountPaid.ElementAt(i))
                {
                    bill.AmountPaid.RemoveAt(i);
                    PaymentFileHelper.Delete(paymentId);
                    break;
                }
            }

            billFileHelper.AddOrUpdate(bill);

            return RedirectToActionPermanent("BillDetails", new { billId = bill.Id, name = bill.Name });
        }

        [Route("Finance/BillDetails/{name}/{billId?}")]
        public ActionResult BillDetails(Guid? billId, string name)
        {
            var billFileHelper = new BillFileHelper();
            if (billId != new Guid() && billId != null)
                return View(billFileHelper.Get(billId ?? new Guid()));

            return View(billFileHelper.Get(name));
        }
    }
}