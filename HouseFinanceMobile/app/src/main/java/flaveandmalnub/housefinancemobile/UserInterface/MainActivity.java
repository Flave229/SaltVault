package flaveandmalnub.housefinancemobile.UserInterface;

import android.os.Bundle;
import android.support.design.widget.TabLayout;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;

import flaveandmalnub.housefinancemobile.R;

public class MainActivity extends AppCompatActivity {

    SimpleFragmentPagerAdapter adapter;
    TabLayout tabLayout;

    public final static String EXTRA_MESSAGE = "com.example.myfirstapp.MESSAGE";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar appToolbar = (Toolbar) findViewById(R.id.appToolbar);
        setSupportActionBar(appToolbar);

        ViewPager viewPager = (ViewPager)findViewById(R.id.viewpager);
        adapter = new SimpleFragmentPagerAdapter(getSupportFragmentManager(), this);
        viewPager.setAdapter(adapter);

        tabLayout = (TabLayout) findViewById(R.id.tabs);
        tabLayout.setupWithViewPager(viewPager);

        for(int i = 0; i < tabLayout.getTabCount(); i++)
        {
            TabLayout.Tab tab = tabLayout.getTabAt(i);
            tab.setCustomView(adapter.getTabView(i));
        }

    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.additemmenu, menu);
        return super.onCreateOptionsMenu(menu);
    }

}
