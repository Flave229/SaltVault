package flaveandmalnub.housefinancemobile.UserInterface.List;


/**
 * Created by Josh on 17/09/2016.
 */
public class BillListObject {
    public String cardName = "";
    public String cardDesc = "";
    public int cardImage = 0;

    public BillListObject(String name, String desc, int image)
    {
        // Base Initialiser
        cardName = name;
        cardDesc = desc;
        cardImage = image;
    }
}
