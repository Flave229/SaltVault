package flaveandmalnub.housefinancemobile;

import java.util.ArrayList;

import flaveandmalnub.housefinancemobile.UserInterface.List.BillListObject;

/**
 * Created by Josh on 25/09/2016.
 */

public class GlobalObjects{

    static ArrayList<BillListObject> _bills;

    public static void SetBills(ArrayList<BillListObject> bills)
    {
        _bills = bills;
    }

    public static ArrayList<BillListObject> GetBills()
    {
        return _bills;
    }
}
