using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

namespace POSM
{
    class CallbackThreads
    {


        public static bool FileCopySuccess = false;
        public static bool db_backup = true;
        public static bool checkhotkey = true;
        public static bool deviceconfig = true;
        public static TimerCallback timerDelegate = null, timerDelegate1 = null, timerDelegate2 = null;
        public static Timer PumpTimer = null, checkHotKey = null, device_config = null;



        public static void TimerCallbackThreading() {

            if (db_backup) {

                timerDelegate = new TimerCallback(Db_Backup);
                PumpTimer = new Timer(timerDelegate, null, 60000, 60000);
            }
            if (checkhotkey) {

                timerDelegate1 = new TimerCallback(checkHotKeyPromotion);
                checkHotKey = new Timer(timerDelegate1, null, 25000, 25000);
            }

            if (deviceconfig) {

                timerDelegate2 = new TimerCallback(device_config_check);
                device_config = new Timer(timerDelegate2, null, 123000, 123000);
            }




        
        }



        private static void device_config_check(object state)
        {
            deviceconfig = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=2 and APP='devconfig'";
            string updatepending = "update ConfigurationTables set Updatepending=0 where SiteKey=2 and APP='devconfig'";

            try
            {


                SqlCommand cmd = DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();


                if (dbr.HasRows)
                {

                    while(dbr.Read()) {

                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString()+Macros.CONFIG_FILE, Macros.DESTI_PATH+Macros.CONFIG_BCK_FILE);
                            DB.CloseConn();
                            DB.ExecuteNonQuery(updatepending);
                            FileCopySuccess = true;
                            

                        
                        }
                    
                    }
                    deviceconfig = true;

                }
                else {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex) {

                Debug.WriteLine("Not read data in Config table");
            }



            //check new device config file  from local db

            //first hit configuration table and get device config xml file accoring site key and app name

            //store xml file from destination folder

            // restart the app
        }

        private static void checkHotKeyPromotion(object state)
        {

            checkhotkey = false;
            string query = "select SiteKey,APP,filepath,Updatepending from ConfigurationTables where SiteKey=1 and APP='hotkeylist'";
            string updatepending = "update ConfigurationTables set Updatepending=0 where SiteKey=1 and APP='hotkeylist'";

            try
            {


                SqlCommand cmd = DB.ExecuteReader(query);
                SqlDataReader dbr = cmd.ExecuteReader();

                
                if (dbr.HasRows)
                {
                    
                    while (dbr.Read())
                    {
                       
                        if (dbr["Updatepending"].Equals(true))
                        {

                            CopyPaste(dbr["filepath"].ToString() + Macros.HOTKEY_FILE, Macros.DESTI_PATH + Macros.HOTKEY_BCK_FILE);
                            DB.CloseConn();
                            DB.ExecuteNonQuery(updatepending);
                            FileCopySuccess = true;
                            

                        }

                    }
                    checkhotkey = true;

                }
                else
                {

                    Debug.WriteLine("Not Found data in Config table");
                }

                cmd.Dispose();
                dbr.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Not read data in Config table");

            }

            //check new hot keys promotion from local db

            //first hit configuration table and get hot key xml file accoring site key

            //store xml file from destination folder


            //restart the app
        }




        private static void Db_Backup(object state)
        {
            db_backup = false;
                string source = Macros.DESTI_PATH+Macros.DB_FILE;
                string destination = Macros.DESTI_PATH + Macros.DB_BCK_FILE;

                CopyPaste( source, destination);
                db_backup = true;
            
            //Backup Script

               //
        }

        public static void CopyPaste(string source, string destination) {

            try
            {
                

                File.Copy( source,  destination,true);
                Debug.WriteLine("File copy in destination folder");
                
            }
            catch (Exception ex)
            {

                Debug.WriteLine("can't copy file");
            }
        
        }



      

    }
}
