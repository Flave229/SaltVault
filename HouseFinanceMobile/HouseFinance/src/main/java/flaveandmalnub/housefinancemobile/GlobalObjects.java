package flaveandmalnub.housefinancemobile;

import java.util.ArrayList;

import flaveandmalnub.housefinancemobile.UserInterface.List.BillListObject;
import flaveandmalnub.housefinancemobile.UserInterface.List.BillListObjectPeople;
import flaveandmalnub.housefinancemobile.WebService.BackgroundService;

/**
 * Created by Josh on 25/09/2016.
 */

public class GlobalObjects{

    static ArrayList<BillListObject> _bills;
    static ArrayList<BillListObjectPeople> _people;

    public static BackgroundService _service;
    public static boolean _bound = false;

    public static void SetBills(ArrayList<BillListObject> bills)
    {
        _bills = bills;
    }

    public static ArrayList<BillListObject> GetBills()
    {
        return _bills;
    }

    public static BillListObject GetBillFromID(String id)
    {
        for (BillListObject bill: _bills) {
            if(id.equals(bill.ID))
            {
                return bill;
            }
        }
        return null;
    }

    public static void SetBillPeopleList(ArrayList<BillListObjectPeople> people)
    {
        _people = people;
    }

    public static BillListObjectPeople GetPersonFromID(String id)
    {
        for (BillListObjectPeople person:_people) {
            if(id.equals(person.ID))
            {
                return person;
            }
        }

        return null;
    }

    public void StartService()
    {
    }
}
