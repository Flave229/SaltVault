package flaveandmalnub.housefinancemobile.UserInterface.Fragments;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;

import flaveandmalnub.housefinancemobile.R;
import flaveandmalnub.housefinancemobile.UserInterface.List.BillListAdapter;
import flaveandmalnub.housefinancemobile.UserInterface.List.BillListObject;

/**
 * Created by Josh on 24/09/2016.
 */

public class StatisticsFragment extends Fragment {

    public StatisticsFragment()
    {

    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        View view = inflater.inflate(R.layout.fragment_blank, container, false);

        RecyclerView rv = (RecyclerView) view.findViewById(R.id.recycler_view);
        rv.setHasFixedSize(true);
        ArrayList<BillListObject> cards = new ArrayList<>();

        for(int i = 0; i < 100; i++)
        {
            cards.add(new BillListObject("Card " + i, "This is card " + i, android.R.drawable.ic_menu_camera));
        }

        if(rv != null) {
            BillListAdapter adapter = new BillListAdapter(cards);
            rv.setAdapter(adapter);

            LinearLayoutManager llm = new LinearLayoutManager(getActivity());
            rv.setLayoutManager(llm);
        }
        return view;
    }
}
