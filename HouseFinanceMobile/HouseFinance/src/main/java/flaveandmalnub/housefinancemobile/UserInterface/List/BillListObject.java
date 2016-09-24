package flaveandmalnub.housefinancemobile.UserInterface.List;


/**
 * Created by Josh on 17/09/2016.
 */
public class BillListObject {
    public String cardName = "";
    public String cardDesc = "";
    public String cardAmount = "";
    public int cardImage = 0;

    public BillListObject(String name, String desc, String amount, int image)
    {
        // Base Initialiser
        cardName = name;
        cardDesc = desc;
        cardImage = image;
        cardAmount = amount;
    }
}
