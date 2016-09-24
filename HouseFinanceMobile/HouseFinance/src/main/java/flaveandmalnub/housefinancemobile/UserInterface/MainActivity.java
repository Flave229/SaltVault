package flaveandmalnub.housefinancemobile.UserInterface;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;
import android.support.design.widget.TabLayout;
import android.support.v4.view.ViewPager;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;

import flaveandmalnub.housefinancemobile.R;
import flaveandmalnub.housefinancemobile.WebService.BackgroundService;

public class MainActivity extends AppCompatActivity {

    BackgroundService _service;
    boolean _bound = false;
    SimpleFragmentPagerAdapter adapter;
    TabLayout tabLayout;

    public final static String EXTRA_MESSAGE = "com.example.myfirstapp.MESSAGE";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Intent serviceIntent = new Intent(this, BackgroundService.class);
        startService(serviceIntent);

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
    protected void onStart()
    {
        super.onStart();
        Intent intent = new Intent(this, BackgroundService.class);
        bindService(intent, _connection, Context.BIND_AUTO_CREATE);
    }

    @Override
    protected void onStop()
    {
        super.onStop();
        if(_bound)
        {
            unbindService(_connection);
            _bound = false;
        }
    }

    private ServiceConnection _connection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            BackgroundService.LocalBinder binder = (BackgroundService.LocalBinder) service;
            _service = binder.getService();
            _bound = true;
        }

        @Override
        public void onServiceDisconnected(ComponentName name) {
            _bound = false;
        }
    };

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.additemmenu, menu);
        return super.onCreateOptionsMenu(menu);
    }

}
