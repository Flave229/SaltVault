using System;
using System.Collections.Generic;
using System.IO;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.FileManagement;
using Npgsql;

namespace HouseFinance.Core.Bills
{
    public class BillRepository
    {
        private readonly NpgsqlConnection _connection;

        public BillRepository()
        {
            var connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
            _connection = new NpgsqlConnection(connectionString);
        }

        public List<BillV2> GetAllBasicBillDetails()
        {
            _connection.Open();

            var command = new NpgsqlCommand("SELECT * FROM public.\"Bill\"", _connection);
            var reader = command.ExecuteReader();

            var bills = new List<BillV2>();

            while (reader.Read())
            {
                bills.Add(new BillV2
                {
                    Id = (int)reader[0],
                    Due= (DateTime)reader[1],
                    Amount = (decimal)reader[2],
                    RecurringType = (RecurringType)reader[3],
                    Name = (string)reader[4]
                });
            }

            _connection.Close();
            return bills;
        }

        public void EnterAllIntoDatabase(List<Bill> bills)
        {
            var paymentFileHelper = new GenericFileHelper(FilePath.Payments);
            _connection.Open();

            foreach (var bill in bills)
            {
                var command = new NpgsqlCommand("INSERT INTO public.\"Bill\" (\"Name\", \"Amount\", \"Due\", \"RecurringType\") " +
                                                $"VALUES ('{bill.Name}', {bill.AmountOwed}, '{bill.Due}', {(int)bill.RecurringType}) " +
                                                "RETURNING \"Id\"", _connection);
                var reader = command.ExecuteReader();
                var rowId = -1;
                while (reader.Read())
                {
                    rowId = (int)reader[0];
                }

                foreach (var paymentId in bill.AmountPaid)
                {
                    var payment = paymentFileHelper.Get<Payment>(paymentId);

                    var personId = -1;

                    if (payment.PersonId == new Guid("e9636bbb-8b54-49b9-9fa2-9477c303032f"))
                        personId = 1;
                    else if (payment.PersonId == new Guid("25c15fb4-b5d5-47d9-917b-c572b1119e65"))
                        personId = 2;
                    else if (payment.PersonId == new Guid("f97a50c9-8451-4537-bccb-e89ba5ade95a"))
                        personId = 3;

                    var paymentCommand = new NpgsqlCommand("INSERT INTO public.\"Payment\" (\"BillId\", \"PersonId\", \"Amount\", \"Created\") " +
                                                           $"VALUES ({rowId}, {personId}, {payment.Amount}, '{payment.Created}'", _connection);
                }
            }

            _connection.Close();
        }
    }
}
