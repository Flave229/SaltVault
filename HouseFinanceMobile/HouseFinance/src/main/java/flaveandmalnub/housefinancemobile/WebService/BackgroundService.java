package flaveandmalnub.housefinancemobile.WebService;

import android.app.Service;
import android.content.Intent;
import android.os.Binder;
import android.os.IBinder;

import java.util.Random;

/**
 * Created by Josh on 24/09/2016.
 */

public class BackgroundService extends Service {

    private final IBinder _binder = new LocalBinder();
    private final Random _generator = new Random();

    public class LocalBinder extends Binder
    {
        public BackgroundService getService()
        {
            return BackgroundService.this;
        }
    }

    @Override
    public IBinder onBind(Intent intent)
    {
        return _binder;
    }

    public int getRandomNumber()
    {
        return _generator.nextInt(100);
    }

}
