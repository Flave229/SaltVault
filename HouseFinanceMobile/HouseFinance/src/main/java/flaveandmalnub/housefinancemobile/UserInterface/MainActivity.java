package flaveandmalnub.housefinancemobile.UserInterface;

import android.app.ActivityManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.Handler;
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
    private Handler _handler;

    private Runnable runnable = new Runnable() {
        @Override
        public void run() {
            if(_service != null)
            {
                _service.contactWebsite();
            }
            else
            {
                if(_handler != null) {
                    _handler.post(runnable);
                }
            }

            // Pings the website every 15 seconds. Will change to 15 mins, or maybe have it user configurable
            if(_handler != null) {
                _handler.postDelayed(runnable, 15000);
            }
        }
    };

    public final static String EXTRA_MESSAGE = "com.example.myfirstapp.MESSAGE";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Intent intent = new Intent(this, BackgroundService.class);
        bindService(intent, _connection, Context.BIND_AUTO_CREATE);

        _handler = new Handler();
        _handler.post(runnable);

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
    public void onDestroy()
    {
        super.onDestroy();
        if(_bound)
        {
            unbindService(_connection);
            _bound = false;
        }
        _handler.removeCallbacksAndMessages(runnable);
        _handler = null;
    }

    private ServiceConnection _connection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            BackgroundService.LocalBinder binder = (BackgroundService.LocalBinder) service;
            _service = binder.getService();
            _bound = true;
            //Toast.makeText(getBaseContext(), "Service Connected", Toast.LENGTH_LONG).show();
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

    private boolean isMyServiceRunning(Class<?> serviceClass)
    {
        ActivityManager mngr = (ActivityManager) getSystemService(ACTIVITY_SERVICE);
        for(ActivityManager.RunningServiceInfo service : mngr.getRunningServices(Integer.MAX_VALUE))
        {
            if(serviceClass.getName().equals(service.service.getClassName()))
            {
                return true;
            }
        }
        return false;
    }

}
