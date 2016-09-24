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
        TextView cardDate;
        TextView cardAmount;
        ImageView cardImage;
        ImageView cardImage2;
        ImageView cardImage3;

        public CardViewHolder(View v)
        {
            super(v);
            view = v;
            cardObject = (CardView) v.findViewById(R.id.general_card);
            cardName = (TextView)v.findViewById(R.id.item_name);
            cardDate = (TextView)v.findViewById(R.id.item_date);
            cardAmount = (TextView)v.findViewById(R.id.item_amount);
            cardImage = (ImageView)v.findViewById(R.id.card_image);
            cardImage2 = (ImageView)v.findViewById(R.id.card_image_2);
            cardImage3 = (ImageView)v.findViewById(R.id.card_image_3);
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
        cvh.cardDate.setText(_cards.get(i).cardDesc);
        cvh.cardImage.setImageResource(_cards.get(i).cardImage);
        cvh.cardImage2.setImageResource(_cards.get(i).cardImage);
        cvh.cardImage3.setImageResource(_cards.get(i).cardImage);
        cvh.cardAmount.setText(_cards.get(i).cardAmount);
    }

    @Override
    public void onAttachedToRecyclerView(RecyclerView rv)
    {
        super.onAttachedToRecyclerView(rv);
    }
}
