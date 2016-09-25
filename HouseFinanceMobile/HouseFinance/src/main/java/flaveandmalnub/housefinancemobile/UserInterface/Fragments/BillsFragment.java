package flaveandmalnub.housefinancemobile.UserInterface.Fragments;

import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import java.util.ArrayList;

import flaveandmalnub.housefinancemobile.GlobalObjects;
import flaveandmalnub.housefinancemobile.R;
import flaveandmalnub.housefinancemobile.UserInterface.List.BillListAdapter;
import flaveandmalnub.housefinancemobile.UserInterface.List.BillListObject;

/**
 * Created by Josh on 24/09/2016.
 */

public class BillsFragment extends Fragment {

    Handler _handler;
    RecyclerView rv;
    BillListAdapter adapter;
    ArrayList<BillListObject> cards;

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            cards = GlobalObjects.GetBills();

            if(GlobalObjects.GetBills() != null) {
                if (adapter.getItemCount() != GlobalObjects.GetBills().size()) {
                    adapter = new BillListAdapter(cards);
                    rv.setAdapter(adapter);
                    rv.setLayoutManager(new LinearLayoutManager(getActivity()));
                }
            }

            _handler.postDelayed(runnable, 10000);
        }
    };

    public BillsFragment()
    {
        // Blank constructor
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

        _handler = new Handler();
        rv = (RecyclerView) view.findViewById(R.id.recycler_view);
        rv.setHasFixedSize(true);
        cards = GlobalObjects.GetBills();

        if(rv != null) {
            adapter = new BillListAdapter(cards);
            rv.setAdapter(adapter);
            rv.setLayoutManager(new LinearLayoutManager(getActivity()));
        }
        _handler.postDelayed(runnable, 10000);
        return view;
    }
}
