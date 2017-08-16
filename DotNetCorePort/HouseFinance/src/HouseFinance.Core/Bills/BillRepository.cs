using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public List<BillOverviewV2> GetAllBasicBillDetails()
        {
            _connection.Open();

            var command = new NpgsqlCommand("SELECT Bill.\"Id\", Bill.\"Name\", Bill.\"Amount\", Bill.\"Due\", Person.\"Id\", Person.\"Image\", Payment.\"Id\", Payment.\"Amount\" " +
                                            "FROM public.\"Person\" AS Person " +
                                            "LEFT OUTER JOIN \"PeopleForBill\" AS PeopleForBill ON PeopleForBill.\"PersonId\" = Person.\"Id\" " +
                                            "LEFT OUTER JOIN \"Bill\" AS Bill ON Bill.\"Id\" = PeopleForBill.\"BillId\" " +
                                            "LEFT OUTER JOIN \"Payment\" AS Payment ON Payment.\"BillId\" = Bill.\"Id\" AND Payment.\"PersonId\" = Person.\"Id\"", _connection);
            var reader = command.ExecuteReader();

            var bills = new List<BillOverviewV2>();

            while (reader.Read())
            {
                var billId = Convert.ToInt32(reader[0]);
                BillOverviewV2 billOverview;

                if (bills.Any(x => x.Id == billId))
                    billOverview = bills.First(x => x.Id == billId);
                else
                {
                    billOverview = new BillOverviewV2
                    {
                        Id = billId,
                        Name = (string) reader[1],
                        TotalAmount = (double) reader[2],
                        FullDateDue = (DateTime) reader[3]
                    };
                }

                var personId = Convert.ToInt32(reader[4]);
                var paymentAmount = (reader[7] == DBNull.Value) ? 0 : (double)reader[7];
                if (billOverview.People.Any(x => x.Id == personId) == false)
                {
                    billOverview.People.Add(new PersonBillDetailsV2
                    {
                        Id = personId,
                        Paid = paymentAmount != 0,
                        ImageLink = (string) reader[5]
                    });
                }
                else
                    billOverview.People.First(x => x.Id == personId).Paid = true;

                billOverview.AmountPaid += paymentAmount;

                if (bills.Any(x => x.Id == billId) == false)
                    bills.Add(billOverview);
            }
            reader.Close();

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
                Int64 rowId = -1;
                while (reader.Read())
                {
                    rowId = (Int64)reader[0];
                }
                reader.Close();

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
                                                           $"VALUES ({rowId}, {personId}, {payment.Amount}, '{payment.Created}')", _connection);
                    reader = paymentCommand.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }

                foreach (var person in bill.People)
                {

                    var personId = -1;

                    if (person == new Guid("e9636bbb-8b54-49b9-9fa2-9477c303032f"))
                        personId = 1;
                    else if (person == new Guid("25c15fb4-b5d5-47d9-917b-c572b1119e65"))
                        personId = 2;
                    else if (person == new Guid("f97a50c9-8451-4537-bccb-e89ba5ade95a"))
                        personId = 3;
                    
                    var paymentCommand = new NpgsqlCommand("INSERT INTO public.\"PeopleForBill\" (\"BillId\", \"PersonId\") " +
                                                           $"VALUES ({rowId}, {personId})", _connection);
                    reader = paymentCommand.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }
            }

            _connection.Close();
        }

        public BillDetailsResponseV2 GetBasicBillDetails(int billId)
        {
            _connection.Open();

            var command = new NpgsqlCommand("SELECT Bill.\"Name\", Bill.\"Amount\", Bill.\"Due\", Payment.\"Id\", Payment.\"Amount\", Payment.\"Created\", Person.\"FirstName\", Person.\"LastName\" " +
                                            "FROM public.\"Bill\" AS Bill " +
                                            "LEFT OUTER JOIN \"Payment\" AS Payment ON Payment.\"BillId\" = Bill.\"Id\" " +
                                            "INNER JOIN \"Person\" AS Person ON Person.\"Id\" = Payment.\"PersonId\" " +
                                            $"WHERE Bill.\"Id\" = {billId}", _connection);
            var reader = command.ExecuteReader();

            BillDetailsResponseV2 bill = null;

            while (reader.Read())
            {
                if (bill == null)
                {
                    bill = new BillDetailsResponseV2
                    {
                        Id = billId,
                        Name = (string)reader[0],
                        TotalAmount = (double)reader[1],
                        FullDateDue = (DateTime)reader[2]
                    };
                }

                var amount = (double) reader[4];
                bill.Payments.Add(new BillPaymentsV2
                {
                    Id = Convert.ToInt32(reader[3]),
                    Amount = amount,
                    DatePaid = (DateTime) reader[5],
                    PersonName = (string) reader[6] + " " + (string) reader[7]
                });

                bill.AmountPaid += amount;
            }
            reader.Close();

            _connection.Close();
            return bill;
        }
    }
}
