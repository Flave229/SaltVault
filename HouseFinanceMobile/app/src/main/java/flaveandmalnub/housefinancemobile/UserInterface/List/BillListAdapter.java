package flaveandmalnub.housefinancemobile.UserInterface.List;

import android.content.Intent;
import android.support.v7.widget.CardView;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;

import java.util.ArrayList;

import flaveandmalnub.housefinancemobile.R;
import flaveandmalnub.housefinancemobile.UserInterface.DisplayMessageActivity;


/**
 * Created by Josh on 17/09/2016.
 */
public class BillListAdapter extends RecyclerView.Adapter<BillListAdapter.CardViewHolder> {
    public static class CardViewHolder extends RecyclerView.ViewHolder
    {
        View view;
        CardView cardObject;
        TextView cardName;
        TextView cardDesc;
        ImageView cardImage;

        public CardViewHolder(View v)
        {
            super(v);
            view = v;
            cardObject = (CardView) v.findViewById(R.id.general_card);
            cardName = (TextView)v.findViewById(R.id.card_name);
            cardDesc = (TextView)v.findViewById(R.id.card_desc);
            cardImage = (ImageView)v.findViewById(R.id.card_image);
            view.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    view.getContext().startActivity(new Intent(view.getContext(), DisplayMessageActivity.class));
                }
            });
        }
    }

    ArrayList<BillListObject> _cards;

    public BillListAdapter(ArrayList<BillListObject> cards){
        _cards = cards;
    }

    @Override
    public int getItemCount()
    {
        return _cards.size();
    }

    @Override
    public CardViewHolder onCreateViewHolder(ViewGroup viewGroup, int i)
    {
        View v = LayoutInflater.from(viewGroup.getContext()).inflate(R.layout.general_list_item, viewGroup, false);
        CardViewHolder cvh = new CardViewHolder(v);
        return cvh;
    }

    @Override
    public void onBindViewHolder(CardViewHolder cvh, int i)
    {
        cvh.cardName.setText(_cards.get(i).cardName);
        cvh.cardDesc.setText(_cards.get(i).cardDesc);
        cvh.cardImage.setImageResource(_cards.get(i).cardImage);
    }

    @Override
    public void onAttachedToRecyclerView(RecyclerView rv)
    {
        super.onAttachedToRecyclerView(rv);
    }
}
