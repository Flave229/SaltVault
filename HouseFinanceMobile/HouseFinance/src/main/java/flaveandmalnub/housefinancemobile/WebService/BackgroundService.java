package flaveandmalnub.housefinancemobile.WebService;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.AsyncTask;
import android.os.Binder;
import android.os.IBinder;
import android.widget.Toast;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import java.io.UnsupportedEncodingException;
import java.net.HttpURLConnection;
import java.net.URL;

/**
 * Created by Josh on 24/09/2016.
 */

public class BackgroundService extends Service {

    private final IBinder _binder = new LocalBinder();

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


    public void contactWebsite()
    {
        ConnectivityManager connMgr = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo networkInfo = connMgr.getActiveNetworkInfo();

        if(networkInfo != null && networkInfo.isConnected())
        {
            //Toast.makeText(getBaseContext(), "Obtaining list of bills", Toast.LENGTH_LONG).show();
            new DownloadJsonString().execute("http://saltavenue.azurewebsites.net/Api/RequestBillList");
        }
        else
        {
            Toast.makeText(getBaseContext(), "No internet connection", Toast.LENGTH_SHORT).show();
        }
    }

    public void websiteResult(String result)
    {
        Toast.makeText(getBaseContext(), result, Toast.LENGTH_SHORT).show();
    }

    private class DownloadJsonString extends AsyncTask<String, Void, String>
    {
        @Override
        protected String doInBackground(String... urls)
        {
            try
            {
                return downloadUrl(urls[0]);
            } catch(IOException e)
            {
                return "unable to contact url";
            }
        }

        @Override
        protected void onPostExecute(String result)
        {
            websiteResult(result);
        }

        private String downloadUrl(String weburl) throws IOException
        {
            InputStream is = null;
            int len = 10000;

            // Might need this at some point
            String authToken = "D2DB7539-634F-47C4-818D-59AD03C592E3";

            try{
                URL url = new URL(weburl + "/" + authToken);
                HttpURLConnection conn = (HttpURLConnection) url.openConnection();
                conn.setReadTimeout(10000);
                conn.setConnectTimeout(15000);
                conn.setRequestMethod("GET");
                conn.setDoInput(true);

                conn.connect();
                int response = conn.getResponseCode();
                //Toast.makeText(getBaseContext(), "The response is: " + String.valueOf(response), Toast.LENGTH_LONG).show();
                is = conn.getInputStream();

                String contentAsString = readIt(is, len);
                return contentAsString;

            } finally {
                if(is != null)
                    is.close();
            }
        }

        public String readIt(InputStream input, int len) throws IOException, UnsupportedEncodingException
        {
            Reader reader = null;
            reader = new InputStreamReader(input, "UTF-8");
            char[] buffer = new char[len];
            reader.read(buffer);
            return new String(buffer);
        }
    }

}
