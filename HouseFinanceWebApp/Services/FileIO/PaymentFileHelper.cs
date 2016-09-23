using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models.FinanceModels;

namespace Services.FileIO
{
    public static class PaymentFileHelper
    {
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Payments\payments.txt";

        private static List<Payment> Open()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) return new List<Payment>();

                var existingPaymentsAsJson = System.IO.File.ReadAllLines(FilePath);
                var existingPaymentAsString = "";

                for (var i = 0; i < existingPaymentsAsJson.Length; i++)
                {
                    existingPaymentAsString = existingPaymentAsString + existingPaymentsAsJson.ElementAt(i);
                }

                return existingPaymentAsString.Equals("") ? new List<Payment>() : JsonConvert.DeserializeObject<List<Payment>>(existingPaymentAsString);
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve Payment data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        private static void Save(List<Payment> payments)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(payments);

                var directoryInfo = new System.IO.FileInfo(FilePath);
                directoryInfo.Directory?.Create();

                System.IO.File.WriteAllText(directoryInfo.FullName, jsonResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to save Payment data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public static List<Payment> Add(List<Payment> payments, Payment paymentToAdd)
        {
            payments.Add(paymentToAdd);

            return payments;
        }

        public static List<Payment> Update(List<Payment> payments, Payment updatedPayment)
        {
            var index = payments.FindIndex(payment => payment.Id.Equals(updatedPayment.Id));
            payments[index] = updatedPayment;

            return payments;
        }

        public static void Delete(Guid paymentId)
        {
            try
            {
                var paymentList = GetPayments();

                for (var i = 0; i < paymentList.Count; i++)
                {
                    if (paymentList[i].Id != paymentId) continue;
                    
                    paymentList.RemoveAt(i);
                    break;
                }

                Save(paymentList);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to delete the Payment.\n Exception: " + exception.Message, exception);
            }
        }

        public static void AddOrUpdate(Payment payment)
        {
            var payments = Open();

            payments = payments.Any(existingPayment => existingPayment.Id.Equals(payment.Id)) ? Update(payments, payment) : Add(payments, payment);

            Save(payments);
        }

        public static void AddOrUpdate(List<Payment> payment)
        {
            for (var i = 0; i < payment.Count; i++)
            {
                AddOrUpdate(payment.ElementAt(i));
            }
        }

        public static Payment GetPayment(Guid paymentId)
        {
            var payments = Open();

            return payments.FirstOrDefault(payment => payment.Id.Equals(paymentId));
        }

        public static List<Payment> GetPayments()
        {
            return Open();
        }
    }
}