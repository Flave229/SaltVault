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

            var bills = new List<BillOverviewV2>();
            try
            {
                var command = new NpgsqlCommand("SELECT Bill.\"Id\", Bill.\"Name\", Bill.\"Amount\", Bill.\"Due\", Person.\"Id\", Person.\"Image\", Payment.\"Id\", Payment.\"Amount\" " +
                                                "FROM public.\"Bill\" AS Bill " +
                                                "LEFT OUTER JOIN \"PeopleForBill\" AS PeopleForBill ON PeopleForBill.\"BillId\" = Bill.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS Person ON Person.\"Id\" = PeopleForBill.\"PersonId\" " +
                                                "LEFT OUTER JOIN \"Payment\" AS Payment ON Payment.\"BillId\" = Bill.\"Id\" AND Payment.\"PersonId\" = Person.\"Id\" " +
                                                "ORDER BY Bill.\"Due\" DESC", _connection);
                var reader = command.ExecuteReader();
                
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
                            Name = (string)reader[1],
                            TotalAmount = Convert.ToDecimal(reader[2]),
                            FullDateDue = (DateTime)reader[3]
                        };
                    }

                    var personId = Convert.ToInt32(reader[4]);
                    var paymentAmount = (reader[7] == DBNull.Value) ? 0 : Convert.ToDecimal(reader[7]);
                    if (billOverview.People.Any(x => x.Id == personId) == false)
                    {
                        billOverview.People.Add(new PersonBillDetailsV2
                        {
                            Id = personId,
                            Paid = paymentAmount != 0,
                            ImageLink = (string)reader[5]
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
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception("An Error occured while getting the bills", exception);
            }
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

            try
            {
                var command = new NpgsqlCommand(
                    "SELECT Bill.\"Name\", Bill.\"Amount\", Bill.\"Due\", Payment.\"Id\", Payment.\"Amount\", Payment.\"Created\", Person.\"FirstName\", Person.\"LastName\" " +
                    "FROM public.\"Bill\" AS Bill " +
                    "LEFT OUTER JOIN \"Payment\" AS Payment ON Payment.\"BillId\" = Bill.\"Id\" " +
                    "LEFT OUTER JOIN \"Person\" AS Person ON Person.\"Id\" = Payment.\"PersonId\" " +
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
                            Name = (string) reader[0],
                            TotalAmount = Convert.ToDecimal(reader[1]),
                            FullDateDue = (DateTime) reader[2]
                        };
                    }

                    if (reader[4] == DBNull.Value)
                        continue;

                    var amount = Convert.ToDecimal(reader[4]);
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
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while getting the bill (ID: {billId})", exception);
            }
        }

        public int AddBill(AddBillRequest bill)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("INSERT INTO public.\"Bill\" (\"Name\", \"Amount\", \"Due\", \"RecurringType\") " +
                                                $"VALUES ('{bill.Name}', {bill.TotalAmount}, '{bill.Due}', {(int)bill.RecurringType}) " +
                                                "RETURNING \"Id\"", _connection);
                Int64 billId = -1;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    billId = (Int64)reader[0];
                }
                reader.Close();

                foreach (var peopleId in bill.PeopleIds)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"PeopleForBill\" (\"BillId\", \"PersonId\") " +
                                                $"VALUES ({billId}, {peopleId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    { }
                    reader.Close();
                }
                _connection.Close();

                return Convert.ToInt32(billId);
            }
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while adding the bill '{bill.Name}'", exception);
            }
        }

        public bool UpdateBill(UpdateBillRequestV2 billRequest)
        {
            _connection.Open();

            try
            {
                var setValues = new List<string>();

                if (billRequest.Name != null)
                    setValues.Add($"\"Name\"='{billRequest.Name}'");
                if (billRequest.TotalAmount != null)
                    setValues.Add($"\"Amount\"={billRequest.TotalAmount}");
                if (billRequest.Due != null)
                    setValues.Add($"\"Due\"='{billRequest.Due}'");
                if (billRequest.RecurringType != null)
                    setValues.Add($"\"RecurringType\"={(int)billRequest.RecurringType}");
                
                var command = new NpgsqlCommand("UPDATE public.\"Bill\" " +
                                                $"SET {string.Join(", ", setValues)} " +
                                                $"WHERE \"Id\" = {billRequest.Id} " +
                                                "RETURNING \"Id\"", _connection);
                Int64 billId = -1;
                var rowUpdated = false;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rowUpdated = true;
                    billId = (Int64)reader[0];
                }
                reader.Close();

                if (billRequest.PeopleIds == null || billRequest.PeopleIds.Count == 0)
                    return rowUpdated;

                command = new NpgsqlCommand("DELETE FROM public.\"PeopleForBill\" " +
                                            $"WHERE \"BillId\" = {billRequest.Id}", _connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                foreach (var peopleId in billRequest.PeopleIds)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"PeopleForBill\" (\"BillId\", \"PersonId\") " +
                                                $"VALUES ({billId}, {peopleId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    { }
                    reader.Close();
                }
                _connection.Close();

                return rowUpdated;
            }
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while updating the bill '{billRequest.Name}'", exception);
            }
        }

        public bool DeleteBill(int billId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("DELETE FROM public.\"Payment\" " +
                                                $"WHERE \"BillId\" = {billId}");
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }

                command = new NpgsqlCommand("DELETE FROM public.\"PeopleForBill\" " +
                                           $"WHERE \"BillId\" = {billId}");
                reader = command.ExecuteReader();
                while (reader.Read())
                { }

                command = new NpgsqlCommand("DELETE FROM public.\"Bill\" " +
                                                $"WHERE \"Id\" = {billId} " +
                                                "RETURNING \"Id\"", _connection);

                var billDeleted = false;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    billDeleted = true;
                }
                reader.Close();

                return billDeleted;
            }
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while deleting the bill (ID: {billId})", exception);
            }
        }
    }

    public class AddBillRequest
    {
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Due { get; set; }
        public List<int> PeopleIds { get; set; }
        public RecurringType RecurringType { get; set; }

        public AddBillRequest()
        {
            PeopleIds = new List<int>();
        }
    }

    public class UpdateBillRequestV2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? Due { get; set; }
        public List<int> PeopleIds { get; set; }
        public RecurringType? RecurringType { get; set; }

        public UpdateBillRequestV2()
        {
            PeopleIds = new List<int>();
        }
    }
}
